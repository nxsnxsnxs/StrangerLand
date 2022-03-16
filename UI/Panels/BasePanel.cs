using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public abstract class BasePanel : MonoBehaviour
    {
        public abstract Layer layer {get;}
        public abstract void OnOpen(params object[] args);
        public virtual void Open()
        {
            GetComponent<Animator>().Play("Open");
        }
        public abstract void OnClose();
        public virtual void Close()
        {
            GetComponent<Animator>().Play("Close");
        }
    }
}