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
    public class ConstructionPanel : BasePanel
    {
        private GameObject buildingCellSkin;
        private Transform buildingCatalogue;
        private UIButton closeButton;
        private Animator animator;

        public override Layer layer => Layer.Panel;

        void Awake()
        {
            buildingCellSkin = ABManager.Instance.LoadAsset<GameObject>("UIElement", "Building Cell");
            
            buildingCatalogue = transform.Find("Bottom").Find("ScrollView").Find("Viewport").Find("Content");
            closeButton = transform.Find("Top").Find("Close Button").GetComponent<UIButton>();
            closeButton.onClick.AddListener(() => PanelManager.Instance.Close("ConstructionPanel"));
            animator = GetComponent<Animator>();
        }
        void InitBuildingCatalogue()
        {
            foreach (var buildingStats in ConstructionManager.Instance.BuildingList)
            {
                GameObject bp = Instantiate(buildingCellSkin, buildingCatalogue);
                BuildingCell cell = bp.AddComponent<BuildingCell>();
                cell.Init(buildingStats);
            }
        }
        void Start()
        {
            //TODO:待有加载顺序后放到start中
            InitBuildingCatalogue();
        }

        public void ConstructBuilding(BuildingStats bs)
        {
            GetComponent<Animator>().Play("Close");
            ActionController ac = FindObjectOfType<Character>().GetComponent<ActionController>();
            ac.DoAction<ConstructionController>(bs);
        }

        public override void OnOpen(params object[] args)
        {
            
        }
        public override void OnClose()
        {
            
        }
    }
}