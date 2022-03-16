using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;

namespace UI
{
    public class InspectPanel : BasePanel
    {
        public override Layer layer => Layer.Panel;
        private InspectWindow inspectWindow;

        public override void OnOpen(params object[] args)
        {
            inspectWindow = GetComponentInChildren<InspectWindow>();
            Inspectable target = args[0] as Inspectable;
            inspectWindow.InitInspect(target);
        }
        public override void Open()
        {
            
        }
        public override void OnClose()
        {
            
        }          
        public override void Close()
        {
            inspectWindow.StopInspect();
        }
    }
}