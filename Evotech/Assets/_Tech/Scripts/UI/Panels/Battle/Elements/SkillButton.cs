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
    }
}