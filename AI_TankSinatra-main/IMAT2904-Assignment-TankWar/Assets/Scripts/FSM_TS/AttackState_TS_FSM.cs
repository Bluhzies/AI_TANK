using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * This is the AttackState_TS_FSM script. 
 */

public class AttackState_TS_FSM : BaseState_TS
{

    /*
     * Firstly, we create an instance of the SmartTank script, so that we can call the functions which have been implemented
     * so that the tank can take actions from the AITank script.
     */

    private SmartTank_FSM_TS tankSinatra;

    /*
     * We then create a public version of the AttackState_TS_FSM which takes in an instance of SmartTank as a value,
     * assigning tankSinatra to be equal to tankSinatra.
     */

    public AttackState_TS_FSM(SmartTank_FSM_TS tankSinatra)
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
     * Below is the StateUpdate function.
     * 
     * Firstly, it will check to see if the value for getHealth is less than 50, getAmmo is less than or equal to 4 and getFuel is less than or equal
     * to 40 and if any of these are true, it will return the typeof RetreatState.
     * Otherwise, it will firstly call the FireOnEnemy function.
     * It will then check to see if getTank or getBase are less than one.
     * If they are, then it will enter the SearchState.
     * If getTank is greater than or equal to one, it will enter the DodgeState.
     */

    public override Type StateUpdate()
    {
         
               
        if (tankSinatra.getHealth() < 50 || tankSinatra.getAmmo() <= 4 || tankSinatra.getFuel() <= 40)
        {

            
            return typeof(RetreatState_TS_FSM);

        }
        else
        {

            tankSinatra.FireOnEnemy();

            

            if (tankSinatra.getTank() < 1 && tankSinatra.getBase() < 1)
            {

                return typeof(SearchState_TS_FSM);

            }
            else if (tankSinatra.getTank() >= 1)
            {

                
                return typeof(DodgeState_TS_FSM);

            }

            return null;

        }
       
    }


}
