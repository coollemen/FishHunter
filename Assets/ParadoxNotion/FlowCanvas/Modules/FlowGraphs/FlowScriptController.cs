//SL--
using NodeCanvas.Framework;
using System.Collections.Generic;
using System;

namespace FlowCanvas
{

    ///Add this component on a game object to be controlled by a FlowScript
    [UnityEngine.AddComponentMenu("FlowCanvas/FlowScript Controller")]
    public class FlowScriptController : GraphOwner<FlowScript>
    {

        ///Calls and returns a value of a custom function in the flowgraph
        public object CallFunction(string name, params object[] args)
        {
            SearchFunction(name);

            if (FunctionNode.TryGetValue(name, out func))
            {
                return func.Invoke(args);
            }
            return null;
        }

        bool searchOnce = false;
        List<Graph> nestedGraph = new List<Graph>(); IInvokable func = null;
        Dictionary<string, IInvokable> FunctionNode = new Dictionary<string, IInvokable>();


        public T CallFunction<T>(string name, params object[] args)
        {
            SearchFunction(name);

            if (FunctionNode.TryGetValue(name, out func))
            {
                return (T)func.Invoke(args);
            }
            return default(T);
        }

        void SearchFunction(string name)
        {
            if (!searchOnce)
            {
                searchOnce = true;
                nestedGraph = behaviour.GetAllNestedGraphs<Graph>(true);
                nestedGraph.Add(behaviour);
                foreach (var g in nestedGraph)
                {
                    if (g.GetType() == typeof(FlowGraph) || g.GetType().IsSubclassOf(typeof(FlowGraph)))
                    {

                        var dict = ((FlowGraph)g).GetFunctions();
                        if (dict != null && dict.Count > 0)
                        {
                            foreach (var d in dict)
                            {
                                if (!FunctionNode.ContainsKey(d.Key))
                                    FunctionNode.Add(d.Key, d.Value);
                            }
                        }

                        var customFunctionNodes = g.GetAllNodesOfType<FlowCanvas.Nodes.CustomFunctionEvent>();
                        if (customFunctionNodes != null && customFunctionNodes.Count > 0)
                        {
                            customFunctionNodes.ForEach((x) => { if (!customFunctionNodeDict.ContainsKey(x.identifier.value)) customFunctionNodeDict.Add(x.identifier.value, x); });
                        }
                    }
                }
            }
        }

        Dictionary<string, FlowCanvas.Nodes.CustomFunctionEvent> customFunctionNodeDict = new Dictionary<string, FlowCanvas.Nodes.CustomFunctionEvent>();
        public void CallFunctionSync<T>(Action<T> callBack, string name, params object[] args)
        {
            SearchFunction(name);
            FlowCanvas.Nodes.CustomFunctionEvent func;
            if (customFunctionNodeDict.TryGetValue(name, out func))
            {
                func.InvokeAsync(new Flow(), (x) => { callBack((T)(func.GetReturnValue())); }, args);
            }
            else
                callBack(default(T));
        }

        public void CallFunctionSync(Action callBack, string name, params object[] args)
        {
            SearchFunction(name);
            FlowCanvas.Nodes.CustomFunctionEvent func;
            if (customFunctionNodeDict.TryGetValue(name, out func))
            {
                func.InvokeAsync(new Flow(), (x) => { callBack(); }, args);
            }
            else
                callBack();
        }
    }
}