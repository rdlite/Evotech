using Core.Data;
using UnityEngine;

namespace Core.UI.Elements
{
    public class UnitStatsInfo : MonoBehaviour
    {
        [SerializeField] private VerticalBar _healthBar, _armorBar;

        private IUnitDynamicStatsProvider _unitStatsProvider;

        public void Init(IUnitDynamicStatsProvider unitStatsProvider)
        {
            _healthBar.Init(unitStatsProvider.GetHealthPercentage());
            _armorBar.Init(unitStatsProvider.GetArmorPercentage());
            _unitStatsProvider = unitStatsProvider;
            _unitStatsProvider.OnModelChanged += UpdateModel;
        }

        private void OnDestroy()
        {
            if (_unitStatsProvider != null)
            {
                _unitStatsProvider.OnModelChanged -= UpdateModel;
            }
        }

        public void SetHealthPercentage(float percentage)
        {
            _healthBar.SetValue(percentage);
        }

        public void SetArmorPercentage(float percentage)
        {
            _armorBar.SetValue(percentage);
        }

        private void UpdateModel(UnitStatsModel statsModel)
        {
            SetHealthPercentage(_unitStatsProvider.GetHealthPercentage());
            SetArmorPercentage(_unitStatsProvider.GetArmorPercentage());
        }
    }
}