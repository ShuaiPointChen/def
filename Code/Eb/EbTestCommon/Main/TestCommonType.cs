using System;
using System.Collections.Generic;
using System.Text;

//-----------------------------------------------------------------------------
public enum _eNodeType : byte
{
    Ec = 0,
    Es = 1
}

//-----------------------------------------------------------------------------
public enum _eMethodType : ushort
{
    nodeEc2EsLogin = 1,
    es2ecOnLogin = 1001,
    ec2esLogin,
}