using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/*
 * This is the SmartTank_FSM_TS script, which inherits from the AITank script.
 * Within this script, we can call functions and values from the AITank script
 * to help us determine how we want to Finite State Machine to work.
 */

public class SmartTank_FSM_TS : AITank
{

    /*
     * Below are the global variables for theSmartTank script.
     * Firstly, we have three dictionaries which are used to store the values for the consumables, bases and enemy tank located in the game.
     * Each of these will take in a Gameobject as the key and a float as the value.
     * We then have three public gameobjects for the position of the consumables, bases and enemy.
     * We use these game objects to assign them as the first keys to be entered into the dictionaries by assigning them
     * in particular functions when one of these game objects has been located and needs to be interacted with in some way.
     * Finally, the float t is used for timing purposes as part of the locateConsumable function so that it
     * randomly changes direction after 10 seconds if it does not locate a consumable.
     */

    public Dictionary<GameObject, float> consumableLocated = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> enemyLocated = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> baseLocated = new Dictionary<GameObject, float>();

    public GameObject conPos;
    public GameObject enPos;
    public GameObject basPos;

    private float t = 0;
    private float speed = 1f;

    /*
     * AITankStart is a function inherited from the AITank script.
     * This works in the same way as a normal start function, however, as we are not inheriting from monobehaviour,
     * this function is used in it's place.
     * This will call the InitializeStateMachine function at the start of the game, to add the different state types
     * to the states dictionary.
     */

    public override void AITankStart()
    {

        InitializeStateMachine();

    }

    /*
     * AITankUpdate is a function inherited from the AITank script.
     * This works in the same way as a normal update function, however, as we are not inheriting from monobehaviour,
     * this function is used in it's place.
     * This function will set the dictionary value's for consumable, enemy and base located to be equal to the those within AITank
     * once one of these game objects has been found in TankSinatra's sensor.
     */

    public override void AITankUpdate()
    {

        consumableLocated = GetAllConsumablesFound;
        enemyLocated = GetAllTargetTanksFound;
        baseLocated = GetAllBasesFound;

    }

    /*
       AIOnCollisionEnter is a function inherited from the AITank script.
     * This works in the same way as a normal update function, however, as we are not inheriting from monobehaviour,
     * this function is used in it's place.
     * As we are not checking for collision in our FSM, this function is empty but
     * must still be implemented.
    */

    public override void AIOnCollisionEnter(Collision collision)
    {
        
        

    }

    /*
     * In InitializeStateMachine, we are creating a dictionary of the different
     * states which are to be added once again taking in a Type as the key and BaseState as the value.
     * In total we have 6 states; Search, Attack, Retreat, Chase, Dodge and Ambush.
     * It will recognise that these are the states that will be called when switching states
     * by setting them in the SetStates function from StateMachine.
     */

    private void InitializeStateMachine()
    {

        Dictionary<Type, BaseState_TS> states = new Dictionary<Type, BaseState_TS>();

        states.Add(typeof(SearchState_TS_FSM), new SearchState_TS_FSM(this));
        states.Add(typeof(AttackState_TS_FSM), new AttackState_TS_FSM(this));
        states.Add(typeof(RetreatState_TS_FSM), new RetreatState_TS_FSM(this));
        states.Add(typeof(ChaseState_TS_FSM), new ChaseState_TS_FSM(this));
        states.Add(typeof(DodgeState_TS_FSM), new DodgeState_TS_FSM(this));
        states.Add(typeof(AmbushState_TS_FSM), new AmbushState_TS_FSM(this));

        GetComponent<StateMachine_TS>().SetStates(states);

    }

    /*
     * This is the AlwaysLocateConsumable function.
     * In this function, it will determine that if the Count value of consumableLocated is greater than 0,
     * then the gameobject conPos will be set to the first entry into the consumableLocated dictionary
     * and move to the consumable and pick it up by calling the FollowPathToPoint function from AITank.
     * If the consumableLocated.Count is not greater than zero, it will reset the value of conPos to null.
     */

    public void AlwaysLocateConsumable()
    {
        
            if (consumableLocated.Count > 0)
            {
               
               conPos = consumableLocated.First().Key;
               FollowPathToPoint(conPos, speed);

            }
            else
            {
                conPos = null;
            }
        
    }

    /*
     * This is the LocateConsumable function. 
     * This is different to the AlwaysLocateConsumable function as we only want to search for a consumable
     * if TankSinatra's fuel is less than or equal to 30, ammo is less than or equal to 4 or health is less than
     * or equal to 50.
     * It will determine that if the Count value of consumableLocated is greater than 0,
     * then the gameobject conPos will be set to the first entry into the consumableLocated dictionary
     * and move to the consumable and pick it up by calling the FollowPathToPoint function from AITank.
     * A timer will then start for the float value t and if t is greater than 10, it will Generate a random point and
     * reset t to 0.
     * If the consumableLocated.Count is not greater than zero, it will reset the value of conPos to null.
     * A timer will also set if the initial values are greater than the values in the if statement,
     * so that it will randomly search instead.
     */

    public void LocateConsumable()
    {

        

        if (GetFuelLevel <= 30 || GetAmmoLevel <= 4 || GetHealthLevel <= 50)
        {

            if (consumableLocated.Count > 0)
            {
                conPos = consumableLocated.First().Key; //can locate the consumable and go to it if generate randomPoint is not on.
                FollowPathToPoint(conPos, speed);
                t += Time.deltaTime;
                if (t > 10)
                {

                    GenerateRandomPoint();
                    t = 0;

                }
            }
            else
            {

                conPos = null;
                FollowPathToRandomPoint(speed);
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

            FollowPathToRandomPoint(speed);

        }
    }

    /*
     * This is the tankDodge function.
     * It will check to see if the value for enemyLocated is greater than 0.
     * If it is, it will get closer to the enemy tank.
     * It will then check to see if the distance between TankSinatra and the enemy tank is less than 30f.
     * If it is, then it will stop the tank from moving and if it is less than 15f, it will start the tank moving again.
     */

    public void tankDodge()
    {

            
        if (enemyLocated.Count > 0)
        {

            FollowPathToPoint(enPos, speed);
            
            if (Vector3.Distance(transform.position, enPos.transform.position) < 30f)
            {
                              
                StopTank();
                
            }
            else if (Vector3.Distance(transform.position, enPos.transform.position) < 15f)
            {

                StartTank();

            }

        }

    }

    /*
     * This is the SearchForEnemy function.
     * It will check to see if the fuel is above 30, ammo above 4 and health above 50 for TankSinatra.
     * If they are, it will check to see if the enemyLocated and baseLocated values are greater than 0.
     * If they are, it will assign enPos to be equal to the first entry into the enemyLocated dictionary or basPos to the first
     * entry into the baseLocated dictionary.
     * If none of these are possible, then enPos and basPos will be reset to null and the FollowPathToRandomPoint function will be called.
     */

    public void SearchForEnemy()
    {

        if (GetFuelLevel > 30 && GetAmmoLevel > 4 && GetHealthLevel > 50)
        {
            if (enemyLocated.Count > 0)
            {

                
                enPos = enemyLocated.First().Key;
                

            }
            else if (baseLocated.Count > 0)
            {

                
                basPos = baseLocated.First().Key;
                

            }
            else
            {

                enPos = null;
                basPos = null;
                FollowPathToRandomPoint(speed);

            }
        }
        
    }

    /*
     * This is the chaseEnemy function.
     * Similar in nature to the SearchForEnemy function, it will check to see if the fuel is above 30, ammo above 4 and health above 50 for TankSinatra.
     * If they are, it will check to see if the enemy or base located values are greater than zero and approach those gameobjects depending on which was located.
     * If nothing has been located, it will reset enPos and basPos to null and follow a path to a random point.
     */

    public void ChaseEnemy()
    {


        if (GetFuelLevel > 30 && GetAmmoLevel > 4 && GetHealthLevel > 50)
        {

            if (enemyLocated.Count > 0)
            {

                FollowPathToPoint(enPos, speed);

            }
            else if (baseLocated.Count > 0)
            {

                FollowPathToPoint(basPos, speed);

            }
            else
            {

                enPos = null;
                basPos = null;
                FollowPathToRandomPoint(speed);

            }

        }
        

    }

    /*
     * This is the getHealth function.
     * It will return the value of the health for TankSinatra.
     */

    public float getHealth()
    {

        return GetHealthLevel;

    }

    /*
     * This is the getAmmo function.
     * It will return the value of the ammo for TankSinatra.
     */

    public float getAmmo()
    {

        return GetAmmoLevel;

    }

    /*
     * This is the getFuel function.
     * It will return the value of the fuel for TankSinatra.
     */

    public float getFuel()
    {

        return GetFuelLevel;

    }

    /*
     * This is the getTank function.
     * It will return the value of enemyLocated.
     */

    public int getTank()
    {

        return enemyLocated.Count;

    }

    /*
     * This is the getBase function.
     * It will return the value of baseLocated.
     */

    public int getBase()
    {

        return baseLocated.Count;

    }

    /*
     * This is the getConsumable function.
     * It will return the value of consumableLocated.
     */

    public int getConsumable()
    {

        return consumableLocated.Count;

    }

    /*
     * This is the StopTheTank function.
     * It will call just the StopTank function from AITank to stop the tank from moving.
     */

    public void StopTheTank()
    {


        StopTank();

        
    }

    /*
     * This is the FireOnEnemy function.
     * 
     * In the event that that enemyLocated's count is greater than 0 and the first entry in the dictionary is not equal to null,
     * it will assign enPos as the first entry in the dictionary.
     * Afterwards, it will check to see if the position between TankSinatra and the enemy tank is less than 30.
     * If it is, then it will fire at the enemy. If not, then it will move closer to them.
     * 
     * In the event that that baseLocated's count is greater than 0 and the first entry in the dictionary is not equal to null,
     * it will assign basPos as the first entry in the dictionary.
     * Afterwards, it will check to see if the position between TankSinatra and the base is less than 30.
     * If it is, then it will fire at the enemy's base. If not, then it will move closer to the bases.
     */

    public void FireOnEnemy()
    {

        if(enemyLocated.Count > 0 && enemyLocated.First().Key != null)
        {
            enPos = enemyLocated.First().Key;

            if (Vector3.Distance(transform.position, enPos.transform.position) < 30f)
            {
                FireAtPoint(enPos);
            }
            else
            {

                FollowPathToPoint(enPos, speed);

            }
        }
        else if(baseLocated.Count > 0 && baseLocated.First().Key !=null)
        {

            basPos = baseLocated.First().Key;

            if(Vector3.Distance(transform.position, basPos.transform.position) < 30f)
            {

                FireAtPoint(basPos);

            }
            else
            {

                FollowPathToPoint(basPos, speed);

            }

        }
        

    }

   
}
