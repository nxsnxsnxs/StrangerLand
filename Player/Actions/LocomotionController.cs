using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Tools;
using Actions;
using Components;

namespace Player.Actions
{
    //LocomotionController是最特殊的一类Action，它包含两种类型的移动
    //一、使用键盘控制的自主移动
    //这类移动开始的标志为按下任意一个移动键，结束的标志为速度为0（没有任何一个移动键被按下，或者同时按下相反方向移动键）
    //二、协程移动
    //这部分与其他action基本相同，只不过不需要actiontrigger
    //此外，协程移动不仅可以被actioncontroller调用，还可以被其他playeraction调用
    //注意当其它playeraction被终止时即使他们正在使用协程移动，也不会调用LocomotionController的interrupt
    //所以外包的协程移动不应该依赖于该脚本内的变量状态，应当是完全独立的一个函数体（协程）

    public class LocomotionController : BaseAction
    {
        private ViewController viewController;

        public override string actionName
        {
            get => "Locomotion";
        }

        void Awake()
        {
            animator = GetComponent<Animator>();
            viewController = GetComponent<ViewController>();
            enabled = false;
        }
        void FixedUpdate()
        {
            Vector3 viewEulerAngle = viewController.thirdPersonCam.transform.eulerAngles;
            Vector3 viewDirForward = viewController.thirdPersonCam.transform.forward;
            Vector3 viewDirRight = viewController.thirdPersonCam.transform.right;
            Vector3 lastPos = transform.position;
            
            float forward = 0, right = 0;
            bool keyboardInput = false;
            if(Input.GetKey(KeyCode.A))
            {
                viewController.model.rotation = Quaternion.Euler(0, viewEulerAngle.y - 90, 0);
                right -= 1;
                keyboardInput = true;
            }
            if(Input.GetKey(KeyCode.D))
            {
                viewController.model.rotation = Quaternion.Euler(0, viewEulerAngle.y + 90, 0);
                right += 1;
                keyboardInput = true;
            }
            if(Input.GetKey(KeyCode.W))
            {
                viewController.model.rotation = Quaternion.Euler(0, viewEulerAngle.y, 0);
                forward += 1;
                keyboardInput = true;
            }            
            if(Input.GetKey(KeyCode.S))
            {
                viewController.model.rotation = Quaternion.Euler(0, viewEulerAngle.y + 180, 0);
                forward -= 1;
                keyboardInput = true;
            }
            Vector3 moveDir = viewDirForward * forward + viewDirRight * right;
            moveDir.y = 0;
            moveDir.Normalize();
            GetComponent<Locomotor>().Move(moveDir);
            //没有任何移动键被按下，也不处于协程移动，退出
            if(!keyboardInput)
            {
                if(!GetComponent<Locomotor>().inMove)
                {
                    animator.SetInteger("MoveState", 0);
                    finish = true;
                }
                return;
            }
            //正处于协程移动时进行自由移动，终止协程移动
            if(GetComponent<Locomotor>().inMove)
            {
                GetComponent<Locomotor>().StopMove();
            }
            if(moveDir == Vector3.zero) animator.SetInteger("MoveState", 0);
            else animator.SetInteger("MoveState", 1);
        }
        public void DoMove(Vector3 dst)
        {
            Locomotor locomotor = GetComponent<Locomotor>();
            animator.SetInteger("MoveState", 1);
            locomotor.StartMove(dst, OnMoveFinish);
        }
        private void OnMoveFinish(bool result)
        {
            animator.SetInteger("MoveState", 0);
            finish = true;
        }

        public override void Begin(params object[] args)
        {
            finish = false;
            if(args.Length > 1 || (args.Length == 1 && !(args[0] is Vector3)))
            {
                Debug.LogError("Action Begin Param Error in " + this.GetType().ToString());
                finish = true;
                return;
            }
            //如果没有指定目的地，即开始键盘的自由移动
            if(args.Length == 0) return;
            else DoMove((Vector3)args[0]);
        }
        public override void Renew(params object[] args)
        {
            if(args.Length != 1 || !(args[0] is Vector3))
            {
                Debug.LogError("Action Renew Param Error in " + this.GetType().ToString());
                return;
            }
            if(GetComponent<Locomotor>().inMove) GetComponent<Locomotor>().RenewMove((Vector3)args[0]);
            else GetComponent<Locomotor>().StartMove((Vector3)args[0], OnMoveFinish);
        }

        public override void Interrupted()
        {
            if(GetComponent<Locomotor>().inMove) GetComponent<Locomotor>().StopMove();
            animator.SetInteger("MoveState", 0);
        }
    }
}