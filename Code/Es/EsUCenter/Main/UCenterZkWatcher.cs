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

    //-------------------------------------------------------------------------
    public UCenterZkWatcher(IComponent co_app)
    {
        mCoApp = co_app as LoginApp<ComponentDef>;
    }

    //-------------------------------------------------------------------------
    public void onLoginNodeInfo(int result, string data, string[] servers, Dictionary<string, object> param)
    {
        mCoApp.OnGetNodeInfo(data);
    }

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
        GateInfo gt = param["gateInfo"] as GateInfo;
        string path = param["path"] as string;

        if (gt.bloginComLock == false)
        {
            EbLog.Error("Error get unlock data: " + path);
            return;
        }
        string[] resul;
        char[] charSeparators = new char[] { ',', ':' };
        resul = data.Split(charSeparators);
        int index = 0;
        while (index < resul.Length)
        {
            string account = resul[index++];
            string serverGroup = resul[index++];
            string logresult = resul[index++];
            mCoApp.onGateBack(serverGroup, account, logresult);
        }
        // 通知gate已经处理了数据.
        mCoApp.getZk().adelete(gt.loginLockComNode, null);
    }

    void _onServerNodeChange(int result, string data, string[] chdn, Dictionary<string, object> param)
    {

        LoginNode<ComponentDef> info = param["LoginNode"] as LoginNode<ComponentDef>;
        string path = param["path"] as string;
        int index = (int)param["index"];

        List<string> remoteServer = chdn.ToList();
        List<string> localServer = info.ServerInfo[index].Keys.ToList<string>();
        IEnumerable<string> add = remoteServer.Except(localServer);
        IEnumerable<string> del = localServer.Except(remoteServer);

        foreach (var sv in del)
        {
            // gate server 要取消监听.
            if (index == (int)eSERVER.GATE_SERVER)
            {
                GateInfo gtInfo;
                info.mGateInfo.TryGetValue(sv, out gtInfo);
                mCoApp.getZk().unsubscribeExists(gtInfo.loginLockNode);
                mCoApp.getZk().unsubscribeExists(gtInfo.loginLockComNode);
                mCoApp.getZk().unsubscribeExists(gtInfo.offlineNode);
                info.mGateInfo.Remove(sv);
            }
            info.ServerInfo[index].Remove(sv);
        }

        foreach (var sv in add)
        {
            string[] resul;
            char[] charSeparators = new char[] { ',', ':' };
            resul = sv.Split(charSeparators);
            if (resul.Length != 3)
            {
                EbLog.Error("PsUCenter.ZkOnOpeResult.onGetChildren path:" + path + "child:" + sv);
                EbLog.Error("Error format , the correct format should be 192.168.1.4:4689,000000005 , (ip:port,id) ");
                continue;
            }
            ServerInfo si = new ServerInfo();
            si.Ip = resul[0];
            si.Port = resul[1];
            si.Id = resul[2];
            info.ServerInfo[index].Add(sv, si);

            if (index == (int)eSERVER.GATE_SERVER)
            {
                GateInfo gtInfo = new GateInfo();
                gtInfo.id = si.Id;
                gtInfo.loginNode = info.LoginQueue + mCoApp.NodeId + "," + gtInfo.id;
                gtInfo.loginComNode = info.LoginCompleteQueue + mCoApp.NodeId + "," + gtInfo.id;
                gtInfo.loginLockNode = info.LoginQueueLock + mCoApp.NodeId + "," + gtInfo.id;
                gtInfo.loginLockComNode = info.LoginCompleteQueueLock + mCoApp.NodeId + "," + gtInfo.id;

                gtInfo.offlineNode = info.LoginOfflineQueue + mCoApp.NodeId + "," + gtInfo.id;
                gtInfo.offlineLock = info.LoginOfflineQueueLock + mCoApp.NodeId + "," + gtInfo.id;

                gtInfo.ipport = si.Ip + ":" + si.Port;

                mCoApp.getZk().subscribeExists(gtInfo.loginLockNode, null);
                mCoApp.getZk().subscribeExists(gtInfo.loginLockComNode, null);
                mCoApp.getZk().subscribeExists(gtInfo.offlineLock, null);

                info.mGateInfo.Add(sv, gtInfo);
            }
        }
    }

    //-------------------------------------------------------------------------
    public void onGetChildren(string path, int result, string[] chdn)
    {
        if (null == chdn)
        {
            chdn = new string[0];
        }

        // 服务器连上或断开.
        foreach (var ser in mCoApp.mServerGroup)
        {
            LoginNode<ComponentDef> info = ser.Value;
            for (int index = 0; index < (int)eSERVER.MAX_SERVER_TYPE_COUNT; index++)
            {
                if (path == info.ServerPath[index])
                {
                    List<string> remoteServer = chdn.ToList();
                    List<string> localServer = ser.Value.ServerInfo[index].Keys.ToList<string>();
                    IEnumerable<string> add = remoteServer.Except(localServer);
                    IEnumerable<string> del = localServer.Except(remoteServer);

                    foreach (var sv in del)
                    {
                        // gate server 要取消监听.
                        if (index == (int)eSERVER.GATE_SERVER)
                        {
                            GateInfo gtInfo;
                            info.mGateInfo.TryGetValue(sv, out gtInfo);
                            mCoApp.getZk().unsubscribeExists(gtInfo.loginLockNode);
                            mCoApp.getZk().unsubscribeExists(gtInfo.loginLockComNode);
                            mCoApp.getZk().unsubscribeExists(gtInfo.offlineNode);
                            info.mGateInfo.Remove(sv);
                        }

                        ser.Value.ServerInfo[index].Remove(sv);
                    }

                    foreach (var sv in add)
                    {
                        string[] resul;
                        char[] charSeparators = new char[] { ',', ':' };
                        resul = sv.Split(charSeparators);
                        if (resul.Length != 3)
                        {
                            EbLog.Error("PsUCenter.ZkOnOpeResult.onGetChildren path:" + path + "child:" + sv);
                            EbLog.Error("Error format , the correct format should be 192.168.1.4:4689,000000005 , (ip:port,id) ");
                            continue;
                        }
                        ServerInfo si = new ServerInfo();
                        si.Ip = resul[0];
                        si.Port = resul[1];
                        si.Id = resul[2];
                        ser.Value.ServerInfo[index].Add(sv, si);

                        if (index == (int)eSERVER.GATE_SERVER)
                        {
                            GateInfo gtInfo = new GateInfo();
                            gtInfo.id = si.Id;
                            gtInfo.loginNode = info.LoginQueue + mCoApp.NodeId + "," + gtInfo.id;
                            gtInfo.loginComNode = info.LoginCompleteQueue + mCoApp.NodeId + "," + gtInfo.id;
                            gtInfo.loginLockNode = info.LoginQueueLock + mCoApp.NodeId + "," + gtInfo.id;
                            gtInfo.loginLockComNode = info.LoginCompleteQueueLock + mCoApp.NodeId + "," + gtInfo.id;

                            gtInfo.offlineNode = info.LoginOfflineQueue + mCoApp.NodeId + "," + gtInfo.id;
                            gtInfo.offlineLock = info.LoginOfflineQueueLock + mCoApp.NodeId + "," + gtInfo.id;

                            gtInfo.ipport = si.Ip + ":" + si.Port;

                            mCoApp.getZk().subscribeExists(gtInfo.loginLockNode, null);
                            mCoApp.getZk().subscribeExists(gtInfo.loginLockComNode, null);
                            mCoApp.getZk().subscribeExists(gtInfo.offlineLock, null);

                            info.mGateInfo.Add(sv, gtInfo);
                        }
                    }
                    break;
                }
            }
        }
    }

    //-------------------------------------------------------------------------
    // Zookeeper自动的反馈.
    public bool handler(WatchedEvent eve)
    {
        string path = eve.Path;
        // 服务器连上或断开.
        foreach (var ser in mCoApp.mServerGroup)
        {
            LoginNode<ComponentDef> info = ser.Value;
            for (int index = 0; index < (int)eSERVER.MAX_SERVER_TYPE_COUNT; index++)
            {
                if (path == info.ServerPath[index])
                {
                    Dictionary<string, object> pa = new Dictionary<string, object>();
                    pa["LoginNode"] = info;
                    pa["path"] = path;
                    pa["index"] = index;
                    mCoApp.getZk().awatchForChilds(path, _onServerNodeChange, pa);
                    return true;
                }
            }

            foreach (var gt in info.mGateInfo)
            {
                if (path == gt.Value.loginLockComNode)
                {
                    if (eve.Type == (int)ZOO_EVENT.CREATED_EVENT_DEF)
                    {
                        gt.Value.bloginComLock = true;

                        Dictionary<string, object> pa = new Dictionary<string, object>();
                        pa["gateInfo"] = gt;
                        pa["path"] = path;
                        mCoApp.getZk().areadData(gt.Value.loginComNode, false, _loginComNode, pa);
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
                        mCoApp.getZk().areadData(gt.Value.offlineNode, false, _onOfflineNode, pa);
                    }
                    return true;
                }
            }
        }
        return false;
    }
}
