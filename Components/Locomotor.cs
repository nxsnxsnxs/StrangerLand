using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Tools;

namespace Components
{
    public class Locomotor : GameComponent
    {
        public float walkSpeed;
        public float dashSpeed;
        private Animator animator;
        private Vector3 currentDst;
        [HideInInspector]public Coroutine currentMove;
        [HideInInspector]public UnityAction<bool> moveDoneCallback;
        [HideInInspector]public UnityEvent moveInterruption;
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            animator = GetComponent<Animator>();
        }
        /// <summary>
        /// 开启协程移动至某点
        /// </summary>
        /// <param name="dst"></param>终点
        public void MoveToPoint(Vector3 dst, bool dash)
        {
            if(currentMove != null && Vector3.Distance(dst, currentDst) < 0.8f) return;
            else if(currentMove != null)
            {
                StopCoroutine(currentMove);
                currentMove = null;
            }
            Debug.Log("findPath");
            List<Vector3> path = MapManager.Instance.FindPath(transform.position, dst);
            Debug.Log("end");
            #if false
            Debug.Log(transform.position);
            Vector3 last = transform.position;
            foreach (var item in path)
            {
                GameObject go = new GameObject();
                go.transform.position = item;
                Debug.DrawLine(last, item, Color.red, 180);
                last = item;
            }
            Debug.Log(dst);
            #endif

            currentMove = StartCoroutine(AutoMove(path, dash));
        }
        private IEnumerator AutoMove(List<Vector3> path, bool dash)
        {
            foreach (var item in path)
            {
                while(!MoveToPos(item, dash)) yield return null;
            }
            currentMove = null;
            moveDoneCallback?.Invoke(true);
        }
        private bool MoveToPos(Vector3 dst, bool dash)
        {
            Vector3 direction = dst - transform.position;
            direction.y = 0;
            direction.Normalize();
            transform.LookAt(new Vector3(dst.x, transform.position.y, dst.z));
            if(transform.position.PlanerDistance(dst) > 0.05f)
            {
                float speed = dash ? dashSpeed : walkSpeed;
                transform.position += direction * Time.deltaTime * speed;
                animator.SetInteger("MoveState", dash ? 2 : 1);
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
            if(currentMove != null)
            {
                moveInterruption?.Invoke();
                moveDoneCallback?.Invoke(false);
                StopCoroutine(currentMove);
                currentMove = null;
                animator.SetInteger("MoveState", 0);
            }
        }
    }
}