using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(feedbackRenderer), PostProcessEvent.AfterStack, "Custom/feedback")]
public sealed class feedback : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("feedback effect intensity.")]
    public FloatParameter Blend = new FloatParameter { value = 0.5f };
}

public sealed class feedbackRenderer : PostProcessEffectRenderer<feedback>
{

    private RenderTexture m_feedbackBufferA;
    private RenderTexture m_feedbackBufferB;

    public override void Render(PostProcessRenderContext context)
    {
        if (m_feedbackBufferA == null || m_feedbackBufferA.width != context.width)
        {
            m_feedbackBufferA = new RenderTexture(context.width, context.height, 0, context.sourceFormat);
            m_feedbackBufferB = new RenderTexture(context.width, context.height, 0, context.sourceFormat);
        }

        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/feedback"));

        sheet.properties.SetTexture("_FeedbackTex", m_feedbackBufferA);
        sheet.properties.SetFloat("_Blend", settings.Blend);
        context.command.BlitFullscreenTriangle(context.source, m_feedbackBufferB, sheet, 0);

        context.command.BlitFullscreenTriangle(m_feedbackBufferB, m_feedbackBufferA);
        context.command.BlitFullscreenTriangle(m_feedbackBufferA, context.destination);
    }
}