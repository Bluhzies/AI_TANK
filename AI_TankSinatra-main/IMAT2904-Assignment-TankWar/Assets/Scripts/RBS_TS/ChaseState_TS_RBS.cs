using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChaseState_TS_RBS : BaseState_TS
{
    private SmartTank_TS_RBS tankSinatra;

    public ChaseState_TS_RBS(SmartTank_TS_RBS tankSinatra)
    {
        // SMART TANK RBS Contructor
        this.tankSinatra = tankSinatra;

    }

    public override Type StateEnter()
    {
        tankSinatra.infoStats["ChaseState"] = true; //State Enter is true
        return null;
    }

    public override Type StateExit()
    {
        tankSinatra.infoStats["ChaseState"] = false; //No longer in a state
        return null;
    }

    public override Type StateUpdate()
    {
        // Search for the enemy
        tankSinatra.LocateEnemy();


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

        // if the health is now low
        if (tankSinatra.infoStats["LowHealth"] == false)
        {
            
            // If the enemy or enemy bases are found 
            if (tankSinatra.GetTank() >= 1 || tankSinatra.GetBase() >= 1)
            {
                // Enter attack state
                return typeof(AttackState_TS_RBS);
            }
        }
        else
        {
            // If the enemy is no longer in range enter retreat state
            return typeof(RetreatState_TS_RBS);
        }
        // Return nothing
        return null;
    }
}
