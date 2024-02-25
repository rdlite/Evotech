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
        private float _currentArmor, _maxArmor, _currentHealth, _maxHealth;

        public void Init(IUnitDynamicStatsProvider unitStatsProvider)
        {
            _healthBar.Init(unitStatsProvider.GetHealthPercentage());
            _armorBar.Init(unitStatsProvider.GetArmorPercentage());
            _unitStatsProvider = unitStatsProvider;
            _unitStatsProvider.OnModelChanged += UpdateModel;

            _currentHealth = unitStatsProvider.GetStatsModel().CurrentHealth;
            _maxHealth = unitStatsProvider.GetStatsModel().MaxHealth;
            _currentArmor = unitStatsProvider.GetStatsModel().CurrentArmor;
            _maxArmor = unitStatsProvider.GetStatsModel().MaxArmor;
        }

        public void ActivateDamageInfo(int damage, int random)
        {
            _damageInfoPanel.SetActive(true);

            float damageToArmor = damage;
            float armorCurr = _currentArmor;
            float damageToHealth =
                damage - (armorCurr / _maxArmor) * damage;

            if (damageToHealth > 0f && _currentHealth > 0f)
            {
                _healthBar.SetPossibleDamageValue((_currentHealth - damageToHealth) / _maxHealth);
            }

            if (damageToArmor > 0f && _currentArmor > 0f)
            {
                _armorBar.SetPossibleDamageValue((_currentArmor - damageToArmor) / _maxArmor);
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
            _healthBar.HideDamageInfo(_currentHealth / _maxHealth);
            _armorBar.HideDamageInfo(_currentArmor / _maxArmor);
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
            _currentHealth = statsModel.CurrentHealth;
            _maxHealth = statsModel.MaxHealth;
            _currentArmor = statsModel.CurrentArmor;
            _maxArmor = statsModel.MaxArmor;
            SetHealthPercentage(_currentHealth / _maxHealth);
            SetArmorPercentage(_currentArmor / _maxArmor);
        }
    }
}