using System;
using UnityEngine;

// The SearchState is initiated when transitioning from either the RetreatState or AttackState
public class SearchState_TS_BT : BaseState_TS
{
    // To use the various functions and variables from the Behavioural Tree SmartTank script a reference 
    // to that script is needed.
    private SmartTank_FSM_TS_BT tankSinatra;

    // A quick Constructor to ensure the SmartTank reference in this script/state is the main SmartTank script
    // for this implementation.
    public SearchState_TS_BT(SmartTank_FSM_TS_BT tankSinatra)
    {
        this.tankSinatra = tankSinatra;
    }

    // The StateEnter function simply changes the speed here so that our tank uses less fuel when just roaming around
    // the map in search of enemies and items. This value is changed in other states.
    public override Type StateEnter()
    {
        Debug.Log(this);

        tankSinatra.Speed = 0.5f;

        return null;
    }

    // This function is used when exiting the state. CanContinue is a boolean variable that is set true if, when
    // checking if any bases or enemies have been found, no consumables have been found (this would be during
    // the second check for consumables in the Search function). When the State is exited, this is set to false
    // so that it is freshly available the next time this state is entered.
    public override Type StateExit()
    {
        tankSinatra.canContinue = false;

        return null;
    }

    // The StateUpdate function serves as the regular Update function. It is used to call the Search function - to
    // look for consumables and enemies and bases. The targetSelector is then run to check if a target is found
    // AND to check if it is allowed to continue (as explained in the comment for the function above). If both
    // conditions are met, then the tank will transition into the ChaseState and move toward the target.
    public override Type StateUpdate()
    {        
        tankSinatra.Search();

        if (tankSinatra.targetSelector.Evaluate() == NodeStates_TS_BT.SUCCESS && tankSinatra.canContinue == true)
        {
            return typeof(ChaseState_TS_BT);
        }
        return null;
    }
}
