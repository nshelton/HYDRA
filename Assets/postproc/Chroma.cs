

using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ChromaRenderer), PostProcessEvent.AfterStack, "Custom/Chroma")]
public sealed class Chroma : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Chroma effect intensity.")]
    public FloatParameter _amount = new FloatParameter();
}

public sealed class ChromaRenderer : PostProcessEffectRenderer<Chroma>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Chroma"));
        sheet.properties.SetFloat("_Amount", 100 * settings._amount.value);
        sheet.properties.SetFloat("_Width", context.width);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}