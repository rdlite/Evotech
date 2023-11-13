using Core.Data;
using TMPro;
using UnityEngine;

namespace Core.UI.Elements
{
    public class UnitStatsInfo : MonoBehaviour
    {
        [SerializeField] private VerticalBar _healthBar, _armorBar;
        [SerializeField] private GameObject _damageInfoPanel;
        [SerializeField] private TextMeshProUGUI _damageText;

        private IUnitDynamicStatsProvider _unitStatsProvider;
        private UnitStatsModel _currentStatsModel;

        public void Init(IUnitDynamicStatsProvider unitStatsProvider)
        {
            _healthBar.Init(unitStatsProvider.GetHealthPercentage());
            _armorBar.Init(unitStatsProvider.GetArmorPercentage());
            _unitStatsProvider = unitStatsProvider;
            _unitStatsProvider.OnModelChanged += UpdateModel;
            _currentStatsModel = _unitStatsProvider.GetStatsModel();
        }

        public void ActivateDamageInfo(int damage, int random)
        {
            _damageInfoPanel.SetActive(true);

            float damageToHealth = (damage - _currentStatsModel.CurrentArmor);
            damageToHealth = Mathf.Max(0, damageToHealth);

            float damageToArmor = (damage - damageToHealth);

            if (damageToHealth > 0f && _currentStatsModel.CurrentHealth > 0f)
            {
                _healthBar.SetPossibleDamageValue((_currentStatsModel.CurrentHealth - damageToHealth) / _currentStatsModel.MaxHealth);
            }

            if (damageToArmor > 0f && _currentStatsModel.CurrentArmor > 0f)
            {
                _armorBar.SetPossibleDamageValue((_currentStatsModel.CurrentArmor - damageToArmor) / _currentStatsModel.MaxArmor);
            }

            if (random > 0)
            {
                _damageText.text = $"{damage - random}-{damage + random}";
            }
            else
            {
                _damageText.text = $"{damage}";
            }
        }
        
        public void DeactivateDamageInfo()
        {
            _damageInfoPanel.SetActive(false);
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