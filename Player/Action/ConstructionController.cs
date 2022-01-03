using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;

namespace Player.Action{
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
        IEnumerator TryConstruct()
        {
            currentBuilding = new GameObject("CurrentConstruct");
            GameObject buildingPrefab = ABManager.Instance.LoadAsset<GameObject>("Building", "Cube");
            GameObject realBuilding = Instantiate(buildingPrefab, currentBuilding.transform);
            realBuilding.transform.localPosition = Vector3.zero;
            realBuilding.transform.rotation = Quaternion.identity;
            Constructable target = currentBuilding.AddComponent<Constructable>();
            target.viewCam = viewController.CurrentViewCam;
            target.building = realBuilding;

            //从文件中加载模型数据
            BuildingStats bs = new BuildingStats();
            bs.length = 4;
            bs.width = 4;
            bs.height = 2;
            target.info.stats = bs;
            target.preBuildFinishCallback += OnConstructionFinish;
            
            while(!preBuildSuccess) yield return null;
            Vector3 dst = target.building.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            target.gameObject.SetActive(false);
            yield return locomotionController.MoveToPoint(dst);
            animator.SetTrigger("Construct");
            yield return new WaitForSeconds(Constants.normal_construct_time);
            target.PlaceBuilding();
            animator.SetTrigger("ConstructEnd");
            while(!triggers["constructfinish"]) yield return null;
            finish = true;
            ResetActionTrigger();
        }
        void OnConstructionFinish(bool result)
        {
            if(result) preBuildSuccess = true;
            else finish = true;
        }

        public override void Begin(params object[] target)
        {
            finish = false;
            preBuildSuccess = false;
            current = StartCoroutine(TryConstruct());
        }

        public override void Interrupted()
        {
            StopCoroutine(current);
            animator.SetTrigger("StopConstruct");
            //处在prebuild阶段
            if(!preBuildSuccess)
            {
                Destroy(currentBuilding);
            }
            else ResetActionTrigger();
        }
    }
}