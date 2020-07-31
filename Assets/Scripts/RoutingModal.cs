using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public static class RoutingModal
{

    private static List<GUIRow> rows = new List<GUIRow>();

    private static GUIFloat target; 

    public static void Init()
    {
        var row = new GUIRow();

        row.Items.Add(new GUITrigger("low", delegate { target.routingType = RoutingType.AudioLow; }));
        row.Items.Add(new GUITrigger("all", delegate { target.routingType = RoutingType.AudioBypass; }));
        row.Items.Add(new GUITrigger("high", delegate { target.routingType = RoutingType.AudioHigh; }));

        rows.Add(row);

        row = new GUIRow();

        row.Items.Add(new GUITrigger("clear", delegate {
            target.routingType = RoutingType.None;
            target.ResetToDefault();
        }));

        rows.Add(row);


        row = new GUIRow();

        row.Items.Add(new GUITrigger("saw", delegate {
            target.routingType = RoutingType.Oscillator;
            target.oscillatorType = OscillatorType.Saw;
        }));

        row.Items.Add(new GUITrigger("square", delegate {
            target.routingType = RoutingType.Oscillator;
            target.oscillatorType = OscillatorType.Square;
        }));

        row.Items.Add(new GUITrigger("sin", delegate {
            target.routingType = RoutingType.Oscillator;
            target.oscillatorType = OscillatorType.Sine;
        }));

        rows.Add(row);

        row = new GUIRow();

        row.Items.Add(new GUITrigger("1", delegate { target.oscillatorFrequency = 1; }));
        row.Items.Add(new GUITrigger("2", delegate { target.oscillatorFrequency = 2; }));
        row.Items.Add(new GUITrigger("4", delegate { target.oscillatorFrequency = 4; }));
        row.Items.Add(new GUITrigger("8", delegate { target.oscillatorFrequency = 8; }));
        row.Items.Add(new GUITrigger("16", delegate { target.oscillatorFrequency = 16; }));

        rows.Add(row);


        // add lerp sliders
    }

 

    public static Rect GetWindowRect(Rect button)
    {
        var width = 300;
        var height = 200;
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
            GUIUtility.ControlModal = null;
            target = null;

        }

        rect.height = 50;
        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].DrawGUI(rect);
            rect.y += rect.height;

        }
    }
}
