using Core.Units;
using UnityEngine;
using UnityEngine.UI;
using Core.UI.Elements;
using System;
using System.Collections.Generic;
using Extensions;
using Core.Data.Skills;

namespace Core.UI
{
    public class UnitManagementPanel : Panel
    {
        public event Action<UnitBaseSkill> OnSkillButtonPressed;

        [SerializeField] private SkillButton _skillButtonPrefab;
        [SerializeField] private Transform _skillsLayout;
        [SerializeField] private Image _untiIconImage;

        private Action<Data.UnitStatsModel> StatsModelChangedCaller;
        private List<SkillButton> _createdSkillButtons = new List<SkillButton>();
        private BaseUnit _currentUnit;

        public void FillInfo(BaseUnit unit, bool isCurrentWalkingUnit)
        {
            _currentUnit = unit;
            StatsModelChangedCaller = null;
            unit.DynamicStatsProvider.OnModelChanged += (statsModel) => StatsModelChangedCaller?.Invoke(statsModel);

            RecreateSkills(isCurrentWalkingUnit);
        }

        private void OnDisable()
        {
            StatsModelChangedCaller = null;
        }

        private void RecreateSkills(bool isCurrentWalkingUnit)
        {
            foreach (var item in _createdSkillButtons)
            {
                Destroy(item.gameObject);
            }

            _createdSkillButtons = new List<SkillButton>();

            foreach (var skill in _currentUnit.GetCurrentSkills())
            {
                SkillButton newSkillButton = Instantiate(_skillButtonPrefab);
                newSkillButton.transform.SetParent(_skillsLayout);
                newSkillButton.transform.ResetLocals();
                newSkillButton.Init(_currentUnit, skill);
                if (isCurrentWalkingUnit)
                {
                    newSkillButton.AddListener(OnSkillPressed);
                }
                else
                {
                    newSkillButton.Disable();
                }
                _createdSkillButtons.Add(newSkillButton);
            }
        }

        private void UnitStatsChanged(Data.UnitStatsModel statsModel)
        {

        }

        private void OnSkillPressed(UnitBaseSkill skill)
        {
            OnSkillButtonPressed?.Invoke(skill);
        }
    }
}