using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Category("Flow Controllers")]
    [Color("FF7F24")]
    [ContextDefinedInputs(typeof(Flow))]
    [ContextDefinedOutputs(typeof(Flow))]
    abstract public class FlowControlNode : FlowNode { }
}