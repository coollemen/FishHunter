#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using FlowCanvas;

namespace NodeCanvas.Framework
{

    partial class Graph
    {

        private Graph _currentChildGraph;
        ///EDITOR ONLY. Responsible for the breacrumb navigation
        public Graph currentChildGraph {
            get { return _currentChildGraph; }
            set
            {
                if ( Application.isPlaying && value != null && EditorUtility.IsPersistent(value) ) {
                    ParadoxNotion.Services.Logger.LogWarning("You can't view sub-graphs in play mode until they are initialized to avoid editing asset references accidentally", "Editor", this);
                    return;
                }

                RecordUndo("Change View");
                if ( value != null ) {
                    value.currentChildGraph = null;
                }
                _currentChildGraph = value;
            }
        }
        //SL--临时记录点击的port
        public List<object> tempPorts;
        //SL--
        public void InsetNodeBetweenConnect(FlowCanvas.BinderConnection connect, Vector2 insertPosition)
        {
            Port sp = connect.sourcePort;
            Port tp = connect.targetPort;

            if (sp.type == typeof(Flow))
            {
                var newNode = (FlowNode)AddNode(typeof(FlowCanvas.Nodes.Dummy), insertPosition);

                RemoveConnection(connect);

                FlowCanvas.BinderConnection.Create(sp, newNode.GetInputPort("In"));
                FlowCanvas.BinderConnection.Create(newNode.GetOutputPort("Out"), tp);
                newNode.GatherPorts();
            }
            else
            {
                var newNode = (FlowNode)AddNode(typeof(FlowCanvas.Nodes.ValuePoint<>), insertPosition);
                RemoveConnection(connect);
                var bc = FlowCanvas.BinderConnection.Create(sp, newNode.GetInputPort("In"));
                //newNode.GatherPorts();
                FlowCanvas.BinderConnection.Create(((FlowCanvas.FlowNode)(bc.targetNode)).GetOutputPort("Out"), tp);
                newNode.GatherPorts();
            }
        }
        ///----------------------------------------------------------------------------------------------

        public GenericMenu CallbackOnCanvasContextMenu(GenericMenu menu, Vector2 canvasMousePos) { return OnCanvasContextMenu(menu, canvasMousePos); }
        public GenericMenu CallbackOnNodesContextMenu(GenericMenu menu, Node[] nodes) { return OnNodesContextMenu(menu, nodes); }
        public void CallbackOnDropAccepted(Object o, Vector2 canvasMousePos) { OnDropAccepted(o, canvasMousePos); }
        public void CallbackOnVariableDropInGraph(Variable variable, Vector2 canvasMousePos) { OnVariableDropInGraph(variable, canvasMousePos); }
        public void CallbackOnGraphEditorToolbar() { OnGraphEditorToolbar(); }

        ///Editor. Override to add extra context sensitive options in the right click canvas context menu
        virtual protected GenericMenu OnCanvasContextMenu(GenericMenu menu, Vector2 canvasMousePos) { return menu; }
        ///Editor. Override to add more entries to the right click context menu when multiple nodes are selected
        virtual protected GenericMenu OnNodesContextMenu(GenericMenu menu, Node[] nodes) { return menu; }
        ///Editor.Handles drag and drop objects in the graph
        virtual protected void OnDropAccepted(Object o, Vector2 canvasMousePos) { }
        ///Editor. Handle what happens when blackboard variable is drag and droped in graph
        virtual protected void OnVariableDropInGraph(Variable variable, Vector2 canvasMousePos) { }
        ///Editor. Append stuff in graph editor toolbar
        virtual protected void OnGraphEditorToolbar() { }

        ///----------------------------------------------------------------------------------------------

    }
}

#endif
