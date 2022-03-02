using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player.Construction;
using Actions;
using Player.Actions;
using Prefabs.Player;

namespace UI
{
    public class ConstructionPanel : MonoBehaviour
    {
        public Transform buildingInspectArea;
        private GameObject buildingPanelSkin;
        private GameObject buildingInspectPanel;
        private GameObject inspectBuilding;
        private Transform buildingCatalogue;
        private UIButton closeButton;

        void Awake()
        {
            buildingPanelSkin = ABManager.Instance.LoadAsset<GameObject>("GameUI", "BuildingPanel");
            buildingInspectPanel = transform.Find("BuildingInspectPanel").gameObject;
            buildingInspectPanel.GetComponentInChildren<UIButton>().onClick.AddListener(CloseInspectPanel);
            buildingCatalogue = transform.Find("Popup Base").Find("Bottom").Find("ScrollView").Find("Viewport").Find("Content");
            closeButton = transform.Find("Popup Base").Find("Top").Find("Close Button").GetComponent<UIButton>();
            closeButton.onClick.AddListener(Close);
        }
        void InitBuildingCatalogue()
        {
            foreach (var buildingStats in ConstructionManager.Instance.BuildingList)
            {
                GameObject bp = Instantiate(buildingPanelSkin, buildingCatalogue);
                bp.GetComponent<BuildingPanel>().Init(buildingStats, this);
            }
        }
        void Start()
        {
            InitBuildingCatalogue();
        }

        void Update()
        {
            
        }
        public void InspectBuilding(BuildingStats bs)
        {
            GameObject buildingSkin = ABManager.Instance.LoadAsset<GameObject>("BuildingPrefab", bs.path);
            if(buildingSkin == null)
            {
                Debug.LogError("No building path " + bs.path);
                return;
            }
            inspectBuilding = Instantiate(buildingSkin, buildingInspectArea);
            inspectBuilding.transform.localPosition = new Vector3(0, 3, 0);
            inspectBuilding.AddComponent<Rigidbody>();
            buildingInspectPanel.SetActive(true);
        }
        public void ConstructBuilding(BuildingStats bs)
        {
            GetComponent<Animator>().Play("Close");
            ActionController ac = FindObjectOfType<Character>().GetComponent<ActionController>();
            ac.DoAction<ConstructionController>(bs);
        }
        public void Close()
        {
            GetComponent<Animator>().Play("Close");
        }
        public void CloseInspectPanel()
        {
            buildingInspectPanel.SetActive(false);
            Destroy(inspectBuilding);
        }
    }
}