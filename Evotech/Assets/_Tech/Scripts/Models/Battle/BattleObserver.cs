using Core.Units;
using Core.UI.Models;
using System.Collections.Generic;

namespace Core.Battle
{
    public class BattleObserver
    {
        private List<BaseUnit> _unitsOnMap = new List<BaseUnit>();
        private List<BaseUnit> _currentShowingUnitsStatsInfo = new List<BaseUnit>();

        private IUnitsUIStatsController _uiStatsController;

        public BattleObserver(IUnitsUIStatsController uiStatsController)
        {
            _uiStatsController = uiStatsController;
        }

        public void Tick()
        {
            _uiStatsController.Tick();
        }

        public void AddUnit(BaseUnit newUnit)
        {
            _unitsOnMap.Add(newUnit);
            newUnit.OnDead += () => HideStatsInfo(newUnit, false);
        }
        
        public void RemoveUnit(BaseUnit newUnit)
        {
            _unitsOnMap.Remove(newUnit);
            newUnit.OnDead -= () => HideStatsInfo(newUnit, false);
        }

        public void ShowUIStatsInfo(BaseUnit unit, bool withScale)
        {
            if (!_currentShowingUnitsStatsInfo.Contains(unit))
            {
                _currentShowingUnitsStatsInfo.Add(unit);
                _uiStatsController.HighlightStatsInfo(unit, withScale);
            }
        }

        public void HideStatsInfo(BaseUnit unit, bool withScale)
        {
            if (_currentShowingUnitsStatsInfo.Contains(unit))
            {
                _currentShowingUnitsStatsInfo.Remove(unit);
                _uiStatsController.HideStatsInfo(unit, withScale);
            }
        }
    }
}