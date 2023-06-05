using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class SearchState_TS_RBS : BaseState_TS
{
    private SmartTank_TS_RBS tankSinatra;

    public SearchState_TS_RBS(SmartTank_TS_RBS tankSinatra)
    {
        this.tankSinatra = tankSinatra;
    }

    public override Type StateEnter()
    {
        //State enter 
        tankSinatra.infoStats["SearchState"] = true;
        return null;
    }

    public override Type StateExit()
    {
        //State exit
        tankSinatra.infoStats["SearchState"] = false;

        return null;
    }

    public override Type StateUpdate()
    {

        //Loop through condition in RuleGetter 
        foreach (var condition in tankSinatra.rule.RuleGetter)
        {
            // Check if the rules are met 
            if (condition.CheckRules(tankSinatra.infoStats) != null)
            {
                // If its true return the condition
                return condition.CheckRules(tankSinatra.infoStats);
            }
        }
        
        // If a consumable is found, always collect them
        if (tankSinatra.GetConsumable() > 0)
        {
            
            tankSinatra.AlwaysLocateConsumable();
        }

        // If the fuel, ammo or health are low
        else if (tankSinatra.infoStats["LowFuel"] == true || tankSinatra.infoStats["LowAmmo"] == true || tankSinatra.infoStats["LowHealth"] == true)
        {
            // Enter retreat state
            return typeof(RetreatState_TS_RBS);
        }
        
        // If the health AND ammo are not low
        else if (tankSinatra.infoStats["LowHealth"] == false && tankSinatra.infoStats["LowAmmo"] == false)
        {
            // Search for the enemies 
            tankSinatra.SearchForEnemy();
          
            // If the enemy tank or enemy bases are found
            if (tankSinatra.GetTank() >= 1 || tankSinatra.GetBase() >= 1)
            {
                // Enter chase state
                return typeof(ChaseState_TS_RBS);
            }

        }

        // If the bases are found 
        else if (tankSinatra.infoStats["BaseFound"] == true)
        {
            // Enter attack state
            return typeof(AttackState_TS_RBS);
        }
        // Return nothing
            return null;

       
    }
}