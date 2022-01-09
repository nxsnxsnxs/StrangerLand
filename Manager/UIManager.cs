using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Components;
using UI;

public class UIManager : ManagerSingleton<UIManager>
{
    private Transform inspectPanel;
    [HideInInspector]public InspectWindow inspectWindow;
    public UIButton constructButton;
    public GameObject constructPanel;
    public UIButton mapButton;
    public GameObject mapPanel;
    public UIButton handbookButton;
    public GameObject handbookPanel;
    
    
    void Awake()
    {
        inspectPanel = transform.Find("Inspect Panel");
        inspectWindow = inspectPanel.GetComponentInChildren<InspectWindow>(true);
    }

    void Start()
    {
        constructButton.onClick.AddListener(() => { OpenPanel(constructPanel); });
        mapButton.onClick.AddListener(() => { OpenPanel(mapPanel); });
        handbookButton.onClick.AddListener(() => { OpenPanel(handbookPanel); });
        constructButton.onClick.AddListener(() => { ClosePanel(constructPanel); });
        mapButton.onClick.AddListener(() => { ClosePanel(mapPanel); });
        handbookButton.onClick.AddListener(() => { ClosePanel(handbookPanel); });
    }
    void OpenPanel(GameObject panel)
    {
        panel.GetComponent<Animator>().Play("Open", 0);
    }
    void ClosePanel(GameObject panel)
    {
        panel.GetComponent<Animator>().Play("Close", 0);
    }
    void Update()
    {
        
    }
}
