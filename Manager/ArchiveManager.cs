using System.Linq;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Tools;
using Prefabs;

public interface IArchiveSave
{
    string GetPersistentData();
}

[JsonObject(Newtonsoft.Json.MemberSerialization.OptIn)]
public class ArchiveManager : ManagerSingleton<ArchiveManager>
{
    [JsonProperty]
    public int archiveID;
    //场景中基本物体的数据
    private Dictionary<int, string> orgPrefabData;
    //场景中动态生成的物体的数据(含prefabcomponent的物体)
    [JsonProperty]
    private List<string> initPrefabData;
    //存档自身的配置数据
    [JsonProperty]
    private Dictionary<string, object> archiveData;
    //其它数据
    private Dictionary<int, IArchiveSave> itemToSave;
    private string dataPath;


    void Awake()
    {
        itemToSave = new Dictionary<int, IArchiveSave>();
        dataPath = Application.persistentDataPath + "/" + archiveID + "/Data.json";
    }
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) Save();
    }
    public bool RegisterSave(int instanceID, IArchiveSave item)
    {
        if(itemToSave.ContainsKey(instanceID))
        {
            Debug.LogError("same instanceID error: " + instanceID);
            return false;
        }
        itemToSave[instanceID] = item;
        return true;
    }
    public void Save()
    {
        PrefabComponent[] prefabs = FindObjectsOfType<PrefabComponent>(true);
        foreach (var item in prefabs)
        {
            var itemData = item.GetPersistentData();
            initPrefabData.Add(itemData);
        }

        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.TypeNameHandling = TypeNameHandling.All;
        string dataStr = JsonConvert.SerializeObject(this);
        FileStream file = new FileStream(dataPath, FileMode.Create);
        byte[] data = System.Text.Encoding.Default.GetBytes(dataStr);
        file.Write(data, 0, data.Length);
        file.Close();
    }

    public T GetArchiveData<T>(string name) where T : UnityEngine.Object
    {
        if(!archiveData.ContainsKey(name))
        {
            Debug.LogError("no archive data name " + name);
            return null;
        }
        return archiveData[name] as T;
    }
}
