using Core.UI;
using Core.Data;
using Core.Units;
using Core.Cameras;
using Core.UI.Models;
using System.Collections.Generic;

namespace Core.Battle
{
    public class BattleObserver
    {
        public event System.Action<BaseUnit> OnUnitAdded, OnUnitRemoved;

        private List<BaseUnit> _unitsOnMap = new List<BaseUnit>();
        private List<BaseUnit> _currentShowingUnitsStatsInfo = new List<BaseUnit>();
        private IUnitsUIStatsController _uiStatsController;
        private IMapDataProvider _mapDataProvider;
        private CameraController _cameraController;
        private bool _isAlwaysShowStats;

        public BattleObserver(
            IUICanvasesResolver uICanvasesResolver, IUnitsUIStatsController uiStatsController, IMapDataProvider mapDataProvider,
            CameraController cameraController)
        {
            _uiStatsController = uiStatsController;
            _mapDataProvider = mapDataProvider;
            _cameraController = cameraController;

            uICanvasesResolver.GetCanvas<BattleCanvas>().GetPanelOfType<MainBattlePanel>().GetUnitsSequencePanel().OnUnitIconClicked += MoveCameraToUnit;
        }

        public void Tick()
        {
            _uiStatsController.Tick();
        }

        public void AddUnit(BaseUnit newUnit)
        {
            _unitsOnMap.Add(newUnit);
            newUnit.OnDead += (unit) => HideStatsInfo(unit, false, true);
            newUnit.OnDead += (unit) => RemoveUnit(unit);
            OnUnitAdded?.Invoke(newUnit);
        }
        
        public void RemoveUnit(BaseUnit newUnit)
        {
            _unitsOnMap.Remove(newUnit);

            _mapDataProvider.GetNodeOfPosition(newUnit.transform.position).NonwalkableFactors--;

            newUnit.OnDead -= (unit) => HideStatsInfo(unit, false, true);
            newUnit.OnDead -= (unit) => RemoveUnit(unit);

            OnUnitRemoved?.Invoke(newUnit);
        }

        public void ClearAdditionalInfo()
        {
            _unitsOnMap.ForEach(unit => DeactivateDamageInfo(unit));
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

        public void ActivateDamageInfo(BaseUnit unit, (float, float) damage)
        {
            if (_currentShowingUnitsStatsInfo.Contains(unit))
            {
                _uiStatsController.ActivateDamageInfo(unit, damage);
            }
        }
        
        public void DeactivateDamageInfo(BaseUnit unit)
        {
            if (_currentShowingUnitsStatsInfo.Contains(unit))
            {
                _uiStatsController.DeactivateDamageInfo(unit);
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

        public List<BaseUnit> GetUnitsOnMap()
        {
            return new List<BaseUnit>(_unitsOnMap);
        }

        private void MoveCameraToUnit(BaseUnit unit)
        {
            _cameraController.SetSmoothLookupPoint(unit.transform.position);
        }
    }
}