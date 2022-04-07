using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public abstract class BasePanel : MonoBehaviour
    {
        public abstract Layer layer {get;}
        //打开前的初始化工作
        public abstract void OnOpen(params object[] args);
        //用于实现具体的打开方式，直接打开渐变打开等
        public virtual void Open()
        {
            GetComponent<Animator>().Play("Open");
        }
        //关闭前的处理（引发事件等）
        public abstract void OnClose();
        //用于实现具体的关闭方式，直接关闭渐变关闭等
        public virtual void Close()
        {
            GetComponent<Animator>().Play("Close");
        }
    }
}