using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Volume component for the chromatic aberration.
/// </summary>
[DisplayInfo(name = "Chromatic Aberration")]
[VolumeComponentMenu("Custom/Chromatic Aberration")]
[VolumeRequiresRendererFeatures(typeof(ChromaticAberrationFeature))]
[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
public sealed class ChromaticAberrationVolume : VolumeComponent, IPostProcessComponent
{
	#region Public Attributes

	public ClampedFloatParameter intensity = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

	#endregion

	#region Methods

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns></returns>
	public bool IsActive()
	{
		return intensity.value > 0.0f;
	}

	#endregion
}