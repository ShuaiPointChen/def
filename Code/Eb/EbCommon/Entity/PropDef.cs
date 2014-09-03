using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    public class PropDef : IDisposable
    {
        //-----------------------------------------------------------------------------
        private string mPropKey;
        private Type mValueType;

        //-----------------------------------------------------------------------------
        public PropDef(string key, Type value_type)
        {
            mPropKey = key;
            mValueType = value_type;
        }

        //-----------------------------------------------------------------------------
        ~PropDef()
        {
            this.Dispose(false);
        }

        //-----------------------------------------------------------------------------
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        //-----------------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        //-----------------------------------------------------------------------------
        public string getKey()
        {
            return mPropKey;
        }

        //-----------------------------------------------------------------------------
        public Type getValueType()
        {
            return mValueType;
        }
    }
}
