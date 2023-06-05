using UnityEngine;
using System;

// The RetreatState is initiated when transitioning from either the AttackState or ChaseState.
public class RetreatState_TS_BT : BaseState_TS
{
    // To use the various functions and variables from the Behavioural Tree SmartTank script a reference 
    // to that script is needed.
    private SmartTank_FSM_TS_BT tankSinatra;

    // Thes variables are used in this state. FirstEntry is used so that when this state is entered, a check
    // in the StateUpdate function can be met. The explanation will be in the comment for that function. The
    // variable t is used for a timer.
    private bool firstEntry;
    private float t;

    // A quick Constructor to ensure the SmartTank reference in this script/state is the main SmartTank script
    // for this implementation.
    public RetreatState_TS_BT(SmartTank_FSM_TS_BT tankSinatra)
    {
        this.tankSinatra = tankSinatra;
    }

    // When entering the state, the speed of the tank is set back to 1 as it is running away from a dangerous
    // situation and needs to escape as quickly as possible. 
    // The checks for health, ammo and fuel are also updated here, as explained in SmartTank.
    // FirstEntry is also set her, as well as t (which is, again, used for a timer).
    public override Type StateEnter()
    {
        Debug.Log(this);

        tankSinatra.Speed = 1f;
        tankSinatra.ChangeCheckInputs(this);
        firstEntry = true;
        t = 0;

        return null;
    }

    // This function is used when exiting the state. This implementation doesn't have anything that needs
    // setting on exit however it comes from an abstract class and so this is an empty implementation.
    public override Type StateExit()
    {
        return null;
    }

    // This function serves as the regular update function. If this is the first time this function is run
    // (which it would be if you've just entered the State) a timer is being run for 6 seconds. The reason
    // for this is because the RetreatState is called after firing in the AttackState. If this happens when
    // all health, ammo and fuel checks are true, the tank would be stuck in a loop of sitting in front of
    // the enemy tank, firing and being fire upon until one tank's health gets low enough to trigger a state
    // change. The timer guarantees that even if the resource check has been met, the RetreatState is stayed
    // in for 6 seconds. This means the SmartTank has plenty of time to escape combat, but also forces the
    // enemy tank to either chase the SmartTank or wander aimlessly (if the SmartTank goes out of range),
    // which means it spends more time using it's fuel. Also, the speed of the SmartTank is set back to 0.5
    // as after 6 seconds, its unlikely to still be in imminent danger.
    // Another Behavioural Tree Sequence is run to check if the health, ammo and fuel are below a certain level.
    // If they are, then the SmartTank will continue to search for Consumables to regain its resources. Once
    // that check returns successful, the SmartTank transitions to the SearchState as it has enough resources
    // to engage in combat.
    public override Type StateUpdate()
    {
        if (firstEntry == true)
        {
            t += Time.deltaTime;
            if (t > 6f)
            {
                firstEntry = false;
                tankSinatra.Speed = 0.5f;
                t = 0;
            }
        }

        if (tankSinatra.retreatSequence.Evaluate() == NodeStates_TS_BT.FAILURE || firstEntry == true)
        {
            tankSinatra.LocateConsumable();
        }
        else
        {
            return typeof(SearchState_TS_BT);
        }
        return null;
    }
}
