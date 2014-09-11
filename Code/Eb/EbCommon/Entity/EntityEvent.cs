using System;
using System.Collections.Generic;
using System.Text;

namespace Eb
{
    public delegate void EntityEventHandler(object sender, EntityEvent e);

    public class EntityEvent : EventArgs
    {
        //---------------------------------------------------------------------
        public EntityEvent() : base() { }

        //---------------------------------------------------------------------
        internal EntityEventPublisher Publisher { get; set; }

        //---------------------------------------------------------------------
        public void send(object sender)
        {
            Publisher._publish(sender, this);
        }
    }

    public class EntityEventPublisher
    {
        //---------------------------------------------------------------------
        public event EntityEventHandler Handler;
        EntityMgr mEntityMgr;
        string mPublisherName = "";

        //---------------------------------------------------------------------
        public EntityEventPublisher(EntityMgr entity_mgr)
        {
            mEntityMgr = entity_mgr;
            mPublisherName = GetType().Name;
        }

        //-------------------------------------------------------------------------
        public void addHandler(Entity entity)
        {
            if (!entity._existEvPublisher(mPublisherName))
            {
                Handler += entity._handleEvent;
                entity._addEvPublisher(mPublisherName);
            }
        }

        //-------------------------------------------------------------------------
        public void removeHandler(Entity entity)
        {
            if (entity._existEvPublisher(mPublisherName))
            {
                Handler -= entity._handleEvent;
                entity._removeEvPublisher(mPublisherName);
            }
        }

        //---------------------------------------------------------------------
        public T genEvent<T>() where T : EntityEvent, new()
        {
            T ev = mEntityMgr._genEvent<T>();
            ev.Publisher = this;
            return ev;
        }

        //---------------------------------------------------------------------
        internal void _publish(object sender, EntityEvent e)
        {
            if (Handler != null)
            {
                Handler(sender, e);
            }

            e.Publisher = null;
            mEntityMgr._freeEvent(e);
        }
    }

    internal class EntityEventMgr
    {
        //---------------------------------------------------------------------
        EntityMgr mEntityMgr;
        Dictionary<string, Queue<EntityEvent>> mMapEventPool = new Dictionary<string, Queue<EntityEvent>>();

        //---------------------------------------------------------------------
        internal EntityEventMgr(EntityMgr entity_mgr)
        {
            mEntityMgr = entity_mgr;
        }

        //---------------------------------------------------------------------
        internal T _genEvent<T>() where T : EntityEvent, new()
        {
            string ev_type = typeof(T).Name;
            T ev = null;

            if (mMapEventPool.ContainsKey(ev_type))
            {
                Queue<EntityEvent> que_ev = mMapEventPool[ev_type];
                if (que_ev.Count > 0)
                {
                    ev = (T)que_ev.Dequeue();
                    return ev;
                }
            }

            ev = new T();
            return ev;
        }

        //---------------------------------------------------------------------
        internal void _freeEvent(EntityEvent ev)
        {
            ev.Publisher = null;

            string ev_type = ev.GetType().Name;
            if (mMapEventPool.ContainsKey(ev_type))
            {
                Queue<EntityEvent> que_ev = mMapEventPool[ev_type];
                que_ev.Enqueue(ev);
            }
            else
            {
                Queue<EntityEvent> que_ev = new Queue<EntityEvent>();
                que_ev.Enqueue(ev);
                mMapEventPool[ev_type] = que_ev;
            }
        }
    }
}
