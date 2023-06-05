using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RetreatState_TS_RBS : BaseState_TS
{

    private SmartTank_TS_RBS tankSinatra;

    public RetreatState_TS_RBS(SmartTank_TS_RBS tankSinatra)
    {
        this.tankSinatra = tankSinatra;
    }

    public override Type StateEnter()
    {
        // State enter
        tankSinatra.infoStats["RetreatState"] = true;
        return null;
    }

    public override Type StateExit()
    {
        // State Exit
        tankSinatra.infoStats["RetreatState"] = false;
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

        // If health or fuel or ammo are low
        if (tankSinatra.infoStats["LowHealth"] == true || tankSinatra.infoStats["LowFuel"] == true || tankSinatra.infoStats["LowAmmo"] == true)
        {
            // Locate the consumables on the map
            tankSinatra.LocateConsumable();

        }
        
        // If the health, ammo or fuel are not low
        else if (tankSinatra.infoStats["LowHealth"] == false && tankSinatra.infoStats["LowAmmo"] == false && tankSinatra.infoStats["LowFuel"] == false)
        {
            // Enter search state to look for the enemies 
            return typeof(SearchState_TS_RBS);
        }
        // Return nothing
        return null;
    }
}