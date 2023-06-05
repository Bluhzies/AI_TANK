using System.Collections.Generic;

// The BaseNode class from which all the Node related classes inherit (excluding NodeStates).
// It features a List which gets used in the Sequence and Selector scripts - the list is used
// in foreach loops and will hold the various of the check functions used in SmartTank.
public abstract class BaseNode_TS_BT
{
    // List which gets used in the Sequence and Selector scripts - the list is used to hold
    // the checks used in the SmartTank script.
    protected List<BaseNode_TS_BT> nodes_bt = new List<BaseNode_TS_BT>();

    // This variable will hold the result of the SUCCESS or FAILURE state.
    protected NodeStates_TS_BT nodeState_bt;

    // A simple getter for the variable above.
    public NodeStates_TS_BT nodeState_BT
    {
        get { return nodeState_bt; }
    }

    // An abstract function which needs to be implemented in sub-classes which will be used to
    // actually run the checks and provide the SUCCESS or FAILURE result.
    public abstract NodeStates_TS_BT Evaluate();
}