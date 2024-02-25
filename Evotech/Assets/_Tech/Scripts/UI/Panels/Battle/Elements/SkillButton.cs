using System;
using Core.Units;
using UnityEngine;
using UnityEngine.UI;
using Core.Data.Skills;

namespace Core.UI.Elements
{
    public class SkillButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _iconImg;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        private UnitBaseSkill _skill;

        public void Init(BaseUnit unit, UnitBaseSkill skill)
        {
            _skill = skill;
            _iconImg.sprite = skill.Icon;
        }

        public void AddListener(Action<UnitBaseSkill> action)
        {
            _button.onClick.AddListener(() => action?.Invoke(_skill));
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

        public void Freeze()
        {
            Disable();
        }

        public void Unfreeze()
        {
            Enable();
        }
    }
}