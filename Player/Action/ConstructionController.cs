using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;

namespace Player.Action{
    using Player.Construction;
    public class ConstructionController : PlayerAction
    {
        public Material preBuildSuccessMat;//可以建造的material
        public Material preBuildFailMat;//无法建造的material
        
        private Animator animator;
        private ViewController viewController;
        private LocomotionController locomotionController;
        private Coroutine current;
        private bool preBuildSuccess;
        private GameObject currentBuilding;
        public override string actionName
        {
            get => "Construct";
        }
        public override int priority
        {
            get => 1;
        }

        void Init()
        {
            animator = GetComponent<Animator>();
            viewController = GetComponent<ViewController>();
            locomotionController = GetComponent<LocomotionController>();
        }
        void Awake()
        {
            Init();
            RegisterTrigger("constructfinish");
        }
        void FixedUpdate()
        {
            
        }
        IEnumerator TryConstruct(BuildingStats bs)
        {
            currentBuilding = new GameObject("CurrentConstruct");
            //加载模型
            GameObject buildingPrefab = ABManager.Instance.LoadAsset<GameObject>("BuildingPrefab", bs.path);
            if(buildingPrefab == null)
            {
                Debug.LogError("No Building Asset named " + bs.name);
                finish = true;
                yield break;
            }
            //生成模型
            GameObject realBuilding = Instantiate(buildingPrefab, currentBuilding.transform);
            realBuilding.transform.localPosition = Vector3.zero;
            realBuilding.transform.rotation = Quaternion.identity;
            //配置Constructable
            Constructable target = currentBuilding.AddComponent<Constructable>();
            target.viewCam = viewController.CurrentViewCam;
            target.building = realBuilding;
            target.info = new BuildingInfo();
            target.info.stats = bs;
            target.preBuildFinishCallback += OnConstructionFinish;
            //等待Constructable的成功回调
            while(!preBuildSuccess) yield return null;
            Vector3 dst = target.building.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            yield return locomotionController.MoveToPoint(dst);
            animator.SetTrigger("Construct");
            yield return new WaitForSeconds(Constants.normal_construct_time);
            //调用Constructable完成最后的放置
            target.PlaceBuilding();
            //TODO: 扣除资源
            animator.SetTrigger("ConstructEnd");
            while(!triggers["constructfinish"]) yield return null;
            finish = true;
            ResetActionTrigger();
        }
        //由Constructable回调
        void OnConstructionFinish(bool result)
        {
            if(result) preBuildSuccess = true;
            else finish = true;
        }

        public override void Begin(params object[] target)
        {
            finish = false;
            preBuildSuccess = false;
            current = StartCoroutine(TryConstruct(target[0] as BuildingStats));
        }

        public override void Interrupted()
        {
            StopCoroutine(current);
            animator.SetTrigger("StopConstruct");
            //处在prebuild阶段
            if(currentBuilding)
            {
                Destroy(currentBuilding);
            }
            else ResetActionTrigger();
        }
    }
}