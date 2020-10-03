using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRenderTextureUpdate : BaseGUIModule
{
    [SerializeField] CustomRenderTexture _Input;

    [SerializeField] float m_Width = 0.1f;
    [SerializeField] float m_Type;
    [SerializeField] float m_Rotate;
    [SerializeField] float m_Margin;
    [SerializeField] float m_Repeat = 1;
    [SerializeField] float m_Amount = 0;
    [SerializeField] float m_Scroll = 0;


    private float m_ScrollPos;
    public override string Name()
    {
        return "primitives";
    }

    void Start()
    {
        _Input.Initialize();

      
    }

    public override void InitInternal()
    {
        Parameters.Add(new GUIFloat("Width", 0, 0.5f, 0.1f,
         delegate (float v) { m_Width = v; }));

        Parameters.Add(new GUIFloat("type", 0, 5f, 0.1f,
            delegate (float v) { m_Type = v; }));

        Parameters.Add(new GUIFloat("rotate", 0, Mathf.PI * 2f, 0.1f,
            delegate (float v) { m_Rotate = v; }));

        Parameters.Add(new GUIFloat("scroll", 0, 0.1f, 0.1f,
            delegate (float v) { m_Scroll = v; }));

        Parameters.Add(new GUIFloat("Margin", 0, 0.1f, 0.1f,
            delegate (float v) { m_Margin = v; }));

        Parameters.Add(new GUIFloat("Repeat", 0, 100f, 1f,
            delegate (float v) { m_Repeat = v; }));

        Parameters.Add(new GUIFloat("Amount", 0, 1, 0f,
            delegate (float v) { m_Amount = v; }));

        foreach (var p in Parameters)
        {
            var r = new GUIRow();
            r.Items.Add(p);
            GUIRows.Add(r);
        }
    }

    void Update()
    {
        m_ScrollPos += Time.deltaTime * m_Scroll;
        _Input.material.SetFloat("_Scroll", m_ScrollPos);
        _Input.material.SetFloat("_Width", m_Width);
        _Input.material.SetFloat("_Type", m_Type);
        _Input.material.SetFloat("_Rotate", m_Rotate);
        _Input.material.SetFloat("_Margin", m_Margin);
        _Input.material.SetFloat("_Repeat", m_Repeat);
        _Input.material.SetFloat("_Amount", m_Amount);
        _Input.Update(1);

    }
}
