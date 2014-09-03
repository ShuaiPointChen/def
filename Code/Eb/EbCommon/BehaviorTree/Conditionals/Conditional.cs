using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    public class Conditional : BehaviorComponent
    {
        //---------------------------------------------------------------------
        private Func<Boolean> mFuncBool;

        //---------------------------------------------------------------------
        // Returns a return code equivalent to the test 
        // -Returns Success if true
        // -Returns Failure if false
        // <param name="test">the value to be tested</param>
        public Conditional(BehaviorTree bt, Func<Boolean> func_bool)
            : base(bt)
        {
            mFuncBool = func_bool;
        }

        //---------------------------------------------------------------------
        public override BehaviorReturnCode Behave()
        {
            try
            {
                switch (mFuncBool.Invoke())
                {
                    case true:
                        ReturnCode = BehaviorReturnCode.Success;
                        return ReturnCode;
                    case false:
                        ReturnCode = BehaviorReturnCode.Failure;
                        return ReturnCode;
                    default:
                        ReturnCode = BehaviorReturnCode.Failure;
                        return ReturnCode;
                }
            }
            catch (Exception e)
            {
                EbLog.Error(e.ToString());
                ReturnCode = BehaviorReturnCode.Failure;
                return ReturnCode;
            }
        }
    }
}
