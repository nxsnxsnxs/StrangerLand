using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

namespace Actions
{
    public class ACScorpion
    {
        public class Run : CommonAction.Move
        {
            public override string actionName => "Run";
            protected override string moveAnimTrigger => "Run";
            protected override string stopmoveAnimTrigger => "StopRun";
            protected override float moveSpeed => Constants.scorpion_run_speed;
        }
        public class PinchAttack : CommonAction.Attack
        {
            string[] animStrs = new string[]{"Attack1", "Attack2"};
            float[] weights = new float[]{0.5f, 1};
            protected override string attackAnimStr
            {
                get
                {
                    return ToolMethod.GetRandomChoice<string>(animStrs, weights);
                }
            }
        }
        public class DoublePinchAttack : CommonAction.DoubleAttack
        {
            protected override string attackAnimStr => "DoublePinchAttack";
            protected override string stopAttackAnimStr => "StopDoublePinchAttack";
        }
        public class TelsonAttack : CommonAction.Attack
        {
            public override string actionName => "TelsonAttack";
            protected override string attackAnimStr => "TelsonAttack";
            protected override string stopAttackAnimStr => "StopTelsonAttack";
        }
    }
}