using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
    public class Delay : FlowControlNode, IUpdatable
    {
        private ValueInput<float> timeInput;
        private FlowOutput onFinish;

        private bool triggered;
        private Flow flow;
        private float willExitTime;
        private float leftTime;

        public override string name
        {
            get
            {
                return string.Format("{0}[{1:N1}/{2:N1}]","Delay" , leftTime, timeInput.value);
            }
        }

        protected override void RegisterPorts() {
            timeInput = AddValueInput<float>("Time");
            onFinish = AddFlowOutput("Finish");
            AddFlowInput("In", Enter);
        }

        void Enter(Flow f) {
            SetStatus(Status.Running);
            triggered = true;
            flow = f;
            willExitTime = Time.time + timeInput.value;
        }

        void IUpdatable.Update() {
            if ( triggered && Time.time > willExitTime ) {
                Exit();
            }
            else if(triggered)
            {
                leftTime = willExitTime - Time.time;
            }
        }

        void Exit() {
            leftTime = 0;
            triggered = false;
            SetStatus(Status.Resting);
            onFinish.Call(flow);
        }
    }
}