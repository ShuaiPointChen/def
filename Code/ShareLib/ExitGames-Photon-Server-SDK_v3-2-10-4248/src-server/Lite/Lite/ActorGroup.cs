using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lite
{
    using System;

    public class ActorGroup : ActorCollection
    {
        public byte GroupId { get; private set; }
        public ActorGroup(byte id)
        :base()
        {
            GroupId = id;
        }
    }

}
