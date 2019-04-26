using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using NodeCanvas.Editor;
#endif
using NodeCanvas.Framework;
using UnityEngine;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using ParadoxNotion;
using Logger = ParadoxNotion.Services.Logger;

namespace FlowCanvas.Nodes{
	[Name("AttachEvent", 102)]
	[Description("Attach a CustomEvent From GraphOwner.")]
	[Category("Events/Custom")]
	[ContextDefinedInputs(typeof(GraphOwner),typeof(GameObject),typeof(Transform),typeof(Flow),typeof(CustomEventListener))]
	[Color("c02520")]
	public class AttachCustomEvent : FlowNode {		
		FlowInput inPutFlow;
		FlowOutput outPutFlow;
		ValueInput<CustomEventListener> eventListener;
		ValueInput<GraphOwner> target;
		ValueInput<List<GraphOwner>> targetList;
		
		[HideInInspector]
		[SerializeField]
		private TargetMode targetMode;

		public TargetMode _TargetMode{
			get {
				return targetMode;
			}
			set {
				if (targetMode!=value)
				{
					targetMode=value;
					GatherPorts();
				}
			}
		}
		
		protected override void RegisterPorts()
		{	
			eventListener=AddValueInput<CustomEventListener>("EventListener","");
			if (targetMode==0)
			{
				target=AddValueInput<GraphOwner>("AttachGraphOwner");
			}else
			{
				targetList=AddValueInput<List<GraphOwner>>("AttachGraphOwners");
			}

			outPutFlow=AddFlowOutput("Out","");
			AddFlowInput("In",(f)=>{
				
				eventListener.value.targetMode=targetMode;
				if (targetMode== 0)
				{	
					if (eventListener.value.targets!=null&&eventListener.value.targets.value!=null&&eventListener.value.targets.value.Count>1)
					{
						eventListener.value.targetMode=TargetMode.MultipleTargets;
					}
					
					if (target.value==null||target.value==eventListener.value.target.value)
					{

					}else
					{	
						if (eventListener.value.target.value==null)
						{
							eventListener.value.target.value=target.value;
						}else
						{
							eventListener.value.targetMode=TargetMode.MultipleTargets;
							
							if (eventListener.value.targets.value==null)
							{	
								eventListener.value.targets.value=new List<GraphOwner>();
							}
							if (!eventListener.value.targets.value.Contains(target.value))
							{	
								eventListener.value.targets.value.Add(target.value);
							}
							if (eventListener.value.target.value!=null&&!eventListener.value.targets.value.Contains(eventListener.value.target.value))
							{	
								eventListener.value.targets.value.Add(eventListener.value.target.value);
							}		
						}							
					}

				}else
				{	
					eventListener.value.targets.value=targetList.value;	
				}
		
				eventListener.value.Register();
				outPutFlow.Call(f);
			});
		}
		#if UNITY_EDITOR
		protected override void OnNodeInspectorGUI()
		{
			base.OnNodeInspectorGUI();
			_TargetMode= (TargetMode)UnityEditor.EditorGUILayout.EnumPopup("AttachTargetMode",targetMode);		
		}
		#endif
		
	}	
	
	[Name("DetachAllEvent", 103)]
	[Description("Detach a CustomEvent From GraphOwner.")]
	[Category("Events/Custom")]
	[ContextDefinedInputs(typeof(GraphOwner),typeof(GameObject),typeof(Transform),typeof(Flow),typeof(CustomEventListener))]
	[Color("c02520")]
	public class DetachAllCustomEvent : FlowNode {		
		FlowInput inPutFlow;
		FlowOutput outPutFlow;
		ValueInput<CustomEventListener> eventListener;
		
		protected override void RegisterPorts()
		{	
			eventListener=AddValueInput<CustomEventListener>("EventListener","");

			outPutFlow=AddFlowOutput("Out","");
			AddFlowInput("In",(f)=>{
					
			eventListener.value.UnRegister();
			//eventListener.value.target=null;
			//eventListener.value.targets=null;
			outPutFlow.Call(f);
			});
		}
	}
	
	[Name("DetachEventFromGraphOwner", 104)]
	[Description("Detach a CustomEvent From GraphOwner.")]
	[Category("Events/Custom")]
	[ContextDefinedInputs(typeof(GraphOwner),typeof(GameObject),typeof(Transform),typeof(Flow),typeof(CustomEventListener))]
	[Color("c02520")]
	public class DetachCustomEvent1 : FlowNode {		
		FlowInput inPutFlow;
		FlowOutput outPutFlow;
		ValueInput<CustomEventListener> eventListener;
		ValueInput<GraphOwner> graphOwner;
		
		protected override void RegisterPorts()
		{	
			eventListener=AddValueInput<CustomEventListener>("EventListener","");
			graphOwner=AddValueInput<GraphOwner>("GraphOwner","");

			outPutFlow=AddFlowOutput("Out","");
			AddFlowInput("In",(f)=>{

				if(eventListener.value.targetMode==TargetMode.MultipleTargets)
				{	
					if(graphOwner.value!=null&&eventListener.value.targets.value.Contains(graphOwner.value))
					{
						eventListener.value.targets.value.Remove(graphOwner.value);

					}
					
					if (eventListener.value.targets.value.Count==1)
					{
						eventListener.value.target.value=eventListener.value.targets.value[0];
						eventListener.value.targets.value=null;
						eventListener.value.targetMode=TargetMode.SingleTarget;
						eventListener.value.UnRegister();
						eventListener.value.Register();
						
					}else if (eventListener.value.targets.value==null||eventListener.value.targets.value.Count==0)
					{	
						eventListener.value.target.value=null;
						eventListener.value.targets.value=null;
						eventListener.value.targetMode=TargetMode.SingleTarget;
						eventListener.value.UnRegister();
					}else
					{	
						eventListener.value.UnRegister();
						eventListener.value.Register();
					}
				}else
				{
					eventListener.value.UnRegister();
				}

				outPutFlow.Call(f);
			});
		}
	}
	[Name("DetachEventFromGraphOwnerList", 105)]
	[Description("Detach a CustomEvent From GraphOwner.")]
	[Category("Events/Custom")]
	[ContextDefinedInputs(typeof(GraphOwner),typeof(GameObject),typeof(Transform),typeof(Flow),typeof(CustomEventListener))]
	[Color("c02520")]
	public class DetachCustomEvent2 : FlowNode {		
		FlowInput inPutFlow;
		FlowOutput outPutFlow;
		ValueInput<CustomEventListener> eventListener;
		ValueInput<List<GraphOwner>> graphOwners;
		
		protected override void RegisterPorts()
		{	
			eventListener=AddValueInput<CustomEventListener>("EventListener","");
			graphOwners=AddValueInput<List<GraphOwner>>("GraphOwner","");

			outPutFlow=AddFlowOutput("Out","");
			AddFlowInput("In",(f)=>{
				if(eventListener.value.targetMode==TargetMode.MultipleTargets)
				{	
					graphOwners.value.ForEach(graphOwner=>{
					if(graphOwner!=null&&eventListener.value.targets.value.Contains(graphOwner))
					{
						eventListener.value.targets.value.Remove(graphOwner);
					}

					});
					
										
					if (eventListener.value.targets.value.Count==1)
					{
						eventListener.value.target.value=eventListener.value.targets.value[0];
						eventListener.value.targets.value=null;
						eventListener.value.targetMode=TargetMode.SingleTarget;
						eventListener.value.UnRegister();
						eventListener.value.Register();
						
					}else if (eventListener.value.targets.value==null||eventListener.value.targets.value.Count==0)
					{	
						eventListener.value.target.value=null;
						eventListener.value.targets.value=null;
						eventListener.value.targetMode=TargetMode.SingleTarget;
						eventListener.value.UnRegister();
					}else
					{
						eventListener.value.UnRegister();
						eventListener.value.Register();
					}
				}else
				{					
					graphOwners.value.ForEach(graphOwner=>{
					if(graphOwner!=null&&eventListener.value.target.value==graphOwner)
					{
						eventListener.value.targets.value=null;
						eventListener.value.UnRegister();
					}
					});
				}

				outPutFlow.Call(f);
			});
		}
	}
	
	[Name("Event_listenr", 100)]
	[Description("Called when a attach event is received on target(s).\n- Receiver, is the object which received the event.\n- Sender, is the object which invoked the event.\n\n- To send an event from a graph use the SendEvent node.\n- To send an event from code use: 'FlowScriptController.SendEvent(string)'")]
	[Category("Events/Custom")]
	[ContextDefinedOutputs(typeof(CustomEventListener))]
	public class CustomEventListener : MessageEventNode<GraphOwner> {
		
		[RequiredField] [DelayedField]
		public BBParameter<string> eventName = "EventName";

		private FlowOutput onReceived;
		private GraphOwner sender;
		private GraphOwner receiver;

		public override string name{
			get {return base.name + string.Format(" [ <color=#DDDDDD>{0}</color> ]", eventName); }
		}

		protected override string[] GetTargetMessageEvents(){
			return new string[]{ "OnCustomEvent" };
		}
		
		public override void OnGraphStarted()
		{
			//base.OnGraphStarted();
		}
		public override void OnGraphStoped()
		{
			//base.OnGraphStarted();
		}
		
		protected override void RegisterPorts(){
			AddValueOutput<CustomEventListener>("Event",()=>this);
			onReceived = AddFlowOutput("Dispatched");
			AddValueOutput<GraphOwner>("Receiver", ()=>{ return receiver; });
			AddValueOutput<GraphOwner>("Sender", ()=>{ return sender; });
		}

		public virtual void OnCustomEvent(ParadoxNotion.Services.MessageRouter.MessageData<EventData> msg){
			if (msg.value.name.ToUpper() == eventName.value.ToUpper()){
				var senderGraph = Graph.GetElementGraph(msg.sender);
				this.sender = senderGraph != null? senderGraph.agent as GraphOwner : null;
				this.receiver = ResolveReceiver(msg.receiver);

				#if UNITY_EDITOR
				if (NodeCanvas.Editor.Prefs.logEvents){
					Logger.Log(string.Format("Event Received from ({0}): '{1}'", receiver.name, msg.value.name), "Event", this);
				}
				#endif

				onReceived.Call(new Flow());
			}
		}
	}
	
	[Name("Event_listenr", 101)]
	[Description("Called when a attach event is received on target(s).\n- Receiver, is the object which received the event.\n- Sender, is the object which invoked the event.\n\n- To send an event from a graph use the SendEvent node.\n- To send an event from code use: 'FlowScriptController.SendEvent(string)'")]
	[Category("Events/Custom")]
	[ContextDefinedOutputs(typeof(CustomEventListener))]
	public class CustomEventListener<T> : CustomEventListener {
		private FlowOutput onReceived;
		private GraphOwner sender;
		private GraphOwner receiver;
		private T receivedValue;
		
		public override string name{
			get {return base.name + string.Format(" <color=#DDDDDD><{0}></color>",typeof(T).FriendlyName()); }
		}
		protected override void RegisterPorts(){
			AddValueOutput<CustomEventListener>("Event",()=>(CustomEventListener)this);
			onReceived = AddFlowOutput("Dispatched");
			AddValueOutput<GraphOwner>("Receiver", ()=>{ return receiver; });
			AddValueOutput<GraphOwner>("Sender", ()=>{ return sender; });
			AddValueOutput<T>("Event Value", ()=> { return receivedValue; });
		}

		public override void OnCustomEvent(ParadoxNotion.Services.MessageRouter.MessageData<EventData> msg){
			if (msg.value.name.ToUpper() == eventName.value.ToUpper()){
				var senderGraph = Graph.GetElementGraph(msg.sender);
				this.sender = senderGraph != null? senderGraph.agent as GraphOwner : null;
				this.receiver = ResolveReceiver(msg.receiver);

				var eventType = msg.value.GetType();
				if (eventType.RTIsGenericType()){
					var valueType = eventType.RTGetGenericArguments().FirstOrDefault();
					if (typeof(T).RTIsAssignableFrom(valueType)){
						receivedValue = (T)msg.value.value;
					}
				}

				#if UNITY_EDITOR
				if (NodeCanvas.Editor.Prefs.logEvents){
					Logger.Log(string.Format("Event Received from ({0}): '{1}'", receiver.name, msg.value.name), "Event", this);
				}
				#endif

				onReceived.Call(new Flow());
			}
		}
	}
}
