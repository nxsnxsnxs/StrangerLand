namespace Player.Construction
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using Player.Actions;

    public class Constructable : MonoBehaviour
    {
        public UnityAction<bool> preBuildFinishCallback;//完成后的回调事件
        public BuildingInfo info;//建筑物信息
        //人物的组件
        public ConstructionController constructionController;
        public GameObject buildingPrefab;
        public Camera viewCam;
        private List<Material> originalMats;//建筑物原本的mat
        private List<Collider> collidersInTrigger;//检测到碰撞的物体（用于确定是否可以放置)
        private bool inPrebuild;
        public GameObject building;

        void Init()
        {
            constructionController = FindObjectOfType<ConstructionController>();
            collidersInTrigger = new List<Collider>();
            originalMats = new List<Material>();
            foreach(MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                originalMats.Add(mr.material);
            }
            inPrebuild = true;
        }
        void Awake()
        {
            Init();
        }
        void Start()
        {
            building.GetComponent<Collider>().isTrigger = true;
            /*Debug.Log(building.GetComponent<Collider>().bounds.center.y - building.transform.position.y);
            float y = building.GetComponent<Collider>().bounds.size.y / 2 - (building.GetComponent<Collider>().bounds.center.y - building.transform.position.y);
            building.transform.localPosition = new Vector3(building.transform.localPosition.x, y, building.transform.localPosition.z);*/
            //设置了刚体才能检测到与其他建筑物的碰撞（因为其他建筑物没有刚体）
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        void FixedUpdate()
        {
            if(inPrebuild)
            {
                FollowMousePos();
                //可以放置
                if(collidersInTrigger.Count == 0) building.GetComponentInChildren<MeshRenderer>().material = constructionController.preBuildSuccessMat;
                else building.GetComponentInChildren<MeshRenderer>().material = constructionController.preBuildFailMat;
                if(Input.GetMouseButtonDown(0))
                {
                    if(collidersInTrigger.Count == 0)
                    {
                        preBuildFinishCallback.Invoke(true);
                        inPrebuild = false;
                    }
                    else
                    {
                        preBuildFinishCallback.Invoke(false);
                        Destroy(gameObject);
                    }
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if(!inPrebuild) return;
            //Debug.Log(other.name);
            if(!collidersInTrigger.Contains(other) &&
            other.gameObject.layer != LayerMask.NameToLayer("ground") &&
            other.gameObject.layer != LayerMask.NameToLayer("building")) collidersInTrigger.Add(other);
        }
        void OnTriggerExit(Collider other)
        {
            if(!inPrebuild) return;
            collidersInTrigger.Remove(other);
        }
        /// <summary>
        /// prebuilding跟随鼠标位置移动
        /// </summary>
        void FollowMousePos()
        {
            Ray ray = viewCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            LayerMask layerMask = 1 << LayerMask.NameToLayer("ground");
            if(Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, layerMask.value))
            {
                GridPos buildPos = GridPos.GetGridPos(raycastHit.point);
                transform.position = buildPos.Pos;              
            }
        }
        /// <summary>
        /// 开始放置建筑物
        /// </summary>
        public void PlaceBuilding()
        {
            //将建筑物替换回原材质
            MeshRenderer[] mrs = building.GetComponentsInChildren<MeshRenderer>();
            for(int i = 0; i < originalMats.Count; ++i)
            {
                mrs[i].material = originalMats[i];
            }
            //恢复建筑物的collider
            building.GetComponent<Collider>().isTrigger = false;
            transform.DetachChildren();
            info.center = GridPos.GetGridPos(transform.position);
            preBuildFinishCallback?.Invoke(true);
            Destroy(gameObject);
        }
    }
}
