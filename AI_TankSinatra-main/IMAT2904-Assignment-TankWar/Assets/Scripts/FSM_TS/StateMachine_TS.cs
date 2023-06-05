using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * This is the StateMachine script for TankSinatra.
 * This script is used to determine when a state should be switched, as part of the Finite State Machine.
 * As we have also incorporated the Finite State Machine into the Rule Based Systems and Behavioural trees, they also utilise
 * this script in tandom with the BaseState script. * 
 */

public class StateMachine_TS : MonoBehaviour
{

    /*
     * Below are two variables which we utilise in the state machine.
     * Firstly, we create a dictionary which takes in a Type and BaseState_TS. 
     * This dictionary is used to store the states which are associated with the FSM.
     * It utilises the Type as the key and BaseState_TS as the state class.
     * Secondly, we are creating an instance of the BaseState class called currentState
     * which is used to return the value of the state which we are changing to.
     * This utilises polymorphism as we do not necessarily know which state
     * is going to be returned.
     */

    private Dictionary<Type, BaseState_TS> states;
    public BaseState_TS currentState;


    /*
     * In the below public BaseState_TS implementation, this allows us to set and get
     * the current state.
     */

    public BaseState_TS CurrentState
    {


        get
        {
            
            return currentState;
            
        }
        private set
        {
            currentState = value;
        }

    }

    /*
     * In the below function SetStates, we are setting the states.
     * Similarly, we are utilising a dictionary to obtain the states values
     * using the Type as a key and the BaseState as the state.
     * The first part, this.states is the global value implemented at the top of the class
     * which stores our values and assigns the intake value of states in the function parameter.
     */

    public void SetStates(Dictionary<Type, BaseState_TS> states)
    {

        this.states = states;

    }

    /*
     * This is the Update function.
     * We firstly need to check if anything has been set as the CurrentState.
     * If the CurrentState is null (not equal to anything) then it will assign the
     * first value available in the states dictionary to be equal to CurrentState.
     * If CurrentState is not null, then we want to obtain the next potential state
     * which is available in the CurrentState's StateUpdate function.
     * Within the else section is a nested if statement which checks to see if the
     * value for nextState is not equal to null andnextState is not equal
     * to CurrentState.GetType. If both are not equal to each other,
     * then we want to switch the state if it wants to switch to a different
     * state.
     */

    void Update()
    {

      

        if(CurrentState == null)
        {

            CurrentState = states.Values.First();

           

        }
        else
        {

            var nextState = CurrentState.StateUpdate();

            if(nextState != null && nextState != CurrentState.GetType())
            {

                SwitchToState(nextState);

            }

        }


    }

    /*
     * This is the SwitchToState function.
     * This takes in a Type of nextState as a parameter.
     * Within the function, if we are switching to a new state,
     * we call the StateExit function to run anything which has been set in the state
     * we intend to move out of. For example, in the RBS, we want to set the bool value
     * of the state within the ruleset to be false as we no longer want to utilise
     * that state.
     * We then want to set the value of CurrentState to be equal to the next state
     * in the states dictionary. As we cannot determine which state is going to be called
     * next, we use the parameter of nextState to determine this for us.
     * Finally, we want to run the StateEnter function of the state we are entering.
     * As another example, with RBS, when we are entering a state we want to set that boolean
     * value in the rule set to be true as we want to use that state.
     */


    void SwitchToState(Type nextState)
    {

        CurrentState.StateExit();

        CurrentState = states[nextState];

        CurrentState.StateEnter();

    }

    

    

}
