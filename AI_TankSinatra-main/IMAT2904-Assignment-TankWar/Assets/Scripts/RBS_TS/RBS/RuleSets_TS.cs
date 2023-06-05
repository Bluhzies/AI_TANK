using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleSets_TS 
{
 
    // Add rule 
    public void AddRule(Rule_TS newRule)
    {
        RuleGetter.Add(newRule);
    }
    // Rule getter dictionary
    public List<Rule_TS> RuleGetter { get;} = new List<Rule_TS>();
}
