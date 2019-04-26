using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
    [Color("151714")]
    [Category("Variables")]
    abstract public class VariableNode : FlowNode
    {

        ///For setting the default variable
        abstract public void SetVariable(object o);
    }
}