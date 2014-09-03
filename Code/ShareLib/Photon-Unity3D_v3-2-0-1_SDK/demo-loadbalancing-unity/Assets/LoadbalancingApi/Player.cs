// ----------------------------------------------------------------------------
// <copyright file="Player.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   Per client in a room, a Player is created. This client's Player is also
//   known as PhotonClient.LocalPlayer and the only one you might change 
//   properties for.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

namespace ExitGames.Client.Photon.LoadBalancing
{
    using System;
    using System.Collections;
    using ExitGames.Client.Photon;

    /// <summary>
    /// Summarizes a "player" within a room, identified (in that room) by ID (or "actorID").
    /// </summary>
    /// <remarks>
    /// Each player has a actorID, valid for that room. It's -1 until assigned by server (and client logic).
    /// </remarks>
    public class Player
    {
        /// <summary>Backing field for property.</summary>
        private int actorID = -1;

        /// <summary>Only one player is controlled by each client. Others are not local.</summary>
        public readonly bool IsLocal;

        /// <summary>
        /// A reference to the LoadbalancingClient which is currently keeping the connection and state.
        /// </summary>
        protected internal LoadBalancingClient LoadBalancingClient { get; set; }

        /// <summary>
        /// Used internally to identify the masterclient of a room.
        /// </summary>
        protected internal Room RoomReference { get; set; }

        /// <summary>Identifier of this player in current room. Also known as: actorNumber or actorID. It's -1 outside of rooms.</summary>
        /// <remarks>The ID is assigned per room and only valid in that context. It will change even on leave and re-join. IDs are never re-used per room.</remarks>
        public int ID
        {
            get { return this.actorID; }
        }

        /// <summary>Background field for Name.</summary>
        private string name;

        /// <summary>Nickname of this player. Also in Properties (a "well known" property which has a byte-typed key).</summary>
        /// <remarks>
        /// A player might change his own playername in a room (it's only a property).
        /// Setting this value updates the server and other players (using an operation).
        /// </remarks>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (!string.IsNullOrEmpty(this.name) && this.name.Equals(value))
                {
                    return;
                }

                this.name = value;

                // update a room, if we changed our name (locally, while being in a room)
                if (this.IsLocal && this.LoadBalancingClient != null && this.LoadBalancingClient.State == ClientState.Joined)
                {
                    this.SetPlayerNameProperty();
                }
            }
        }

        /// <summary>
        /// The player with the lowest actorID is the master and could be used for special tasks.
        /// The LoadBalancingClient.LocalPlayer is not master unless in a room (this is the only player which exists outside of rooms, to store a name).
        /// </summary>
        public bool IsMasterClient
        {
            get
            {
                if (this.RoomReference == null)
                {
                    return false;
                }

                return this.ID == this.RoomReference.MasterClientId;
            }
        }

        /// <summary>Read-only cache for custom properties of player. Set via Player.SetCustomProperties.</summary>
        /// <remarks>
        /// Don't modify the content of this Hashtable. Use SetCustomProperties and the 
        /// properties of this class to modify values. When you use those, the client will
        /// sync values with the server.
        /// </remarks>
        public Hashtable CustomProperties { get; private set; }

        /// <summary>Creates a Hashtable with all properties (custom and "well known" ones).</summary>
        /// <remarks>Creates new Hashtables each time used, so if used more often, cache this.</remarks>
        public Hashtable AllProperties
        {
            get
            {
                Hashtable allProps = new Hashtable();
                allProps.Merge(this.CustomProperties);
                allProps[ActorProperties.PlayerName] = this.name;
                return allProps;
            }
        }
		
		/// <summary>Custom object associated with this Player.</summary>
		public object Tag;

        /// <summary>
        /// Creates a player instance.
        /// To extend and replace this Player, override LoadBalancingPeer.CreatePlayer().
        /// </summary>
        /// <param name="name">Name of the player (a "well known property").</param>
        /// <param name="actorID">ID or ActorNumber of this player in the current room (a shortcut to identify each player in room)</param>
        /// <param name="isLocal">If this is the local peer's player (or a remote one).</param>
        protected internal Player(string name, int actorID, bool isLocal) : this(name, actorID, isLocal, null)
        {
        }

        /// <summary>
        /// Creates a player instance.
        /// To extend and replace this Player, override LoadBalancingPeer.CreatePlayer().
        /// </summary>
        /// <param name="name">Name of the player (a "well known property").</param>
        /// <param name="actorID">ID or ActorNumber of this player in the current room (a shortcut to identify each player in room)</param>
        /// <param name="isLocal">If this is the local peer's player (or a remote one).</param>
        /// <param name="playerProperties"> </param>
        protected internal Player(string name, int actorID, bool isLocal, Hashtable playerProperties)
        {
            this.IsLocal = isLocal;
            this.actorID = actorID;
            this.Name = name;

            this.CustomProperties = new Hashtable();
            this.CacheProperties(playerProperties);
        }

        /// <summary>Caches properties for new Players or when updates of remote players are received. Use SetCustomProperties() for a synced update.</summary>
        /// <remarks>
        /// This only updates the CustomProperties and doesn't send them to the server.
        /// Mostly used when creating new remote players, where the server sends their properties.
        /// </remarks>
        public virtual void CacheProperties(Hashtable properties)
        {
            if (properties == null || properties.Count == 0 || this.CustomProperties.Equals(properties))
            {
                return;
            }

            if (properties.ContainsKey(ActorProperties.PlayerName))
            {
                string nameInServersProperties = (string)properties[ActorProperties.PlayerName];
                if (nameInServersProperties != null)
                {
                    if (this.IsLocal)
                    {
                        // the local playername is different than in the properties coming from the server
                        // so the local name was changed and the server is outdated -> update server
                        // update property instead of using the outdated name coming from server
                        if (!nameInServersProperties.Equals(this.name))
                        {
                            this.SetPlayerNameProperty();
                        }
                    }
                    else
                    {
                        this.Name = nameInServersProperties;
                    }
                }
            }

            this.CustomProperties.MergeStringKeys(properties);
        }

        /// <summary>
        /// This Player name and custom properties as string.
        /// </summary>
        public override string ToString()
        {
            return this.Name + " " + SupportClass.DictionaryToString(this.CustomProperties);
        }

        /// <summary>
        /// If players are equal.
        /// </summary>
        public override bool Equals(object p)
        {
            Player pp = p as Player;
            return (pp != null && this.GetHashCode() == pp.GetHashCode());
        }

        /// <summary>
        /// Accompanies Equals, using the ID (actorNumber) as HashCode to return.
        /// </summary>
        public override int GetHashCode()
        {
            return this.ID;
        }

        /// <summary>
        /// Used internally, to update this client's playerID when assigned (doesn't change after assignment).
        /// </summary>
        protected internal void ChangeLocalID(int newID)
        {
            if (!this.IsLocal)
            {
                //Debug.LogError("ERROR You should never change Player IDs!");
                return;
            }

            this.actorID = newID;
        }

        /// <summary>
        /// Updates and synchronizes the named properties of this Player with the values of propertiesToSet.
        /// </summary>
        /// <remarks>
        /// Any player's properties are available in a Room only and only until the player disconnect or leaves.
        /// Access any player's properties by: Player.CustomProperties (read-only!) but don't modify that hashtable.
        /// 
        /// New properties are added, existing values are updated.
        /// Other values will not be changed, so only provide values that changed or are new.
        /// To delete a named (custom) property of this player, use null as value.
        /// Only string-typed keys are applied (everything else is ignored).
        /// 
        /// Local cache is updated immediately, other players are updated through Photon with a fitting operation.
        /// To reduce network traffic, set only values that actually changed.
        /// </remarks>
        /// <param name="propertiesToSet">Hashtable of props to udpate, set and sync. See description.</param>
        public void SetCustomProperties(Hashtable propertiesToSet)
        {
            Hashtable customProps = propertiesToSet.StripToStringKeys() as Hashtable;

            // merge (delete null-values)
            this.CustomProperties.Merge(customProps);
            this.CustomProperties.StripKeysWithNullValues();

            // send (sync) these new values if in room
            if (this.RoomReference != null && this.RoomReference.IsLocalClientInside)
            {
                this.RoomReference.LoadBalancingClient.OpSetCustomPropertiesOfActor(this.actorID, customProps);
            }
        }

        /// <summary>Uses OpSetPropertiesOfActor to sync this player's name (server is being updated with this.Name).</summary>
        private void SetPlayerNameProperty()
        {
            Hashtable properties = new Hashtable();
            properties[ActorProperties.PlayerName] = this.name;
            this.LoadBalancingClient.OpSetPropertiesOfActor(this.ID, properties);
        }
    }
}