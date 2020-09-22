using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using Boo.Lang.Environments;

public static class RoutingModal
{

    private static List<GUIRow> rows = new List<GUIRow>();

    private static GUIFloat target; 

    private static int ROW_HEIGHT = 50;


    public static GUIToggle m_lowButton;
    public static GUIToggle m_midButton;
    public static GUIToggle m_highButton;

    public static GUIToggle m_saw;
    public static GUIToggle m_square;
    public static GUIToggle m_sine;

    public static GUIToggle m_1;
    public static GUIToggle m_2;
    public static GUIToggle m_4;
    public static GUIToggle m_8;
    public static GUIToggle m_16;

    public static GUIFloat m_lerp;
    public static GUIFloat m_duty;
    public static GUIFloat m_power;


    public static void Init()
    {
        rows.Clear();
        var row = new GUIRow();
        m_lowButton = new GUIToggle("low", delegate { target.routingType = RoutingType.AudioLow; });
        row.Items.Add(m_lowButton);
        m_midButton = new GUIToggle("all", delegate { target.routingType = RoutingType.AudioBypass; });
        row.Items.Add(m_midButton);
        m_highButton = new GUIToggle("high", delegate { target.routingType = RoutingType.AudioHigh; });
        row.Items.Add(m_highButton);

        rows.Add(row);

        row = new GUIRow();

        row.Items.Add(new GUITrigger("clear", delegate {
            target.routingType = RoutingType.None;
            target.ResetToDefault();
            Close();
        }));

        rows.Add(row);


        row = new GUIRow();

        m_saw = new GUIToggle("saw", delegate {
            target.routingType = RoutingType.Oscillator;
            target.oscillatorType = OscillatorType.Saw;
        });

        m_square = new GUIToggle("square", delegate {
            target.routingType = RoutingType.Oscillator;
            target.oscillatorType = OscillatorType.Square;
        });

        m_sine = new GUIToggle("sin", delegate {
            target.routingType = RoutingType.Oscillator;
            target.oscillatorType = OscillatorType.Sine;
        });

        row.Items.Add(m_saw);
        row.Items.Add(m_square);
        row.Items.Add(m_sine);

        rows.Add(row);

        row = new GUIRow();

        m_1 = new GUIToggle("1", delegate { target.oscillatorFrequency = 1; });
        m_2 = new GUIToggle("2", delegate { target.oscillatorFrequency = 2; });
        m_4 = new GUIToggle("4", delegate { target.oscillatorFrequency = 4; });
        m_8 = new GUIToggle("8", delegate { target.oscillatorFrequency = 8; });
        m_16 = new GUIToggle("16", delegate { target.oscillatorFrequency = 16; });

        row.Items.Add(m_1);
        row.Items.Add(m_2);
        row.Items.Add(m_4);
        row.Items.Add(m_8);
        row.Items.Add(m_16);

        rows.Add(row);

        row = new GUIRow();
        m_lerp = new GUIFloat("lerp", 0, 1, 0.9f, delegate (float v)  {  target.lerp = v;  });
        row.Items.Add(m_lerp);
        rows.Add(row);
        
        row = new GUIRow();
        m_duty = new GUIFloat("duty", 0, 1, 0.9f, delegate (float v)  {  target.duty = v;  });
        row.Items.Add(m_duty);
        rows.Add(row);

        row = new GUIRow();
        m_power = new GUIFloat("power", 0, 2, 1f, delegate (float v)  {  target.power = v;  });
        row.Items.Add(m_power);
        rows.Add(row);
    }


    public static void SetTarget(GUIFloat target)
    {
        GUIUtility.ActiveControl = target;
        GUIUtility.ControlModal = target;

        // set toggles to the state of the float
        bool isOsc = target.routingType == RoutingType.Oscillator;
        m_saw.Enabled = isOsc && target.oscillatorType == OscillatorType.Saw;
        m_sine.Enabled = isOsc && target.oscillatorType == OscillatorType.Sine;
        m_square.Enabled = isOsc && target.oscillatorType == OscillatorType.Square;

        m_lowButton.Enabled = target.routingType == RoutingType.AudioLow;
        m_midButton.Enabled = target.routingType == RoutingType.AudioBypass;
        m_highButton.Enabled = target.routingType == RoutingType.AudioHigh;

        m_1.Enabled = isOsc && target.oscillatorFrequency == 1;
        m_2.Enabled = isOsc && target.oscillatorFrequency == 2;
        m_4.Enabled = isOsc && target.oscillatorFrequency == 4;
        m_8.Enabled = isOsc && target.oscillatorFrequency == 8;
        m_16.Enabled = isOsc && target.oscillatorFrequency == 16;

        m_lerp.value = target.lerp;
        m_duty.value = target.duty;
        m_power.value = target.power;

    }

    public static Rect GetWindowRect(Rect button)
    {
        var width = GUIManager.MODULEWIDTH;
        var height = rows.Count * ROW_HEIGHT;
        var c = button.center;
        return new Rect(c.x - width / 2, c.y - height / 2, width, height);
    }

    public static void Update()
    {
        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].Update();
        }
    }

    public static void UIUpdate()
    {
        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].UIUpdate();
        }
    }

    public static void Close()
    {
        GUIUtility.ControlModal = null;
        target = null;
    }

    public static void DrawGUI()
    {
        target = GUIUtility.ControlModal as GUIFloat;

        if ( target == null) 
        {
            Debug.LogError("Can't do modal for non-float parameter");
            return;
        }

        var rect = GetWindowRect(target.currentRect);

        if (Event.current.button == 0 && !GetWindowRect(rect).Contains(Event.current.mousePosition))
        {
            Close();
            return;
        }

        GUI.DrawTexture(rect, GUIUtility.BlackTexture);

        rect.x = rect.xMax;
        rect.width = 10;
        float normVal = (target.value - target.min) / (target.max - target.min);
        rect.y += rect.height *( 1 - normVal);
        rect.height *= normVal;

        GUI.DrawTexture(rect, GUIUtility.GreenTexture);


        rect = GetWindowRect(target.currentRect);
        rect.height = ROW_HEIGHT;
        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].DrawGUI(rect);
            rect.y += rect.height;

        }
      

    }
}
