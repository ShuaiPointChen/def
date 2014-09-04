using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;
using Photon.SocketServer;
using Photon.SocketServer.Diagnostics;
using Photon.SocketServer.ServerToServer;
using Eb;
using Es;
using Zk;

public class ServerInfo
{
    public string Id;     // ZooKeeper 的唯一id
    public string Ip;     // server的ip
    public string Port;   // server的port
}

public class LoginServerInfo
{
    public string id;
    public string loginLockNode;       // Login --> Gate
    public string loginLockComNode;    // Gate --> Login
    public string loginNode;       // Login --> Gate
    public string loginComNode;    // Gate --> Login
    public string offlineNode;     // Gate --> Login
    public string offlineLock;

    public bool bloginLock = false;
    public bool bloginComLock = false;
    public bool bofflineLock = false;
    public string ip;
    public string port;
}

public class LoginPlayer
{
    public string Account;   // 玩家账号.
    public string LoginNodeId; // Send LoginServer zk id.
    public string ServerGroup; // 服务器组.
}

public class ServerUCenter<T> : Component<T> where T : ComponentDef, new()
{
    //------------------------------------------------------------------------------------
    public delegate void delegateOnLogin(string acc, string token);
    delegateOnLogin mFuncOnLogin;
    ServerUCenterZkWatcher mZkWatcher;
    // 这两个参数从配置文件读取.
    public string mLoginNodeInfoPath = "/Login/LoginNodeInfo";
    public string mLoginPath = "/Login/LoginServer";
    // login server 通知gateserver的node.
    string mCurLoginNodePath = "";
    string mCurLoginLockPath = "";
    // 处理好了以后 通知Login的node.
    string mCurLoginCompleteNodePath = "";
    string mCurLoginCompleteLockPath = "";
    // 玩家下线通知Login
    string mCurOfflineNodePath = "";
    string mCurOfflineLockPath = "";
    Dictionary<string, LoginServerInfo> mLoginServer = new Dictionary<string, LoginServerInfo>();
    Dictionary<string, LoginPlayer> mLoginPlayer = new Dictionary<string, LoginPlayer>();
    // 下线玩家列表.
    Queue<string> mOfflineList = new Queue<string>();

    //------------------------------------------------------------------------------------
    public string ProjectName { get; internal set; }

    //------------------------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("ServerUCenter.init()");

        mZkWatcher = new ServerUCenterZkWatcher(this);
        PhotonApp photon_app = (PhotonApp)ApplicationBase.Instance;
        photon_app.ZkClient.addHandler(mZkWatcher.handler);
        ProjectName = photon_app.ProjectName;

        // get login server
        string data = getZkClient().sread(mLoginNodeInfoPath, true);
        if (null != data)
        {
            _onGetNodeInfo(data);
        }

        getZkClient().subscribeChildChanges(mLoginPath, _onLoginServerList);
    }

    //------------------------------------------------------------------------------------
    public override void release()
    {
        EbLog.Note("ServerUCenter.release()");
    }

    //------------------------------------------------------------------------------------
    public override void update(float elapsed_tm)
    {
        foreach (var svr in mLoginServer)
        {
            if (!svr.Value.bloginComLock && mLoginPlayer.Count > 0)
            {
                // 找到需要反馈给当前LoginServer的Player.
                var playerList = mLoginPlayer.Where(pl => pl.Value.LoginNodeId.Equals(svr.Value.id));
                string LoginResult = "";
                List<string> del = new List<string>();
                foreach (var player in playerList)
                {
                    // 创建 EtPlayerLogin........
                    string singleInfo = string.Format("{0}:{1}:{2}:", player.Value.Account, player.Value.ServerGroup, "success");
                    LoginResult += singleInfo;
                    del.Add(player.Key);

                    // 将登陆结果通知应用层
                    if (mFuncOnLogin != null) mFuncOnLogin(player.Key, "token123");
                }

                foreach (var sigl in del)
                {
                    mLoginPlayer.Remove(sigl);
                }

                if (LoginResult != "")
                {
                    LoginResult = LoginResult.Substring(0, LoginResult.Length - 1); // 去掉多余的分号.
                    EbLog.Note("LoginResult:" + LoginResult);
                    getZkClient().awriteData(svr.Value.loginComNode, LoginResult, null);
                    getZkClient().acreate(svr.Value.loginLockComNode, "", ZK_CONST.ZOO_EPHEMERAL, null);
                    svr.Value.bloginComLock = true;
                    //break;
                }
            }

            if (!svr.Value.bofflineLock && mOfflineList.Count > 0)
            {
                string data = "";
                while (mOfflineList.Count > 0)
                {
                    data += mOfflineList.Dequeue() + ",";
                }
                getZkClient().awriteData(svr.Value.offlineNode, data, null);
                getZkClient().acreate(svr.Value.offlineLock, "", ZK_CONST.ZOO_EPHEMERAL, null);
                svr.Value.bofflineLock = true;
            }
        }
    }

    //------------------------------------------------------------------------------------
    public override void onChildInit(Entity child)
    {
    }

    //------------------------------------------------------------------------------------
    public override void onRpcPropSync(RpcSession session, byte from_node, ushort reason)
    {
    }

    //------------------------------------------------------------------------------------
    public override void handleEvent(object sender, EntityEvent e)
    {
    }

    //------------------------------------------------------------------------------------
    // 应用模块调用的登出接口
    public void logout(string account)
    {
        mOfflineList.Enqueue(account);
    }

    //------------------------------------------------------------------------------------
    // 设置登陆接口回调
    public void setLoginDelegate(delegateOnLogin on_login)
    {
        mFuncOnLogin = on_login;
    }

    //------------------------------------------------------------------------------------
    public IZkClient getZkClient()
    {
        PhotonApp photon_app = (PhotonApp)ApplicationBase.Instance;
        return photon_app.ZkClient;
    }

    //------------------------------------------------------------------------------------
    // servers, 当前zookeeper中的 Loginserver的所有结点.
    internal void _onLoginServerList(int result, string data, string[] servers, Dictionary<string, object> param)
    {
        List<string> remoteServer = servers.ToList();
        List<string> localServer = mLoginServer.Keys.ToList<string>();
        IEnumerable<string> add = remoteServer.Except(localServer);
        IEnumerable<string> del = localServer.Except(remoteServer);

        // 有新的Login server 结点要添加
        foreach (var ser in add)
        {
            string[] resul;
            char[] charSeparators = new char[] { ',', ':' };
            resul = ser.Split(charSeparators);
            if (resul.Length != 3)
            {
                EbLog.Error("Error format , the correct format should be 192.168.1.4:4689,000000005 , (ip:port,id) ");
                continue;
            }

            PhotonApp photon_app = (PhotonApp)ApplicationBase.Instance;
            //return photon_app.ZkClient;
            LoginServerInfo info = new LoginServerInfo();
            info.ip = resul[0];
            info.port = resul[1];
            info.id = resul[2];
            string ctlqNode = string.Format("{0}{1},{2}", mCurLoginNodePath, info.id, photon_app.NodeId);
            string ctlcqNode = string.Format("{0}{1},{2}", mCurLoginCompleteNodePath, info.id, photon_app.NodeId);
            string offlineNode = string.Format("{0}{1},{2}", mCurOfflineNodePath, info.id, photon_app.NodeId);

            getZkClient().acreate(ctlqNode, "", ZK_CONST.ZOO_EPHEMERAL, null);
            getZkClient().acreate(ctlcqNode, "", ZK_CONST.ZOO_EPHEMERAL, null);
            getZkClient().acreate(offlineNode, "", ZK_CONST.ZOO_EPHEMERAL, null);

            // 以下两个结点用来watch，create.
            string ctlqLock = string.Format("{0}{1},{2}", mCurLoginLockPath, info.id, photon_app.NodeId);
            string ctlcqLock = string.Format("{0}{1},{2}", mCurLoginCompleteLockPath, info.id, photon_app.NodeId);
            string offlineLock = string.Format("{0}{1},{2}", mCurOfflineLockPath, info.id, photon_app.NodeId);

            info.loginNode = ctlqNode;
            info.loginComNode = ctlcqNode;
            info.loginLockNode = ctlqLock;
            info.loginLockComNode = ctlcqLock;
            info.offlineLock = offlineLock;
            info.offlineNode = offlineNode;

            Dictionary<string, object> pa = new Dictionary<string, object>();
            param["LoginServerInfo"] = info;
            getZkClient().subscribeExists(info.loginLockNode, _onLoginLockChange, pa);

            mLoginServer.Add(ser, info);
        }

        // 有Login server 结点要删除.
        foreach (var ser in del)
        {
            LoginServerInfo info = null;
            mLoginServer.TryGetValue(ser, out info);
            getZkClient().adelete(info.loginNode, null);
            getZkClient().adelete(info.loginComNode, null);

            // 如果是Login server removed , loginLockNode 肯定也会随之消失.
            if (info.bloginComLock)
            {
                getZkClient().adelete(info.loginLockComNode, null);
            }
            mLoginServer.Remove(ser);
        }
    }

    //------------------------------------------------------------------------------------
    void _onGetNodeInfo(string data)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(data);
        XmlNodeList chd = doc.SelectSingleNode("ServerGroups").ChildNodes;
        if (null != chd)
        {
            foreach (XmlNode child in chd)
            {
                if (ProjectName != child.Name) continue;

                mCurLoginNodePath = child.Attributes["LoginQueue"].Value;
                mCurLoginLockPath = child.Attributes["LoginQueueLock"].Value;
                mCurLoginCompleteNodePath = child.Attributes["LoginCompleteQueue"].Value;
                mCurLoginCompleteLockPath = child.Attributes["LoginCompleteQueueLock"].Value;
                mCurOfflineNodePath = child.Attributes["PlayerOfflineNode"].Value;
                mCurOfflineLockPath = child.Attributes["PlayerOfflineLock"].Value;

                //mZonePath = child.Attributes["ZoneServer"].Value;
            }
        }
    }

    //------------------------------------------------------------------------------------
    void _onLoginLockChange(int result, string data, string[] chdn, Dictionary<string, object> param)
    {
        LoginServerInfo info = param["LoginServerInfo"] as LoginServerInfo;
        info.bloginLock = true;
        getZkClient().areadData(info.loginNode, false, _onLoginNodeData);
    }

    //------------------------------------------------------------------------------------
    // 解析login传过来的登陆数据
    void _onLoginNodeData(int result, string data, string[] chdn, Dictionary<string, object> param)
    {
        if (data == null) return;
        // 处理玩家登陆.
        // LoginNodeId
        string[] resul;
        char[] charSeparators = new char[] { ',', ':' };
        resul = data.Split(charSeparators);
        if (resul.Length != 2)
        {
            EbLog.Error("Error format , the correct format should be 000000006,000000005 -> (accountId,LoginId) ");
            return;
        }

        LoginPlayer player = new LoginPlayer();
        player.Account = resul[0];
        player.LoginNodeId = resul[1];
        player.ServerGroup = ProjectName;
        mLoginPlayer.Add(player.Account, player);
    }
}
