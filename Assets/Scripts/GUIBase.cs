using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoutingType
{
    None,
    AudioLow,
    AudioBypass,
    AudioHigh,
    Oscillator,
}

public enum OscillatorType
{
    Saw,
    Square,
    Sine
}

public class GUIBase 
{

    public string name = "undefined";
    public RoutingType routingType;
    public OscillatorType oscillatorType;
    public int oscillatorFrequency = 1;

    public float lerp = 1;
    public float power = 1;
    public float duty = 1;

    public virtual void UIUpdate()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void ResetToDefault()
    {

    }

    public virtual void DrawGUI(Rect areaRect)
    {
      
    }

    public virtual void SetFromFieldsString(string s)
    {
        throw new NotImplementedException();
    }

    public virtual string SerializeData()
    {
        throw new NotImplementedException();
    }
}

