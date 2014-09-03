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
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net.Config;
using Photon.SocketServer;
using Photon.SocketServer.Diagnostics;
using Photon.SocketServer.ServerToServer;
using MySql.Data;
using MySql.Data.MySqlClient;
using Eb;
using Es;
using Zk;

public class GateBackInfo
{
    public string serverName;
    public string returnCode;
}

// 当前ZooKeeper Clinet操作的反馈.
public class ZkOnOpeResult : IZkOnOpeResult
{
    //-------------------------------------------------------------------------
    LoginApp<ComponentDef> mCoApp;

    //-------------------------------------------------------------------------
    public void onCreated(string path, string fullPath, int result)
    {
    }

    //-------------------------------------------------------------------------
    public void onDeleted(string path, int result)
    {
    }

    //-------------------------------------------------------------------------
    public void onSet(string path, int result)
    {
    }

    //-------------------------------------------------------------------------
    public void onGet(string path, int result, string data)
    {
        if (mCoApp == null)
        {
            PhotonApp photon_app = (PhotonApp)ApplicationBase.Instance;
            mCoApp = photon_app.EntityMgr.findFirstEntity("EtApp").getComponent<LoginApp<ComponentDef>>();
        }

        if (path == mCoApp.mLoginNodeInfoPath)
        {
            mCoApp.OnGetNodeInfo(data);
            return;
        }

        foreach (var ser in mCoApp.mServerGroup)
        {
            LoginNode<ComponentDef> info = ser.Value;
            foreach (var gate in info.mGateInfo)
            {
                GateInfo gt = gate.Value;
                if (path == gt.loginComNode)
                {
                    if (gt.bloginComLock == false)
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
                        string serverGroup = resul[index++];
                        string logresult = resul[index++];
                        mCoApp.onGateBack(serverGroup, account, logresult);
                    }
                    // 通知gate已经处理了数据.
                    mCoApp.getZk().sdelete(gt.loginLockComNode);
                    return;
                }

                if (path == gt.offlineNode)
                {
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
                    return;
                }
            }
        }
    }

    //-------------------------------------------------------------------------
    public void onExists(string path, int result)
    {
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
                            //App.GetZk().unsubscribeDataChanges(gtInfo.loginNode);
                            //App.GetZk().unsubscribeDataChanges(gtInfo.loginComNode);
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

                            mCoApp.getZk().subscribeExists(gtInfo.loginLockNode);
                            mCoApp.getZk().subscribeExists(gtInfo.loginLockComNode);
                            mCoApp.getZk().subscribeExists(gtInfo.offlineLock);

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
    public void handler(WatchedEvent eve)
    {
        //mLog.Info(eve.ToString());

        string path = eve.Path;
        // 服务器连上或断开.
        foreach (var ser in mCoApp.mServerGroup)
        {
            LoginNode<ComponentDef> info = ser.Value;
            for (int index = 0; index < (int)eSERVER.MAX_SERVER_TYPE_COUNT; index++)
            {
                if (path == info.ServerPath[index])
                {
                    mCoApp.getZk().awatchForChilds(path);
                    return;
                }
            }

            foreach (var gt in info.mGateInfo)
            {
                if (path == gt.Value.loginLockComNode)
                {
                    if (eve.Type == (int)ZOO_EVENT.CREATED_EVENT_DEF)
                    {
                        gt.Value.bloginComLock = true;
                        mCoApp.getZk().areadData(gt.Value.loginComNode, false);
                    }
                    return;
                }
                if (path == gt.Value.loginLockNode)
                {
                    if (eve.Type == (int)ZOO_EVENT.DELETED_EVENT_DEF)
                    {
                        gt.Value.bloginLock = false;
                    }
                    return;
                }
                if (path == gt.Value.offlineLock)
                {
                    if (eve.Type == (int)ZOO_EVENT.CREATED_EVENT_DEF)
                    {
                        gt.Value.bofflineLock = true;
                        mCoApp.getZk().areadData(gt.Value.offlineNode, false);
                    }
                    return;
                }
            }
        }
    }
}
