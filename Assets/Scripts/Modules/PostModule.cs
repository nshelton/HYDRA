using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PostModule : BaseGUIModule
{

    private PostProcessProfile m_profile;

    public override string Name() { return "postfx"; }

    private void AddParameters<T>(PostProcessProfile profile) where T : PostProcessEffectSettings
    {
        foreach (var effect in profile.settings)
        {
            var effectName = effect.GetType().Name;
            var cast = effect as T;

            if (cast != null)
            {
                foreach (var thisVar in cast.GetType().GetFields())
                {
                    UnityEngine.Rendering.PostProcessing.FloatParameter item = thisVar.GetValue(cast) as UnityEngine.Rendering.PostProcessing.FloatParameter;
                    if (item != null)
                    {
                        var row = new GUIRow();
                        var parameter = new GUIFloat(effectName + "." + thisVar.Name,
                            0, 1, 0, delegate (float v)
                            {
                                item.value = v;
                            });
                        Parameters.Add(parameter);
                        row.Items.Add(parameter);
                        GUIRows.Add(row);
                    }
                }
                // add some spacing
                GUIRows.Add(new GUIRow());
            }
        }
    }

    public override void InitInternal()
    {
        m_profile = GetComponent<PostProcessVolume>().profile;
        AddParameters<Chroma>(m_profile);
        AddParameters<Mirror>(m_profile);
        AddParameters<vhs>(m_profile);
        AddParameters<feedback>(m_profile);
    }
}
