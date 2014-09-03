using System;
using System.Collections.Generic;
using System.Text;
using EventDataXML;
using Eb;

public interface INodeTriggerCondition
{
    //-------------------------------------------------------------------------
    string getId();

    //-------------------------------------------------------------------------
    void setEntity(CNode entity);

    //-------------------------------------------------------------------------
    bool excute(Group xml_group);
}