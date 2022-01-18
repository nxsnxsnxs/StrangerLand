using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface x
{
    void Do();
}
public class Test2 : MonoBehaviour
{
    
    void Start()
    {
        Debug.Log(FindObjectsOfType(typeof(x), true));
    }
}
