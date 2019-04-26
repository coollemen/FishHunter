using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
    [Name("Gate")]
    [Description("Gate Control Flow Open Or Close")]
    public class Gate : FlowControlNode
    {
        bool doOnce = false;
        private bool isOpen = false;
        private ValueInput<bool> InitGateState;
        public override string name { get { return string.Format("Gate[{0}]", isOpen ? "Open" : "Close"); } }

        protected override void RegisterPorts()
        {
            var fOut = AddFlowOutput("Out");
            AddFlowInput("In", (f) => {
                if(!doOnce)
                {
                    doOnce = true;
                    isOpen = InitGateState.value;
                }

                if (isOpen) fOut.Call(f); });

            AddFlowInput("Open", (f) =>
            {
                isOpen = true;
            });

            AddFlowInput("Close", (f) =>
            {
                isOpen = false;
            });

            AddFlowInput("Toggle", (f) =>
            {
                isOpen = !isOpen;
            });

            InitGateState = AddValueInput<bool>("isOpen");
            AddValueOutput<bool>("currentState", () => isOpen);
        }

        public override void OnGraphStarted()
        {
            if (!doOnce)
            {
                doOnce = true;
                isOpen = InitGateState.value;
            }
            base.OnGraphStarted();
        }
    }

    //[Name("")]
    [Description("只做N次,Only DO N Count")]
    public class DoN : FlowControlNode
    {
        private int count = 0;
        bool doOnce = false;
        private ValueInput<int> StartCount;
        public override string name { get { return string.Format("Do[{0}]", Application.isPlaying? count: StartCount.value); } }
        protected override void RegisterPorts()
        {
            var fOut = AddFlowOutput("Out");
            AddFlowInput("In", (f) => {         
                if(!doOnce)
                {
                    count = StartCount.value;
                    doOnce = true;
                }
                if (count > 0)
                {
                    count = count - 1;
                    //Debug.Log(count);
                    fOut.Call(f);
                }else
                {
                    count = 0;
                }
            });

            AddFlowInput("Reset", (f) =>
            {
                count = StartCount.value;
            });

            AddValueOutput("Counter",()=> count);

            StartCount = AddValueInput<int>("Do Count");
        }
    }

    #region PlateForm
    [Name("PlatForm Flow")]
    [Color("FF7F24")]
    [Category("Flow Controllers/PlatForm")]
    [Description("平台,当前为Editor平台时,其他平台禁用")]
    [ContextDefinedInputs(typeof(Flow))]
    public class G_PlatForm : FlowNode
    {

        private FlowOutput Editor;
        private FlowOutput StandAlone_Win;
        private FlowOutput Andriod;
        private FlowOutput Iphone;

        private bool editor = false;
        private bool standAlone_win = false;
        private bool andriod = false;
        private bool iphone = false;

        protected override void RegisterPorts()
        {
            Editor = AddFlowOutput("Editor");
            StandAlone_Win = AddFlowOutput("StandAlone_Win");
            Andriod = AddFlowOutput("Andriod");
            Iphone = AddFlowOutput("Iphone");

#if UNITY_EDITOR
            editor = true;
#endif
#if UNITY_STANDALONE_WIN
            standAlone_win = true;
#endif
#if UNITY_ANDROID
            andriod = true;
#endif
#if UNITY_IPHONE
            iphone = true;
#endif


            AddFlowInput("In", (f) =>
            {

                if (editor)
                {
                    Editor.Call(f);
                    return;
                }

                if (standAlone_win)
                    StandAlone_Win.Call(f);

                if (andriod)
                    Andriod.Call(f);

                if (iphone)
                    Iphone.Call(f);
            });
        }
    }


    #endregion
}