using System.Collections.Generic;
using UnityEngine;

namespace Core.Units
{
    public class GhostCopy : MonoBehaviour
    {
        private int ALPHA_SHADER_HASH = Shader.PropertyToID("_Alpha");

        private List<Renderer> _renderers = new List<Renderer>();
        private bool _isChangeAlpha;
        private float _targetAlpha = 0f;
        private float _currentAlpha = 0f;
        private float _alphaMultiplier;
        private bool _isDestroyAfterChange;

        public void Activate()
        {
            _targetAlpha = 1f;
            _isChangeAlpha = true;
        }

        public void Destroy()
        {
            _targetAlpha = 0f;
            _isChangeAlpha = true;
            _isDestroyAfterChange = true;
        }

        private void Update()
        {
            if (_isChangeAlpha)
            {
                _currentAlpha = Mathf.MoveTowards(_currentAlpha, _targetAlpha, Time.deltaTime * 5f);

                foreach (var renderer in _renderers)
                {
                    for (int i = 0; i < renderer.materials.Length; i++)
                    {
                        renderer.materials[i].SetFloat(ALPHA_SHADER_HASH, _currentAlpha * _alphaMultiplier);
                    }
                }

                if (_currentAlpha == _targetAlpha)
                {
                    _isChangeAlpha = false;

                    if (_isDestroyAfterChange)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }

        public void Init(Material ghostMaterial)
        {
            Destroy(GetComponent<BaseUnit>());

            AbstractFigure childFigure = GetComponentInChildren<AbstractFigure>();

            if (childFigure != null)
            {
                childFigure.transform.localRotation = Quaternion.identity;
                Destroy(childFigure);
            }

            DestroyComponents<SpriteRenderer>();
            DestroyComponents<Canvas>();
            DestroyComponents<Collider>();
            DestroyComponents<Rigidbody>();

            _alphaMultiplier = ghostMaterial.GetFloat(ALPHA_SHADER_HASH);

            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                if (item is not MeshRenderer && item is not SkinnedMeshRenderer) continue;

                Material[] mats = new Material[item.materials.Length];
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = Instantiate(ghostMaterial);
                    mats[i].SetFloat(ALPHA_SHADER_HASH, 0f);
                }
                item.materials = mats;

                item.gameObject.layer = LayerMask.NameToLayer("Default");

                _renderers.Add(item);
                item.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Activate();
        }

        private void DestroyComponents<T>() where T : Object
        {
            foreach (var item in GetComponentsInChildren<T>())
            {
                Destroy(item);
            }
        }
    }
}