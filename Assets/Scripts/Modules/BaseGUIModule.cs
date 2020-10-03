using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PresetDatabase;

public abstract class BaseGUIModule : MonoBehaviour
{
    public bool m_hidden = false;

    public List<GUIRow> GUIRows = new List<GUIRow>();

    public List<GUIBase> Parameters = new List<GUIBase>();
    private Dictionary<string, GUIBase> parameterNameMap = new Dictionary<string, GUIBase>();

    private Dictionary<string, Preset> PresetNameToStateMap = new Dictionary<string, Preset>();
    private Dictionary<string, GUIPresetTrigger> PresetNameToButtonMap = new Dictionary<string, GUIPresetTrigger>();

    public string GetState()
    {
        string s = String.Empty;

        for (int i = 0; i < Parameters.Count; i++)
        {
            if (Parameters[i] is GUIFloat || Parameters[i] is GUIToggle)
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

    public virtual bool ShowPresets() { return true; }
    public virtual bool ShowTitle() { return true; }

    public void SavePreset(string name)
    {
       
        var state = GetState();
        PresetNameToStateMap[name].Data = state;
        PresetNameToButtonMap[name].Assigned = true;
        PresetDatabase.SavePreset(Name(), name, state);

    }

    public Dictionary<string, Preset> LoadPresetData()
    {
        Dictionary<string, Preset> Presets = new Dictionary<string, Preset>();

        for(int i = 0; i < 5; i ++)
        {
            Presets[i.ToString()] = TryGetPreset(Name(), i.ToString());
        }

        return Presets;
    }


    public void SetPreset(string name)
    {
        if (PresetNameToStateMap.ContainsKey(name))
        {
            var m = PresetNameToStateMap[name];
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
            Debug.LogError($"Cound not find Preset {name}");
            return;
        }

        foreach( var m in PresetNameToButtonMap)
        {
            m.Value.Active = false;
        }

        PresetNameToButtonMap[name].Active = true;
    }


    GUIRow CreatePresetRow()
    {
        PresetNameToStateMap = LoadPresetData();

        GUIRow PresetRow = new GUIRow();

        foreach(var m in PresetNameToStateMap)
        {
            var PresetButton = createPresetButton(m.Value);
            PresetNameToButtonMap[m.Value.Name] = PresetButton;
            PresetRow.Items.Add(PresetButton);
        }

        return PresetRow;
    }

    GUIPresetTrigger createPresetButton(Preset m)
    {
        var trigger = new GUIPresetTrigger(m.Name,
            delegate { this.SetPreset(m.Name); },
            delegate { this.SavePreset(m.Name); }
        );

        trigger.Assigned = m.Data != String.Empty;

        return trigger;
    }
    public virtual void InitInternal() { }

    public virtual void Init()
    {
        GUIRows.Clear();
        Parameters.Clear();

        if (ShowTitle())
        {
            var titleRow = new GUIRow();
            titleRow.GUIColor = Color.cyan;
            titleRow.Items.Add(new GUITrigger(name, delegate { this.m_hidden = !this.m_hidden; }));
            GUIRows.Add(titleRow);
        }

        if (ShowPresets())
        {
            var PresetRow = CreatePresetRow();  
            GUIRows.Add(PresetRow);
        }

        InitInternal();

        foreach (GUIBase p in Parameters)
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
            height += GUIRows[i].Height;

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
            // if hidden still update title row and Preset row
            GUIRows[0].UIUpdate();

            if (ShowPresets())
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
            rowRect.height = row.Height;
            row.DrawGUI(rowRect);
            rowRect.y += rowRect.height;
        }
    }

    public virtual void Update() { }// Dont implement Monobehaviour update, update should be controlled by manager

}
