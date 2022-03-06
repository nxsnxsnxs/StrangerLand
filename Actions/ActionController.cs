using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Tools;
using Components;

namespace Actions
{
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
    //任何生物的任何动作（即需要与动画交互的部分）都需要继承BaseAction，挂载在物体上（unenable），并通过ActionController来完成动作
    //流程是这样的，由决策层（人物为input controller和ui input，其它生物为brain）和事件处理（EventHanler）触发调用doaction
    //如果成功调用，ActionController会调用Begin()开始动作，并且会将相应的Action组件enable，使其可以接收update
    //然后ActionController开始等待两类信号
    //如果有新的更高优先级的Action被触发，将会调用当前Action的Interrupt()结束动作并触发新动作
    //否则会一直等待直到当前action完成（由action把finish置为true标志着action完成），然后进入空闲状态
    //动作结束后会将action disable

    //实际动作的进行（在调用begin()后）主要分为两种，一种是放在Update中，另一种是放在协程中，除了LocomotionController以外
    //几乎所有的action都是后者，update中最多只会包含一些辅助功能

    //在整个流程中有两个变量非常重要
    //finish：  标志着动作顺利完成
    //triggers：  在awake中调用RegisterTrigger()来注册Trigger，然后在协程中用它来控制进度
    //此外应当强调的是ActionController仅用于动画的控制和调用组件完成实际工作
    //action内部不应当包含任何完成实际动作的代码

    public abstract class BaseAction : MonoBehaviour
    {
        //动作名称
        public abstract string actionName{ get; }
        //动作顺利完成的标志
        [HideInInspector]public bool finish;
        //actiontrigger注册表
        public Dictionary<string, ActionTrigger> triggers = new Dictionary<string, ActionTrigger>();
        protected Animator animator => GetComponent<Animator>();
        //动作启动函数
        public abstract void Begin(params object[] args);
        //被打断
        public abstract void Interrupted();
        //相同动作的更新(一般是参数更新，无需更改动画的情况)
        //注意不应当重载这个函数来使得同类动作不相互替换（这是决策层该做出的判断）
        //而是应该确定有需要更新的内容但不希望执行interrupt+begin重新开始(动画会不连贯甚至代码会有bug)时通过重载此函数达到目的
        //注意只要动作相同就会调用该Renew函数，也就是说可能是在两个不同决策下的相同动作的Renew，
        //所以对于无参动作也应当重新启动
        public virtual void Renew(params object[] args)
        {
            Interrupted();
            Begin(args);
        }
        //注册ActionTrigger
        protected void RegisterTrigger(string name)
        {
            triggers[name] = new ActionTrigger(name);
        }
        //重置所有ActionTrigger
        public virtual void ResetActionTrigger()
        {
            foreach (var key in triggers.Keys)
            {
                triggers[key].Reset();
            }
        }
    }
    public class ActionController : MonoBehaviour
    {
        public delegate void ActionBeginCallback();
        public delegate void ActionFinishCallback(bool finish);
        
        public bool debug;
        //长期监听回调
        public ActionBeginCallback onActionBegin;
        public ActionFinishCallback onActionFinish;
        private BaseAction currentAction;
        //对当前动作的监听
        private ActionFinishCallback onCurrentActionFinish;

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
                if(debug) Debug.Log("Action Finish: " + currentAction.actionName);
                currentAction.ResetActionTrigger();
                currentAction.enabled = false;
                currentAction = null;
                if(onCurrentActionFinish != null)
                {
                    onCurrentActionFinish(true);
                    onCurrentActionFinish = null;
                }
                if(onActionFinish != null) onActionFinish(true);
            } 
        }
        public bool DoAction<T>(params object[] args) where T : BaseAction
        {
            T action = GetComponent<T>();
            if(action == null || action.GetType() != typeof(T)) action = gameObject.AddComponent<T>();
            if(currentAction != null)
            {
                if(currentAction.GetType() == typeof(T))
                {
                    currentAction.Renew(args);
                    if(debug) Debug.Log("ActionRenew: " + currentAction.actionName);
                    return true;
                }
                currentAction.enabled = false;
                currentAction.Interrupted();
                currentAction.ResetActionTrigger();
                if(onCurrentActionFinish != null)
                {
                    onCurrentActionFinish(false);
                    onCurrentActionFinish = null;
                }
                if(onActionFinish != null) onActionFinish(false);
                if(debug) Debug.Log("ActionInterrupted: " + currentAction.actionName + " by " + action.actionName);
            }
            currentAction = action;
            currentAction.finish = false;
            currentAction.Begin(args);
            currentAction.enabled = true;
            if(onActionBegin != null) onActionBegin();
            if(debug) Debug.Log("DoAction: " + action.actionName);
            return true;
        }
        public void StopAction(Type type = null)
        {
            if(currentAction == null) return;
            if(type != null && type != currentAction.GetType()) return;
            currentAction.Interrupted();
            currentAction = null;
        }
        public void SetActionTrigger(string triggerName)
        {
            if(triggerName == null || !currentAction || currentAction.finish || !currentAction.triggers.ContainsKey(triggerName))
            {
                Debug.LogError("action trigger set error " + triggerName + " " + currentAction);
                return;
            }
            currentAction.triggers[triggerName].Set();
        }
        public bool isDoing<T>()
        {
            if(currentAction == null) return false;
            return typeof(T) == currentAction.GetType();
        }
        public void SetCallback(ActionFinishCallback callback)
        {
            if(currentAction == null) return;
            onCurrentActionFinish = callback;
        }
    }
}