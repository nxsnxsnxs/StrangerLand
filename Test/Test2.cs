using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    void Start()
    {
        Debug.Log(GetComponent<Test1>().enabled);
    }
}
