using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    abstract class Test
    {
        //---------------------------------------------------------------------
        public abstract void init();

        //---------------------------------------------------------------------
        public abstract void release();

        //---------------------------------------------------------------------
        public abstract void update();
    }
}
