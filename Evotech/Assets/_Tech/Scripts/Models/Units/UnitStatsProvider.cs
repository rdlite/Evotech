namespace Core.Data
{
    public class UnitStatsProvider : IUnitStatsProvider
    {
        private UnitSettingsContainer _unitSettingsContainer;

        public UnitStatsProvider(UnitSettingsContainer unitSettingsContainer)
        {
            _unitSettingsContainer = unitSettingsContainer;
        }

        public float GetWalkRange(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass)
        {
            float defaultWalkRange = _unitSettingsContainer.GetUnitGeneralSettingsType(unitType).DefaultWalkRange;
            float movementClassMultiplier = _unitSettingsContainer.GetUnitSettingsOfClassType(unitClass).MoveRangeMultiplier;
            return defaultWalkRange * movementClassMultiplier;
        }
    }

    public interface IUnitStatsProvider
    {
        public float GetWalkRange(Enums.UnitGeneralType unitType, Enums.UnitClass unitClass);
    }
}