using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using ParadoxNotion;

namespace NodeCanvas.Tasks.Actions
{
    [Category("Movement/Direct")]
    [Description("Moves the agent towards to target per frame without pathfinding")]
    public class MoveTowardsVector3 : ActionTask<Transform>
    {

        [RequiredField]
        public BBParameter<Vector3> target;
        public BBParameter<float> speed = 2;
        public BBParameter<float> stopDistance = 0.1f;
        public bool waitActionFinish;

        protected override void OnUpdate()
        {
            if ((agent.position - target.value).magnitude <= stopDistance.value)
            {
                EndAction();
                return;
            }

            agent.position = Vector3.MoveTowards(agent.position, target.value, speed.value * Time.deltaTime);
            if (!waitActionFinish)
            {
                EndAction();
            }
        }
    }

    [Name("TargetPosition Within Distance")]
    [Category("GameObject")]
    public class CheckDistanceToPosition : ConditionTask<Transform>
    {

        [RequiredField]
        public BBParameter<Vector3> checkTarget;
        public CompareMethod checkType = CompareMethod.LessThan;
        public BBParameter<float> distance = 10;

        [SliderField(0, 0.1f)]
        public float floatingPoint = 0.05f;

        protected override string info
        {
            get { return "Distance" + OperationTools.GetCompareString(checkType) + distance + " to " + checkTarget; }
        }

        protected override bool OnCheck()
        {
            return OperationTools.Compare(Vector3.Distance(agent.position, checkTarget.value), distance.value, checkType, floatingPoint);
        }

        public override void OnDrawGizmosSelected()
        {
            if (agent != null)
            {
                Gizmos.DrawWireSphere(agent.position, distance.value);
            }
        }
    }

}