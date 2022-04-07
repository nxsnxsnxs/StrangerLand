using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Components;

namespace Prefabs
{
    [FlagsAttribute]
    public enum GameTag
    {
        creature = 0x0001,
        player = 0x0002,
        neutral = 0x0004,
        evil = 0x0008
    }
    public abstract class PrefabComponent : MonoBehaviour
    {
        public class PersistentPrefabData
        {
            //pos
            public float x;
            public float y;
            public float z;
            //rot
            public float pitch;
            public float yaw;
            public float roll;
            //size
            public float xsize;
            public float ysize;
            public float zsize;
            //AssetName(AssetBundle)
            public string loadPath;
            //all data of gamecomponents it have
            public Dictionary<string, object> componentData;
            void Generate()
            {
                GameObject target = ABManager.Instance.LoadAsset<GameObject>("Prefab", loadPath);
                GameObject go = GameObject.Instantiate(target, new Vector3(x, y, z), Quaternion.Euler(pitch, yaw, roll));
                go.transform.localScale = new Vector3(xsize, ysize, zsize);
                PrefabComponent pc = go.GetComponent<PrefabComponent>();
                pc.InitComponentsData(componentData);
            }
        }
        public abstract string bundleName {get;}
        private GameTag gameTag;

        void Awake()
        {
            DefaultInit();
        }
        public void AddTag(GameTag tag)
        {
            gameTag |= tag;
        }
        public bool HasTag(GameTag tag)
        {
            return (gameTag & tag) != 0;
        }

        public string GetPersistentData()
        {
            PersistentPrefabData data = new PersistentPrefabData();
            data.x = transform.position.x;
            data.y = transform.position.y;
            data.z = transform.position.z;
            data.pitch = transform.rotation.eulerAngles.x;
            data.yaw = transform.rotation.eulerAngles.y;
            data.roll = transform.rotation.eulerAngles.z;
            data.xsize = transform.localScale.x;
            data.ysize = transform.localScale.y;
            data.zsize = transform.localScale.z;
            data.loadPath = bundleName;
            foreach (var item in GetComponents<GameComponent>())
            {
                item.SaveData(data.componentData);
            }
            return JsonConvert.SerializeObject(data);
        }
        public abstract void DefaultInit();
        public void InitComponentsData(Dictionary<string, object> componentData)
        {
            foreach (var item in GetComponents<GameComponent>())
            {
                item.InitData(componentData);
            }
        }
    }
}
