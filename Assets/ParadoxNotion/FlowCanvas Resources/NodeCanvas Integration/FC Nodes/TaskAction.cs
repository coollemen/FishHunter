﻿using System.Collections;
using System.Reflection;
using System.Linq;
using NodeCanvas.Framework;
using NodeCanvas;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes{

	///Task Action is used to run ActionTasks within the flowscript in a simplified manner without exposing ports
    [Category("Functions")]
    [ContextDefinedInputs(typeof(Flow))]
    [ContextDefinedOutputs(typeof(Flow))]
    [Description("Execute an encapsulated action without exposing any value ports")]
	public class TaskAction : FlowNode, ITaskAssignable<ActionTask>,IUpdatable {
		
		[SerializeField]
		private ActionTask _action;

		private FlowOutput fOut;
		private Coroutine coroutine;

		public override string name{
			get {return action != null? action.name : "Action";}
		}

		public ActionTask action{
			get {return _action;}
			set
			{
				if (_action != value){
					_action = value;
					GatherPorts();
				}
			}
		}

		public Task task{
			get {return action;}
			set {action = (ActionTask)value;}
		}

		public override void OnGraphStarted(){ coroutine = null; }
		public override void OnGraphStoped(){
			if (coroutine != null){
				StopCoroutine(coroutine);
				coroutine = null;
			}

			if (action != null){
				action.EndAction(null);
			}
		}

		public override void OnGraphPaused(){
			if (action != null){
				action.PauseAction();
			}
		}
        private bool triggered;
        protected override void RegisterPorts(){
			fOut = AddFlowOutput(" ");
			AddFlowInput(" ", (f)=>
			{
				if (action == null){
					fOut.Call(f);
					return;
				}
                Enter(f);
			});
		}

        Flow flow;
        void Enter(Flow f)
        {
            SetStatus(Status.Running);
            triggered = true;
            flow = f;
        }

        void IUpdatable.Update()
        {
            if (triggered && !(graph.isPaused || action.ExecuteAction(graphAgent, graphBlackboard) == Status.Running))
            {
                Exit();
            }
        }

        void Exit()
        {
            triggered = false;
            fOut.Call(flow);
            SetStatus(Status.Resting);
        }
  //      IEnumerator DoUpdate(Flow f){
		//	SetStatus(Status.Running);
		//	while(graph.isPaused || action.ExecuteAction(graphAgent, graphBlackboard) == Status.Running){
		//		yield return null;
		//	}
			
		//	coroutine = null;
		//	fOut.Call(f);
		//	SetStatus(Status.Resting);
		//}
		
	}
}