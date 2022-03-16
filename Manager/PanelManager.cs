using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

namespace UI
{
    public enum Layer
    {
        Panel, Tip
    }
    public class PanelManager : ManagerSingleton<PanelManager>
    {
        private Transform panelLayer;
        private Transform tipLayer;
        private Dictionary<string, BasePanel> panels;
        private Dictionary<Layer, Transform> roots;
        
        void Awake()
        {
            panelLayer = transform.Find("Panels");
            tipLayer = transform.Find("Tips");
            roots = new Dictionary<Layer, Transform>();
            roots[Layer.Panel] = panelLayer;
            roots[Layer.Tip] = tipLayer;
            panels = new Dictionary<string, BasePanel>();
        }
        public T Open<T>(string name, params object[] args) where T : BasePanel
        {
            GameObject skin = ABManager.Instance.LoadAsset<GameObject>("Panel", name);
            if(!skin)
            {
                Debug.LogError("wrong panel name " + name);
                return null;
            }
            GameObject go = Instantiate(skin);
            T panel = go.AddComponent<T>();
            go.transform.SetParent(roots[panel.layer], false);
            panel.OnOpen(args);
            panel.Open();
            panels[name] = panel;
            return panel;
        }
        public void Close(string name)
        {
            if(!panels.ContainsKey(name)) return;
            BasePanel panel = panels[name];
            panel.OnClose();
            panel.Close();
            Destroy(panel.gameObject);
            panels.Remove(name);
        }
    }
}