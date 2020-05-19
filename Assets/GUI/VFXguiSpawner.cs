using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
using static UnityEngine.UI.Slider;


public class ParameterContext 
{
    private string m_name;
    private VisualEffect m_target;
    public ParameterContext(VisualEffect target, string name)
    {
        m_name = name;
        m_target = target;
    }

    public void SetValue(float f)
    {
        m_target.SetFloat(m_name, f);
    }

}

public class VFXguiSpawner : MonoBehaviour
{
    [SerializeField] GameObject GuiScaffold;
    [SerializeField] GameObject SliderObject;

    [SerializeField] VisualEffect Target;
    [SerializeField] string[] floatParams;

    private ParameterContext[] m_contexts;
    void Start()
    {
        m_contexts = new ParameterContext[floatParams.Length];
        var binder = gameObject.AddComponent<VFXPropertyBinder>();
        var gui = Instantiate(GuiScaffold, transform);
        var canvas = gui.GetComponentInChildren<Canvas>();


        for( int i = 0; i < floatParams.Length; i ++)
        {
            string name = floatParams[i];

            var slider = Instantiate(SliderObject, canvas.transform);
            slider.GetComponentInChildren<Text>().text = name;

            m_contexts[i] = new ParameterContext(Target, name);
            slider.GetComponent<Slider>().onValueChanged.AddListener(m_contexts[i].SetValue);
            slider.transform.position = slider.transform.position + Vector3.down * 30f * i;
        }
    }
}
