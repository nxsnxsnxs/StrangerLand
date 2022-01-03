namespace Components
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using Player.Action;

    //json数据存储
    public struct BuildingStats
    {
        //xz单位为网格
        public byte length;//x
        public byte width;//z
        public float height;//y
    }
    /// <summary>
    /// 记录一个建筑物存储的和运行时的所有信息
    /// </summary>
    public struct BuildingInfo
    {
        public GridPos center;
        public BuildingStats stats;
    }
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
        }
        void Awake()
        {
            Init();
        }
        void Start()
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            SetCoveredCollider(info, collider);//设置collider信息
            foreach(Collider col in GetComponentsInChildren<Collider>())
            {
                col.isTrigger = true;
            }
            //设置了刚体才能检测到与其他建筑物的碰撞（因为其他建筑物没有刚体）
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        void FixedUpdate()
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
                }
                else
                {
                    preBuildFinishCallback.Invoke(false);
                    Destroy(gameObject);
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            //Debug.Log(other.name);
            if(!collidersInTrigger.Contains(other) &&
            other.gameObject.layer != LayerMask.NameToLayer("ground") &&
            other.gameObject.layer != LayerMask.NameToLayer("building")) collidersInTrigger.Add(other);
        }
        void OnTriggerExit(Collider other)
        {
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
                GridPos buildPos = GridPos.GetGridPos(raycastHit.point + Vector3.up * info.stats.height / 2);
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
            foreach(Collider col in building.GetComponentsInChildren<Collider>())
            {
                col.isTrigger = false;
            }
            transform.DetachChildren();
            info.center = GridPos.GetGridPos(transform.position);
            MapManager.Instance.AddBuilding(info);
            preBuildFinishCallback?.Invoke(true);
            Destroy(gameObject);
        }
        /// <summary>
        /// 通过BuildingInfo配置一块不可放置区域碰撞体
        /// </summary>
        /// <param name="buildingInfo"></param>
        /// <param name="collider"></param>
        static void SetCoveredCollider(BuildingInfo buildingInfo, BoxCollider collider)
        {
            collider.center = Vector3.zero;
            collider.size = new Vector3(buildingInfo.stats.length, buildingInfo.stats.height, buildingInfo.stats.width);
            collider.isTrigger = true;
        }
    }
}
