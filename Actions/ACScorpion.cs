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
                get => randomAttackStr;
            }
            protected string randomAttackStr;
            public override void Begin(params object[] args)
            {
                randomAttackStr = ToolMethod.GetRandomChoice<string>(animStrs, weights);
                base.Begin(args);
            }
        }
        public class DoublePinchAttack : CommonAction.DoubleAttack
        {
            protected override string attackAnimStr => "DoublePinchAttack";
        }
        public class TelsonAttack : CommonAction.Attack
        {
            public override string actionName => "TelsonAttack";
            protected override string attackAnimStr => "TelsonAttack";
            protected override string stopAttackAnimStr => "StopTelsonAttack";
        }
    }
}