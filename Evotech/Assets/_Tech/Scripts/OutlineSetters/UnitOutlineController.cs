using System.Linq;
using UnityEngine;
using QOutline.Core;
using QOutline.Configs;
using System.Collections.Generic;

namespace Core.Battle.Outline
{
    public class UnitOutlineController : MonoBehaviour
    {
        private List<Renderer> _childRenderers;
        private List<int> _childRendererLayers;
        private OutlineConfigs _outlineConfig;
        private int _defaultLayer;
        private int _currentBatchID;
        private int _outlineRequestersAmount;
        private bool _isActive;
        private bool _isSnapped;

        public void Init(OutlineConfigs outlineConfig)
        {
            _outlineConfig = outlineConfig;
            _childRendererLayers = new List<int>();

            _childRenderers = GetComponentsInChildren<Renderer>(true).Where(renderer => (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)).ToList();
            _childRenderers.ForEach((renderer) => _childRendererLayers.Add(renderer.gameObject.layer));
        }

        public void AddObjectsToBatch()
        {
            _outlineRequestersAmount++;

            if (_isActive || _isSnapped) return;

            _isActive = true;

            if (_outlineRequestersAmount == 1)
            {
                OutlineBatchesResolver.OutlineDataToStore batch = new OutlineBatchesResolver.OutlineDataToStore(_currentBatchID, _outlineConfig, _childRenderers, _childRendererLayers);
                _currentBatchID = OutlineBatchesResolver.AddBacth(batch);
            }
        }

        public void RemoveObjectsFromBatch()
        {
            _outlineRequestersAmount = Mathf.Clamp(_outlineRequestersAmount - 1, 0, int.MaxValue);

            if (!_isActive || _isSnapped) return;

            _isActive = false;
            
            if (_outlineRequestersAmount == 0)
            {
                OutlineBatchesResolver.RemoveBatch(_currentBatchID);
            }
        }

        public void SnapOutline(bool isSnap)
        {
            _isSnapped = isSnap;
        }
    }
}