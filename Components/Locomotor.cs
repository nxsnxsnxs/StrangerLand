using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Tools;
using Player;

namespace Components
{
    public class Locomotor : GameComponent
    {
        //********need set************
        public float defaultMoveSpeed;
        //****************************
        public float moveSpeed;
        public bool debug;
        private float collSize;
        [HideInInspector]private Coroutine currentMove;
        [HideInInspector]private UnityAction<bool> moveDoneCallback;

        public bool inMove
        {
            get
            {
                return currentMove != null;
            }
        }

        void Awake()
        {
            Collider coll = GetComponent<Collider>();
            if(coll) collSize = Mathf.Max(coll.bounds.size.x, coll.bounds.size.z) / 2;
        }
        public void StartMove(Vector3 dst, UnityAction<bool> doneCallback = null)
        {
            if(currentMove != null) StopMove();
            moveDoneCallback = doneCallback;
            currentMove = StartCoroutine(MoveToPoint(dst));
        }
        public void StartMove(GameObject target, UnityAction<bool> doneCallback = null)
        {
            if(currentMove != null) StopMove();
            moveDoneCallback = doneCallback;
            currentMove = StartCoroutine(MoveToTarget(target));
        }
        public void RenewMove(Vector3 dst, UnityAction<bool> doneCallback = null)
        {
            if(doneCallback != null) moveDoneCallback = doneCallback;
            StopCoroutine(currentMove);
            currentMove = StartCoroutine(MoveToPoint(dst));
        }
        public void RenewMove(GameObject target, UnityAction<bool> doneCallback = null)
        {
            if(doneCallback != null) moveDoneCallback = doneCallback;
            StopCoroutine(currentMove);
            currentMove = StartCoroutine(MoveToTarget(target));
        }
        public void Move(Vector3 dir)
        {
            transform.position += dir * Time.deltaTime * moveSpeed;
        }
        /// <summary>
        /// 开启协程移动至某点
        /// </summary>
        /// <param name="dst"></param>终点
        private IEnumerator MoveToPoint(Vector3 dst)
        {
            List<Vector3> path = MapManager.Instance.FindPath(transform.position, dst, collSize);

            if(debug)
            {
                Vector3 last = transform.position;
                foreach (var item in path)
                {
                    GameObject go = new GameObject();
                    go.transform.position = item;
                    Vector3 start = last;
                    Vector3 end = item;
                    start.y = end.y = 0.02f;
                    Debug.DrawLine(start, end, Color.blue, 100);
                    last = item;
                }
            }

            yield return AutoMove(path);
        }
        private IEnumerator MoveToTarget(GameObject target)
        {
            List<Vector3> path = MapManager.Instance.FindPath(transform.position, target, collSize);

            #if true
            //Debug.Log(transform.position);
            Vector3 last = transform.position;
            foreach (var item in path)
            {
                GameObject go = new GameObject();
                go.transform.position = item;
                Debug.DrawLine(last, item, Color.blue, 180);
                last = item;
            }
            #endif

            yield return AutoMove(path);
        }

        private IEnumerator AutoMove(List<Vector3> path)
        {
            foreach (var item in path)
            {
                while(!MoveToPos(item)) yield return null;
            }
            moveDoneCallback?.Invoke(true);
            StopMove();
        }
        private bool MoveToPos(Vector3 dst)
        {
            Vector3 direction = dst - transform.position;
            direction.y = 0;
            direction.Normalize();
            if(GetComponent<ViewController>()) GetComponent<ViewController>().model.transform.LookAt(new Vector3(dst.x, transform.position.y, dst.z));
            else transform.LookAt(new Vector3(dst.x, transform.position.y, dst.z));
            if(transform.position.PlanerDistance(dst) > 0.1f)
            {
                transform.position += direction * Time.deltaTime * moveSpeed;
                return false;
            }
            else return true;
        }
        public void StopMove()
        {
            if(currentMove == null) return;
            StopCoroutine(currentMove);
            currentMove = null;
            if(moveDoneCallback != null) moveDoneCallback = null;
        }
    }
}