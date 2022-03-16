using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class GamePanel : BasePanel
    {
        public override Layer layer => Layer.Panel;
        public UIButton constructionButton;
        public UIButton mapButton;
        public UIButton handbookButton;
        
        
        void Awake()
        {
            constructionButton = transform.Find("Construction Button").GetComponentInChildren<UIButton>();
            handbookButton = transform.Find("Handbook Button").GetComponentInChildren<UIButton>();
        }

        void Start()
        {
            constructionButton.onClick.AddListener(() => { PanelManager.Instance.Open<ConstructionPanel>("ConstructionPanel"); });
            handbookButton.onClick.AddListener(() => { PanelManager.Instance.Open<HandbookPanel>("HandbookPanel"); });
        }
        public override void OnOpen(params object[] args)
        {
            
        }
        public override void Open()
        {
            
        }
        public override void OnClose()
        {
            
        }
        public override void Close()
        {
            
        }
    }
}