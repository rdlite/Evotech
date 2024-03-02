using Qnject;
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Utils.Decal
{
    public class DecalWrapper : MonoBehaviour
    {
        [SerializeField] private Transform _scaleObject;

        private IUpdateProvider _updateProvider;
        private DecalProjector _decal;
        private float _currentScale = 1f;
        private bool _isHiding;
        private bool _isShowed;

        private void Awake()
        {
            _decal = GetComponentInChildren<DecalProjector>();
        }

        [Inject]
        private void Construct(IUpdateProvider updateProvider)
        {
            _updateProvider = updateProvider;
        }

        private void Start()
        {
            _updateProvider.AddUpdate(Tick);
        }

        private void OnDisable()
        {
            _updateProvider.RemoveUpdate(Tick);
        }

        private void Tick()
        {
            if (_isHiding || !_isShowed) return;

            _scaleObject.localScale = Vector3.Lerp(_scaleObject.localScale, Vector3.one * _currentScale, 10f * Time.deltaTime);
        }

        public void Show(float duration, float delay, Ease ease = Ease.InSine, Action callback = null)
        {
            _scaleObject.localScale = Vector3.zero;
            _scaleObject.DOScale(Vector3.one * _currentScale, duration).SetDelay(delay).SetEase(ease).OnComplete(() => {
                _isShowed = true;
                callback?.Invoke();
            });
        }

        public void Hide(float duration, float delay, Ease ease = Ease.OutSine, Action callback = null)
        {
            _isHiding = true;
            _scaleObject.DOScale(Vector3.zero, duration).SetDelay(delay).From(_scaleObject.localScale).SetEase(ease).OnComplete(() => callback?.Invoke());
        }

        public void SetMaxScale()
        {
            _currentScale = .75f;
            _decal.fadeFactor = .9f;
        }

        public void SetMediumScale()
        {
            _currentScale = .5f;
            _decal.fadeFactor = .8f;
        }

        public void SetMinScale()
        {
            _currentScale = .2f;
            _decal.fadeFactor = .7f;
        }
    }
}