using System.Collections.Generic;


// The Selector works Similarly to the Sequence script by checking
// the list of nodes to see if the resulting states are acceptable.
// The difference with the Selector is that it only requires one
// node to return a SUCCESS state to pass, unlike the Sequence
// which requires all nodes to return a SUCCESS state.
public class Selector_TS_BT : BaseNode_TS_BT
{
    // Simple constructor to ensure the passed in list of nodes are the
    // ones check in the Evaluate function.
    public Selector_TS_BT(List<BaseNode_TS_BT> nodes_bt)
    {
        this.nodes_bt = nodes_bt;
    }

    // The Evaluate function here uses a foreach loop to check each
    // node in the list. If a node returns a FAILURE, the loop continues
    // as a Selector does not require every single node to return a
    // SUCCESS state.
    public override NodeStates_TS_BT Evaluate()
    {
        foreach (BaseNode_TS_BT node_bt in nodes_bt)
        {
            switch(node_bt.Evaluate())
            {
                case NodeStates_TS_BT.FAILURE:
                    continue;
                case NodeStates_TS_BT.SUCCESS:
                    nodeState_bt = NodeStates_TS_BT.SUCCESS;
                    return nodeState_bt;
                default:
                    continue;
            }
        }
        nodeState_bt = NodeStates_TS_BT.FAILURE;
        return nodeState_bt;
    }
}