using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
 * This is the BaseState class for TankSinatra.
 * Within this class are three functions, of which all of the derived classes will inherit.
 * This script is used in conjunction with the Finite State Machine, Rule Based Systems and Behavioural Tree scripts we have made.
 * StateUpdate will update every frame to check to see if, for example, a particular function
 * needs to run or if a condition has been met in which a state will change.
 * StateEnter is used in conjunction with the Rule Based Systems scripts, to set the actual states indicated
 * in the rules to be true. This is because in the rule based systems, the states can only be entered if they are true
 * and coupled with certain parameter changes, such as the health or ammo status of the tank, will cause
 * other state changes as well.
 * StateExit is similar to StateEnter, but will set the state to be false to indicate that we are not leaving that paricular state
 * as long as the conditions of the rules have been met.
 * As this is an abstract class, any derived classes must inherit all three of these functions regardless of whether they
 * are used or not.
 * Each of these functions will return a Type.
 */

public abstract class BaseState_TS
{

    public abstract Type StateUpdate();

    public abstract Type StateEnter();

    public abstract Type StateExit();

}
