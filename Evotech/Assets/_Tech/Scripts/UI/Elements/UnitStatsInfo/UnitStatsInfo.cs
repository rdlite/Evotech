using Core.Data;
using UnityEngine;

namespace Core.UI.Elements
{
    public class UnitStatsInfo : MonoBehaviour
    {
        [SerializeField] private VerticalBar _healthBar, _armorBar;

        public void Init(IUnitDynamicStatsProvider unitStatsProvider)
        {
            _healthBar.Init(unitStatsProvider.GetHealthPercentage());
            _armorBar.Init(unitStatsProvider.GetArmorPercentage());
        }

        public void SetHealthPercentage(float percentage)
        {
            _healthBar.SetValue(percentage);
        }

        public void SetArmorPercentage(float percentage)
        {
            _armorBar.SetValue(percentage);
        }
    }
}