using UnityEngine;
using System;

// The AttackState is initiated when transitioned to from ChaseState.
public class AttackState_TS_BT : BaseState_TS
{
    // To use the various functions and variables from the Behavioural Tree SmartTank script a reference 
    // to that script is needed.
    private SmartTank_FSM_TS_BT tankSinatra;

    // Float vairable used for a timer.
    private float t;

    // A quick Constructor to ensure the SmartTank reference in this script/state is the main SmartTank script
    // for this implementation.
    public AttackState_TS_BT(SmartTank_FSM_TS_BT tankSinatra)
    {
        this.tankSinatra = tankSinatra;
    }

    // On entry, the health, ammo and fuel checks are updated to be in line for the requirements of this state.
    // The specific values can be found in the SmartTank script.
    public override Type StateEnter()
    {
        Debug.Log(this);

        tankSinatra.ChangeCheckInputs(this);
        t = 0;        

        return null;
    }

    // This function is used when exiting the state. This implementation doesn't have anything that needs
    // setting on exit however it comes from an abstract class and so this is an empty implementation.
    public override Type StateExit()
    {
        return null;
    }

    // The StateUpdate operates as a regular Update function and runs the Behavioural Tree Sequence to check
    // if it is able to fire. If it is, then the firing process will be started. A timer is also begun.
    // If the target is the enemy tank, 2.25 seconds are waited before heading into the RetreatState, this
    // is to ensure the SmartTank gets off a single shot before moving away. The timer helps make sure only
    // one shot is fired as the overall firing process takes about 2 seconds. The tank only fires one shot to
    // ensure it conserves ammo, to prevent the two tanks from simply sitting in front of each other firing
    // until has lost enough health to run away, and to ensure the SmartTank has time to run away and ensure
    // the enemy tank chases, then roams - wasting it's fuel.
    // If the target is instead an enemy base, once it has been destroyed the SmartTank is put into the
    // SearchState to seek out either the other base, the enemy or a consumable.
    public override Type StateUpdate()
    {
        if (tankSinatra.attackSequence.Evaluate() == NodeStates_TS_BT.SUCCESS)
        {
            tankSinatra.FireOnEnemy();
            t += Time.deltaTime;

            if (tankSinatra.getTank() == 1)
            {
                if (t > 2.25f) return typeof(RetreatState_TS_BT);
            }
            else if (tankSinatra.getBase() < 1)
            {
                return typeof(SearchState_TS_BT);
            }      
            
        }
        return null;
    }
}
