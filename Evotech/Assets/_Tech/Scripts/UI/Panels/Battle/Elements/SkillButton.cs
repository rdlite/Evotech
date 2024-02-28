using System;
using Core.Units;
using UnityEngine;
using UnityEngine.UI;
using Core.Data.Skills;
using Extensions;
using Qnject;
using Utils;

namespace Core.UI.Elements
{
    public class SkillButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _iconImg;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _highlightSelectionForeground;
        
        private UnitBaseSkill _skill;
        private IUpdateProvider _updateProvider;
        private float _defaultHighlightAlpha;
        private float _highlightTimer;
        private float _highlightSpeed = 3f;
        private bool _isCurrentlySelected;

        [Inject]
        public void Construct(IUpdateProvider updateProvider)
        {
            _updateProvider = updateProvider;
            _updateProvider.AddUpdate(Tick);
        }

        public void Init(BaseUnit unit, UnitBaseSkill skill)
        {
            _skill = skill;
            _iconImg.sprite = skill.Icon;
            _defaultHighlightAlpha = _highlightSelectionForeground.color.a;
        }

        public void AddListener(Action<UnitBaseSkill> action)
        {
            _button.onClick.AddListener(() => action?.Invoke(_skill));
        }

        private void OnEnable()
        {
            _updateProvider?.AddUpdate(Tick);
        }

        private void OnDisable()
        {
            _updateProvider?.RemoveUpdate(Tick);
        }

        private void Tick()
        {
            if (_isCurrentlySelected)
            {
                _highlightTimer += Time.unscaledDeltaTime * _highlightSpeed;

                float interpolator = (Mathf.Cos(_highlightTimer + Mathf.PI) + 1f) / 2f;

                _highlightSelectionForeground.SetAlpha(Mathf.Lerp(0f, _defaultHighlightAlpha, interpolator));
            }
        }

        public void Enable()
        {
            _button.enabled = true;
            _canvasGroup.alpha = 1f;
        }

        public void Disable()
        {
            _button.enabled = false;
            _canvasGroup.alpha = .5f;
        }

        public void SetSelected(bool isCurrentlySelected)
        {
            _isCurrentlySelected = isCurrentlySelected;
            ResetHighlightSelectionForeground();
        }

        
        public void SwitchSelection()
        {
            _isCurrentlySelected = !_isCurrentlySelected;
            ResetHighlightSelectionForeground();
        }

        public void Freeze()
        {
            Disable();
        }

        public void Unfreeze()
        {
            Enable();
        }

        public bool IsSkill(UnitBaseSkill skill)
        {
            return _skill == skill;
        }

        private void ResetHighlightSelectionForeground()
        {
            _highlightTimer = 0;
            _highlightSelectionForeground.gameObject.SetActive(_isCurrentlySelected);
            _highlightSelectionForeground.SetAlpha(0f);
        }
    }
}