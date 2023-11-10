using System;
using UnityEngine;

namespace Core.Data
{
    public class UnitDynamicStatsProvider : IUnitDynamicStatsProvider
    {
        public event Action OnHealthZeroed;

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
            _stats.CurrentHealth -= damage;

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
    }

    public interface IUnitDynamicStatsProvider
    {
        public event Action OnHealthZeroed;

        public void TakeDamage(InstantDamageInfo damageInfo);
        public float GetHealthPercentage();
        public float GetArmorPercentage();
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