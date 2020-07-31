using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MacroDatabase;

public abstract class BaseGUIModule : MonoBehaviour
{
    public bool m_hidden = false;

    public List<GUIRow> GUIRows = new List<GUIRow>();

    public List<GUIBase> Parameters = new List<GUIBase>();
    private Dictionary<string, GUIBase> parameterNameMap = new Dictionary<string, GUIBase>();

    private Dictionary<string, Macro> MacroNameToStateMap = new Dictionary<string, Macro>();
    private Dictionary<string, GUIMacroTrigger> MacroNameToButtonMap = new Dictionary<string, GUIMacroTrigger>();

    public string GetState()
    {
        string s = String.Empty;

        for (int i = 0; i < Parameters.Count; i++)
        {
            if (Parameters[i] is GUIFloat)
            {
                var name = Parameters[i].name;
                var data = Parameters[i].SerializeData();

                s += $"|{name}#{data}";
            }
        }

        return s;
    }

    public void SetState(string state)
    {
        var items = state.Split('|');

        for(int i = 0; i < items.Length; i ++)
        {
            if (items[i].Length > 0)
            {
                var fields = items[i].Split('#');
                var name = fields[0];
                var data = fields[1];

                if (parameterNameMap.ContainsKey(name))
                {
                    parameterNameMap[name].SetFromFieldsString(data);

                }
                else
                {
                    Debug.LogError($"Couldn't find parameter {name}");
                }
            }
        }
    }

    public virtual bool ShowMacros() { return true; }

    public void SaveMacro(string name)
    {
       
        var state = GetState();
        MacroNameToStateMap[name].Data = state;
        MacroNameToButtonMap[name].Assigned = true;
        MacroDatabase.SaveMacro(Name(), name, state);
    }

    public Dictionary<string, Macro> LoadMacroData()
    {
        Dictionary<string, Macro> macros = new Dictionary<string, Macro>();

        for(int i = 0; i < 5; i ++)
        {
            macros[i.ToString()] = TryGetMacro(Name(), i.ToString());
        }

        return macros;
    }


    public void SetMacro(string name)
    {
        if (MacroNameToStateMap.ContainsKey(name))
        {
            var m = MacroNameToStateMap[name];
            if (m.Data != String.Empty)
            {
                Debug.Log("SET" + m.Data);
                SetState(m.Data);
            }
            else
            {
                foreach (GUIBase parameter in Parameters)
                {
                    parameter.ResetToDefault();
                }
            }
        }
        else
        {
            Debug.LogError($"Cound not find macro {name}");
            return;
        }

        foreach( var m in MacroNameToButtonMap)
        {
            m.Value.Active = false;
        }

        MacroNameToButtonMap[name].Active = true;
    }


    void SetupMacros()
    {
        MacroNameToStateMap = LoadMacroData();

        GUIRow macroRow = new GUIRow();

        foreach(var m in MacroNameToStateMap)
        {
            var macroButton = createMacroButton(m.Value);
            MacroNameToButtonMap[m.Value.Name] = macroButton;
            macroRow.Items.Add(macroButton);
        }

        GUIRows.Insert(0, macroRow);

    }
    GUIMacroTrigger createMacroButton(Macro m)
    {
        var trigger = new GUIMacroTrigger(m.Name,
            delegate { this.SetMacro(m.Name); },
            delegate { this.SaveMacro(m.Name); }
        );

        trigger.Assigned = m.Data != String.Empty;

        return trigger;
    }

    public virtual void Init()
    {
        if (ShowMacros())
        {
            SetupMacros();

        }

        var titleRow = new GUIRow();
        titleRow.Items.Add(new GUITrigger(name, delegate { this.m_hidden = !this.m_hidden; }));
        GUIRows.Insert(0, titleRow);

        foreach(GUIBase p in Parameters)
        {
            parameterNameMap[p.name] = p;
        }

    }
    public abstract string Name();

    public virtual float GetHeight()
    {
        float height = 0;

  
        for (int i = 0; i < GUIRows.Count; i++)
        {
            height += GUIRows[i].GetHeight();

            if (m_hidden && i > 1)
                break;
        }
 
        return height;
    }


    public virtual void ManagerUpdate()
    {
        for (int i = 0; i < GUIRows.Count; i++)
        {
            GUIRows[i].Update();
        }
    }

    public virtual void UIUpdate()
    {
        if (!m_hidden)
        {
            for (int i = 0; i < GUIRows.Count; i++)
            {
                GUIRows[i].UIUpdate();
            }
        }
        else
        {
            // if hidden still update title row and macro row
            GUIRows[0].UIUpdate();

            if (ShowMacros())
            {
                GUIRows[1].UIUpdate();
            }
        }
    }

    public virtual void DrawGUI(Rect area)
    {
        var rowRect = new Rect(
            area.x,
            area.y + GUIUtility.BaseHeight * 3,
            area.width,
            0);

        for (int i = 0; i < GUIRows.Count; i++)
        {

            if (i > 1 && m_hidden)
                return;

            var row = GUIRows[i];
            rowRect.height = row.GetHeight();
            row.DrawGUI(rowRect);
            rowRect.y += rowRect.height;
        }
    }

    public virtual void Update() { }// Dont implement Monobehaviour update, update should be controlled by manager

}
