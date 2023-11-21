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

        public bool TakeDamage(InstantDamageInfo damageInfo)
        {
            if (_stats.CurrentHealth <= 0f)
            {
                return false;
            }

            float damage = damageInfo.Damage;
            float armorCurr = _stats.CurrentArmor;

            _stats.CurrentArmor -= damage;
            _stats.CurrentArmor = Mathf.Max(0f, _stats.CurrentArmor);

            float damageToHealth =
                damage - (armorCurr / _stats.MaxArmor) * damage;

            if (damageToHealth > 0f)
            {
                _stats.CurrentHealth -= damageToHealth;
            }

            OnModelChanged?.Invoke(_stats);

            if (_stats.CurrentHealth <= 0f)
            {
                OnHealthZeroed?.Invoke();
            }

            return true;
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

        public bool TakeDamage(InstantDamageInfo damageInfo);
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