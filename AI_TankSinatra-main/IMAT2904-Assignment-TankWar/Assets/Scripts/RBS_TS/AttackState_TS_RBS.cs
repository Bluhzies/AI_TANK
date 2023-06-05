using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState_TS_RBS : BaseState_TS
{
    private SmartTank_TS_RBS tankSinatra;
   

    public AttackState_TS_RBS(SmartTank_TS_RBS tankSinatra)
    {
        //Constructor for Attack state from Smart Tank
        this.tankSinatra = tankSinatra;
    }

    public override Type StateEnter()
    {
        //State enter
        tankSinatra.infoStats["AttackState"] = true;
        return null;
    }

    public override Type StateExit()
    {
        //State Exit
        tankSinatra.infoStats["AttackState"] = false;
        return null;
    }

    public override Type StateUpdate()
    {
        // Fire on the enemy
        tankSinatra.FireOnEnemy();

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

        // If the  low health and ammo are true
        if (tankSinatra.infoStats["LowHealth"] == true || tankSinatra.infoStats["LowAmmo"] == true)
        {
            // Enter retreat state
            return typeof(RetreatState_TS_RBS);
        }

        // Else if enemy tank or the bases are found 
        else if (tankSinatra.GetTank() <= 1 && tankSinatra.GetBase() <= 1)
        {
            // Enter search state
            return typeof(SearchState_TS_RBS);
        }

        else
        {
            // Return nothing
            return null;

        } 
    }
}
