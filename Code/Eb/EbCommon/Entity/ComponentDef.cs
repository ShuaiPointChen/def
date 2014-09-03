using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Eb
{
    public class ComponentDef
    {
        //---------------------------------------------------------------------
        // key=prop_name
        private Dictionary<string, PropDef> mMapPropDef = new Dictionary<string, PropDef>();
        // key=prop_name
        private Dictionary<string, IProp> mMapProp = new Dictionary<string, IProp>();

        //---------------------------------------------------------------------
        public virtual ushort getComponentId()
        {
            return 0;
        }

        //---------------------------------------------------------------------
        public virtual void defAllProp(Dictionary<string, string> map_param)
        {
        }

        //---------------------------------------------------------------------
        public bool hasPropDef(string prop_name)
        {
            return mMapPropDef.ContainsKey(prop_name);
        }

        //---------------------------------------------------------------------
        public PropDef getPropDef(string prop_name)
        {
            if (!mMapPropDef.ContainsKey(prop_name)) return null;
            else return mMapPropDef[prop_name];
        }

        //---------------------------------------------------------------------
        public bool hasProp(string prop_name)
        {
            return mMapProp.ContainsKey(prop_name);
        }

        //---------------------------------------------------------------------
        public IProp getProp(string prop_name)
        {
            if (!mMapProp.ContainsKey(prop_name)) return null;
            else return mMapProp[prop_name];
        }

        //---------------------------------------------------------------------
        public Prop<T> defProp<T>(Dictionary<string, string> map_param, string key, T default_value)
        {
            PropDef prop_def = new PropDef(key, typeof(T));
            mMapPropDef[prop_def.getKey()] = prop_def;
            Prop<T> prop = new Prop<T>(prop_def, default_value);
            mMapProp[prop_def.getKey()] = prop;

            if (map_param != null && map_param.ContainsKey(prop_def.getKey()))
            {
                string json = map_param[prop_def.getKey()];
                if (!string.IsNullOrEmpty(json.Trim()))
                {
                    prop.set(EbJsonHelper.deserialize<T>(json));
                }
            }

            return prop;
        }

        //---------------------------------------------------------------------
        public Dictionary<string, IProp> getMapProp()
        {
            return mMapProp;
        }

        //---------------------------------------------------------------------
        public Dictionary<string, string> getMapProp4SaveDb(byte current_nodetype)
        {
            if (mMapProp.Count == 0) return null;

            Dictionary<string, string> map_ret = new Dictionary<string, string>();

            FieldInfo[] list_fieldinfo = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var fi in list_fieldinfo)
            {
                if (!fi.FieldType.BaseType.Equals(typeof(IProp))) continue;

                Attribute[] attrs = System.Attribute.GetCustomAttributes(fi);
                foreach (Attribute attr in attrs)
                {
                    if (!(attr is PropAttrDistribution)) continue;

                    PropAttrDistribution a = (PropAttrDistribution)attr;
                    if (a.NodePrimary == current_nodetype && a.Save2Db)
                    {
                        IProp p = (IProp)fi.GetValue(this);
                        map_ret[p.getKey()] = EbJsonHelper.serialize(p.getValue());
                    }
                    break;
                }
            }

            return map_ret;
        }

        //---------------------------------------------------------------------
        public Dictionary<string, string> getMapProp4NetSync(byte current_nodetype, byte to_nodetype)
        {
            if (mMapProp.Count == 0) return null;

            Dictionary<string, string> map_ret = new Dictionary<string, string>();

            FieldInfo[] list_fieldinfo = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var fi in list_fieldinfo)
            {
                if (!fi.FieldType.BaseType.Equals(typeof(IProp))) continue;

                Attribute[] attrs = System.Attribute.GetCustomAttributes(fi);
                foreach (Attribute attr in attrs)
                {
                    if (!(attr is PropAttrDistribution)) continue;

                    PropAttrDistribution a = (PropAttrDistribution)attr;
                    if (a.NodePrimary == current_nodetype && a.NodeDistribution != null)
                    {
                        foreach (var i in a.NodeDistribution)
                        {
                            if (i == to_nodetype)
                            {
                                IProp p = (IProp)fi.GetValue(this);
                                map_ret[p.getKey()] = EbJsonHelper.serialize(p.getValue());
                                break;
                            }
                        }
                    }
                    break;
                }
            }

            return map_ret;
        }

        //---------------------------------------------------------------------
        public Dictionary<string, string> getMapProp4All()
        {
            Dictionary<string, string> map_ret = new Dictionary<string, string>();
            foreach (var prop in mMapProp)
            {
                map_ret[prop.Key] = EbJsonHelper.serialize(prop.Value.getValue());
            }
            return map_ret;
        }
    }
}