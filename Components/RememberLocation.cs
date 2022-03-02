using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class RememberLocation : GameComponent
    {
        public Dictionary<string, Vector3> locations;

        void Awake()
        {
            locations = new Dictionary<string, Vector3>();
        }
        public void Remember(string name, Vector3 location)
        {
            locations[name] = location;
        }
    }
}
