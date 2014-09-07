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
using Eb;
using Es;
using Zk;

// 当前ZooKeeper Clinet操作的反馈.
public class ServerUCenterZkWatcher
{
    //-------------------------------------------------------------------------
    ServerUCenter<ComponentDef> mCoUCenter;

    //-------------------------------------------------------------------------
    public ServerUCenterZkWatcher(IComponent co_ucenter)
    {
        mCoUCenter = (ServerUCenter<ComponentDef>)co_ucenter;
    }

    //-------------------------------------------------------------------------
    // Zookeeper自动的反馈.
    public bool handler(WatchedEvent eve)
    {
        string path = eve.Path;
        string lastPath = path.Substring(path.LastIndexOf('/') + 1);
        if (mCoUCenter.mLoginPath == path)
        {
            mCoUCenter.getZkClient().awatchForChilds(path, mCoUCenter._onLoginServerList);
            return true;
        }

        if ((int)ZOO_EVENT.DELETED_EVENT_DEF == eve.Type)
        {
           if(mCoUCenter.onLockNodeDel(eve.Path))
           {
               return true;
           }
        }
        
        //mCoUCenter.OnLoginNodeChange(path, (int)ZOO_EVENT.CREATED_EVENT_DEF == eve.Type, null);
        return false;
    }

    //-------------------------------------------------------------------------
    //public void onGet(string path, int result, string data)
    //{
    //    mCoUCenter.OnLoginNodeChange(path, false, data);
    //}

    //-------------------------------------------------------------------------
    //public void onGetChildren(string path, int result, string[] chdn)
    //{
    //    if (mCoUCenter.mLoginPath == path)
    //    {
    //        mCoUCenter.OnLoginServerList(chdn);
    //    }
    //}
}
