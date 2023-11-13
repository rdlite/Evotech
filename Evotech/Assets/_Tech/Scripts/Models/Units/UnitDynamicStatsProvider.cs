using System;
using UnityEngine;

namespace Core.Data
{
    public class UnitDynamicStatsProvider : IUnitDynamicStatsProvider
    {
        public event Action OnHealthZeroed;
        public event Action<UnitStatsModel> OnModelChanged;

        private UnitStatsModel _stats;

        public UnitDynamicStatsProvider(UnitStatsModel stats)
        {
            _stats = stats;
        }

        public void TakeDamage(InstantDamageInfo damageInfo)
        {
            if (_stats.CurrentHealth <= 0f)
            {
                return;
            }

            float damage = damageInfo.Damage;

            float damageToHealth = (damage - _stats.CurrentArmor);

            damageToHealth = Mathf.Max(0, damageToHealth);

            _stats.CurrentArmor -= (damage - damageToHealth);
            _stats.CurrentArmor = Mathf.Max(0, _stats.CurrentArmor);

            if (damageToHealth > 0f)
            {
                _stats.CurrentHealth -= damage;
            }

            OnModelChanged?.Invoke(_stats);

            if (_stats.CurrentHealth <= 0f)
            {
                OnHealthZeroed?.Invoke();
            }
        }

        public float GetHealthPercentage()
        {
            return Mathf.Clamp01(_stats.CurrentHealth / _stats.MaxHealth);
        }

        public float GetArmorPercentage()
        {
            return Mathf.Clamp01(_stats.CurrentArmor / _stats.MaxArmor);
        }

        public UnitStatsModel GetStatsModel()
        {
            return _stats;
        }
    }

    public interface IUnitDynamicStatsProvider
    {
        public event Action OnHealthZeroed;
        public event Action<UnitStatsModel> OnModelChanged;

        public void TakeDamage(InstantDamageInfo damageInfo);
        public float GetHealthPercentage();
        public float GetArmorPercentage();
        public UnitStatsModel GetStatsModel();
    }

    public struct UnitStatsModel
    {
        public float MaxHealth;
        public float CurrentHealth;
        public float MaxArmor;
        public float CurrentArmor;
        public float WalkRange;
    }
}