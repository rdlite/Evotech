using QOutline.Configs;
using QOutline.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Battle.Outline
{
    public class UnitOutlineController : MonoBehaviour
    {
        private List<Renderer> _childRenderers;
        private OutlineConfigs _outlineConfig;
        private int _defaultLayer;
        private int _currentBatchID;
        private bool _isActive;

        public void Init(OutlineConfigs outlineConfig)
        {
            _outlineConfig = outlineConfig;
            _childRenderers = GetComponentsInChildren<Renderer>().Where(renderer => (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)).ToList();

            Invoke(nameof(EnableOutline), .2f);
        }

        private void EnableOutline()
        {
            AddObjectsToBatch();
        }

        public void AddObjectsToBatch()
        {
            if (_isActive) return;

            _isActive = true;

            OutlineBatchesResolver.OutlineDataToStore batch = new OutlineBatchesResolver.OutlineDataToStore(_currentBatchID, _outlineConfig, _childRenderers, _defaultLayer);
            _currentBatchID = OutlineBatchesResolver.AddBacth(batch);
        }

        public void RemoveObjectsFromBatch()
        {
            if (!_isActive) return;

            _isActive = false;

            OutlineBatchesResolver.RemoveBatch(_currentBatchID);
        }
    }
}