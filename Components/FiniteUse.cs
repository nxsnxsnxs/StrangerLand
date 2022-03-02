using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class FiniteUse : GameComponent
    {
        //********need set************
        public int maxUse;
        public int currUse;
        //****************************
        public override void SaveData(Dictionary<string, object> data)
        {
            data["currUse"] = currUse;
        }
        public override void InitData(Dictionary<string, object> data)
        {
            if(data.ContainsKey("currUse")) currUse = (int)data["currUse"];
        }
    }
}