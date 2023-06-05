using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * This is the ChaseState_TS_FSM script. 
 */

public class ChaseState_TS_FSM : BaseState_TS
{

    /*
     * Firstly, we create an instance of the SmartTank script, so that we can call the functions which have been implemented
     * so that the tank can take actions from the AITank script.
     */

    private SmartTank_FSM_TS tankSinatra;

    /*
     * We then create a public version of the ChaseState_TS_FSM which takes in an instance of SmartTank as a value,
     * assigning tankSinatra to be equal to tankSinatra.
     */

    public ChaseState_TS_FSM(SmartTank_FSM_TS tankSinatra)
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
     * In StateUpdate, we are checking to see if the value for getHealth is greater than 50, getAmmo is greater than or equal to 4 and
     * getFuel is greater than or equal to 30.
     * If they are, it will call the ChaseEnemy function from SmartTank and if the value for getTank or getBase is still greater than or
     * equal to one, it will return the AttackState.
     * Otherwise, if the initial values are less than the others, it will enter the RetreatState.
     */

    public override Type StateUpdate()
    {
              

        if (tankSinatra.getHealth() > 50 && tankSinatra.getAmmo() >= 4 && tankSinatra.getFuel() >= 30)
        {

            tankSinatra.ChaseEnemy();

            if (tankSinatra.getTank() >= 1 || tankSinatra.getBase() >= 1)
            {

                return typeof(AttackState_TS_FSM);

            }

        }
        else
        {

            return typeof(RetreatState_TS_FSM);

        }

        return null;

    }


}
