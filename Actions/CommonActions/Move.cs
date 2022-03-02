using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Components;

namespace Actions
{
    public abstract partial class CommonAction
    {
        public class Move : BaseAction
        {
            protected virtual string moveAnimTrigger
            {
                get => "Move";
            }
            protected virtual string stopmoveAnimTrigger
            {
                get => "StopMove";
            }
            protected virtual float moveSpeed
            {
                get => locomotor.defaultMoveSpeed;
            }
            private ActionController actionController;
            private Locomotor locomotor;

            public override string actionName
            {
                get => "Move";
            }

            void Awake()
            {
                animator = GetComponent<Animator>();
                actionController = GetComponent<ActionController>();
                locomotor = GetComponent<Locomotor>();
            }
            public override void Begin(params object[] args)
            {
                if(args.Length != 1)
                {
                    finish = true;
                    return;
                }
                locomotor.moveSpeed = moveSpeed;
                if(args[0] is Vector3) locomotor.StartMove((Vector3)args[0], OnMoveFinish);
                else if(args[0] is GameObject) locomotor.StartMove(args[0] as GameObject, OnMoveFinish);
                else
                {
                    finish = true;
                    return;
                }
                animator.SetTrigger(moveAnimTrigger);
            }
            public void OnMoveFinish(bool result)
            {
                finish = true;
                animator.SetTrigger(stopmoveAnimTrigger);
            }

            public override void Interrupted()
            {
                animator.SetTrigger(stopmoveAnimTrigger);
                locomotor.StopMove();
            }
            public override void Renew(params object[] args)
            {
                if(args[0] is Vector3) locomotor.RenewMove((Vector3)args[0]);
                else locomotor.RenewMove(args[0] as GameObject);
            }
        }
    }
}