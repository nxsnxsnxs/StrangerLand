using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using Tools;
using Actions;
using Events;
using Player;


namespace Prefabs.Player
{
    public abstract class Character : PrefabComponent
    {
        public override void DefaultInit()
        {
            AddTag(GameTag.creature);
            AddTag(GameTag.player);
            EffectHandler effectHandler = gameObject.AddGameComponent<EffectHandler>();
        }
    }
}