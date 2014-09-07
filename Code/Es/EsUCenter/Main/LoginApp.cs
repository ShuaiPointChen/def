using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Xml;
using Photon.SocketServer;
using Photon.SocketServer.Diagnostics;
using Photon.SocketServer.ServerToServer;
using MySql.Data;
using MySql.Data.MySqlClient;
using Eb;
using Es;
using Zk;

//public enum _eUCenterEtType : short
//{
//    EtApp = 0,
//    EtLogin,
//    EtSession
//}

////-----------------------------------------------------------------------------
//public enum _eUCenterPartType : byte
//{
//    UCenter = 0,
//    Client
//}

public class LoginApp<T> : Component<T> where T : ComponentDef, new()
{
    //-------------------------------------------------------------------------
    public Dictionary<string, LoginNode<ComponentDef>> mServerGroup = new Dictionary<string, LoginNode<ComponentDef>>();
    // 读配置文件.
    //public readonly static string mLoginNodeInfoPath = "/LoginServices";
    UCenterApp mUCenterApp = (UCenterApp)ApplicationBase.Instance;
    UCenterZkWatcher mZkWatcher;

    //-------------------------------------------------------------------------
    public uint NodeId { get { return mUCenterApp.NodeId; } }
    public string NodeIdStr { get { return mUCenterApp.NodeIdStr; } }
    //-------------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("LoginApp.init()");

        EntityMgr.getDefaultEventPublisher().addHandler(Entity);

        //string[] child = mUCenterApp.ZkClient.sget_children ("/" + mUCenterApp.ProjectName +"/" +  mLoginNodeInfoPath, true);

        mUCenterApp.ZkClient.subscribeChildChanges("/" + mUCenterApp.ProjectName + "/"
            + _eConstLoginNode.LoginServices.ToString(), _onServerGroupChange);

        //string data = mUCenterApp.ZkClient.sread(mLoginNodeInfoPath, true);
        //if (null != data)
        //{
        //    OnGetNodeInfo(data);
        //}

        mZkWatcher = new UCenterZkWatcher(this);
        PhotonApp photon_app = (PhotonApp)ApplicationBase.Instance;
        photon_app.ZkClient.addHandler(mZkWatcher.handler);
        //mUCenterApp.ZkClient.subscribeDataChanges(mLoginNodeInfoPath, mZkWatcher.onLoginNodeInfo);
    }

    //-------------------------------------------------------------------------
    public override void release()
    {
        EbLog.Note("LoginApp.release()");
    }

    //-------------------------------------------------------------------------
    public override void update(float elapsed_tm)
    {
    }

    //-------------------------------------------------------------------------
    public override void handleEvent(object sender, EntityEvent e)
    {
        if (e is EvMainSessionEvent)
        {
            EvMainSessionEvent ev = (EvMainSessionEvent)e;
            byte local_type = (byte)ev.Se.node_type_local;
            byte remote_type = (byte)ev.Se.node_type_remote;

            if (ev.Se.connect)
            {
                // 连接消息
                Dictionary<string, object> cache_data = new Dictionary<string, object>();
                cache_data["RemoteSession"] = ev.Se.session;
                Entity et_ucentersession = EntityMgr.createEmptyEntity("EtUCenterSession", cache_data, Entity);
                et_ucentersession.addComponent<LoginUCenterSession<DefUCenterSession>>();
            }
            else
            {
                // 断开消息

                // do nothing
            }

            //if (localType == (byte)_ePartType.Login && remoteType == (byte)_ePartType.Client)
            //{
            //    string accountName = (string)(vec_param[2]);
            //    string password = (string)(vec_param[3]);
            //    string server = (string)(vec_param[4]);

            //    //Es.CPhotonServerPeerS peer = (Es.CPhotonServerPeerS)vec_param[5];
            //    //mLog.InfoFormat("player loging , account:{0},password:{1},server:{2}"
            //    //    , accountName, password, server);

            //    if (accountName == "" || password == "" || server == "")
            //    {
            //        //mLog.ErrorFormat("player loging , account:{0},password:{1},server:{2}"
            //        //, accountName, password, server);
            //        return;
            //    }

            //    ClientLoginInfo loginInfo = new ClientLoginInfo();
            //    loginInfo.account = accountName;
            //    loginInfo.password = password;
            //    loginInfo.server = server;
            //    loginInfo.peer = peer;

            //    if (!addLoginPlayer(loginInfo))
            //    {
            //        //mLog.ErrorFormat("{0} not exists! ", server);
            //    }
            //}
        }
    }

    //-------------------------------------------------------------------------
    private void _onServerGroupChange(int result, string data, string[] chdn, Dictionary<string, object> param)
    {
        if (result != 0) return;

        List<string> remoteServer = chdn.ToList();
        List<string> localServer = mServerGroup.Keys.ToList<string>();
        IEnumerable<string> add = remoteServer.Except(localServer);
        IEnumerable<string> del = localServer.Except(remoteServer);

        foreach (var sv in del)
        {
            // todo: 取消监听.
            mServerGroup.Remove(sv);
        }

        foreach (var sv in add)
        {
            Dictionary<string, object> cache_data = new Dictionary<string, object>();
            cache_data["parent"] = this;
            cache_data["ProjectName"] = mUCenterApp.ProjectName;
            cache_data["ServerGroupName"] = sv;
            Entity et_node = EntityMgr.createEmptyEntity("EtNode", cache_data, null);
            LoginNode<ComponentDef> co_node = et_node.addComponent<LoginNode<ComponentDef>>();

            Dictionary<string, object> pa = new Dictionary<string, object>();
            pa["LoginNode"] = co_node;
            mUCenterApp.ZkClient.areadData("/" + mUCenterApp.ProjectName + "/"+ _eConstLoginNode.LoginServices.ToString() + "", false, _onGetServersLoginNeedInfo, pa);

        }
    }

    private void _onGetServersLoginNeedInfo(int result, string data, string[] chdn, Dictionary<string, object> param)
    {
        if(result != 0 ) return ;
        LoginNode<ComponentDef> co_node = param["LoginNode"] as LoginNode<ComponentDef>;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(data);
        XmlNode zkinfo = doc.SelectSingleNode("ServerInfo/Login");
        string loginNode = zkinfo.Attributes["LoginNode"].Value;
        string  connStr = zkinfo.Attributes["ConnectionString"].Value;
        co_node.onGetLoginInfo(loginNode, connStr);

        Dictionary<string, Object> pa = new Dictionary<string, object>();
        pa["LoginNode"] = co_node;
        mUCenterApp.ZkClient.subscribeChildChanges(loginNode, mZkWatcher._onServerNodeChange, pa);

    }

    //-------------------------------------------------------------------------
    public override void onChildInit(Entity child)
    {
    }

    //-------------------------------------------------------------------------
    public IZkClient getZk()
    {
        return mUCenterApp.ZkClient;
    }

    //-------------------------------------------------------------------------
    //public void OnGetNodeInfo(string data)
    //{
    //    XmlDocument doc = new XmlDocument();
    //    doc.LoadXml(data);
    //    XmlNodeList chd = doc.SelectSingleNode("ServerGroups").ChildNodes;
    //    if (null != chd)
    //    {
    //        foreach (XmlNode child in chd)
    //        {
    //            if (mServerGroup.ContainsKey(child.Name)) continue;

    //            if (child.NodeType == XmlNodeType.Comment) continue;

    //            Dictionary<string, object> cache_data = new Dictionary<string, object>();
    //            cache_data["parent"] = this;
    //            cache_data["LoginQueue"] = child.Attributes["LoginQueue"].Value;
    //            cache_data["LoginCompleteQueue"] = child.Attributes["LoginCompleteQueue"].Value;
    //            cache_data["LoginQueueLock"] = child.Attributes["LoginQueueLock"].Value;
    //            cache_data["LoginCompleteQueueLock"] = child.Attributes["LoginCompleteQueueLock"].Value;
    //            cache_data["DbConnectionStr"] = child.Attributes["ConnectionString"].Value;
    //            cache_data["PlayerOfflineNode"] = child.Attributes["PlayerOfflineNode"].Value;
    //            cache_data["PlayerOfflineLock"] = child.Attributes["PlayerOfflineLock"].Value;
    //            cache_data["GateServer"] = child.Attributes["GateServer"].Value;
    //            cache_data["ZoneServer"] = child.Attributes["ZoneServer"].Value;
    //            cache_data["DBServer"] = child.Attributes["DBServer"].Value;
    //            cache_data["ConnectionString"] = child.Attributes["ConnectionString"].Value;
    //            cache_data["ServerGroupName"] = child.Name;

    //            Entity et_node = EntityMgr.createEmptyEntity("EtNode", cache_data, null);
    //            LoginNode<ComponentDef> co_node = et_node.addComponent<LoginNode<ComponentDef>>();
    //            mServerGroup[child.Name] = co_node;
    //        }
    //    }
    //}

    //-------------------------------------------------------------------------
    public bool addLoginPlayer(string serverGroup, string account, string password, string channel, RpcSession s)
    {
        LoginNode<ComponentDef> serverinfo;
        if (mServerGroup.TryGetValue(serverGroup, out serverinfo))
        {
            return serverinfo.addLoginPlayer(account, password, channel, s);
        }
        return false;
    }

    //-------------------------------------------------------------------------
    public void onGateBack(string server, string account, string result)
    {
        LoginNode<ComponentDef> serverinfo;
        if (mServerGroup.TryGetValue(server, out serverinfo))
        {
            serverinfo.gateBackPlayerLoginResult(account, result);
        }
    }
}
