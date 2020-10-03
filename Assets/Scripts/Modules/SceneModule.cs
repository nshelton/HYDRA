using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneModule : BaseGUIModule
{
    public GameObject[] m_scenes;

    public override string Name() { return "scenes"; }
    public override bool ShowPresets() { return false; }

    public override void InitInternal()
    {
        for (int i = 0; i < m_scenes.Length; i+=2)
        {
            var row = new GUIRow();

            var scene = m_scenes[i];
            var p = new GUIToggle( scene.name, scene.SetActive );
            Parameters.Add(p);
            row.Items.Add(p);

            var scene2 = m_scenes[i+1];
            var p2 = new GUIToggle( scene2.name, scene2.SetActive );
            Parameters.Add(p2);
            row.Items.Add(p2);

            GUIRows.Add(row);
        }
    }

}
