using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    //-------------------------------------------------------------------------
    public enum _eUCenterNodeType : byte
    {
        Client = 1,
        UCenter = 255,
    }

    //-------------------------------------------------------------------------
    public enum _eUCenterEtType : ushort
    {
        EtUCenterSession = 1,
    }

    //-------------------------------------------------------------------------
    public enum _eUCenterCoType : ushort
    {
        Session = 10001,
    }

    //-------------------------------------------------------------------------
    public enum _eUCenterMethodType : ushort
    {
        // Session
        login2ClientOnLogin = 10001,
        client2LoginLogin,
    }

    //-------------------------------------------------------------------------
    public enum _eConstLoginNode
    {
        LoginQueue,                         // center -->  Servers  玩家上线注册服务器组
        LoginCompleteQueue,                 // Servers --> center   玩家注册服务器组反馈Center
        LoginQueueLock,
        LoginCompleteQueueLock,
        PlayerOfflineNode,                  // Servers --> center 玩家从服务器组下线
        PlayerOfflineLock,
        LoginReceiveNode,                   // 服务器组处理登陆server.
        //DbConnectionStr,                  // 账号数据库登陆字符串.
        LoginServices,                      // 提供登陆服务的节点.
        //NodeNumLength = 10,               // zookeeper的节点id数字字符串长度.
    }

    //-------------------------------------------------------------------------
    public class _eUCenter
    {
        public readonly static string UCenterProjectName = "UCenter";
        public readonly static string UCenterNodeName = "UCenter";
    }

    //-------------------------------------------------------------------------
    public struct _tLoginResponseInfo
    {
        public string acc;
        public string server_group;
        public string result;
        public Dictionary<byte, object> map_userdata;
    }
}
