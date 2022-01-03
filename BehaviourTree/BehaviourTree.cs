using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class BehaviourTree
    {
        public BehaviourTreeNode root;
        public GameObject go;
        public float interval;
        public string debugStr;
        private float lastTick = 0;
        private float sleepTime = 0;
        private Dictionary<string, object> blackboard;
        public BehaviourTree(GameObject _go, float _interval)
        {
            go = _go;
            interval = _interval;
            blackboard = new Dictionary<string, object>();
        }
        public BehaviourTree(GameObject _go, BehaviourTreeNode _root, float _interval)
        {
            go = _go;
            root = _root;
            interval = _interval;
            blackboard = new Dictionary<string, object>();
        }
        public object TryGetValue(string key)
        {
            if(blackboard.ContainsKey(key)) return blackboard[key];
            return null;
        }
        public void SetValue(string key, object value)
        {
            blackboard[key] = value;
        }
        public void RemoveValue(string key)
        {
            if(blackboard.ContainsKey(key)) blackboard.Remove(key);
        }
        public void Tick()
        {
            if(root == null) return;
            if(sleepTime > 0)
            {
                sleepTime -= Time.deltaTime;
                return;
            }
            if(lastTick + interval > Time.time) return;
            lastTick = Time.time;
            root.Tick();
            root.Step();
            
            Debug.Log(debugStr);
            debugStr = "";
        }
        public void ForceTick()
        {
            lastTick = Time.time;
            root.Tick();
            root.Step();
        }
        public void Sleep(float time)
        {
            root.Reset();
            sleepTime = time;
        }
    }
}