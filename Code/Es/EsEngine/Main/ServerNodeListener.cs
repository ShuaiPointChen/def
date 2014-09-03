using System;
using System.Collections.Generic;
using System.Text;

namespace Es
{
    public enum _eStopReason
    {
        Success = 0,
        Failed
    }

    public interface IServerNodeListener
    {
        void onStart();

        void onRun();

        void onStop(_eStopReason stop_reason);
    }
}