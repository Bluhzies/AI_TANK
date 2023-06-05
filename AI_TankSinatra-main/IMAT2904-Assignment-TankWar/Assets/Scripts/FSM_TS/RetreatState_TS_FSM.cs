using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * This is the RetreatState_TS_FSM script. 
 */

public class RetreatState_TS_FSM : BaseState_TS
{

    /*
     * Firstly, we create an instance of the SmartTank script, so that we can call the functions which have been implemented
     * so that the tank can take actions from the AITank script.
     */

    private SmartTank_FSM_TS tankSinatra;

    /*
     * We then create a public version of the RetreatState_TS_FSM which takes in an instance of SmartTank as a value,
     * assigning tankSinatra to be equal to tankSinatra.
     */

    public RetreatState_TS_FSM(SmartTank_FSM_TS tankSinatra)
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
     * In StateUpdate, it will check to see if the value for getFuel is less than or equal to 30, getHealth is less than or equal to 50 and getAmmo is less than or equal to 4.
     * If any of these are true, it will call the LocateConsumable function.
     * If they are the opposite, it will then enter the SearchState.
     */

    public override Type StateUpdate()
    {

        

        if (tankSinatra.getFuel() <= 30 || tankSinatra.getHealth() <= 50 || tankSinatra.getAmmo() <= 4)
        {

            tankSinatra.LocateConsumable();

            

        }
        else if (tankSinatra.getHealth() >= 50 && tankSinatra.getFuel() >= 30 && tankSinatra.getAmmo() >= 4)
        {

            return typeof(SearchState_TS_FSM);

        }

        return null;

    }

   


}
