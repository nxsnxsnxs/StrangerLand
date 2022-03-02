using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class BehaviourTree
    {
        public BehaviourTreeNode root;
        public GameObject gameObject;
        public float interval;
        private float sleepTime = 0;
        private Dictionary<string, object> blackboard;
        //debug
        private string runningNode;
        public BehaviourTree(GameObject _go, BehaviourTreeNode _root, float _interval)
        {
            root = _root;
            root.parent = null;
            gameObject = _go;
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
            root.Tick();
            root.Step();
        }
        public void ForceTick()
        {
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