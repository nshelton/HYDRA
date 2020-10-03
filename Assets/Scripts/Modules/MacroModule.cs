using System.Collections.Generic;
using UnityEngine;
using static PresetDatabase;

public class MacroModule : BaseGUIModule
{

    [SerializeField] private GUIManager m_manager;


    public class Macro
    {
        public string Name;
        public Dictionary<string, Preset> Modules = new Dictionary<string, Preset>();
    }

    private Dictionary<string, Macro> m_macros = new Dictionary<string, Macro>();

    private Dictionary<string, GUIPresetTrigger> m_buttonMap = new Dictionary<string, GUIPresetTrigger>();

    private Dictionary<string, BaseGUIModule> m_modules = new Dictionary<string, BaseGUIModule>();

    public override string Name()
    {
        return "macros";
    }

    public override bool ShowPresets()
    {
        return false;
    }
    public override bool ShowTitle()
    {
        return false;
    }

    private Macro TryLoadMacro(string macroName)
    {
        Macro m = new Macro();
        m.Name = macroName;
        bool isValid = false;
        foreach(string modName in m_modules.Keys)
        {
            Preset p = PresetDatabase.TryGetMacroPreset(modName, macroName);
            if (p.Data != string.Empty)
            {
                isValid = true;
                m.Modules[modName] = p;
            }
        }

        if (!isValid)
        {
            m = null;
        }

        return m;
    }

    private void LoadMacros()
    {
        m_macros.Clear();

        for(int i = 0; i < 16; i++)
        {
            string name = i.ToString();
            m_macros[name] = TryLoadMacro(name);
        }
    }

    GUIPresetTrigger CreateMacroButton(string name, Macro m)
    {
        var trigger = new GUIPresetTrigger(name,
            delegate { this.SetMacro(name); },
            delegate { this.SaveMacro(name); }
        );

        trigger.Assigned = m != null;

        return trigger;
    }

    private void SetMacro(string name)
    {
        var macro = m_macros[name];
        if ( macro != null)
        {
            foreach(string moduleName in m_modules.Keys)
            {
                if (macro.Modules.ContainsKey(moduleName))
                {
                    m_modules[moduleName].SetState(macro.Modules[moduleName].Data);
                }
            }

            foreach (var m in m_buttonMap) { m.Value.Active = false; }
            m_buttonMap[name].Active = true;
        }
    }

    private void SaveMacro(string name)
    {
        Macro macro = new Macro();
        macro.Name = name;

        foreach (var moduleName in m_modules.Keys)
        {
            var data = m_modules[moduleName].GetState();
            macro.Modules[moduleName] = new Preset { Data = data, ModuleName = moduleName, Name = name };
            SaveMacroPreset(moduleName, name, data);
        }

        m_macros[name] = macro;
        m_buttonMap[name].Assigned = true;
    }

    public override void InitInternal()
    {
        m_buttonMap.Clear();
        m_macros.Clear();
        m_modules.Clear();

        foreach (var module in m_manager.m_modules)
        {
            if (!m_modules.ContainsKey(module.name))
            {
                m_modules[module.name] = module;
            }
        }

        LoadMacros();

        for(int i = 0; i < 16; i += 4)
        {

            GUIRow row = new GUIRow();
            row.Height = 100;

            for(int j = 0; j < 4; j++)
            {
                string name = (i + j).ToString();
                var button = CreateMacroButton(name, m_macros[name]);
                m_buttonMap[name] = button;
                row.Items.Add(button);
            }

            GUIRows.Add(row);

        }
    }
}
