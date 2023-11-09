using Utils;
using Qnject;
using UnityEngine;
using QOutline.Core;
using QOutline.Render;
using QOutline.Configs;
using System.Reflection;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace QOutline.Tools
{
    public class OutlineSetter : MonoBehaviour
    {
        private OutlineFeature _outlineFeature;
        private OutlineBatchesResolver _outlinesResolver;
        private OutlinesContainer _container;
        private IUpdateProvider _updateProvider;

        [Inject]
        private void Construct(OutlinesContainer outlinesContainer, IUpdateProvider updateProvider)
        {
            UpdateCurrenRenderFeature();

            _container = outlinesContainer;
            _updateProvider = updateProvider;
            _outlinesResolver = new OutlineBatchesResolver(_outlineFeature, _container.DisposeBatchMask);
            _updateProvider.AddUpdate(Tick);
        }

        private void OnEnable()
        {
            _updateProvider.AddUpdate(Tick);
        }

        private void OnDisable()
        {
            _updateProvider.RemoveUpdate(Tick);
        }

        private void Tick()
        {
            _outlinesResolver.Tick();
        }

        private void UpdateCurrenRenderFeature()
        {
            var pipeline = ((UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset);
            FieldInfo propertyInfo = pipeline.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
            ScriptableRendererData[] scriptableRendererData = ((ScriptableRendererData[])propertyInfo?.GetValue(pipeline));

            for (int i = 0; i < scriptableRendererData.Length; i++)
            {
                foreach (var feature in scriptableRendererData[i].rendererFeatures)
                {
                    if (feature is OutlineFeature)
                    {
                        _outlineFeature = feature as OutlineFeature;
                        break;
                    }
                }
            }
        }
    }
}