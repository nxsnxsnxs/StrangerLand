using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Brains;
namespace MyBehaviourTree
{
    //ActionNode的Reset中应当关注于自己Node内变量的Reset
    //不需要关心Component的Reset，Component的Reset由ActionController的Interrupt负责
    public abstract class ActionNode : BehaviourTreeNode
    {
        public delegate bool BehaviourFn();
        public GameObject gameObject;
        public string name;
        public ActionNode(GameObject _gameObject, string _name)
        {
            gameObject = _gameObject;
            name = _name;
            gameObject.GetComponent<Brain>().AddLeafs(this);
        }
    }
}