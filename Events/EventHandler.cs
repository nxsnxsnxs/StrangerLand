using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events{
    public abstract class BaseEvent{
        public abstract string name { get; }
        //每个事件产生的效果=fn+EventNode（动画）
        //fn为执行实际效果的函数，有些事件可能不会触发动画但仍要产生效果
        //比如某boss在召唤时受到伤害，则不会有受击动画，但仍应当受到实际伤害，对应的代码应放在fn中
        public abstract void fn(GameObject gameObject, params object[] args);
    }
    public class EventHandler : MonoBehaviour
    {
        public delegate void Eventfn(params object[] args);
        private Dictionary<string, Eventfn> events;

        void Awake()
        {
            events = new Dictionary<string, Eventfn>();
        }

        public void ListenEvent(string name, Eventfn fn)
        {
            events[name] = fn;
        }
        public void ListenEvent(BaseEvent e)
        {
            ListenEvent(e.name, (object[] args) => { e.fn(gameObject, args); });
        }
        public void RaiseEvent(string name, params object[] args)
        {
            if(!events.ContainsKey(name)) return;
            events[name](args);
        }
    }
}