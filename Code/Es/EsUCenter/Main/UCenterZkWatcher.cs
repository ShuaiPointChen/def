using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Eb;
using Es;
using Zk;


// 当前ZooKeeper Clinet操作的反馈.
public class UCenterZkWatcher
{
    //-------------------------------------------------------------------------
    LoginApp<ComponentDef> mCoApp;

    public UCenterZkWatcher(IComponent co_app)
    {
        mCoApp = co_app as LoginApp<ComponentDef>;
    }


   // public void onLoginNodeInfo(int result, string data, string[] servers, Dictionary<string, object> param)
   //{
   //    mCoApp.OnGetNodeInfo(data);
   //}

    //-------------------------------------------------------------------------
    void _onOfflineNode(int result, string data, string[] servers, Dictionary<string, object> param)
    {
        if (result != 0 || data == null) return; 
        GateInfo gt = param["gateInfo"] as GateInfo;
        string path = param["path"] as string;
        LoginNode<ComponentDef> info = param["LoginNode"] as LoginNode<ComponentDef>;

        if (gt.bofflineLock == false)
        {
            EbLog.Error("Error get unlock data: " + path);
        }
        string[] resul;
        char[] charSeparators = new char[] { ',', ':' };
        resul = data.Split(charSeparators);
        int index = 0;
        while (index < resul.Length)
        {
            string account = resul[index++];
            info.onPlayerOffline(account);
        }
        // 通知gate已经处理了数据.
        mCoApp.getZk().sdelete(gt.offlineLock);
    }

    void _loginComNode(int result, string data, string[] servers, Dictionary<string, object> param)
    {
        _tLoginResponseInfo lgRepInfo = EbJsonHelper.deserialize<_tLoginResponseInfo>(data);

        GateInfo gt = param["gateInfo"] as GateInfo;
        string path = param["path"] as string;

        if (gt.bloginComLock == false)
        {
            EbLog.Error("Error get unlock data: " + path);
            return;
        }
        mCoApp.onGateBack(lgRepInfo.server_group, lgRepInfo.acc, lgRepInfo.result, lgRepInfo.map_userdata);
        // 通知gate已经处理了数据.
        mCoApp.getZk().adelete(gt.loginLockComNode, null);
    }

    public void _onServerNodeChange(int result, string data, string[] chdn, Dictionary<string, object> param)
    {
        if (result != 0) return;

        if (null == chdn)
        {
            chdn = new string[0];
        }

        LoginNode<ComponentDef> info = param["LoginNode"] as LoginNode<ComponentDef>;
        string path = param["path"] as string ;

        List<string> remoteServer = chdn.ToList();
        List<string> localServer = info.mGateInfo.Keys.ToList<string>();
        IEnumerable<string> add = remoteServer.Except(localServer);
        IEnumerable<string> del = localServer.Except(remoteServer);

        foreach (var sv in del)
        {
            // gate server 要取消监听.
            GateInfo gtInfo;
            info.mGateInfo.TryGetValue(sv, out gtInfo);
            mCoApp.getZk().unsubscribeExists(gtInfo.loginLockNode);
            mCoApp.getZk().unsubscribeExists(gtInfo.loginLockComNode);
            mCoApp.getZk().unsubscribeExists(gtInfo.offlineLock);
            info.mGateInfo.Remove(sv);
        }

        foreach (var sv in add)
        {
            string[] resul;
            char[] charSeparators = new char[] { ',', ':' };
            resul = sv.Split(charSeparators);
            if (resul.Length != 3)
            {
                EbLog.Error("PsUCenter.UCenterZkWatcher._onServerNodeChange path:" + path + "child:" + sv);
                EbLog.Error("Error format , the correct format should be 192.168.1.4:4689,000000005 , (ip:port,id) ");
                continue;
            }

            string Ip = resul[0]; 
            string Port = resul[1];
            string Id = resul[2];

            string nodeIdString = mCoApp.NodeIdStr;

            GateInfo gtInfo = new GateInfo();
            gtInfo.id = Id;
            // 生成登陆监听节点.
            gtInfo.loginNode = info.LoginQueue + nodeIdString + "," + gtInfo.id;
            gtInfo.loginComNode = info.LoginCompleteQueue + nodeIdString + "," + gtInfo.id;
            gtInfo.loginLockNode = info.LoginQueueLock + nodeIdString + "," + gtInfo.id;
            gtInfo.loginLockComNode = info.LoginCompleteQueueLock + nodeIdString + "," + gtInfo.id;
            gtInfo.offlineNode = info.LoginOfflineQueue + nodeIdString + "," + gtInfo.id;
            gtInfo.offlineLock = info.LoginOfflineQueueLock + nodeIdString + "," + gtInfo.id;
            gtInfo.ipport = Ip + ":" + Port;

            // 创建节点.
            mCoApp.getZk().acreate(gtInfo.loginNode, "",ZK_CONST.ZOO_EPHEMERAL , null);
            mCoApp.getZk().acreate(gtInfo.loginComNode, "",ZK_CONST.ZOO_EPHEMERAL ,null);
            mCoApp.getZk().acreate(gtInfo.offlineNode,"",ZK_CONST.ZOO_EPHEMERAL , null);

            // 监听节点的变化.
            mCoApp.getZk().subscribeExists(gtInfo.loginLockNode, null);
            mCoApp.getZk().subscribeExists(gtInfo.loginLockComNode, null);
            mCoApp.getZk().subscribeExists(gtInfo.offlineLock, null);

            info.mGateInfo.Add(sv, gtInfo);
        }
    }

    //-------------------------------------------------------------------------
    public bool handler(WatchedEvent eve)
    {
        string path = eve.Path;

        if(mCoApp.mServersPath == path)
        {
            mCoApp.getZk().awatchForChilds(path, mCoApp.onServerGroupChange);
            return true;
        }

        // 服务器连上或断开.
        foreach (var ser in mCoApp.mServerGroup)
        {
            LoginNode<ComponentDef> info = ser.Value;

            if (path == info.LoginNodePath)
            {
                Dictionary<string, object> pa = new Dictionary<string, object>();
                pa["LoginNode"] = info;
                pa["path"] = path;
                mCoApp.getZk().awatchForChilds(path, _onServerNodeChange, pa);
                return true;
            }

            foreach (var gt in info.mGateInfo)
            {
                if (path == gt.Value.loginLockComNode)
                {
                    if (eve.Type == (int)ZOO_EVENT.CREATED_EVENT_DEF)
                    {
                        gt.Value.bloginComLock = true;

                        Dictionary<string, object> pa = new Dictionary<string, object>();
                        pa["gateInfo"] = gt.Value;
                        pa["path"] = path;
                        mCoApp.getZk().areadData(gt.Value.loginComNode, false, _loginComNode , pa);
                    }
                    return true;
                }
                if (path == gt.Value.loginLockNode)
                {
                    if (eve.Type == (int)ZOO_EVENT.DELETED_EVENT_DEF)
                    {
                        gt.Value.bloginLock = false;
                    }
                    return true;
                }
                if (path == gt.Value.offlineLock)
                {
                    if (eve.Type == (int)ZOO_EVENT.CREATED_EVENT_DEF)
                    {
                        gt.Value.bofflineLock = true;
                        Dictionary<string, object> pa = new Dictionary<string, object>();
                        pa["gateInfo"] = gt;
                        pa["LoginNode"] = info;
                        pa["path"] = path;
                        mCoApp.getZk().areadData(gt.Value.offlineNode, false, _onOfflineNode , pa );
                    }
                    return true;
                }
            }
        }
        return false;
    }
}
