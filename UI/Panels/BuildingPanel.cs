using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player.Construction;

namespace UI
{
    public class BuildingPanel : MonoBehaviour
    {
        private Text nameText;
        private Text woodRequestText;
        private Text stoneRequestText;
        private Text crystalRequestText;
        private UIButton inspectButton;
        private UIButton constructButton;
        private GameObject buildingInspectWindow;

        private BuildingStats buildingStats;
        void Awake()
        {
            nameText = transform.Find("NameText").GetComponent<Text>();
            woodRequestText = transform.Find("Resource Requests").Find("Wood").Find("Count Text").GetComponent<Text>();
            stoneRequestText = transform.Find("Resource Requests").Find("Stone").Find("Count Text").GetComponent<Text>();
            crystalRequestText = transform.Find("Resource Requests").Find("Crystal").Find("Count Text").GetComponent<Text>();
            inspectButton = transform.Find("Inspect Button").GetComponent<UIButton>();
            constructButton = transform.Find("Construct Button").GetComponent<UIButton>();
        }
        public void Init(BuildingStats bs, ConstructionPanel constructionPanel)
        {
            nameText.text = bs.name;
            woodRequestText.text = bs.wood_request.ToString();
            stoneRequestText.text = bs.stone_request.ToString();
            crystalRequestText.text = bs.crystal_request.ToString();
            inspectButton.onClick.AddListener(() => { constructionPanel.InspectBuilding(bs); });
            constructButton.onClick.AddListener(() => { constructionPanel.ConstructBuilding(bs); });

            buildingStats = bs;
        }
        void Start()
        {
            
        }

        void Update()
        {
            
        }
    }
}