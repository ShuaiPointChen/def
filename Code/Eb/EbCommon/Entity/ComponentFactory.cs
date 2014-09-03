using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    public abstract class IComponentFactory
    {
        //-------------------------------------------------------------------------
        protected EntityMgr mEntityMgr = null;

        //-------------------------------------------------------------------------
        public IComponentFactory(EntityMgr entity_mgr)
        {
            mEntityMgr = entity_mgr;
        }

        //-------------------------------------------------------------------------
        public abstract string getName();

        //-------------------------------------------------------------------------
        public abstract IComponent createComponent(Entity container, Dictionary<string, string> map_param);
    }

    public class ComponentFactory<T> : IComponentFactory where T : IComponent, new()
    {
        //-------------------------------------------------------------------------
        public ComponentFactory(EntityMgr entity_mgr)
            :base(entity_mgr)
        {
        }

        //-------------------------------------------------------------------------
        public override string getName()
        {
            //Type t = typeof(T);
            //System.Attribute[] attrs = System.Attribute.GetCustomAttributes(t);
            //foreach (System.Attribute attr in attrs)
            //{
            //    if (attr is ComponentName)
            //    {
            //        ComponentName a = (ComponentName)attr;
            //        //System.Console.WriteLine("{0}, version {1:f}", a.getName(), a.version);
            //        break;
            //    }
            //}

            //Type tb = t.GetGenericTypeDefinition();
            //EbLog.Note("GetGenericTypeDefinition Name=" + tb.Name);
            ////return typeof(T).Name;
            //return tb.Name;

            return mEntityMgr.getComponentName<T>();
        }

        //-------------------------------------------------------------------------
        public override IComponent createComponent(Entity container, Dictionary<string, string> map_param)
        {
            T component = new T();
            component.Entity = container;
            component.EntityDef = container._getEntityDef();
            component.EntityMgr = mEntityMgr;
            component.EnableUpdate = true;
            component._genDef(map_param);
            return component;
        }
    }
}
