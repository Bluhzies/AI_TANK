using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * This is the AmbushState_TS_FSM script. 
 */

public class AmbushState_TS_FSM : BaseState_TS
{

    /*
     * Firstly, we create an instance of the SmartTank script, so that we can call the functions which have been implemented
     * so that the tank can take actions from the AITank script.
     * We also create a float value (t) to be used in a timer in the Update function.
     */

    private SmartTank_FSM_TS tankSinatra;

    float t = 0;

    /*
     * We then create a public version of the AmbushState_TS_FSM which takes in an instance of SmartTank as a value,
     * assigning tankSinatra to be equal to tankSinatra.
     */

    public AmbushState_TS_FSM(SmartTank_FSM_TS tankSinatra)
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
     * In StateUpdate, we are checking to see if the value for getConsumable is greater than 0.
     * If it is, a timer will start of value t and run the StopTheTank function.
     * If getTank is then greater than or equal to 1, it will enter the ChaseState, otherwise, after 2.5 seconds
     * have passed, it will go and collect the consumable it was waiting to ambush at so it is not necessarily lost, but
     * will depend on when it actually spawned and was located by TankSinatra.
     * If the value for getConsumable is less than 1, the timer is reset and it will enter the SearchState.
     */

    public override Type StateUpdate()
    {

        if (tankSinatra.getConsumable() > 0)
        {

            t += Time.deltaTime;

            tankSinatra.StopTheTank();

            if(tankSinatra.getTank() >= 1)
            {

                return typeof(ChaseState_TS_FSM);

            }
            else if(t >= 2.5f)
            {

                tankSinatra.AlwaysLocateConsumable();

                
            }

            


        }
        else if(tankSinatra.getConsumable() < 1)
        {
            t = 0;

            return typeof(SearchState_TS_FSM);

        }


        return null;

    }

}
