using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using UnityEditor.Animations;
using Actions;

namespace Actions
{
    public class ACGolemEarth
    {
        public class Dash : CommonAction.Move
        {
            public override string actionName
            {
                get => "Dash";
            }
            protected override string moveAnimTrigger
            {
                get => "Dash";
            }
            protected override string stopmoveAnimTrigger
            {
                get => "StopDash";
            }
            protected override float moveSpeed => Constants.golem_dash_speed;
        }
        public class SpinAttack : CommonAction.DoubleAttack
        {
            public override string actionName
            {
                get => "SpinAttack";
            }
            protected override string attackAnimStr
            {
                get => "SpinAttack";
            }
        }
    }
}