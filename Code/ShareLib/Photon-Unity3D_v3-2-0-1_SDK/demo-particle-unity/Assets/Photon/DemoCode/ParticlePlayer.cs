// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Exit Games GmbH">
//   Exit Games GmbH, 2012
// </copyright>
// <summary>
//   The "Particle" demo is a load balanced and Photon Cloud compatible "coding" demo.
//   The focus is on showing how to use the Photon features without too much "game" code cluttering the view.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

namespace ExitGames.Client.DemoParticle
{
    using ExitGames.Client.Photon;
    using ExitGames.Client.Photon.LoadBalancing;
    using System.Collections;
    
    /// <summary>
    /// Extends Player with some Particle Demo specific properties and methods.
    /// </summary>
    /// <remarks>
    /// Instances of this class are created by GameLogic.CreatePlayer.
    /// There's a GameLogic.LocalPlayer field, that represents this user's player (in the room).
    /// 
    /// This class does not make use of networking directly. It's updated by incoming events but
    /// the actual sending and receiving is handled in GameLogic.
    /// 
    /// The WriteEv* and ReadEv* methods are simple ways to create event-content per player. 
    /// Only the LocalPlayer per client will actually send data. This is used to update the remote
    /// clients of position (and color, etc). 
    /// Receiving clients identify the corresponding Player and call ReadEv* to update that 
    /// (remote) player.
    /// Read the remarks in WriteEvMove.
    /// </remarks>
    public class ParticlePlayer : Player
    {
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Color { get; set; }

        private int LastUpdateTimestamp { get; set; }
        public int UpdateAge { get { return GameLogic.Timestamp - this.LastUpdateTimestamp; } }

        /// <summary>
        /// Stores this client's "group interest currently set on server" of this player (not necessarily the current one).
        /// </summary>
        public byte VisibleGroup { get; set; }

        public ParticlePlayer(string name, int actorID, bool isLocal, Hashtable actorProperties) : base(name, actorID, isLocal, actorProperties)
        {
            if (isLocal)
            {
                // we pick a random color when we create a local player
                this.RandomizeColor();
            }
        }

        /// <summary>Creates a random color by generating a new integer and setting the highest byte to 255.</summary>
        /// <remarks>RGB colors can be represented as integer (3 bytes, ignoring the first which represents alpha).</remarks>
        internal void RandomizeColor()
        {
            this.Color = (int)(SupportClass.ThreadSafeRandom.Next() | 0xFF000000);
        }

        /// <summary>Randomizes position within the gridSize.</summary>
        internal void RandomizePosition()
        {
            int gridSize = 16;
            if (this.RoomReference != null)
            {
                gridSize = ((ParticleRoom)this.RoomReference).GridSize;
            }

            this.PosX = SupportClass.ThreadSafeRandom.Next() % gridSize;
            this.PosY = SupportClass.ThreadSafeRandom.Next() % gridSize;
        }

        /// <summary>
        /// Simple method to make the "player" move even without input. This way, we get some
        /// movement even if one developer tests with many running clients.
        /// </summary>
        internal void MoveRandom()
        {
            this.PosX += (SupportClass.ThreadSafeRandom.Next() % 3) - 1;
            this.PosY += (SupportClass.ThreadSafeRandom.Next() % 3) - 1;
            this.ClampPosition();
        }

        /// <summary>Checks if a position is in the grid (still on the board) and corrects it if needed.</summary>
        public void ClampPosition()
        {
            int gridSize = ((ParticleRoom)this.RoomReference).GridSize;

            if (this.PosX < 0)
            {
                this.PosX = 0;
            }

            if (this.PosX >= gridSize - 1)
            {
                this.PosX = gridSize - 1;
            }

            if (this.PosY < 0)
            {
                this.PosY = 0;
            }

            if (this.PosY > gridSize - 1)
            {
                this.PosY = gridSize - 1;
            }
        }

        /// <summary>
        /// Converts the player info into a string.
        /// </summary>
        /// <returns>String showing basic info about this player.</returns>
        public override string ToString()
        {
            return this.ID + "'" + this.Name + "':" + this.GetGroup() + " " + this.PosX + ":" + this.PosY + " PlayerProps: " + SupportClass.DictionaryToString(this.CustomProperties);
        }

        /// <summary>Creates the "custom content" Hashtable that is sent as position update.</summary>
        /// <remarks>
        /// As with event codes, the content of this event is arbitrary and "made up" for this demo.
        /// Your game (e.g.) could use floats as positions or you send a height and actions or state info.
        /// It makes sense to use numbers (best: bytes) as Hashtable key type, cause they are use less space.
        /// But this is not a requirement as you see in WriteEvColor.
        /// 
        /// The position can only go up to 128 in this demo, so a byte[] technically is the best (leanest) 
        /// choice here.
        /// </remarks>
        /// <returns>Hashtable for event "move" to update others</returns>
        public Hashtable WriteEvMove()
        {
            Hashtable evContent = new Hashtable();
            evContent[(byte)1] = new byte[] { (byte)this.PosX, (byte)this.PosY };
            return evContent;
        }

        /// <summary>Reads the "custom content" Hashtable that is sent as position update.</summary>
        /// <returns>Hashtable for event "move" to update others</returns>
        public void ReadEvMove(Hashtable evContent)
        {
            byte[] posArray = (byte[])evContent[(byte)1];
            this.PosX = posArray[0];
            this.PosY = posArray[1];
            this.LastUpdateTimestamp = GameLogic.Timestamp;
        }

        /// <summary>Creates the "custom content" Hashtable that is sent as color update.</summary>
        /// <returns>Hashtable for event "color" to update others</returns>
        public Hashtable WriteEvColor()
        {
            Hashtable evContent = new Hashtable();
            evContent[1] = this.Color;
            return evContent;
        }

        /// <summary>Reads the "custom content" Hashtable that is sent as color update.</summary>
        /// <returns>Hashtable for event "color" to update others</returns>
        public void ReadEvColor(Hashtable evContent)
        {
            this.Color = (int)evContent[1];
            this.LastUpdateTimestamp = GameLogic.Timestamp;
        }

        /// <summary>
        /// Gets the "Interest Group" this player is in, based on it's position (in this demo).
        /// </summary>
        /// <returns>The group id this player is in.</returns>
        public byte GetGroup()
        {
            ParticleRoom pr = this.RoomReference as ParticleRoom;
            if (pr != null)
            {
                return pr.GetGroup(this.PosX, this.PosY);
            }

            return 0;
        }
    }
}
