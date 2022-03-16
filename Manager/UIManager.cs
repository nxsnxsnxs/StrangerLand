using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Components;
using UI;

public class UIManager : ManagerSingleton<UIManager>
{
    
    void Start()
    {
        PanelManager.Instance.Open<GamePanel>("GamePanel");
    }
}
