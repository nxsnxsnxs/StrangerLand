using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Tools;
using Components;

namespace Player.Action
{
    using Prefab;

    //ActionTrigger是动画帧事件与动作交互的唯一方式
    //动画帧通过调用ActionController的SetActionTrigger(name)来触发ActionTrigger
    //当前正在进行的动作会检测对应的trigger并进入到下一阶段（产生实质性的效果）
    public class ActionTrigger
    {
        private bool value = false;
        private string triggerName;
        public ActionTrigger(string name)
        {
            triggerName = name;
        }
        public static implicit operator bool(ActionTrigger at)
        {
            return at.value;
        }
        public void Set()
        {
            value = true;
        }
        public void Reset()
        {
            value = false;
        }
    }
    //人物的任何动作（即需要与动画交互的部分）都需要继承PlayerAction，挂载在人物上（unenable），并通过ActionController来完成动作
    //流程是这样的，由两类输入信号（input controller 和 ui input）触发调用action
    //如果成功调用，ActionController会调用Begin()开始动作，并且会将相应的Action组件enable，使其可以接收update
    //然后ActionController开始等待两类信号
    //如果更高优先级的Action被触发，将会调用当前Action的Interrupt()结束动作
    //否则会一直等待直到当前action完成（由action把finish置为true标志着action完成），然后进入空闲状态
    //动作结束后会将action disable

    //实际动作的进行（在调用begin()后）主要分为两种，一种是放在Update中，另一种是放在协程中，除了LocomotionController以外
    //几乎所有的action都是后者，update中最多只会包含一些辅助功能

    //在整个流程中有两个变量非常重要
    //finish：  请在action完成时置为true，在begin时将其置为false
    //triggers：  在awake中调用RegisterTrigger()来注册Trigger，然后在协程中用它来控制进度，
    //            请确保在动作顺利完成时和Interrupt()函数中调用ResetTrigger()来重置Trigger

    public abstract class PlayerAction : MonoBehaviour
    {
        //动作名称
        public abstract string actionName{ get; }
        //优先级  higher number means higher priority
        public abstract int priority{ get; }
        //动作顺利完成的标志
        [HideInInspector]public bool finish;
        //actiontrigger注册表
        public Dictionary<string, ActionTrigger> triggers = new Dictionary<string, ActionTrigger>();
        //动作启动函数
        public abstract void Begin(params object[] target);
        //被打断
        public abstract void Interrupted();
        //注册ActionTrigger
        protected void RegisterTrigger(string name)
        {
            triggers[name] = new ActionTrigger(name);
        }
        //重置所有ActionTrigger
        protected virtual void ResetActionTrigger()
        {
            foreach (var key in triggers.Keys)
            {
                triggers[key].Reset();
            }
        }
    }
    public class ActionController : MonoBehaviour
    {
        private Animator animator;
        private PlayerAction currentAction;

        void Init()
        {
            
        }
        void Awake()
        {
            Init();
        }
        
        void FixedUpdate()
        {
            if(currentAction == null) return;
            //Debug.Log("CurrentAction: " + currentAction.actionName);
            if(currentAction.finish)
            {
                Debug.Log("Action Finish: " + currentAction.actionName);
                currentAction.enabled = false;
                currentAction = null;
            } 
        }
        public bool DoAction<T>(params object[] target) where T : PlayerAction //Coroutine action, int priority, UnityAction interruptCallback)
        {
            T action = GetComponent<T>();
            if(currentAction != null)
            {
                //相同动作不能打断，不同动作相同优先级或更高优先级可以打断
                if(currentAction.actionName != action.actionName && currentAction.priority <= action.priority)
                {
                    currentAction.Interrupted();
                    Debug.Log("ActionInterrupted: " + currentAction.actionName + " by " + action.actionName);
                }
                else return false;
            }
            currentAction = action;
            currentAction.Begin(target);
            currentAction.enabled = true;
            Debug.Log("DoAction: " + action.actionName);
            return true;
        }
        public void StopAction()
        {
            if(currentAction == null) return;
            currentAction.Interrupted();
            currentAction = null;
        }
        public void SetActionTrigger(string triggerName)
        {
            if(triggerName == null || !currentAction || currentAction.finish || !currentAction.triggers.ContainsKey(triggerName))
            {
                Debug.LogError("action trigger set error" + triggerName + " " + currentAction);
                return;
            }
            currentAction.triggers[triggerName].Set();
        }
    }
}