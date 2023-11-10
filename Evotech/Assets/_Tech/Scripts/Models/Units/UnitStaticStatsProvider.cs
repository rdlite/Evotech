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

        public float GetAtackDamage(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass)
        {
            float attack = _unitSettingsContainer.GetUnitSettingsOfClassType(unitClass).AttackDamage;
            float attackRandom = _unitSettingsContainer.GetUnitSettingsOfClassType(unitClass).AttackRandom;
            return attack + UnityEngine.Random.Range(-attackRandom, attackRandom);
        }
    }

    public interface IUnitStaticStatsProvider
    {
        public float GetWalkRange(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass);
        public float GetMaxHealth(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass);
        public float GetAtackDamage(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass);
    }
}