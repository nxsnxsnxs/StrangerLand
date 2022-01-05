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
    public UIButton taskButton;
    public GameObject taskPanel;
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
        taskButton.OnButtonClick.AddListener(() => { OpenPanel(taskPanel); });
        constructButton.OnButtonClick.AddListener(() => { OpenPanel(constructPanel); });
        mapButton.OnButtonClick.AddListener(() => { OpenPanel(mapPanel); });
        handbookButton.OnButtonClick.AddListener(() => { OpenPanel(handbookPanel); });
        taskButton.OnButtonClick.AddListener(() => { ClosePanel(taskPanel); });
        constructButton.OnButtonClick.AddListener(() => { ClosePanel(constructPanel); });
        mapButton.OnButtonClick.AddListener(() => { ClosePanel(mapPanel); });
        handbookButton.OnButtonClick.AddListener(() => { ClosePanel(handbookPanel); });
    }
    void OpenPanel(GameObject panel)
    {
        taskPanel.GetComponent<Animator>().Play("Open", 0);
    }
    void ClosePanel(GameObject panel)
    {
        taskPanel.GetComponent<Animator>().Play("Close", 0);
    }
    void Update()
    {
        
    }
}
