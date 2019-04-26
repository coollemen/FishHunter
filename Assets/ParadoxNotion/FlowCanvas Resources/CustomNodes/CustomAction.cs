using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using FlowCanvas;
using FlowCanvas.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace NodeCanvas.Tasks.Actions{
	[Category("FlowScript")]
	public class CallFunction<T> : ActionTask<FlowScriptController> {
		
		public BBParameter<string> FunctionName;
		public BBParameter<object> parameter1;
		public BBParameter<object> parameter2;
		public BBParameter<object> parameter3;
		public BBParameter<object> parameter4;
		public BBParameter<object> parameter5;

		public BBParameter<T> saveAs;
		
		protected override string info{
			get {return "CallFuntion:<b><color=yellow>"+(string.IsNullOrEmpty(FunctionName.value)?"NULL":FunctionName.value) +"</color></b> <<b><color=yellow> " + typeof(T).Name +" </color></b>> SaveAs "+saveAs ; }
		}
		bool IsParameterNull()
		{
			return parameter1.value!=null||parameter2.value!=null||parameter3.value!=null||parameter4.value!=null||parameter5.value!=null;
		}
		protected override void OnExecute(){
			if (IsParameterNull())
			{
				saveAs.value= agent.CallFunction<T>(FunctionName.value,parameter1.value,parameter2.value,parameter3.value,parameter4.value,parameter5.value);
			}else
			{
				saveAs.value= agent.CallFunction<T>(FunctionName.value,null);
			}
			EndAction();
		}

        #region InspectorGUI
        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
        #if UNITY_EDITOR
        Dictionary<Graph,CustomFunctionEvent> sourceFunctionNodeDict=new Dictionary<Graph,CustomFunctionEvent>();
        protected override void OnTaskInspectorGUI()
        {
            base.OnTaskInspectorGUI();
            if (!string.IsNullOrEmpty(FunctionName.value) && GUILayout.Button("Find Source FunctionNode"))
            {
                List<Graph> allGraph = new List<Graph>();
                sourceFunctionNodeDict = new Dictionary<Graph, CustomFunctionEvent>();
#if UNITY_2018_3_OR_NEWER
                var prefabType = UnityEditor.PrefabUtility.GetPrefabAssetType(agent.gameObject);
                if (prefabType != UnityEditor.PrefabAssetType.NotAPrefab|| prefabType != UnityEditor.PrefabAssetType.MissingAsset)

#else
                if (UnityEditor.PrefabUtility.GetPrefabType(agent.gameObject) == UnityEditor.PrefabType.Prefab)
#endif
                {
                    var graphOwners = agent.GetComponents<GraphOwner>();

                    graphOwners.ForEach(x =>
                    {
                        if (x.graph != null)
                        {
                            allGraph.Add(x.graph);
                            var childGraph = x.graph.GetAllNestedGraphs<Graph>(true);
                            if (childGraph.Count > 0)
                            {
                                allGraph.AddRange(childGraph);
                            }
                        }
                    });
                }
                else
                {
                    var allGraphOwner = UnityEngine.Object.FindObjectsOfType<GraphOwner>();
                    allGraphOwner.ForEach(x => {
                        if (x.graph != null)
                        {
                            var rootGraph = x.graph;
                            allGraph.Add(rootGraph);
                            var childGraph = rootGraph.GetAllNestedGraphs<Graph>(true);
                            if (childGraph.Count > 0)
                            {
                                allGraph.AddRange(childGraph);
                            }
                        }
                    });
                }

                allGraph.ForEach(y =>
                {
                    var targetFunctionNode = y.GetAllNodesOfType<CustomFunctionEvent>().FirstOrDefault(z => z.identifier.value == FunctionName.value);
                    if (targetFunctionNode != null)
                    {
                        if(!sourceFunctionNodeDict.ContainsKey(y))
                            sourceFunctionNodeDict.Add(y, targetFunctionNode);
                    }
                });
            }

            if (sourceFunctionNodeDict.Count > 0)
            {
                GUILayout.BeginVertical();
                foreach (var item in sourceFunctionNodeDict)
                {
                    if (GUILayout.Button(string.Format("functionNode:{0} in graph:{1}", item.Value.identifier.value, item.Key.name)))
                    {
                        if (item.Key.agent.gameObject != null)
                        {
                            UnityEditor.Selection.activeGameObject = item.Key.agent.gameObject;
                            UnityEditor.Selection.selectionChanged();
                        }
                        UnityEditor.Selection.activeObject = item.Key;
                        UnityEditor.Selection.selectionChanged();

                        if (ParadoxNotion.Services.MonoManager.current == null)
                        {
                            var _current = UnityEngine.Object.FindObjectOfType<ParadoxNotion.Services.MonoManager>();
                            if (_current != null)
                            {
                                UnityEngine.Object.DestroyImmediate(_current.gameObject);
                            }

                            var current = new GameObject("_MonoManager").AddComponent<ParadoxNotion.Services.MonoManager>();
                            current.StartCoroutine(waitForGraphChange(item.Value));
                        }
                        else
                        {
                            ParadoxNotion.Services.MonoManager.current.StartCoroutine(waitForGraphChange(item.Value));
                        }

                    }
                }
                GUILayout.EndVertical();
            }
        }

        System.Collections.IEnumerator waitForGraphChange(FlowNode n)
        {
            //Debug.Log("double press F to focus:" + n
            yield return new WaitForSecondsRealtime(0.15f);
            NodeCanvas.Editor.GraphEditorUtility.activeElement = n;
        }            
#endif

#endregion
            }
	
	[Category("FlowScript")]
	public class CallFunctionAction : ActionTask<FlowScriptController> {
	
		public BBParameter<string> FunctionName;
		public BBParameter<object> parameter1;
		public BBParameter<object> parameter2;
		public BBParameter<object> parameter3;
		public BBParameter<object> parameter4;
		public BBParameter<object> parameter5;

		
		protected override string info{
			get {return "CallFuntion: <b><color=yellow>"+FunctionName+"</color></b>" ; }
		}
		
		bool IsParameterNull()
		{
			return parameter1.value!=null||parameter2.value!=null||parameter3.value!=null||parameter4.value!=null||parameter5.value!=null;
		}
		protected override void OnExecute(){
			if (IsParameterNull())
			{
				agent.CallFunction(FunctionName.value,parameter1.value,parameter2.value,parameter3.value,parameter4.value,parameter5.value);
			}else
			{
				agent.CallFunction(FunctionName.value,null);
			}
			EndAction();
		}

        #region InspectorGUI
        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR
        Dictionary<Graph, CustomFunctionEvent> sourceFunctionNodeDict = new Dictionary<Graph, CustomFunctionEvent>();
        protected override void OnTaskInspectorGUI()
        {
            base.OnTaskInspectorGUI();
            if (!string.IsNullOrEmpty(FunctionName.value) && GUILayout.Button("Find Source FunctionNode"))
            {   
                List<Graph> allGraph = new List<Graph>();
                sourceFunctionNodeDict = new Dictionary<Graph, CustomFunctionEvent>();
#if UNITY_2018_3_OR_NEWER
                var prefabType = UnityEditor.PrefabUtility.GetPrefabAssetType(agent.gameObject);
                if (prefabType != UnityEditor.PrefabAssetType.NotAPrefab || prefabType != UnityEditor.PrefabAssetType.MissingAsset)

#else
                if (UnityEditor.PrefabUtility.GetPrefabType(agent.gameObject) == UnityEditor.PrefabType.Prefab)
#endif
                {
                    var graphOwners = agent.GetComponents<GraphOwner>();

                    graphOwners.ForEach(x =>
                    {
                        if (x.graph != null)
                        {
                            allGraph.Add(x.graph);
                            var childGraph = x.graph.GetAllNestedGraphs<Graph>(true);
                            if (childGraph.Count > 0)
                            {
                                allGraph.AddRange(childGraph);
                            }
                        }
                    });
                }else{
                    var allGraphOwner = UnityEngine.Object.FindObjectsOfType<GraphOwner>();
                    allGraphOwner.ForEach(x => {
                        if (x.graph != null)
                        {
                            var rootGraph = x.graph;
                            allGraph.Add(rootGraph);
                            var childGraph = rootGraph.GetAllNestedGraphs<Graph>(true);
                            if (childGraph.Count > 0)
                            {
                                allGraph.AddRange(childGraph);
                            }
                        }
                    });
                }

                allGraph.ForEach(y =>
                {
                    var targetFunctionNode = y.GetAllNodesOfType<CustomFunctionEvent>().FirstOrDefault(z => z.identifier.value == FunctionName.value);
                    if (targetFunctionNode != null)
                    {   
                        if(!sourceFunctionNodeDict.ContainsKey(y))
                            sourceFunctionNodeDict.Add(y, targetFunctionNode);
                    }
                });
            }

            if (sourceFunctionNodeDict.Count > 0)
            {
                GUILayout.BeginVertical();
                foreach (var item in sourceFunctionNodeDict)
                {
                    if (GUILayout.Button(string.Format("functionNode:{0} in graph:{1}", item.Value.identifier.value, item.Key.name)))
                    {

                        if (item.Key.agent.gameObject != null)
                        {
                            UnityEditor.Selection.activeGameObject = item.Key.agent.gameObject;
                            UnityEditor.Selection.selectionChanged();
                        }
                        UnityEditor.Selection.activeObject = item.Key;
                        UnityEditor.Selection.selectionChanged();


                        if (ParadoxNotion.Services.MonoManager.current == null)
                        {
                            var _current = UnityEngine.Object.FindObjectOfType<ParadoxNotion.Services.MonoManager>();
                            if (_current != null)
                            {
                                UnityEngine.Object.DestroyImmediate(_current.gameObject);
                            }

                            var current = new GameObject("_MonoManager").AddComponent<ParadoxNotion.Services.MonoManager>();
                            current.StartCoroutine(waitForGraphChange(item.Value));
                        }
                        else
                        {
                            ParadoxNotion.Services.MonoManager.current.StartCoroutine(waitForGraphChange(item.Value));
                        }

                    }
                }
                GUILayout.EndVertical();
            }
        }

        System.Collections.IEnumerator waitForGraphChange(FlowNode n)
        {
            //Debug.Log("double press F to focus:" + n
            yield return new WaitForSecondsRealtime(0.15f);
            NodeCanvas.Editor.GraphEditorUtility.activeElement = n;
        }
#endif

        #endregion
	}

    [Category("FlowScript")]
    public class CallFunctionActionSync : ActionTask<FlowScriptController> {

        public BBParameter<string> FunctionName;
        public BBParameter<object> parameter1;
        public BBParameter<object> parameter2;
        public BBParameter<object> parameter3;
        public BBParameter<object> parameter4;
        public BBParameter<object> parameter5;


        protected override string info{
            get {return "CallFuntionSync: <b><color=yellow>"+FunctionName+"</color></b>" ; }
        }
	    bool IsParameterNull()
	    {
		    return parameter1.value!=null||parameter2.value!=null||parameter3.value!=null||parameter4.value!=null||parameter5.value!=null;
	    }
	    protected override void OnExecute(){
		    if (IsParameterNull())
		    {
            	agent.CallFunctionSync(()=>EndAction(),FunctionName.value,parameter1.value,parameter2.value,parameter3.value,parameter4.value,parameter5.value);
		    }else
		    {
			    agent.CallFunctionSync(()=>EndAction(),FunctionName.value,null);
		    }
		}
        #region InspectorGUI
        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
        #if UNITY_EDITOR
        Dictionary<Graph,CustomFunctionEvent> sourceFunctionNodeDict=new Dictionary<Graph,CustomFunctionEvent>();
        protected override void OnTaskInspectorGUI()
        {

            base.OnTaskInspectorGUI();
            if (!string.IsNullOrEmpty(FunctionName.value) && GUILayout.Button("Find Source FunctionNode"))
            {
                List<Graph> allGraph = new List<Graph>();
                sourceFunctionNodeDict = new Dictionary<Graph, CustomFunctionEvent>();
#if UNITY_2018_3_OR_NEWER
                var prefabType = UnityEditor.PrefabUtility.GetPrefabAssetType(agent.gameObject);
                if (prefabType != UnityEditor.PrefabAssetType.NotAPrefab || prefabType != UnityEditor.PrefabAssetType.MissingAsset)

#else
                if (UnityEditor.PrefabUtility.GetPrefabType(agent.gameObject) == UnityEditor.PrefabType.Prefab)
#endif
                {
                    var graphOwners = agent.GetComponents<GraphOwner>();

                    graphOwners.ForEach(x =>
                    {
                        if (x.graph != null)
                        {
                            allGraph.Add(x.graph);
                            var childGraph = x.graph.GetAllNestedGraphs<Graph>(true);
                            if (childGraph.Count > 0)
                            {
                                allGraph.AddRange(childGraph);
                            }
                        }
                    });
                }
                else
                {
                    var allGraphOwner = UnityEngine.Object.FindObjectsOfType<GraphOwner>();
                    allGraphOwner.ForEach(x =>
                    {
                        if (x.graph != null)
                        {
                            var rootGraph = x.graph;
                            allGraph.Add(rootGraph);
                            var childGraph = rootGraph.GetAllNestedGraphs<Graph>(true);
                            if (childGraph.Count > 0)
                            {
                                allGraph.AddRange(childGraph);
                            }
                        }
                    });
                }

                allGraph.ForEach(y =>
                {
                    var targetFunctionNode = y.GetAllNodesOfType<CustomFunctionEvent>().FirstOrDefault(z => z.identifier.value == FunctionName.value);
                    if (targetFunctionNode != null)
                    {
                        if(!sourceFunctionNodeDict.ContainsKey(y))
                            sourceFunctionNodeDict.Add(y, targetFunctionNode);
                    }
                });
            }

            if (sourceFunctionNodeDict.Count > 0)
            {
                GUILayout.BeginVertical();
                foreach (var item in sourceFunctionNodeDict)
                {
                    if (GUILayout.Button(string.Format("functionNode:{0} in graph:{1}", item.Value.identifier.value, item.Key.name)))
                    {
                        if (item.Key.agent.gameObject != null)
                        {
                            UnityEditor.Selection.activeGameObject = item.Key.agent.gameObject;
                            UnityEditor.Selection.selectionChanged();
                        }
                        UnityEditor.Selection.activeObject = item.Key;
                        UnityEditor.Selection.selectionChanged();

                        if (ParadoxNotion.Services.MonoManager.current == null)
                        {
                            var _current = UnityEngine.Object.FindObjectOfType<ParadoxNotion.Services.MonoManager>();
                            if (_current != null)
                            {
                                UnityEngine.Object.DestroyImmediate(_current.gameObject);
                            }

                            var current = new GameObject("_MonoManager").AddComponent<ParadoxNotion.Services.MonoManager>();
                            current.StartCoroutine(waitForGraphChange(item.Value));
                        }
                        else
                        {
                            ParadoxNotion.Services.MonoManager.current.StartCoroutine(waitForGraphChange(item.Value));
                        }

                    }
                }
                GUILayout.EndVertical();
            }
        }
        System.Collections.IEnumerator waitForGraphChange(FlowNode n)
        {
            //Debug.Log("double press F to focus:" + n
            yield return new WaitForSecondsRealtime(0.15f);
            NodeCanvas.Editor.GraphEditorUtility.activeElement = n;
        }            
        #endif

        #endregion
    }

    [Category("FlowScript")]
    public class CallFunctionActionSync<T> : ActionTask<FlowScriptController> {

        public BBParameter<string> FunctionName;
        public BBParameter<object> parameter1;
        public BBParameter<object> parameter2;
        public BBParameter<object> parameter3;
        public BBParameter<object> parameter4;
        public BBParameter<object> parameter5;

        public BBParameter<T> saveAs;

        protected override string info{
            get {return "CallFuntionSync: <b><color=yellow>"+FunctionName+"</color></b>" ; }
        }
	    bool IsParameterNull()
	    {
		    return parameter1.value!=null||parameter2.value!=null||parameter3.value!=null||parameter4.value!=null||parameter5.value!=null;
	    }
	    protected override void OnExecute(){
	    	if (IsParameterNull())
	    	{
		    	agent.CallFunctionSync<T>((x)=>{saveAs.value=x;EndAction();},FunctionName.value,parameter1.value,parameter2.value,parameter3.value,parameter4.value,parameter5.value);
	    	}else
	    	{
		    	agent.CallFunctionSync<T>((x)=>{saveAs.value=x;EndAction();},FunctionName.value,null);
	    	}
        }

        #region InspectorGUI
        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
        #if UNITY_EDITOR
        Dictionary<Graph,CustomFunctionEvent> sourceFunctionNodeDict=new Dictionary<Graph,CustomFunctionEvent>();
        protected override void OnTaskInspectorGUI()
        {
            base.OnTaskInspectorGUI();
            if (!string.IsNullOrEmpty(FunctionName.value) && GUILayout.Button("Find Source FunctionNode"))
            {
                List<Graph> allGraph = new List<Graph>();
                sourceFunctionNodeDict = new Dictionary<Graph, CustomFunctionEvent>();
#if UNITY_2018_3_OR_NEWER
                var prefabType = UnityEditor.PrefabUtility.GetPrefabAssetType(agent.gameObject);
                if (prefabType != UnityEditor.PrefabAssetType.NotAPrefab || prefabType != UnityEditor.PrefabAssetType.MissingAsset)

#else
                if (UnityEditor.PrefabUtility.GetPrefabType(agent.gameObject) == UnityEditor.PrefabType.Prefab)
#endif
                {
                    var graphOwners = agent.GetComponents<GraphOwner>();

                    graphOwners.ForEach(x =>
                    {
                        if (x.graph != null)
                        {
                            allGraph.Add(x.graph);
                            var childGraph = x.graph.GetAllNestedGraphs<Graph>(true);
                            if (childGraph.Count > 0)
                            {
                                allGraph.AddRange(childGraph);
                            }
                        }
                    });
                }
                else
                {
                    var allGraphOwner = UnityEngine.Object.FindObjectsOfType<GraphOwner>();
                    allGraphOwner.ForEach(x => {
                        if (x.graph != null)
                        {
                            var rootGraph = x.graph;
                            allGraph.Add(rootGraph);
                            var childGraph = rootGraph.GetAllNestedGraphs<Graph>(true);
                            if (childGraph.Count > 0)
                            {
                                allGraph.AddRange(childGraph);
                            }
                        }
                    });
                }

                allGraph.ForEach(y =>
                {
                    var targetFunctionNode = y.GetAllNodesOfType<CustomFunctionEvent>().FirstOrDefault(z => z.identifier.value == FunctionName.value);
                    if (targetFunctionNode != null)
                    {
                        if(!sourceFunctionNodeDict.ContainsKey(y))
                            sourceFunctionNodeDict.Add(y, targetFunctionNode);
                    }
                });
            }

            if (sourceFunctionNodeDict.Count > 0)
            {
                GUILayout.BeginVertical();
                foreach (var item in sourceFunctionNodeDict)
                {
                    if (GUILayout.Button(string.Format("functionNode:{0} in graph:{1}", item.Value.identifier.value, item.Key.name)))
                    {
                        if (item.Key.agent.gameObject != null)
                        {
                            UnityEditor.Selection.activeGameObject = item.Key.agent.gameObject;
                            UnityEditor.Selection.selectionChanged();
                        }
                        UnityEditor.Selection.activeObject = item.Key;
                        UnityEditor.Selection.selectionChanged();

                        if (ParadoxNotion.Services.MonoManager.current == null)
                        {
                            var _current = UnityEngine.Object.FindObjectOfType<ParadoxNotion.Services.MonoManager>();
                            if (_current != null)
                            {
                                UnityEngine.Object.DestroyImmediate(_current.gameObject);
                            }

                            var current = new GameObject("_MonoManager").AddComponent<ParadoxNotion.Services.MonoManager>();
                            current.StartCoroutine(waitForGraphChange(item.Value));
                        }
                        else
                        {
                            ParadoxNotion.Services.MonoManager.current.StartCoroutine(waitForGraphChange(item.Value));
                        }

                    }
                }
                GUILayout.EndVertical();
            }
        }

        System.Collections.IEnumerator waitForGraphChange(FlowNode n)
        {
            //Debug.Log("double press F to focus:" + n
            yield return new WaitForSecondsRealtime(0.15f);
            NodeCanvas.Editor.GraphEditorUtility.activeElement = n;
        }            
        #endif

        #endregion
    }
	
	[Category("FlowScript")]
	public class CallFunctionCondition : ConditionTask<FlowScriptController> {
	
		public BBParameter<string> FunctionName;
		public BBParameter<object> parameter1;
		public BBParameter<object> parameter2;
		public BBParameter<object> parameter3;
		public BBParameter<object> parameter4;
		public BBParameter<object> parameter5;

		
		protected override string info{
			get {return "CallFuntion: <b><color=yellow>"+FunctionName+"</color></b>" ; }
		}
		bool IsParameterNull()
		{
			return parameter1.value!=null||parameter2.value!=null||parameter3.value!=null||parameter4.value!=null||parameter5.value!=null;
		}
		protected override bool OnCheck(){
			if (IsParameterNull())
			{
				return agent.CallFunction<bool>(FunctionName.value,parameter1.value,parameter2.value,parameter3.value,parameter4.value,parameter5.value);			
			}else
			{
				return agent.CallFunction<bool>(FunctionName.value,null);			
			}
		}

        #region InspectorGUI
        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
        #if UNITY_EDITOR
        Dictionary<Graph,CustomFunctionEvent> sourceFunctionNodeDict=new Dictionary<Graph,CustomFunctionEvent>();
        protected override void OnTaskInspectorGUI()
        {
            base.OnTaskInspectorGUI();
            if (!string.IsNullOrEmpty(FunctionName.value) && GUILayout.Button("Find Source FunctionNode"))
            {
                List<Graph> allGraph = new List<Graph>();
                sourceFunctionNodeDict = new Dictionary<Graph, CustomFunctionEvent>();
#if UNITY_2018_3_OR_NEWER
                var prefabType = UnityEditor.PrefabUtility.GetPrefabAssetType(agent.gameObject);
                if (prefabType != UnityEditor.PrefabAssetType.NotAPrefab || prefabType != UnityEditor.PrefabAssetType.MissingAsset)

#else
                if (UnityEditor.PrefabUtility.GetPrefabType(agent.gameObject) == UnityEditor.PrefabType.Prefab)
#endif
                {
                    var graphOwners = agent.GetComponents<GraphOwner>();

                    graphOwners.ForEach(x =>
                    {
                        if (x.graph != null)
                        {
                            allGraph.Add(x.graph);
                            var childGraph = x.graph.GetAllNestedGraphs<Graph>(true);
                            if (childGraph.Count > 0)
                            {
                                allGraph.AddRange(childGraph);
                            }
                        }
                    });
                }
                else
                {
                    var allGraphOwner = UnityEngine.Object.FindObjectsOfType<GraphOwner>();
                    allGraphOwner.ForEach(x => {
                        if (x.graph != null)
                        {
                            var rootGraph = x.graph;
                            allGraph.Add(rootGraph);
                            var childGraph = rootGraph.GetAllNestedGraphs<Graph>(true);
                            if (childGraph.Count > 0)
                            {
                                allGraph.AddRange(childGraph);
                            }
                        }
                    });
                }

                allGraph.ForEach(y =>
                {
                    var targetFunctionNode = y.GetAllNodesOfType<CustomFunctionEvent>().FirstOrDefault(z => z.identifier.value == FunctionName.value);
                    if (targetFunctionNode != null)
                    {
                        if(!sourceFunctionNodeDict.ContainsKey(y))
                            sourceFunctionNodeDict.Add(y, targetFunctionNode);
                    }
                });
            }

            if (sourceFunctionNodeDict.Count > 0)
            {
                GUILayout.BeginVertical();
                foreach (var item in sourceFunctionNodeDict)
                {
                    if (GUILayout.Button(string.Format("functionNode:{0} in graph:{1}", item.Value.identifier.value, item.Key.name)))
                    {
                        if (item.Key.agent.gameObject != null)
                        {
                            UnityEditor.Selection.activeGameObject = item.Key.agent.gameObject;
                            UnityEditor.Selection.selectionChanged();
                        }
                        UnityEditor.Selection.activeObject = item.Key;
                        UnityEditor.Selection.selectionChanged();

                        if (ParadoxNotion.Services.MonoManager.current == null)
                        {
                            var _current = UnityEngine.Object.FindObjectOfType<ParadoxNotion.Services.MonoManager>();
                            if (_current != null)
                            {
                                UnityEngine.Object.DestroyImmediate(_current.gameObject);
                            }

                            var current = new GameObject("_MonoManager").AddComponent<ParadoxNotion.Services.MonoManager>();
                            current.StartCoroutine(waitForGraphChange(item.Value));
                        }
                        else
                        {
                            ParadoxNotion.Services.MonoManager.current.StartCoroutine(waitForGraphChange(item.Value));
                        }

                    }
                }
                GUILayout.EndVertical();
            }
        }

        System.Collections.IEnumerator waitForGraphChange(FlowNode n)
        {
            //Debug.Log("double press F to focus:" + n
            yield return new WaitForSecondsRealtime(0.15f);
            NodeCanvas.Editor.GraphEditorUtility.activeElement = n;
        }            
        #endif

        #endregion
	}
}