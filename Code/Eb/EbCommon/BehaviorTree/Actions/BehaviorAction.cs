using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    public class BehaviorAction : BehaviorComponent
    {
        //---------------------------------------------------------------------
        //private Func<BehaviorReturnCode> mAction;

        //---------------------------------------------------------------------
        public BehaviorAction(BehaviorTree bt)
            : base(bt)
        {
        }

        //---------------------------------------------------------------------
        //public BehaviorAction(Func<BehaviorReturnCode> action)
        //{
        //    mAction = action;
        //}

        //---------------------------------------------------------------------
        public override BehaviorReturnCode Behave()
        {
            return BehaviorReturnCode.Success;
            //try
            //{
            //    switch (mAction.Invoke())
            //    {
            //        case BehaviorReturnCode.Success:
            //            ReturnCode = BehaviorReturnCode.Success;
            //            return ReturnCode;
            //        case BehaviorReturnCode.Failure:
            //            ReturnCode = BehaviorReturnCode.Failure;
            //            return ReturnCode;
            //        case BehaviorReturnCode.Running:
            //            ReturnCode = BehaviorReturnCode.Running;
            //            return ReturnCode;
            //        default:
            //            ReturnCode = BehaviorReturnCode.Failure;
            //            return ReturnCode;
            //    }
            //}
            //catch (Exception e)
            //{
            //    EbLog.Error(e.ToString());
            //    ReturnCode = BehaviorReturnCode.Failure;
            //    return ReturnCode;
            //}
        }
    }
}
