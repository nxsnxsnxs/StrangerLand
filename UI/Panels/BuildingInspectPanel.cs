using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Construction;

namespace UI
{
    public class BuildingInspectPanel : BasePanel
    {
        public override Layer layer => Layer.Panel;
        private Transform inspectArea;
        private GameObject inspectBuilding;
        private UIButton closeButton;

        void Init()
        {
            closeButton = GetComponentInChildren<UIButton>();
            closeButton.onClick.AddListener(() => { PanelManager.Instance.Close("BuildingInspectPanel"); });
            inspectArea = GameObject.Find("Inspect Area").transform;
        }
        public override void OnClose()
        {
            Destroy(inspectBuilding);
        }

        public override void OnOpen(params object[] args)
        {
            Init();
            BuildingStats bs = args[0] as BuildingStats;
            GameObject buildingSkin = ABManager.Instance.LoadAsset<GameObject>("BuildingPrefab", bs.path);
            if(buildingSkin == null)
            {
                Debug.LogError("No building path " + bs.path);
                return;
            }
            inspectBuilding = Instantiate(buildingSkin, inspectArea);
            inspectBuilding.transform.localPosition = new Vector3(0, 3, 0);
            inspectBuilding.AddComponent<Rigidbody>();
        }
    }
}