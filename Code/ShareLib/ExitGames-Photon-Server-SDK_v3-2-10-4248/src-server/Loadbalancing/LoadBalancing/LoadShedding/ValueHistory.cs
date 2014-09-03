// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueHistory.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ValueHistory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.LoadShedding
{
    using System.Collections.Generic;
    using System.Linq;

    using ExitGames.Logging;

    internal class ValueHistory : Queue<int>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly int capacity;

        public ValueHistory(int capacity)
            : base(capacity)
        {
            this.capacity = capacity;
        }

        public void Add(int value)
        {
            if (this.Count == this.capacity)
            {
                this.Dequeue();
            }

            this.Enqueue(value);

            ////if (log.IsDebugEnabled)
            ////{
            ////    string s = value + ":";
            ////    foreach (int val in this)
            ////        s += val.ToString() + ",";
            ////    s += ">>" + ((double)this.Average()).ToString();
            ////    log.Debug(s);
            ////}
        }
    }
}