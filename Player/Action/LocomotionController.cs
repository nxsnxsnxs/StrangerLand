using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PathFinding;
using Tools;

namespace Player.Action
{
    //LocomotionController是最特殊的一类Action，它包含两种类型的移动
    //一、使用键盘控制的自主移动
    //这类移动开始的标志为按下任意一个移动键，结束的标志为速度为0（没有任何一个移动键被按下，或者同时按下相反方向移动键）
    //二、协程移动
    //这部分与其他action基本相同，只不过不需要actiontrigger
    //此外，协程移动不仅可以被actioncontroller调用，还可以被其他playeraction调用
    //注意当其它playeraction被终止时即使他们正在使用协程移动，也不会调用LocomotionController的interrupt
    //所以外包的协程移动不应该依赖于该脚本内的变量状态，应当是完全独立的一个函数体（协程）

    public class LocomotionController : PlayerAction
    {
        public float runSpeed;
        private Animator animator;
        private ViewController viewController;
        private AttackController attackController;
        private ActionController actionController;
        private Coroutine currentMove;
        private float defaultStopDistance;

        public override string actionName
        {
            get => "Locomotion";
        }
        public override int priority
        {
            get => 1;
        }

        void Awake()
        {
            animator = GetComponent<Animator>();
            viewController = GetComponent<ViewController>();
            attackController = GetComponent<AttackController>();
            actionController = GetComponent<ActionController>();
            defaultStopDistance = GetComponent<CapsuleCollider>().radius + 0.3f;
            enabled = false;
        }
        void FixedUpdate()
        {
            Vector3 viewEulerAngle = viewController.thirdPersonCam.transform.eulerAngles;
            Vector3 viewDirForward = viewController.thirdPersonCam.transform.forward;
            Vector3 viewDirRight = viewController.thirdPersonCam.transform.right;
            Vector3 lastPos = transform.position;
            
            float forward = 0, right = 0;
            if(Input.GetKey(KeyCode.A))
            {
                viewController.model.rotation = Quaternion.Euler(0, viewEulerAngle.y - 90, 0);
                right -= 1;
            }
            if(Input.GetKey(KeyCode.D))
            {
                viewController.model.rotation = Quaternion.Euler(0, viewEulerAngle.y + 90, 0);
                right += 1;
            }
            if(Input.GetKey(KeyCode.W))
            {
                viewController.model.rotation = Quaternion.Euler(0, viewEulerAngle.y, 0);
                forward += 1;
            }            
            if(Input.GetKey(KeyCode.S))
            {
                viewController.model.rotation = Quaternion.Euler(0, viewEulerAngle.y + 180, 0);
                forward -= 1;
            }
            Vector3 moveDir = viewDirForward * forward + viewDirRight * right;
            moveDir.y = 0;
            moveDir.Normalize();
            transform.position += moveDir * Time.deltaTime * runSpeed;
            
            if(moveDir != Vector3.zero)
            {
                animator.SetInteger("MoveState", 1);
                //正处于协程移动时进行自由移动，终止协程移动
                if(currentMove != null)
                {
                    StopCoroutine(currentMove);
                    currentMove = null;
                }
            }
            //没有任何移动键被按下，也不处于协程移动，退出
            else if(currentMove == null)
            {
                animator.SetInteger("MoveState", 0);
                finish = true;
            }
        }

        public IEnumerator MoveToPoint(Vector3 dst)
        {
            yield return MoveToPoint(dst, defaultStopDistance);
        }
        public IEnumerator MoveToPoint(Vector3 dst, float stopDistance)
        {
            List<Vector3> path = MapManager.Instance.FindPath(transform.position, dst);

            #if false
            //Debug.Log(transform.position);
            Vector3 last = transform.position;
            foreach (var item in path)
            {
                GameObject go = new GameObject();
                go.transform.position = item;
                Debug.DrawLine(last, item, Color.red, 180);
                last = item;
            }
            //Debug.Log(dst);
            #endif

            yield return AutoMove(path, stopDistance);
            finish = true;
        }

        private IEnumerator AutoMove(List<Vector3> path, float stopDistance)
        {
            foreach (var item in path)
            {
                while(!MoveToPos(item, stopDistance)) yield return null;
            }
            StopMove();
        }
        private bool MoveToPos(Vector3 dst, float stopDistance)
        {
            Vector3 direction = dst - transform.position;
            direction.y = 0;
            direction.Normalize();
            viewController.model.transform.LookAt(new Vector3(dst.x, transform.position.y, dst.z));
            if(transform.position.PlanerDistance(dst) > stopDistance)
            {
                transform.position += direction * Time.deltaTime * runSpeed;
                animator.SetInteger("MoveState", 1);
                return false;
            }
            else
            {
                animator.SetInteger("MoveState", 0);
                return true;
            } 
        }
        public void StopMove()
        {
            if(currentMove == null) return;
            StopCoroutine(currentMove);
            currentMove = null;
        }

        public override void Begin(params object[] target)
        {
            finish = false;
            //如果没有指定目的地，即开始键盘的自由移动
            if(target.Length == 0) return;
            //if(!(target is Vector3)) return;
            else currentMove = StartCoroutine(MoveToPoint((Vector3)target[0]));
        }

        public override void Interrupted()
        {
            if(currentMove != null)
            {
                StopCoroutine(currentMove);
            }
        }
    }
}
