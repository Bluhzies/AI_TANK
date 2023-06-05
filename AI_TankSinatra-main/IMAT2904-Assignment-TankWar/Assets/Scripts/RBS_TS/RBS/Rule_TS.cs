using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Rule_TS
{
    public string ruleA, ruleB;
    public Type conditionState;
    public eStatement evaluate; 
        
    public enum eStatement
    { AND, OR, NAND}

    //Constructor
    public Rule_TS(string ruleA, string ruleB, Type conditionState, eStatement evaluate)
    {
        this.ruleA = ruleA;
        this.ruleB = ruleB;
        this.conditionState = conditionState;
        this.evaluate = evaluate;

    }

    // Checvk for rules
    public Type CheckRules(Dictionary<string, bool> infoStats)
    {
        bool bRuleA = infoStats[ruleA];
        bool bRuleB = infoStats[ruleB];

        switch(evaluate)
        {
            // If Rule A and Rule B, then return the condition 
            case eStatement.AND:
                if(bRuleA && bRuleB)
                {
                    return conditionState;
                }
                else
                {
                    return null;
                }
            // If Rule A nand Rule B, then return the condition 
            case eStatement.NAND:
                if(!bRuleA && !bRuleB)
                {
                    return conditionState;
                }
                else
                {
                    return null;
                }
            // If Rule A or Rule B, then return the condition 
            case eStatement.OR:
                if(bRuleA || bRuleB)
                {
                    return conditionState;
                }
                else
                {
                    // Return nothing
                    return null;
                }
            default:
                return null;
        }
    }
}
