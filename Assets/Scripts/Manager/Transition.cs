using System.Collections;
using System.Collections.Generic;
using DATN2.Assets.Scripts.BehaviorEditor.State;
using DATN2.Scripts.BehaviorEditor.State;
using UnityEngine;
namespace DATN2.Assets.Scripts.Manager
{
    public class Transition
    {
        public Condition condition;
        public State targetState;
        public bool disable;
    }

}