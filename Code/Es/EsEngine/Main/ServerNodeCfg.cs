using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Eb;

namespace Es
{
    public struct _tServerNode
    {
        public byte servernode_type;
        public int id;
        public string ip;
        public short port;
        public string app_name;
        public List<int> vec_before;
        public List<int> vec_after;
    }

    public class ServerNodeCfg
    {
        //---------------------------------------------------------------------
        Dictionary<int, _tServerNode> mMapServerNode = new Dictionary<int, _tServerNode>();

        //---------------------------------------------------------------------
        public void load(string cfg_file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(cfg_file);

            XmlNodeList node_list = doc.GetElementsByTagName("ServerNode");
            foreach (var i in node_list)
            {
                _tServerNode server_node = new _tServerNode();

                XmlElement el = (XmlElement)i;
                string servernode_type = el.GetAttribute("Type");
                server_node.servernode_type = byte.Parse(servernode_type);
                string app_name = el.GetAttribute("Name");
                server_node.app_name = app_name;
                string id = el.GetAttribute("Id");
                server_node.id = int.Parse(id);
                string ip = el.GetAttribute("Ip");
                server_node.ip = ip;
                string port = el.GetAttribute("Port");
                server_node.port = short.Parse(port);

                server_node.vec_before = new List<int>();
                server_node.vec_after = new List<int>();
                {
                    XmlElement el_before = el["Before"];
                    string str_idlist = el_before.GetAttribute("IdList");
                    string[] list_str = str_idlist.Split(';');

                    foreach (var str in list_str)
                    {
                        if (str.Length > 0) server_node.vec_before.Add(int.Parse(str));
                    }
                }

                {
                    XmlElement el_after = el["After"];
                    string str_idlist = el_after.GetAttribute("IdList");
                    string[] list_str = str_idlist.Split(';');

                    foreach (var str in list_str)
                    {
                        if (str.Length > 0) server_node.vec_after.Add(int.Parse(str));
                    }
                }

                mMapServerNode[server_node.id] = server_node;
            }
        }

        //---------------------------------------------------------------------
        public _tServerNode getServerNode(int id)
        {
            return mMapServerNode[id];
        }
    }
}