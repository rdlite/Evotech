using UnityEngine;
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
        private bool _isAlwaysShowStats;

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
            newUnit.OnDead += () => HideStatsInfo(newUnit, false, true);
        }
        
        public void RemoveUnit(BaseUnit newUnit)
        {
            _unitsOnMap.Remove(newUnit);
            newUnit.OnDead -= () => HideStatsInfo(newUnit, false, true);
        }

        public void HighlightUIStatsInfo(BaseUnit unit, bool withScale)
        {
            if (!unit.IsDead)
            {
                if (!_currentShowingUnitsStatsInfo.Contains(unit))
                {
                    _currentShowingUnitsStatsInfo.Add(unit);
                }

                _uiStatsController.HighlightStatsInfo(unit, withScale);
            }
        }

        public void HideStatsInfo(BaseUnit unit, bool withScale, bool forceHide)
        {
            if (_currentShowingUnitsStatsInfo.Contains(unit))
            {
                if (!_isAlwaysShowStats)
                {
                    _currentShowingUnitsStatsInfo.Remove(unit);
                }
                _uiStatsController.HideStatsInfo(unit, withScale, forceHide);
            }
        }

        public bool SwitchStatsState()
        {
            _isAlwaysShowStats = !_isAlwaysShowStats;

            if (_isAlwaysShowStats)
            {
                _uiStatsController.SetAlwaysShowStats(_isAlwaysShowStats);
                _unitsOnMap.ForEach(unit => HighlightUIStatsInfo(unit, false));
            }
            else
            {
                _uiStatsController.SetAlwaysShowStats(_isAlwaysShowStats);
                _unitsOnMap.ForEach(unit => HideStatsInfo(unit, false, false));
            }

            return _isAlwaysShowStats;
        }
    }
}