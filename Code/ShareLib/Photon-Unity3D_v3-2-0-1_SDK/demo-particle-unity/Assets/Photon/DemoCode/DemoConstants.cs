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
    /// <summary>
    /// Class to define a few constants used in this demo (for event codes, properties, etc).
    /// </summary>
    /// <remarks>
    /// These values are something made up for this particular demo! 
    /// You can define other values (and more) in your games, as needed.
    /// </remarks>
    public static class DemoConstants
    {
        /// <summary>(1) Event defining a color of a player.</summary>
        public const byte EvColor = 1;

        /// <summary>(2) Event defining the position of a player.</summary>
        public const byte EvPosition = 2;

        /// <summary>("s") Property grid size currently used in this room.</summary>
        public const string GridSizeProp = "s";

        /// <summary>("m") Property map (map / level / scene) currently used in this room.</summary>
        public const string MapProp = "m";

        /// <summary>Types available as map / level / scene.</summary>
        public enum MapType { Forest, Town, Sea }
    }
}