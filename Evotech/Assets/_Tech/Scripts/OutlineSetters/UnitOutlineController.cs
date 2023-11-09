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
        private OutlineConfigs _outlineConfig;
        private int _defaultLayer;
        private int _currentBatchID;
        private bool _isActive;
        private bool _isSnapped;

        public void Init(OutlineConfigs outlineConfig)
        {
            _outlineConfig = outlineConfig;
            _childRenderers = GetComponentsInChildren<Renderer>().Where(renderer => (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)).ToList();
        }

        public void AddObjectsToBatch()
        {
            if (_isActive || _isSnapped) return;

            _isActive = true;

            OutlineBatchesResolver.OutlineDataToStore batch = new OutlineBatchesResolver.OutlineDataToStore(_currentBatchID, _outlineConfig, _childRenderers, _defaultLayer);
            _currentBatchID = OutlineBatchesResolver.AddBacth(batch);
        }

        public void RemoveObjectsFromBatch()
        {
            if (!_isActive || _isSnapped) return;

            _isActive = false;

            OutlineBatchesResolver.RemoveBatch(_currentBatchID);
        }

        public void SnapOutline(bool isSnap)
        {
            _isSnapped = isSnap;
        }
    }
}