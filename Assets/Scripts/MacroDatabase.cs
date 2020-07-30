using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MacroDatabase 
{

    public class Macro
    {
        public string Name;
        public string ModuleName;
        public string Data;
    }

    //private static 


    private static Dictionary<string, Dictionary<string, string>> Data = new Dictionary<string, Dictionary<string, string>>();

    private static string GetFilename(string module, string name)
    {
        return $"Macros/{module}.{name}";
    }

    public static Macro TryGetMacro(string module, string name)
    {
        var data = string.Empty;

        var path = GetFilename(module, name);

        if (System.IO.File.Exists(path))
        {
            data = System.IO.File.ReadAllText(path);
        }

        return new Macro()
        {
            Name = name,
            ModuleName = module,
            Data = data
        };
    }

    public static void SaveMacro(string module, string name, string data)
    {
        var path = GetFilename(module, name);
        System.IO.File.WriteAllText(path, data);

    }

}
