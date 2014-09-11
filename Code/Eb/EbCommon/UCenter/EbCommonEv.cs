using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    //-------------------------------------------------------------------------
    public class EvEntityCreateRemote : EntityEvent
    {
        public EvEntityCreateRemote() : base() { }
        public Entity entity;
        public EntityData entity_data;
    }
}
