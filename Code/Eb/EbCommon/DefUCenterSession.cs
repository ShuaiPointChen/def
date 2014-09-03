using System;
using System.Collections.Generic;
using System.Text;
using Eb;

public class DefUCenterSession : ComponentDef
{
    //-------------------------------------------------------------------------
    public override ushort getComponentId()
    {
        return (ushort)1001;
    }

    //-------------------------------------------------------------------------
    public override void defAllProp(Dictionary<string, string> map_param)
    {
    }
}
