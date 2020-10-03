using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PresetDatabase 
{

    public class Preset
    {
        public string Name;
        public string ModuleName;
        public string Data;
    }

    //private static 
    private static Dictionary<string, Dictionary<string, string>> Data = new Dictionary<string, Dictionary<string, string>>();

    private static string GetFilename(string module, string name)
    {
        return $"Presets/{module}.{name}";
    }

    private static string GetMacroFilename(string module, string name)
    {
        return $"Macros/{module}.{name}";
    }


    public static Preset TryGetMacroPreset(string module, string name)
    {
        var data = string.Empty;

        var path = GetMacroFilename(module, name);

        if (System.IO.File.Exists(path))
        {
            data = System.IO.File.ReadAllText(path);
        }

        return new Preset()
        {
            Name = name,
            ModuleName = module,
            Data = data
        };
    }

    public static Preset TryGetPreset(string module, string name)
    {
        var data = string.Empty;

        var path = GetFilename(module, name);

        if (System.IO.File.Exists(path))
        {
            data = System.IO.File.ReadAllText(path);
        }

        return new Preset()
        {
            Name = name,
            ModuleName = module,
            Data = data
        };
    }


    public static void SaveMacroPreset(string module, string name, string data)
    {
        var path = GetMacroFilename(module, name);
        System.IO.File.WriteAllText(path, data);
    }

    
    public static void SavePreset(string module, string name, string data)
    {
        var path = GetFilename(module, name);
        System.IO.File.WriteAllText(path, data);
    }

}
