using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

namespace Player.Construction
{
    //json数据存储
    [System.Serializable]
    public class BuildingStats
    {
        //Prefab加载路径Building/path
        public string path;
        //名称（中文）
        public string name;
        //大小（xz单位为网格）
        //public byte length;//x
        //public byte width;//z
        //public float height;//pivot在y轴高度（即当建筑物底部着地时的y轴坐标，用于确定建筑物的放置坐标）
        //建造价格
        public int wood_request;
        public int stone_request;
        public int crystal_request;
    }
    /// <summary>
    /// 记录一个建筑物存储的和运行时的所有信息
    /// </summary>
    public class BuildingInfo
    {
        public GridPos center;
        public BuildingStats stats;
    }
    public class ConstructionManager : ManagerSingleton<ConstructionManager>
    {
        [System.Serializable]
        private class BuildingCatalogue
        {
            public List<BuildingStats> buildings;
        }
        private BuildingCatalogue catalogue;
        
        public List<BuildingStats> BuildingList
        {
            get => catalogue.buildings;
        }

        void Awake()
        {
            TextAsset textAsset = ABManager.Instance.LoadAsset<TextAsset>("GameStats", "BuildingCatalogue.json");
            catalogue = JsonUtility.FromJson<BuildingCatalogue>(textAsset.text);
        }
    }
}