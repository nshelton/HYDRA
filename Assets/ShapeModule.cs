using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeModule : BaseGUIModule
{

    public override string Name() { return "shape"; }
    [SerializeField] public Material _material; 

    public override void InitInternal()
    {
        Parameters.Add(new GUIFloat("speed", 0, 0.5f, 0.1f, delegate (float v) { _material.SetFloat("_Size", v); }));
        Parameters.Add(new GUIFloat("num", 0, 10, 1, delegate (float v) { _material.SetFloat("_Num", v); }));
        Parameters.Add(new GUIFloat("numY", 0, 10, 1, delegate (float v) { _material.SetFloat("_NumY", v); }));
        Parameters.Add(new GUIFloat("offset", 0, 1, 0, delegate (float v) { _material.SetFloat("_Offset", v); }));
        Parameters.Add(new GUIFloat("Type", 0, 2, 0, delegate (float v) { _material.SetFloat("_Type", v); }));
        Parameters.Add(new GUIFloat("Rotation", 0, 3.141592f, 0, delegate (float v) { _material.SetFloat("_Rotation", v); }));

        foreach (var p in Parameters)
        {
            var row = new GUIRow();
            row.Items.Add(p);
            GUIRows.Add(row);
        }
    }
}
