using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class Tool : GameComponent
    {
        //********need set************
        public List<WorkToolType> toolTypes;
        //****************************
        public bool CanWork(Workable workable)
        {
            return toolTypes.Contains(workable.toolType);
        }
    }
}