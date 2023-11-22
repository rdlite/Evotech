using UnityEngine;

namespace Core.Data
{
    public class UnitStaticStatsProvider : IUnitStaticStatsProvider
    {
        private UnitSettingsContainer _unitSettingsContainer;

        public UnitStaticStatsProvider(UnitSettingsContainer unitSettingsContainer)
        {
            _unitSettingsContainer = unitSettingsContainer;
        }

        public float GetWalkRange(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass)
        {
            float defaultWalkRange = _unitSettingsContainer.GetUnitGeneralSettingsType(unitType).DefaultWalkRange;
            float movementClassMultiplier = _unitSettingsContainer.GetUnitSettingsOfClassType(unitClass).MoveRangeMultiplier;
            return defaultWalkRange * movementClassMultiplier;
        }

        public float GetMaxHealth(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass)
        {
            float defaultHealth = _unitSettingsContainer.GetUnitGeneralSettingsType(unitType).DefaultHealth;
            float healthMultiplier = _unitSettingsContainer.GetUnitSettingsOfClassType(unitClass).HealthMultiplier;
            return defaultHealth * healthMultiplier;
        }
        
        public float GetMaxArmor(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass)
        {
            float defaultArmor = _unitSettingsContainer.GetUnitGeneralSettingsType(unitType).DefaultArmor;
            float armorMultiplier = _unitSettingsContainer.GetUnitSettingsOfClassType(unitClass).ArmorMultiplier;
            return defaultArmor * armorMultiplier;
        }
        
        public float GetMaxInitiative(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass)
        {
            float defaultInitiative = _unitSettingsContainer.GetUnitGeneralSettingsType(unitType).DefaultInitiative;
            float initiativeMultiplier = _unitSettingsContainer.GetUnitSettingsOfClassType(unitClass).InitiativeMultiplier;
            return defaultInitiative * initiativeMultiplier;
        }

        public float GetRandomizedAttackDamage(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass)
        {
            float attack = _unitSettingsContainer.GetUnitSettingsOfClassType(unitClass).AttackDamage;
            float attackRandom = _unitSettingsContainer.GetUnitSettingsOfClassType(unitClass).AttackRandom;
            return attack + Random.Range(-attackRandom, attackRandom);
        }

        public (float, float) GetDecomposedDamage(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass)
        {
            float attack = _unitSettingsContainer.GetUnitSettingsOfClassType(unitClass).AttackDamage;
            float attackRandom = _unitSettingsContainer.GetUnitSettingsOfClassType(unitClass).AttackRandom;

            return (attack, attackRandom);
        }
    }

    public interface IUnitStaticStatsProvider
    {
        public float GetWalkRange(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass);
        public float GetMaxHealth(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass);
        public float GetMaxArmor(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass);
        public float GetMaxInitiative(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass);
        public float GetRandomizedAttackDamage(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass);
        public (float, float) GetDecomposedDamage(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass);
    }
}