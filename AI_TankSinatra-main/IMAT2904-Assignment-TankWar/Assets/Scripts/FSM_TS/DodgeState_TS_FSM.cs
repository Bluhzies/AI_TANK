using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * This is the DodgeState_TS_FSM script. 
 */

public class DodgeState_TS_FSM : BaseState_TS
{

    /*
     * Firstly, we create an instance of the SmartTank script, so that we can call the functions which have been implemented
     * so that the tank can take actions from the AITank script.
     * We also create a float value (t) to be used in a timer in the Update function.
     */

    private SmartTank_FSM_TS tankSinatra;

    float t;

    /*
     * We then create a public version of the DodgeState_TS_FSM which takes in an instance of SmartTank as a value,
     * assigning tankSinatra to be equal to tankSinatra.
     */

    public DodgeState_TS_FSM(SmartTank_FSM_TS tankSinatra)
    {

        this.tankSinatra = tankSinatra;

    }

    /*
     * This is the StateEnter function which is inherited from the BaseState class.
     * As this does not do anything, it will return null.
     */

    public override Type StateEnter()
    {
        return null;
    }

    /*
     * This is the StateExit function which is inherited from the BaseState class.
     * As this does not do anything, it will return null.
     */

    public override Type StateExit()
    {
        return null;
    }

    /*
     * In the StateUpdate function, a timer is started by increasing the value of t when it
     * and call the tankDodge function.
     * If the value of t is greater than 2f, it will reset the timer to zero and then return the AttackState.
     */

    public override Type StateUpdate()
    {

        t += Time.deltaTime;

        tankSinatra.tankDodge();

        if (t > 2f)
        {

            t = 0;

            return typeof(AttackState_TS_FSM);

        }

        return null;

    }

}
