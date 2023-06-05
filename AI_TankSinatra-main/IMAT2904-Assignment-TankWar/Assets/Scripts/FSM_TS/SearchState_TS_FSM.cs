using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * This is the SearchState_TS_FSM script. 
 */

public class SearchState_TS_FSM : BaseState_TS
{
    /*
     * Firstly, we create an instance of the SmartTank script, so that we can call the functions which have been implemented
     * so that the tank can take actions from the AITank script.
     */

    private SmartTank_FSM_TS tankSinatra;
       
    /*
     * We then create a public version of the SearchState_TS_FSM which takes in an instance of SmartTank as a value,
     * assigning tankSinatra to be equal to tankSinatra.
     */

    public SearchState_TS_FSM(SmartTank_FSM_TS tankSinatra)
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
     * This is the StateUpdate function which is inherited from the BaseState class.
     * In this function, we are checking to see if the value for getHealth is greater than or equal to 50, the ammo is greater than or equal to 4 and the fuel is
     * greater than or equal to 30. If these are less than these values, then it will enter the Retreatstate.
     * If it is, then it will search for the enemy by calling the SearchForEnemy function.
     * If the value for getTank or getBase is greater than or equal to one, then it will return the typeof ChaseState.
     * In the event that getConsumable is greater than or equal to one, it will enter the Ambush state.
     * 
     */

    public override Type StateUpdate()
    {

        if (tankSinatra.getHealth() >= 50 && tankSinatra.getAmmo() >= 4 && tankSinatra.getFuel() >= 30)
        {

            tankSinatra.SearchForEnemy();

            if (tankSinatra.getTank() >= 1 || tankSinatra.getBase() >= 1)
            {

                return typeof(ChaseState_TS_FSM);

            }
            else if (tankSinatra.getConsumable() >= 1)
            {

                return typeof(AmbushState_TS_FSM);

            }

        }        
        else
        {

            return typeof(RetreatState_TS_FSM);

        }

        return null;

    }
}
