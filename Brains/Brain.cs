using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBehaviourTree;

public abstract class Brain : MonoBehaviour
{
    public BehaviourTree bt;
    public abstract void InitBehaviourTree();
    public abstract void InitBlackBoard();

    void Start()
    {
        InitBehaviourTree();
        InitBlackBoard();
    }
    void FixedUpdate()
    {
        bt.Tick();
    }
}