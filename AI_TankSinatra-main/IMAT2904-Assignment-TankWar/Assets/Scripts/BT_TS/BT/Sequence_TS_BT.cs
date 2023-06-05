using System.Collections.Generic;

// This is the Sequence script which ultimately runs through the list of
// passed in nodes, to check if all of them pass the checks. The Sequence
// works by only passing the SUCCESS/FAILURE check when all nodes in the
// Sequence pass the check too. If any one of them fails, the Sequence is
// broken out of. 
public class Sequence_TS_BT : BaseNode_TS_BT
{
    // Simple constructor to ensure the passed in list of nodes are the
    // ones check in the Evaluate function.
    public Sequence_TS_BT(List<BaseNode_TS_BT> nodes_bt)
    {
        this.nodes_bt = nodes_bt;
    }

    // Here, the Evaluate function works by running through the whole list
    // of nodes passed in through the constructor. If any of them return
    // a FAILURE then the foreach loop is broken out of. The Sequence requires
    // every node to return a SUCCESS state for the Sequence as a whole to
    // be successful.
    public override NodeStates_TS_BT Evaluate()
    {
        bool failed = false;

        foreach (BaseNode_TS_BT node_bt in nodes_bt)
        {
            if (failed == true) break;

            switch (node_bt.Evaluate())
            {
                case NodeStates_TS_BT.FAILURE:
                    nodeState_bt = NodeStates_TS_BT.FAILURE;
                    failed = true;
                    break;
                case NodeStates_TS_BT.SUCCESS:
                    nodeState_bt = NodeStates_TS_BT.SUCCESS;
                    continue;                
                default:
                    nodeState_bt = NodeStates_TS_BT.FAILURE;
                    failed = true;
                    break;
            }
        }
        return nodeState_bt;
    }
}