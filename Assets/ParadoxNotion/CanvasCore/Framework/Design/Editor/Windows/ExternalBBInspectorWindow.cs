#if UNITY_EDITOR

using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEditor;
using UnityEngine;
//SL--
namespace NodeCanvas.Editor
{
    //[CustomEditor(typeof(Blackboard))]
    public class ExternalBBInspectorWindow : EditorWindow
    {
        private Blackboard bb
        {
            get { return GraphEditor.currentGraph!=null?(Blackboard)GraphEditor.currentGraph.blackboard:null; }
        }
        private object currentSelection;
        private Vector2 scrollPos;

        public static void ShowWindow() {
            var window = GetWindow(typeof(ExternalBBInspectorWindow)) as ExternalBBInspectorWindow;
            window.Show();
            Prefs.showBlackboard = false;
        }

        void OnEnable() {
            titleContent = new GUIContent("BlackBoard", StyleSheet.canvasIcon);
        }

        void OnDestroy() {
            Prefs.showBlackboard = true;
        }

        void Update()
        {
            if (currentSelection != GraphEditorUtility.activeElement)
            {
                Repaint();
            }
        }

        void OnGUI() {
            if (bb == null|| EditorApplication.isCompiling)
                return;
            if ( EditorGUIUtility.isProSkin ) {
                GUI.Box(new Rect(0, 0, Screen.width, Screen.height), string.Empty, Styles.shadowedBackground);
            }
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUILayout.Space(15);
            Prefs.showBlackboard = GUILayout.Toggle(Prefs.showBlackboard, "ShowCanvasBlackBoard");

            if ( GraphEditor.currentGraph == null ) {
                GUILayout.Label("No current NodeCanvas Graph open");
                return;
            }

            if ( EditorApplication.isCompiling ) {
                ShowNotification(new GUIContent("Compiling Please Wait..."));
                return;
            }

            currentSelection = GraphEditorUtility.activeElement;

            //if ( currentSelection == null ) {
            //    GUILayout.Label("No Node Selected in Canvas");
            //    return;
            //}


            GUILayout.Space(5);
            BlackboardEditor.ShowVariables(bb, bb);
            EditorUtils.EndOfInspector();
            if (Application.isPlaying || Event.current.isMouse)
            {
                Repaint();
            }
            //if ( currentSelection is Node ) {
            //    var node = (Node)currentSelection;
            //    Title(node.name);
            //    Node.ShowNodeInspectorGUI(node);
            //}

            //if ( currentSelection is Connection ) {
            //    Title("Connection");
            //    Connection.ShowConnectionInspectorGUI(currentSelection as Connection);
            //}

            EditorUtils.EndOfInspector();
            GUILayout.EndScrollView();
        }

        void Title(string text) {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal("box", GUILayout.Height(28));
            GUILayout.FlexibleSpace();
            GUILayout.Label("<b><size=16>" + text + "</size></b>");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorUtils.BoldSeparator();
        }
    }
}

#endif