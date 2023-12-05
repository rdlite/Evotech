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
        [SerializeField] private SkillButton _skillButtonPrefab;
        [SerializeField] private Transform _skillsLayout;
        [SerializeField] private Image _untiIconImage;

        private Action<Data.UnitStatsModel> StatsModelChangedCaller;
        private List<SkillButton> _createdSkillButtons = new List<SkillButton>();
        private BaseUnit _currentUnit;

        public void FillInfo(BaseUnit unit)
        {
            _currentUnit = unit;
            StatsModelChangedCaller = null;
            unit.DynamicStatsProvider.OnModelChanged += (statsModel) => StatsModelChangedCaller?.Invoke(statsModel);

            RecreateSkills();
        }

        private void OnDisable()
        {
            StatsModelChangedCaller = null;
        }

        private void RecreateSkills()
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
                newSkillButton.AddListener(OnSkillPressed);
                _createdSkillButtons.Add(newSkillButton);
            }
        }

        private void UnitStatsChanged(Data.UnitStatsModel statsModel)
        {

        }

        private void OnSkillPressed(UnitBaseSkill skill)
        {
            Debug.Log(skill.SkillName);
        }
    }
}