using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Eb
{
    public abstract class IProp : IDisposable
    {
        //-----------------------------------------------------------------------------
        public delegate void _funcPropChanged(IProp prop, object param);
        protected PropDef mPropDef = null;

        //-----------------------------------------------------------------------------
        public _funcPropChanged OnChanged { get; set; }

        //-----------------------------------------------------------------------------
        public IProp(PropDef prop_def)
        {
            mPropDef = prop_def;
        }

        //-----------------------------------------------------------------------------
        ~IProp()
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
            return mPropDef.getKey();
        }

        //-----------------------------------------------------------------------------
        public Type getValueType()
        {
            return mPropDef.getValueType();
        }

        //-----------------------------------------------------------------------------
        public PropDef getPropertyDef()
        {
            return mPropDef;
        }

        //-----------------------------------------------------------------------------
        public abstract object getValue();

        //-----------------------------------------------------------------------------
        public abstract void fromJsonString(string json_str);

        //-----------------------------------------------------------------------------
        public abstract string toJsonString();
    };

    public class Prop<T> : IProp
    {
        //-----------------------------------------------------------------------------
        protected T mValue;

        //-----------------------------------------------------------------------------
        public Prop(PropDef prop_def, T default_value)
            : base(prop_def)
        {
            mValue = default_value;
        }

        //-----------------------------------------------------------------------------
        public T get()
        {
            return mValue;
        }

        //-----------------------------------------------------------------------------
        public void set(T value)
        {
            mValue = value;

            if (OnChanged != null)
            {
                OnChanged(this, null);
            }
        }

        //-----------------------------------------------------------------------------
        public void set(T value, object param)
        {
            mValue = value;

            if (OnChanged != null)
            {
                OnChanged(this, param);
            }
        }

        //-----------------------------------------------------------------------------
        public override object getValue()
        {
            return mValue;
        }

        //-----------------------------------------------------------------------------
        public override void fromJsonString(string json_str)
        {
            mValue = EbJsonHelper.deserialize<T>(json_str);
        }

        //-----------------------------------------------------------------------------
        public override string toJsonString()
        {
            return EbJsonHelper.serialize(mValue);
        }
    }
}
