using System.Collections.Generic;
using UnityEngine.VFX;

public class VFXModule : BaseGUIModule
{

    private VisualEffect m_effect;

    private string m_name;
    public override string Name() { return m_name; }

    public override void InitInternal()
    {
        m_effect = GetComponent<VisualEffect>();

        m_name = "VFX." + m_effect.name;

        List<VFXExposedProperty> exposedProperties = new List<VFXExposedProperty>();

         m_effect.visualEffectAsset.GetExposedProperties(exposedProperties);

        foreach( var prop in exposedProperties)
        {
            if (prop.type == typeof(float))
            {
                var row = new GUIRow();
                var p = new GUIFloat(prop.name, 0, 1, 0.5f, delegate (float v)
                {
                    m_effect.SetFloat(prop.name, v);
                });
                row.Items.Add(p);
                Parameters.Add(p);
                GUIRows.Add(row);
            }
        }
    }
}
