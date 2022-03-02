using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class Commander : GameComponent
    {
        private List<GameObject> soldiers;
        public int SoliderCount
        {
            get => soldiers.Count;
        }

        void Awake()
        {
            soldiers = new List<GameObject>();
        }
        public void AddSoldier(GameObject soldier)
        {
            soldiers.Add(soldier);
            soldier.GetComponent<Health>().onDie += OnSoldierDie;
        }
        public void OnSoldierDie(GameObject soldier)
        {
            soldiers.Remove(soldier);
        }
        public void ShareTarget(GameObject target = null)
        {
            GameObject sharedTarget = target ?? GetComponent<Combat>().target;
            foreach (var soldier in soldiers)
            {
                soldier.GetComponent<Combat>().SuggestTarget(sharedTarget);
            }
        }
    }
}