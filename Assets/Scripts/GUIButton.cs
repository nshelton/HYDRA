﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

 
public class GUITrigger : GUIBase
{
    protected CustomButton Button;

    public GUITrigger(string name, Action effectL, Action effectR)
    {
        Button = new CustomButton(name, effectL, effectR);
        this.name = name;
    }

    public GUITrigger(string name, Action effectL)
    {
        Button = new CustomButton(name, effectL, null);
        this.name = name;
    }

    public override void DrawGUI(Rect buttonRect)
    {
        Button.DrawGUI(buttonRect, false);
    }
    public override void Update()
    {
        Button.Update();
        base.Update();
    }
}
 
public class GUIToggle : GUIBase
{
    bool Enabled;
    private CustomButton Button;
    public GUIToggle(string name, Action<bool> effect)
    {
        this.Enabled = false;
        this.name = name;

        Button = new CustomButton(this.name, delegate
        {
            this.Enabled = !this.Enabled;
            effect(Enabled);

        }, null);
    }

    public override void DrawGUI(Rect buttonRect)
    {
        Button.DrawGUI(buttonRect, Enabled);
    }

    public override void Update()
    {
        Button.Update();
        base.Update();
    }
}

public class CustomButton 
{

    Action actionLeft;
    Action actionRight;

    string name;

    public CustomButton(string name, Action actionLeft, Action actionRight)
    {
        this.actionLeft = actionLeft;
        this.actionRight = actionRight;
        this.name = name;
    }

    Rect currentRect = new Rect();
    public void Update()
    {
        Vector2 mouse = Input.mousePosition;
        mouse.y = Screen.height - mouse.y;

        if (currentRect.Contains(mouse))
        {
            if (Input.GetMouseButtonUp(0))
            {
                actionLeft();
            }
            if (Input.GetMouseButtonUp(1) && actionRight != null)
            {
                actionRight();
            }
        }
    }

    public void DrawGUI(Rect area, bool enabled)
    {
        currentRect = area;
        GUI.Label(area, name );

        GUI.DrawTexture(area, enabled ? GUIUtility.GreenTexture : GUIUtility.WhiteTexture );
    }

    public void DrawGUI(Rect area, Texture2D texture)
    {
        currentRect = area;
        GUI.Label(area, name);
        GUI.DrawTexture(area, texture);
    }
}


public class GUIMacroTrigger : GUITrigger
{
    public bool Assigned = false;
    public bool Active = false;

    public GUIMacroTrigger(string name, Action effectL, Action effectR) :
        base(name, effectL, effectR)
    {
    }

    public override void DrawGUI(Rect buttonRect)
    {
        var texture = GUIUtility.GrayTexture;

        if (Active)
        {
            texture = GUIUtility.GreenTexture;
        }
        else if (Assigned)
        {
            texture = GUIUtility.WhiteTexture;
        }

        Button.DrawGUI(buttonRect, texture);

    }
}