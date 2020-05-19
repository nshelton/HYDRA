using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.UI.Slider;

public class guiSpawner : MonoBehaviour
{
    [SerializeField] GameObject GuiScaffold;
    [SerializeField] GameObject SliderObject;
    [SerializeField] GameObject ToggleObject;

    [SerializeField] SliderEvent[] Parameters;
    [SerializeField] Toggle.ToggleEvent[] Toggles;

    [SerializeField] bool AutoGlobSiblings = false;

    void Start()
    {
        var gui = Instantiate(GuiScaffold, transform);
        var canvas = gui.GetComponentInChildren<Canvas>();


        for( int i = 0; i < Parameters.Length; i ++)
        {
            string name = Parameters[i].GetPersistentMethodName(0).Replace("set_", "");

            var slider = Instantiate(SliderObject, canvas.transform);
            slider.GetComponentInChildren<Text>().text = name;

            slider.GetComponent<Slider>().onValueChanged = Parameters[i];
            slider.transform.position = slider.transform.position + Vector3.down * 30f * i;
        }

        for (int i = 0; i < Toggles.Length; i++)
        {
            string name = Toggles[i].GetPersistentMethodName(0).Replace("set_", "");

            var slider = Instantiate(ToggleObject, canvas.transform);
            slider.GetComponentInChildren<Text>().text = name;

            slider.GetComponent<Toggle>().onValueChanged = Toggles[i];
            slider.transform.position = slider.transform.position + Vector3.right * 100f * i;
        }

    }

}
