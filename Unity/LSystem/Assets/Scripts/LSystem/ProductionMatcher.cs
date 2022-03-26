using System;
using System.Collections.Generic;
using UnityEngine;

public static class ProductionMatcher
{
    public static Production Match(string module, Dictionary<string, List<Production>> productions)
    {
        if (!productions.ContainsKey(module))
            return null;
        List<Production> matches = productions[module];
        if (matches.Count == 1)
            return matches[0];
        float chance = UnityEngine.Random.value, accProbability = 0;
        foreach (var match in matches)
        {
            accProbability += match.probability;
            if (accProbability <= chance)
                return match;
        }
        // TODO: throw an assertion!
        throw new Exception("Should never happen!");
    }

    public static bool CheckProbabilities(Dictionary<string, List<Production>> productions)
    {
        foreach (var matches in productions.Values)
        {
            if (matches.Count == 1 && !Mathf.Approximately(matches[0].probability, 1))
                return false;
            float accProbabilities = 0;
            foreach (var match in matches)
                accProbabilities += match.probability;
            Debug.Log(accProbabilities);
            if (!Mathf.Approximately(accProbabilities, 1))
                return false;
        }
        return true;
    }

}