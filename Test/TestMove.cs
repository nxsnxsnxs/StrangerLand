using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    public float speed;
    void FixedUpdate()
    {
        transform.position += speed * Time.deltaTime * transform.right;
    }
}
