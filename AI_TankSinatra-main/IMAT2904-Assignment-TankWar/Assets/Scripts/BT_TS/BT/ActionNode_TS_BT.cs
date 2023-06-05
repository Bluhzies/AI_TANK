// The ActionNode is what the actual performs a check against the provided NodeState result
public class ActionNode_TS_BT : BaseNode_TS_BT
{
    // Here we have a delegate variable which essentially acts as a pointer to a function.
    // We use it here to store the functions we call in a variable. This would probably be
    // more useful if there were multiple functions to call.
    public delegate NodeStates_TS_BT ActionNodeFunction();
    private ActionNodeFunction actionNode_bt;

    // A constructor for this class which assigns the local actionNode_bt to the global
    // actionNode_bt. This ensures that whatever action we're passing in is correctly
    // reffered to in the Evaluate function below.
    public ActionNode_TS_BT(ActionNodeFunction actionNode_bt)
    {
        this.actionNode_bt = actionNode_bt;
    }

    // The Evaluate functrion takes in the desired action of choice. If the check, in
    // SmartTank, returns SUCCESS then the state of the node is set to SUCCESS. This
    // means that when this function is called, the nodeState returned will be the
    // ultimate result for this node. This is then used in the Sequence and Selector
    // scripts to decide if a Sequence has all been met or if one of the options for
    // the Selector has been picked.
    public override NodeStates_TS_BT Evaluate()
    {
        switch (actionNode_bt())
        {
            case NodeStates_TS_BT.SUCCESS:
                nodeState_bt = NodeStates_TS_BT.SUCCESS;
                return nodeState_bt;
            case NodeStates_TS_BT.FAILURE:
                nodeState_bt = NodeStates_TS_BT.FAILURE;
                return nodeState_bt;
            default:
                nodeState_bt = NodeStates_TS_BT.FAILURE;
                return nodeState_bt;
        }
    }
}