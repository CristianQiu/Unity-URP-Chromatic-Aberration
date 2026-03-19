using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

/// <summary>
/// The chromatic aberration render pass.
/// </summary>
public sealed class ChromaticAberrationRenderPass : ScriptableRenderPass
{
	#region Definitions

	/// <summary>
	/// Holds the data needed by the execution of the render pass.
	/// </summary>
	private class PassData
	{
		public TextureHandle source;

		public Material material;
		public int materialPassIndex;
	}

	#endregion

	#region Private Attributes

	private static readonly int DisplacementId = Shader.PropertyToID("_Displacement");
	private static readonly int IntensityId = Shader.PropertyToID("_Intensity");

	private Material material;

	#endregion

	#region Initialization Methods

	public ChromaticAberrationRenderPass(Material material) : base()
	{
		profilingSampler = new ProfilingSampler("Chromatic Aberration");
		renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
		requiresIntermediateTexture = false;

		this.material = material;
	}

	#endregion

	#region Scriptable Render Pass Methods

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="renderGraph"></param>
	/// <param name="frameData"></param>
	public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
	{
		UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
		TextureHandle blitHandle = CreateRenderGraphTextures(renderGraph, resourceData);

		using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass("Chromatic Aberration", out PassData passData, profilingSampler))
		{
			passData.source = resourceData.activeColorTexture;
			passData.material = material;
			passData.materialPassIndex = 0;

			builder.SetRenderAttachment(blitHandle, 0, AccessFlags.WriteAll);
			builder.UseTexture(resourceData.activeColorTexture);
			builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
		}

		resourceData.cameraColor = blitHandle;
	}

	#endregion

	#region Methods

	/// <summary>
	/// Creates and returns the necessary render graph texture handle to blit to.
	/// </summary>
	/// <param name="renderGraph"></param>
	/// <param name="resourceData"></param>
	/// <returns></returns>
	private TextureHandle CreateRenderGraphTextures(RenderGraph renderGraph, UniversalResourceData resourceData)
	{
		return renderGraph.CreateTexture(resourceData.activeColorTexture, "_ChromaticAberration");
	}

	/// <summary>
	/// Updates the material parameters according to the volume settings.
	/// </summary>
	/// <param name="material"></param>
	private static void UpdateMaterialParameters(Material material)
	{
		ChromaticAberrationVolume volume = VolumeManager.instance.stack.GetComponent<ChromaticAberrationVolume>();

		material.SetFloat(DisplacementId, volume.displacement.value * 0.1f);
		material.SetFloat(IntensityId, volume.intensity.value);
	}

	/// <summary>
	/// Executes the pass with the information from the pass data.
	/// </summary>
	/// <param name="passData"></param>
	/// <param name="context"></param>
	private static void ExecutePass(PassData passData, RasterGraphContext context)
	{
		UpdateMaterialParameters(passData.material);

		Blitter.BlitTexture(context.cmd, passData.source, Vector2.one, passData.material, passData.materialPassIndex);
	}

	#endregion
}