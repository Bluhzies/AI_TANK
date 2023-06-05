using UnityEngine;
using System;

// The ChaseState is initiated when transitioning from the SearchState.
public class ChaseState_TS_BT : BaseState_TS
{
    // To use the various functions and variables from the Behavioural Tree SmartTank script a reference 
    // to that script is needed.
    private SmartTank_FSM_TS_BT tankSinatra;

    // A quick Constructor to ensure the SmartTank reference in this script/state is the main SmartTank script
    // for this implementation.
    public ChaseState_TS_BT(SmartTank_FSM_TS_BT tankSinatra)
    {
        this.tankSinatra = tankSinatra;
    }

    // The StateEnter function simply changes the speed here so that our tank uses less fuel when just roaming around
    // the map in search of enemies and items. This value is changed in other states.
    public override Type StateEnter()
    {
        Debug.Log(this);

        tankSinatra.Speed = 0.75f;

        return null;
    }

    // This function is used when exiting the state. This implementation doesn't have anything that needs
    // setting on exit however it comes from an abstract class and so this is an empty implementation.
    public override Type StateExit()
    {
        return null;
    }

    // This StateUpdate function works like a regular update function. The LocateEnemy function is called
    // from the main SmartTank class, which focuses on getting close to the enemy/base that was found
    // in the SearchState. It runs a selection sequence, to check which target it is looking at, and if it
    // passes the check, then the tank transitions into the AttackState. If it has lost the enemy tank/base
    // then it transition to the RetreatState.
    public override Type StateUpdate()
    {
        tankSinatra.LocateEnemy();

        if (tankSinatra.targetSelector.Evaluate() == NodeStates_TS_BT.SUCCESS)
        {
            return typeof(AttackState_TS_BT);
        }
        else
        {
            return typeof(RetreatState_TS_BT);
        }
    }
}
