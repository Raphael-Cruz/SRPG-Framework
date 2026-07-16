using System;
using System.Collections.Generic;


public class CombatFacts
{
    /*
     * Core facts.
     * These are facts the engine always understands.
     */

    public int Distance { get; set; }

    public bool IsValidTarget { get; set; }



    /*
     * Dynamic facts.
     * Future systems can add new information.
     *
     * Examples:
     * Flanked
     * HighGround
     * Poisoned
     * Covered
     */

    private readonly Dictionary<string, object> dynamicFacts 
        = new Dictionary<string, object>();



    public void Set<T>(string key, T value)
    {
        dynamicFacts[key] = value;
    }



    public bool TryGet<T>(string key, out T value)
    {
        if(dynamicFacts.TryGetValue(key, out object result))
        {
            if(result is T typedValue)
            {
                value = typedValue;
                return true;
            }
        }


        value = default;
        return false;
    }



    public bool Has(string key)
    {
        return dynamicFacts.ContainsKey(key);
    }
}