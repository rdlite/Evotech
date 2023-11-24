using Utils;
using Qnject;
using Core.Units;
using Extensions;
using UnityEngine;
using System.Collections.Generic;

namespace Core.UI.Elements
{
    public class UnitsSequencePanel : MonoBehaviour
    {
        [SerializeField] private UnitIcon _unitIconPrefab;
        [SerializeField] private Transform _layout;

        private Dictionary<BaseUnit, UnitIcon> _createdUnitIcons = new Dictionary<BaseUnit, UnitIcon>();
        private IUpdateProvider _updatesProvider;

        [Inject]
        private void Construct(IUpdateProvider updatesProvider)
        {
            _updatesProvider = updatesProvider;
            _updatesProvider.AddUpdate(Tick);
        }

        private void OnEnable()
        {
            _updatesProvider?.AddUpdate(Tick);
        }

        private void OnDisable()
        {
            _updatesProvider?.RemoveUpdate(Tick);
        }

        public void SetHighlightedPulsate(BaseUnit unit, bool value)
        {
            _createdUnitIcons[unit].SetHighlightedPulsate(value);
        }

        public void SetHighlightedSelected(BaseUnit unit, bool value)
        {
            _createdUnitIcons[unit].SetHighlightedSelected(value);
        }

        public void AddUnitsIcon(BaseUnit unit)
        {
            UnitIcon newUnitIcon = Instantiate(_unitIconPrefab);
            newUnitIcon.transform.SetParent(_layout);
            newUnitIcon.transform.ResetLocals();
            newUnitIcon.Init(unit);
            _createdUnitIcons.Add(unit, newUnitIcon);
        }

        public void RemoveIconOfUnit(BaseUnit unit)
        {
            _createdUnitIcons[unit].Remove();
        }

        public void SortIconsAccordingly(List<BaseUnit> sortedUnitsList)
        {
            Dictionary<BaseUnit, UnitIcon> sortedDictionary = new Dictionary<BaseUnit, UnitIcon>();

            for (int i = sortedUnitsList.Count - 1; i >= 0; i--)
            {
                _createdUnitIcons[sortedUnitsList[i]].transform.SetAsFirstSibling();
                sortedDictionary.Add(sortedUnitsList[i], _createdUnitIcons[sortedUnitsList[i]]);
            }

            sortedUnitsList.Reverse();

            _createdUnitIcons = sortedDictionary;
        }

        private void Tick()
        {
            foreach (var item in _createdUnitIcons)
            {
                if (item.Value != null)
                {
                    item.Value.Tick();
                }
            }
        }
    }
}