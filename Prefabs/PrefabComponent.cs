using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Components;

namespace Prefabs
{
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
            //AssetName(AssetBundle)
            public string loadPath;
            //all data of gamecomponents it have
            public Dictionary<string, object> componentData;
            void Generate()
            {
                GameObject target = ABManager.Instance.LoadAsset<GameObject>("Prefab", loadPath);
                GameObject go = GameObject.Instantiate(target, new Vector3(x, y, z), Quaternion.Euler(pitch, yaw, roll));
                PrefabComponent pc = go.GetComponent<PrefabComponent>();
                pc.Init(componentData);
            }
        }
        public abstract string loadPath
        {
            get;
        }
        public void Init(Dictionary<string, object> componentData)
        {
            DefaultInit();
            if(componentData != null) InitComponentsData(componentData);
        }
        public KeyValuePair<int, string> GetPersistentData()
        {
            PersistentPrefabData data = new PersistentPrefabData();
            data.x = transform.position.x;
            data.y = transform.position.y;
            data.z = transform.position.z;
            data.pitch = transform.rotation.eulerAngles.x;
            data.yaw = transform.rotation.eulerAngles.y;
            data.roll = transform.rotation.eulerAngles.z;
            data.loadPath = loadPath;
            foreach (var item in GetComponents<GameComponent>())
            {
                item.SaveData(data.componentData);
            }
            return new KeyValuePair<int, string>(GetInstanceID(), JsonConvert.SerializeObject(data));
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
