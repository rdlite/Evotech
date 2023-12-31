using UnityEngine;
using QOutline.Core;
using UnityEngine.Rendering;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class RenderMultipleObjectsPass : ScriptableRenderPass
{
    private List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>() { new ShaderTagId("UniversalForward"), new ShaderTagId("SRPDefaultUnlit"), new ShaderTagId("UniversalForwardOnly") };

    private RTHandle _destination;

    private List<FilteringSettings> _filteringSettings;
    private RenderStateBlock _renderStateBlock;
    private List<OutlineBatchesResolver.OutlineBatch> _batchesToRender = new List<OutlineBatchesResolver.OutlineBatch>(10);
    private bool _isClipDepth;

    public RenderMultipleObjectsPass(ref RTHandle destination, ref List<OutlineBatchesResolver.OutlineBatch> batchesToRender, bool isClipDepth)
    {
        _destination = destination;
        _batchesToRender = batchesToRender;

        _isClipDepth = isClipDepth;

        _filteringSettings = new List<FilteringSettings>(10);
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        var colorDesc = renderingData.cameraData.cameraTargetDescriptor;
        colorDesc.depthBufferBits = 0;
        colorDesc.colorFormat = RenderTextureFormat.ARGBHalf;

        _filteringSettings.Clear();

        foreach (var batch in _batchesToRender)
        {
            _filteringSettings.Add(new FilteringSettings(RenderQueueRange.all, batch.CurrentRenderLayer));
        }

        RenderingUtils.ReAllocateIfNeeded(
            ref _destination, 
            colorDesc, 
            wrapMode: TextureWrapMode.Clamp, 
            name: "_DestinationRenderTexture");

        RTHandle rtCameraDepth = renderingData.cameraData.renderer.cameraDepthTargetHandle;

        cmd.GetTemporaryRT(0, colorDesc);

        if (_isClipDepth)
        {
            ConfigureTarget(_destination, rtCameraDepth);
        }
        else
        {
            ConfigureTarget(_destination);
        }

        ConfigureClear(ClearFlag.Color, new Color(0, 0, 0, 0));
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();

        if (_filteringSettings != null && _filteringSettings.Count != 0)
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            using (new ProfilingScope(cmd, new ProfilingSampler("Camera render layers")))
            {
                SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
                DrawingSettings drawingSettings = CreateDrawingSettings(_shaderTagIdList, ref renderingData, sortingCriteria);

                int it = 0;

                RTHandle rtCameraDepth = renderingData.cameraData.renderer.cameraDepthTargetHandle;

                foreach (var filteringSetting in _filteringSettings)
                {
                    drawingSettings.overrideMaterial = _batchesToRender[it].OverrideMaterial;
                    FilteringSettings settings = filteringSetting;

                    context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref settings, ref _renderStateBlock);

                    it++;
                }
            }
        }

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    public RTHandle GetCurrentDestination()
    {
        return _destination;
    }
}