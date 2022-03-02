using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Player.Actions;
using Actions;
using Components;

namespace Events.Player
{
    public class PlayerEvents
    {
        public abstract class PlayerEvent : BaseEvent
        {
            public virtual int Level
            {
                get => 1;
            }
        }
        public class Move : PlayerEvent
        {
            public override string name
            {
                get => "Move";
            }
            //params:
            //
            //target
            public override void fn(GameObject gameObject, params object[] args)
            {
                ActionController actionController = gameObject.GetComponent<ActionController>();
                if(args.Length == 0 && !actionController.isDoing<LocomotionController>()) actionController.DoAction<LocomotionController>(args);
                else if(args.Length == 1) actionController.DoAction<LocomotionController>(args);
            }
        }
        public class AutoCraft : PlayerEvent
        {
            public override string name
            {
                get => "AutoCraft";
            }
            //params:
            //
            public override void fn(GameObject gameObject, params object[] args)
            {
                ActionController actionController = gameObject.GetComponent<ActionController>();
                if(!actionController.isDoing<CraftController>()) actionController.DoAction<CraftController>(args);
            }
        }
        public class Work : PlayerEvent
        {
            public override string name
            {
                get => "Work";
            }
            //params:
            //target
            public override void fn(GameObject gameObject, params object[] args)
            {
                InventoryController inventoryController = gameObject.GetComponent<InventoryController>();
                ActionController actionController = gameObject.GetComponent<ActionController>();
                Workable workable = args[0] as Workable;
                if(inventoryController.handEquipment.GetComponent<Tool>().CanWork(workable))
                {
                    actionController.DoAction<CraftController>(args);
                }
            }
        }
        public class Pick : PlayerEvent
        {
            public override string name
            {
                get => "Pick";
            }
            //params:
            //target
            public override void fn(GameObject gameObject, params object[] args)
            {
                ActionController actionController = gameObject.GetComponent<ActionController>();
                actionController.DoAction<CraftController>(args);
            }
        }
        public class AutoAttack : PlayerEvent
        {
            public override string name
            {
                get => "AutoAttack";
            }
            //params:
            //
            public override void fn(GameObject gameObject, params object[] args)
            {
                ActionController actionController = gameObject.GetComponent<ActionController>();
                if(!actionController.isDoing<AttackController>()) actionController.DoAction<AttackController>(args);
            }
        }
        public class Attack : PlayerEvent
        {
            public override string name
            {
                get => "Attack";
            }
            //params:
            //target
            public override void fn(GameObject gameObject, params object[] args)
            {
                ActionController actionController = gameObject.GetComponent<ActionController>();
                actionController.DoAction<AttackController>(args);
            }
        }
        public class GetHit : PlayerEvent
        {
            public override int Level
            {
                get => 2;
            }
            public override string name
            {
                get => "GetHit";
            }
            //params:
            //attacker damage
            public override void fn(GameObject gameObject, params object[] args)
            {
                ActionController actionController = gameObject.GetComponent<ActionController>();
                Combat combat = gameObject.GetComponent<Combat>();
                combat.GetHit(args[0] as GameObject, (float)args[1]);
                Health health = gameObject.GetComponent<Health>();
                if(health.health > 0)
                {
                    actionController.DoAction<GetHitController>();
                } 
                else gameObject.GetComponent<EventHandler>().RaiseEvent("Die");
            }
        }
        public class Die : PlayerEvent
        {
            public override int Level
            {
                get => 3;
            }
            public override string name
            {
                get => "Die";
            }

            public override void fn(GameObject gameObject, params object[] args)
            {
                Debug.Log("Player Die");
                GameObject.Destroy(gameObject);
            }
        }
        public class Drop : PlayerEvent
        {
            public override string name
            {
                get => "Drop";
            }

            public override void fn(GameObject gameObject, params object[] args)
            {
                ActionController actionController = gameObject.GetComponent<ActionController>();
                actionController.DoAction<DropController>(args);
            }
        }
        public class UnEquip : PlayerEvent
        {
            public override string name
            {
                get => "UnEquip";
            }

            public override void fn(GameObject gameObject, params object[] args)
            {
                ActionController actionController = gameObject.GetComponent<ActionController>();
                actionController.DoAction<UnEquipController>(args);
            }
        }
    }
}