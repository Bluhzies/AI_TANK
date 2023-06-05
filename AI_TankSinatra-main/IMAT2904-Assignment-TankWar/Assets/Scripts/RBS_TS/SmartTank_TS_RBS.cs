using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SmartTank_TS_RBS : AITank
{
    /****************************************** RULED BASED SYSTEM **********************************************/

    public Dictionary<GameObject, float> consumableLocated = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> enemyLocated = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> enemyBases = new Dictionary<GameObject, float>();

    public GameObject conPos;
    public GameObject enPos;
    public GameObject basePos;

    float t = 0;


    //Dictonary to store current stats of Rule A and Rule b
    public Dictionary<string, bool> infoStats = new Dictionary<string, bool>();
    public RuleSets_TS rule = new RuleSets_TS();

    public bool baseFound;
    public bool enemyNearby;
    public bool lowHealth;
    public bool lowAmmo;
    public bool lowFuel;
    private void RBS_Start()
    {
        //Add keys info the dictionary 
        infoStats.Add("LowHealth", lowHealth);
        infoStats.Add("LowAmmo", lowAmmo);
        infoStats.Add("LowFuel", lowFuel);
        infoStats.Add("EnemyNearby", false);
        infoStats.Add("SearchState", false);
        infoStats.Add("AttackState", false);
        infoStats.Add("RetreatState", false);
        infoStats.Add("BaseFound", false);
        infoStats.Add("ChaseState", false);
        infoStats.Add("EnemyApproach", false);
        infoStats.Add("BaseApproach", false);

        
        // add rules into dictionary. if RULE A, RULE B then return the state
        rule.AddRule(new Rule_TS("SearchState", "EnemyNearby", typeof(ChaseState_TS_RBS), Rule_TS.eStatement.AND));
        rule.AddRule(new Rule_TS("SearchState", "BaseFound", typeof(ChaseState_TS_RBS), Rule_TS.eStatement.AND));
        rule.AddRule(new Rule_TS("ChaseState", "EnemyApproach", typeof(AttackState_TS_RBS), Rule_TS.eStatement.AND));
        rule.AddRule(new Rule_TS("ChaseState", "BaseApproach", typeof(AttackState_TS_RBS), Rule_TS.eStatement.AND));
        rule.AddRule(new Rule_TS("AttackState", "LowHealth", typeof(RetreatState_TS_RBS), Rule_TS.eStatement.AND));
        rule.AddRule(new Rule_TS("AttackState", "LowAmmo", typeof(RetreatState_TS_RBS), Rule_TS.eStatement.AND));
        rule.AddRule(new Rule_TS("SearchState", "LowAmmo", typeof(RetreatState_TS_RBS), Rule_TS.eStatement.AND));
        rule.AddRule(new Rule_TS("SearchState", "LowHealth", typeof(RetreatState_TS_RBS), Rule_TS.eStatement.AND));
        rule.AddRule(new Rule_TS("SearchState", "LowFuel", typeof(RetreatState_TS_RBS), Rule_TS.eStatement.AND));
        rule.AddRule(new Rule_TS("ChaseState", "LowFuel", typeof(RetreatState_TS_RBS), Rule_TS.eStatement.AND));
    }

    public override void AITankStart()
    {
        InitializeStateMachine();
        RBS_Start();
    }

    public override void AITankUpdate()
    {
        //Update the values in infoStats and compare them, if its true or false
        infoStats["LowHealth"] = GetHealthLevel < 20;
        infoStats["LowAmmo"] = GetAmmoLevel < 1;
        infoStats["LowFuel"] = GetFuelLevel < 50;


        consumableLocated = GetAllConsumablesFound;
        enemyLocated = GetAllTargetTanksFound;
        enemyBases = GetAllBasesFound;
    }

    public void InitializeStateMachine()
    {
        //Dictionary of enemy bases
        Dictionary<Type, BaseState_TS> states = new Dictionary<Type, BaseState_TS>();

        //Add sets of rules 
        states.Add(typeof(SearchState_TS_RBS), new SearchState_TS_RBS(this));
        states.Add(typeof(ChaseState_TS_RBS), new ChaseState_TS_RBS(this));
        states.Add(typeof(RetreatState_TS_RBS), new RetreatState_TS_RBS(this));
        states.Add(typeof(AttackState_TS_RBS), new AttackState_TS_RBS(this));

        // Access state machine component
        GetComponent<StateMachine_TS>().SetStates(states);
    }


    public override void AIOnCollisionEnter(Collision collision)
    {


    }

    public void GenerateRandomPointFunction()
    {

        t += Time.deltaTime;
        if (t > 5) // Currently set to 0.1 as it seems to generate a random point normally when the if statement is used for now.
        {
            GenerateRandomPoint();
        }
    }

    public void LocateConsumable()
    {
        //If the fuel is less than 50 or ammo is less than 1 and health is less than 20. Locate the collectibles and move towards them.
        if (GetFuelLevel < 50 || GetAmmoLevel < 1 || GetHealthLevel < 20)
        {
            // Set low fuel to true
            infoStats["LowFuel"] = true;
            // Set low ammo to true
            infoStats["LowAmmo"] = true;
            // Set low health to true
            infoStats["LowHealth"] = true;

            // If a consumable is found
            if (consumableLocated.Count > 0)
            {
                //Get first consumable
                conPos = consumableLocated.First().Key;
                // Init Variable
                float closeDist = float.MaxValue; 
                // Init Variable as null
                GameObject closeDistConsumable = null; 

                //Loop through consumables 
                foreach (var firstConsumable in consumableLocated.Keys)
                {
                    //Check distance between consumable and the tank
                    float distance = Vector3.Distance(transform.position, firstConsumable.transform.position); 
                    //If the distance is close to the tank
                    if (distance < closeDist)
                    {
                        // Update the closest distance
                        closeDist = distance;
                        closeDistConsumable = firstConsumable; 
                    }
                }

                // If the closest consumable has been found
                if (closeDistConsumable != null)
                {
                    //Set the distance as the target to follow
                    conPos = closeDistConsumable;
                    // Call the function and move towards
                    FollowPathToPoint(conPos, 1f);
                    // Check if the timer 
                    t += Time.deltaTime;
                    // If the timer is more than 10
                    if (t > 10)
                    {
                        //Generate new random point
                        GenerateRandomPoint();
                        //Reset the time
                        t = 0;
                    }
                }
            }
            else
            {
                // Reset consumable 
                conPos = null;
                // Move to a new random point 
                FollowPathToRandomPoint(1f);
            }
        }
        else
        {
            //Check the timer
            t += Time.deltaTime;
            // If the timer is greater than 10
            if (t > 10)
            {
                // if its greater generate new random point
                GenerateRandomPoint();
                // Reset the timer
                t = 0;
            }
            // Move towards new random point
            FollowPathToRandomPoint(1f);
        }
    }

    //Dont care about current status of health, ammo or fuel. Collect them anyway
    public void AlwaysLocateConsumable()
    {
        // If the consumable is found
        if (consumableLocated.Count > 0)
        {
            // Get the first sonsumable from the dictionary
            conPos = consumableLocated.First().Key;
            // Move towards to collect consumable
            FollowPathToPoint(conPos, 1f);
        }
    }

    //Return the amount of consumables located.
    public int GetConsumable()
    {

        return consumableLocated.Count;
    }
    //Return value of health.
    public float GetHealth()
    {
        
        return GetHealthLevel;
    }
    //Return value of ammo.
    public float GetAmmo()
    {
    
        return GetAmmoLevel;
    }
    //Return value of fuel.
    public float GetFuel()
    {

        return GetFuelLevel;
    }
    //Return the amount of enemies located.
    public int GetTank()
    {

        return enemyLocated.Count;

    }
    //Return the amount of enemy bases.
    public int GetBase()
    {

        return enemyBases.Count;

    }

    public void SearchForEnemy()
    {
        //If the health is higher than 40 or ammo level less than 1. search for the enemy.
        //Add the tank into dictionary and register something has been found.

        if (GetFuelLevel > 40 || GetAmmoLevel > 1)
        {
            infoStats["LowAmmo"] = false; //stat LowAmmo is false
            infoStats["LowFuel"] = false; //stat LowFuel is false
            // If the enemy tank has been found
            if (enemyLocated.Count > 0)
            {
                // Get the first enemy tank in the dictionary
                enPos = enemyLocated.First().Key; //can locate the consumable and go to it if generate randomPoint is not on.
  
            }
            // If the enemy base has been found
            else if (enemyBases.Count > 0)
            {
                // Get the first base in the dictionary
                basePos = enemyBases.First().Key;  
            }
            else
            {
                // Set enemy position to null (no longer visibile)
                enPos = null;
                // Set enemy base position to null (no longer visible)
                basePos = null;
                // Follow to a new random point
                FollowPathToRandomPoint(1f);
            }
        }
        else
        {
            // Check the timer
            t += Time.deltaTime;
            // If the timer is more than 10
            if (t > 10)
            {
                // Generate new random point
                GenerateRandomPoint();
                // Reset time
                t = 0;

            }
            // Move towards new random point
            FollowPathToRandomPoint(1f);
        }
    }

    public void LocateEnemy()
    {
       //If the health is above 40 and ammo is above 1, move toward the enemy tank and base.
        if (GetFuelLevel > 40 && GetAmmoLevel > 1)
        {
           
            if (enemyLocated.Count > 0)
            {
                //If the tank is found. Go to it.
                FollowPathToPoint(enPos, 1f);

            }
            else if (enemyBases.Count > 0)
            {
                //If the base is found. Go to it.
                FollowPathToPoint(basePos, 1f);

            }
            else
            {
            //If the tank has reached the enemy or the base set them to null and generate new random point
                enPos = null;
                basePos = null;
                FollowPathToRandomPoint(1f);
            }
        }
        else
        {
            //Track the time and after 10 seconds generate a new random point.
            t += Time.deltaTime;
            // If the timer is more than 10
            if (t > 10)
            {
                // Generate new random point
                GenerateRandomPoint();
                // Reset timer
                t = 0;

            }
            // Follow to a new random point
            FollowPathToRandomPoint(1f);
        }
    }
    public void FireOnEnemy()
    {
        //If an enemy has been located and registered move towards them and shoot the bullets
        if (enemyLocated.Count > 0 && enemyLocated.First().Key != null)
        {
            enPos = enemyLocated.First().Key;

            if (Vector3.Distance(transform.position, enPos.transform.position) < 35f)
            {
                FireAtPoint(enPos);
            }
            else
            {
                FollowPathToPoint(enPos, 1f);
            }
        }
        //If an enemy base has been located and registered move towards them and shoot the bullets
        else if (enemyBases.Count > 0 && enemyBases.First().Key != null)
        {

            basePos = enemyBases.First().Key;

            if (Vector3.Distance(transform.position, basePos.transform.position) < 35f)
            {
                FireAtPoint(basePos);
            }
            else
            {
                FollowPathToPoint(basePos, 1f);
            }
        }
    }
}
