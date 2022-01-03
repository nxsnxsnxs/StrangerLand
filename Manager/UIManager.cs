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
    public UIButton mapButton;
    public UIButton handbookButton;
    
    
    void Awake()
    {
        inspectPanel = transform.Find("Inspect Panel");
        inspectWindow = inspectPanel.GetComponentInChildren<InspectWindow>(true);
    }

    void Start()
    {
        taskButton.OnButtonClick.AddListener(OpenPanel);

    }
    void OpenPanel()
    {
        taskPanel.GetComponent<Animator>().Play("Open", 0);
        //StartCoroutine(Utils.Popup(taskPanel, 0.8f));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
