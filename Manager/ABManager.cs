using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Tools;

using Object = UnityEngine.Object;
public class ABManager : ManagerSingleton<ABManager>
{
    //已加载的AB包
    private Dictionary<string, AssetBundle> abLoadedDic = new Dictionary<string, AssetBundle>();
    //主ab包
    private AssetBundle mainAB = null;
    //主ab包的manifest
    private AssetBundleManifest manifest = null;
    //AB包文件夹路径
    private string ABPath
    {
        get
        {
            #if UNITY_EDITOR
                return Application.dataPath + "/StreamingAssets";
            #else
                return Application.dataPath;
            #endif
        }
    }
    //主AB包名
    private string MainName
    {
        get
        {
            #if UNITY_IOS
                return "iOS";
            #elif UNITY_ANDROID
                return "Android";
            #else
                return "StandaloneWindows";
            #endif
        }
    }
    //加载AB包依赖项
    public void LoadDependencies(string bundleName)
    {
        if(mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(ABPath + "/" + MainName);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        string[] dps = manifest.GetAllDependencies(bundleName);
        foreach (var dp in dps)
        {
            if(!abLoadedDic.ContainsKey(dp))
            {
                abLoadedDic[dp] = AssetBundle.LoadFromFile(ABPath + "/" + dp);
            }
        }
    }
    public IEnumerator LoadDependenciesAsync(string bundleName)
    {
        if(mainAB == null)
        {
            AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(ABPath + "/" + bundleName);
            yield return abcr;
            mainAB = abcr.assetBundle;
            AssetBundleRequest abr = mainAB.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
            yield return abr;
        }
        string[] dps = manifest.GetAllDependencies(bundleName);
        foreach (var dp in dps)
        {
            if(!abLoadedDic.ContainsKey(dp))
            {
                AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(ABPath + "/" + dp);
                yield return abcr;
                abLoadedDic[dp] = abcr.assetBundle;
            }
        }
    }
    //加载AB包
    public AssetBundle LoadBundle(string bundleName)
    {
        if(!abLoadedDic.ContainsKey(bundleName))
        {
            abLoadedDic[bundleName] = AssetBundle.LoadFromFile(ABPath + "/" + bundleName);
        }
        return abLoadedDic[bundleName];
    }
    public IEnumerator LoadBundleAsync(string bundleName)
    {
        if(!abLoadedDic.ContainsKey(bundleName))
        {
            AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(ABPath + "/" + bundleName);
            yield return abcr;
            abLoadedDic[bundleName] = abcr.assetBundle;
        }
    }
    public Object LoadAsset(string bundleName, string assetName)
    {
        #if UNITY_EDITOR
        return Resources.Load(bundleName + "/" + assetName);
        #else
        LoadDependencies(bundleName);

        AssetBundle bundle = LoadBundle(bundleName);
        return bundle.LoadAsset(assetName);
        #endif
    }
    public Object LoadAsset(Type type, string bundleName, string assetName)
    {
        #if UNITY_EDITOR
        return Resources.Load(bundleName + "/" + assetName, type);
        #else
        LoadDependencies(bundleName);

        AssetBundle bundle = LoadBundle(bundleName);
        return bundle.LoadAsset(assetName, type);
        #endif
    }
    public T LoadAsset<T>(string bundleName, string assetName) where T : Object
    {
        #if UNITY_EDITOR
        return Resources.Load<T>(bundleName + "/" + assetName);
        #else
        LoadDependencies(bundleName);

        AssetBundle bundle = LoadBundle(bundleName);
        return bundle.LoadAsset<T>(assetName);
        #endif
    }
    public void LoadAssetAsync(string bundleName, string assetName, UnityAction<Object> loadDoneCallBack)
    {
        StartCoroutine(LoadAssetCoroutine(bundleName, assetName, loadDoneCallBack));
    }
    public void LoadAssetAsync(Type type, string bundleName, string assetName, UnityAction<Object> loadDoneCallBack)
    {
        StartCoroutine(LoadAssetCoroutine(bundleName, assetName, type, loadDoneCallBack));
    }
    public void LoadAssetAsync<T>(string bundleName, string assetName, UnityAction<T> loadDoneCallBack) where T : Object
    {
        StartCoroutine(LoadAssetCoroutine<T>(bundleName, assetName, loadDoneCallBack));
    }
    private IEnumerator LoadAssetCoroutine(string bundleName, string assetName, UnityAction<Object> loadOverCallback)
    {
        yield return LoadDependenciesAsync(bundleName);

        yield return LoadBundleAsync(bundleName);

        AssetBundleRequest abr = abLoadedDic[bundleName].LoadAssetAsync(assetName);
        yield return abr;
        loadOverCallback(abr.asset);
    }
    private IEnumerator LoadAssetCoroutine(string bundleName, string assetName, System.Type type, UnityAction<Object> loadOverCallback)
    {
        yield return LoadDependenciesAsync(bundleName);

        yield return LoadBundleAsync(bundleName);

        AssetBundleRequest abr = abLoadedDic[bundleName].LoadAssetAsync(assetName, type);
        yield return abr;
        loadOverCallback(abr.asset);
    }
    private IEnumerator LoadAssetCoroutine<T>(string bundleName, string assetName, UnityAction<T> loadOverCallback) where T : Object
    {
        yield return LoadDependenciesAsync(bundleName);

        yield return LoadBundleAsync(bundleName);

        AssetBundleRequest abr = abLoadedDic[bundleName].LoadAssetAsync<T>(assetName);
        yield return abr;
        loadOverCallback(abr.asset as T);
    }
    public void UnLoadAB(string abName)
    {
        if(abLoadedDic.ContainsKey(abName))
        {
            abLoadedDic[abName].Unload(false);
            abLoadedDic.Remove(abName);
        }
    }
    public void Clear()
    {
        mainAB = null;
        manifest = null;
        abLoadedDic.Clear();
    }

}