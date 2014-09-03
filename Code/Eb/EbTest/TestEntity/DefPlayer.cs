using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    public class DefPlayer : ComponentDef
    {
        //---------------------------------------------------------------------
        public Prop<bool> mPropIsBot = null;// 是否是机器人
        public Prop<string> mPropNickName = null;// 昵称
        public Prop<bool> mPropGender = null;// 性别，男=true，女=false
        public Prop<EntityTransform> mPropTransform = null;

        //---------------------------------------------------------------------
        public override void defAllProp(Dictionary<string, string> map_param)
        {
            mPropIsBot = defProp<bool>(map_param, "IsBot", false);
            mPropNickName = defProp<string>(map_param, "NickName", "");
            mPropGender = defProp<bool>(map_param, "Gender", false);
            mPropTransform = defProp<EntityTransform>(map_param, "Transform", new EntityTransform());
        }
    }
}
