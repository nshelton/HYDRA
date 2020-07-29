using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public abstract class BaseGUIModule : MonoBehaviour
{
    public bool m_hidden = false;

    public List<GUIRow> GUIRows = new List<GUIRow>();


    public List<GUIBase> Parameters = new List<GUIBase>();
    private Dictionary<string, GUIBase> parameterNameMap = new Dictionary<string, GUIBase>();


    private Dictionary<string, string> MacroNameToStateMap = new Dictionary<string, string>();

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

    public void SaveMacro(int i)
    {
        MacroNameToStateMap[i.ToString()] = GetState();
    }

    public void SetMacro(int i)
    {
        if ( i == 0) {
            foreach (GUIBase parameter in Parameters)
            {
                parameter.ResetToDefault();
            }
        } 
        else
        {
            var macroName = i.ToString();
            if (MacroNameToStateMap.ContainsKey(macroName))
            {
                SetState(MacroNameToStateMap[i.ToString()]);
            } else
            {
                Debug.LogError($"Cound not find macro {macroName}");
            }
        }
    }

    GUITrigger createMacroButton(int i)
    {
        return  new GUITrigger(i.ToString(),
            delegate { this.SetMacro(i); },
            delegate { this.SaveMacro(i); }
        );
    }

    public virtual void Init()
    {
        if (ShowMacros())
        {
            var macroRow = new GUIRow();

            macroRow.Items.Add(createMacroButton(0));
            macroRow.Items.Add(createMacroButton(1));
            macroRow.Items.Add(createMacroButton(2));
            macroRow.Items.Add(createMacroButton(3));

            GUIRows.Insert(0, macroRow);

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

    public virtual void Update()
    {
        foreach (var row in GUIRows)
        {
            row.Update();
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
}
