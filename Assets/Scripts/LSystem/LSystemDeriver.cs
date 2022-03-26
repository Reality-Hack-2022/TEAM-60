using System;
using System.Collections.Generic;

public static class LSystemDeriver
{
    public static string Derive(string axiom, int derivations, Dictionary<string, List<Production>> productions)
    {
        var moduleString = axiom;
        for (int i = 0; i < Math.Max(1, derivations); i++)
        {
            moduleString = DeriveOneStep(moduleString, productions);
        }
        return moduleString;
    }


    public static string DeriveOneStep(string axiom, Dictionary<string, List<Production>> productions)
    {
        var moduleString = axiom;
        string newModuleString = "";
        for (int j = 0; j < moduleString.Length; j++)
        {
            string module = moduleString[j] + "";
            if (!productions.ContainsKey(module))
            {
                newModuleString += module;
                continue;
            }
            var production = ProductionMatcher.Match(module, productions);
            newModuleString += production.successor;
        }
        return newModuleString;
    }

}

