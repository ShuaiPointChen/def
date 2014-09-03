using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    // 属性：描述组件依赖关系，自动添加依赖的组件，例如传送门组件依赖触发器组件
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class ComponentRequire : Attribute
    {
        //---------------------------------------------------------------------
        public Type Type { get; private set; }

        //---------------------------------------------------------------------
        public ComponentRequire(Type t)
        {
            Type = t;
        }
    }
}
