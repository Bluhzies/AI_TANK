using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// The main SmartTank class that is attached to our tank. It features functions which access the data or functions found in the parent
// class, AITank, as well as functions specific our Behavioural Tree AI technique implementation.

// The Behavioural Tree implementation focuses on defensiveness and resource management. It achieves this by prioritising locating
// consumables over enemies and bases, reducing it's speed (thus reducing fuel consumption) when simply searching the map.
// It also spends less time in combat by only firing a single shot, per encounter, before retreating. This results in the enemy tank
// giving chase and eventually giving up before the next potential encounter. Since this tank focuses on finding resources,
// most times it will simply outlast the other tank and win by having the most fuel. This is the main objective of this tank.
// It results in a more defensive strategy than our other implementations.

public class SmartTank_FSM_TS_BT : AITank
{
    // These are the global variables of the script.
    // These dictionaries are for containing the located objects that help our Tank to make its choices.
    public Dictionary<GameObject, float> consumableLocated = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> enemyLocated = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> baseLocated = new Dictionary<GameObject, float>();

    // GameObject representations for the locations of the objects that our Tank makes it's choices around.
    public GameObject conPos;
    public GameObject enPos;
    public GameObject basPos;

    // A float variable used for timers.
    float t = 0;

    // Behavioural Tree variables. Action nodes are where the individual checks happen to make sure certain requirements are met before
    // our Tank makes its decision.
    // The Sequence variables are for the iterative sequences that are run. These sequences call the
    // checks (the action node variables) and if each check passes, the sequence moves on to the next check until either all checks
    // are a success or if 1 is a failure.
    // The Selector variable is different to the sequence as while it runs through it's necessary checks iteratively, it does not
    // require all checks to be a success - only 1 is required for an outcome to happen.
    public ActionNode_TS_BT healthCheck, ammoCheck, fuelCheck, tankCheck, baseCheck;
    public Sequence_TS_BT attackSequence, retreatSequence, searchSequence;
    public Selector_TS_BT targetSelector;

    // These floats are for speed and for changing the checked values for 3 of the action nodes - having changeable inputs reduces code
    // duplication. A more in depth explanation is detailed further down in this script.
    public float speed, healthCheckInput, ammoCheckInput, fuelCheckInput;

    // A boolean variable used in a function below to allow a function to continue. More detailed explanation further in the script.
    public bool canContinue;

    // A get and set for our speed variable. It will change the speed of the tank depending on different conditions met/states in.
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    // The Start function which is used to intialise variables and set everything up for the project.
    public override void AITankStart()
    {
        //Debug.Log("Testing");

        // Function which sets all the Finite State Machine states;
        InitializeStateMachine();

        // Assigning the check variables/nodes to their functions.
        healthCheck = new ActionNode_TS_BT(HealthCheck);
        ammoCheck = new ActionNode_TS_BT(AmmoCheck);
        fuelCheck = new ActionNode_TS_BT(FuelCheck);
        tankCheck = new ActionNode_TS_BT(TankCheck);
        baseCheck = new ActionNode_TS_BT(BaseCheck);

        // Defining the action nodes that are checked in the Sequences and Selector.
        attackSequence = new Sequence_TS_BT(new List<BaseNode_TS_BT> { healthCheck, ammoCheck, fuelCheck });
        retreatSequence = new Sequence_TS_BT(new List<BaseNode_TS_BT> { healthCheck, ammoCheck, fuelCheck });
        targetSelector = new Selector_TS_BT(new List<BaseNode_TS_BT> { baseCheck, tankCheck });
        searchSequence = new Sequence_TS_BT(new List<BaseNode_TS_BT> { healthCheck, ammoCheck, fuelCheck });

        // Assigning values to the extra variables used in the script and explained above.
        Speed = 0.5f;
        canContinue = false;
    }     

    // The Update function which takes vales from the AITank script and brings it into this script so that our states can access the data
    // relating to consumables, enemies and bases found.
    public override void AITankUpdate()
    {
        consumableLocated = GetAllConsumablesFound;
        enemyLocated = GetAllTargetTanksFound;
        baseLocated = GetAllBasesFound;
    }

    // The OnCollisionEnter function. The Behavioural Tree implementation does not require this however as it comes from an Abstract class,
    // some form of implementation is necessary, even if it is empty.
    public override void AIOnCollisionEnter(Collision collision)
    {        
    }

    // Function to set up our State Machine by creating a dictionary of our states and adding them to it.
    // The Behavioural Tree implementation only makes use of 4 of our states.
    private void InitializeStateMachine()
    {
        Debug.Log("Init");

        Dictionary<Type, BaseState_TS> states = new Dictionary<Type, BaseState_TS>();

        states.Add(typeof(SearchState_TS_BT), new SearchState_TS_BT(this));
        states.Add(typeof(AttackState_TS_BT), new AttackState_TS_BT(this));
        states.Add(typeof(RetreatState_TS_BT), new RetreatState_TS_BT(this));
        states.Add(typeof(ChaseState_TS_BT), new ChaseState_TS_BT(this));

        GetComponent<StateMachine_TS>().SetStates(states);
    }

    // This function uses a timer to create a random point using a function from the AITank script.
    public void GenerateRandomPointFunction()
    {
        t += Time.deltaTime;
        if (t > 5)
        {
            GenerateRandomPoint();
        }
    }

    // This function is used to specifically look for consumables in the world. Timers are used to go back to looking for
    // traversing the map either after a consumable has been collected or if it despawns and is thus no longer located
    // by our tank.
    public void LocateConsumable()
    {
        if (consumableLocated.Count > 0)
        {
            conPos = consumableLocated.First().Key;
            FollowPathToPoint(conPos, Speed);

            t += Time.deltaTime;
            if (t > 10)
            {
                GenerateRandomPoint();
                t = 0;
            }
        }
        else
        {
            t += Time.deltaTime;
            if (t > 10)
            {
                GenerateRandomPoint();
                t = 0;
            }
            FollowPathToRandomPoint(Speed);
        }

    }

    // This Search function combines the LocateConsumable function above as well as functionality to search for the enemy tank and enemy bases.
    // It also features a second consumable check after checking for enemies and bases - even if a base or enemy is found, a second
    // check is done for nearby consumables, which if true will esentially end the execution of the function. The function is in an update
    // function and so will be called again almost instantly, thus the search for consumables will start again and since one will still be close,
    // the tank will immediately head toward it until it picks it up. This is one feature unique to the Behavioural Tree implementation, as it's focus
    // is on defensiveness and resource management.
    public void Search()
    {
        if (consumableLocated.Count > 0)
        {
            conPos = consumableLocated.First().Key;
            FollowPathToPoint(conPos, Speed);

            t += Time.deltaTime;
            if (t > 15)
            {
                GenerateRandomPoint();
                t = 0;
            }
        }
        else if (baseLocated.Count > 0)
        {
            basPos = baseLocated.First().Key;
            if (consumableLocated.Count > 0) return;
            else canContinue = true;
        }
        else if (enemyLocated.Count > 0)
        {
            enPos = enemyLocated.First().Key;
            if (consumableLocated.Count > 0) return;
            else canContinue = true;
        }
        else
        {
            conPos = null;
            enPos = null;
            basPos = null;
            FollowPathToRandomPoint(Speed);
        }
    }

    // This function follows on from Search. In this, either the enemy tank or enemy bases have been found and are now moved toward
    // so that they can be fired on from elsewhere in the script. Like in LocateConsumable, a timer is also used to move to a
    // random point if vision of the enemy or base is lost.
    public void LocateEnemy()
    {    
        if (enemyLocated.Count > 0)
        {
            //Debug.Log("Moving to Tank");

            FollowPathToPoint(enPos, Speed);
        }
        else if (baseLocated.Count > 0)
        {
            //Debug.Log("Moving to Base");

            FollowPathToPoint(basPos, Speed);
        }
        else
        {
            enPos = null;
            basPos = null;

            t += Time.deltaTime;
            if (t > 10)
            {

                GenerateRandomPoint();
                t = 0;

            }
            FollowPathToRandomPoint(Speed);
        }
    }

    // All these functions are used purely to get data from the AITank script to be used in various places.
    public float getHealth()
    {
        return GetHealthLevel;
    }

    public float getAmmo()
    {
        return GetAmmoLevel;
    }

    public float getFuel()
    {
        return GetFuelLevel;
    }

    public int getTank()
    {
        return enemyLocated.Count;
    }

    public int getBase()
    {
        return baseLocated.Count;
    }

    public Vector3 getTankLocation()
    {
        return enPos.transform.position;
    }

    public void StopTheTank()
    {
        StopTank();
    }

    // This function is used to call the FireAtPoint function from AITank. This function makes multiple checks on
    // distance and to double check the enemy object is located. If it cannot fire on the enemy yet, then it will
    // move toward the enemy at the given speed.
    public void FireOnEnemy()
    {
        if (enemyLocated.Count > 0 && enemyLocated.First().Key != null)
        {
            enPos = enemyLocated.First().Key;

            if (Vector3.Distance(transform.position, enPos.transform.position) <= 27f)
            {
                FireAtPoint(enPos);                 
            }
            else
            {
                FollowPathToPoint(enPos, Speed);
            }
        }
        else if (baseLocated.Count > 0 && baseLocated.First().Key != null)
        {
            basPos = baseLocated.First().Key;

            if (Vector3.Distance(transform.position, basPos.transform.position) <= 27f)
            {
                FireAtPoint(basPos);
            }
            else
            {
                FollowPathToPoint(basPos, Speed);
            }
        }       
    }

    // These functions serve as Conditions and check health, ammo and fuel. The current values are checked
    // against the values inputted in the function below. If the current values are less than the values
    // checked against, a failure is returned. These nodes are called in a Behavioural Tree Sequence so if
    // 1 of the checks provides a fail, the whole sequence will fail.
    private NodeStates_TS_BT HealthCheck()
    {
        if (getHealth() < healthCheckInput)
        {
            //Debug.Log("Not Enough Health");
            return NodeStates_TS_BT.FAILURE;
        }
        else
        {
            return NodeStates_TS_BT.SUCCESS;
        }
    }

    private NodeStates_TS_BT AmmoCheck()
    {
        if (getAmmo() < ammoCheckInput)
        {
            //Debug.Log("Not Enough Ammo");
            return NodeStates_TS_BT.FAILURE;
        }
        else
        {
            return NodeStates_TS_BT.SUCCESS;
        }
    }

    private NodeStates_TS_BT FuelCheck()
    {
        if (getFuel() < fuelCheckInput)
        {
            //Debug.Log("Not Enough Fuel");
            return NodeStates_TS_BT.FAILURE;
        }
        else
        {
            return NodeStates_TS_BT.SUCCESS;
        }
    }

    // This function is called when the AttackState, RetreatState or SearchState are entered. This function was made
    // so that a different value could be checked against (in the health, ammo and fuel checks above) depending on which
    // state it was called from. This cuts down on code duplication as the action nodes only have to be defined once
    // but different values can be checked against easily.
    public void ChangeCheckInputs(BaseState_TS currentState)
    {
        switch (currentState)
        {
            case AttackState_TS_BT:
                healthCheckInput = 40f;
                ammoCheckInput = 4f;
                fuelCheckInput = 30f;
                break;
            case RetreatState_TS_BT:
                healthCheckInput = 50f;
                ammoCheckInput = 6f;
                fuelCheckInput = 50f;
                break;
            case SearchState_TS_BT:
                healthCheckInput = 125f;
                ammoCheckInput = 15f;
                fuelCheckInput = 125f;
                break;
            default:
                Debug.Log("Should not be possible");
                break;
        }
    }

    // Like the 3 node checks above, these 2 functions are used in the Behavioural Tree implementation to check
    // if either a tank or base is currently visible to our tank, a success is returned.
    private NodeStates_TS_BT TankCheck()
    {
        if (getTank() >= 1)
        {
            return NodeStates_TS_BT.SUCCESS;
        }
        else
        {
            return NodeStates_TS_BT.FAILURE;
        }
    }

    private NodeStates_TS_BT BaseCheck()
    {
        if (getBase() >= 1)
        {
            return NodeStates_TS_BT.SUCCESS;
        }
        else
        {
            return NodeStates_TS_BT.FAILURE;
        }
    }
}
