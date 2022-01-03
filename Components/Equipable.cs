using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public enum EquipSlotType
    {
        Hand, Head, Body
    }
    public class Equipable : GameComponent
    {
        public AnimatorOverrideController overrideAnimator;
        public EquipSlotType equipSlotType;
        public Vector3 pos;
        public Quaternion rot;
        public void Equip(Transform parent)
        {
            transform.SetParent(parent);
            transform.localPosition = pos;
            transform.localRotation = rot;
        }
    }
}