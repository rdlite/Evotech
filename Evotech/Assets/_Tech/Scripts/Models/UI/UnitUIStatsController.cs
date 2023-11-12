using Core.Data;
using Core.Units;
using Extensions;
using UnityEngine;
using DG.Tweening;
using Core.UI.Elements;
using System.Collections.Generic;

namespace Core.UI.Models
{
    public class UnitUIStatsController : IUnitsUIStatsController
    {
        private Dictionary<BaseUnit, UnitStatsInfo> _currentStats = new Dictionary<BaseUnit, UnitStatsInfo>();

        private IUICanvasesResolver _canvasesResolver;
        private AssetsContainer _assetsContainer;
        private UnitsInfoPanel _statsPanel;

        public UnitUIStatsController(IUICanvasesResolver canvasesResolver, AssetsContainer assetsContainer)
        {
            _canvasesResolver = canvasesResolver;
            _assetsContainer = assetsContainer;
        }

        public void Init()
        {
            _statsPanel = _canvasesResolver.GetCanvas<BattleCanvas>().GetPanelOfType<UnitsInfoPanel>();
        }

        public void Tick()
        {
            UpdateStatPositions();
        }

        public void HighlightStatsInfo(BaseUnit unit, bool withScale)
        {
            if (!_currentStats.ContainsKey(unit) && _statsPanel != null)
            {
                UnitStatsInfo unitStatsInfo = Object.Instantiate(_assetsContainer.UIElements.UnitStatsInfo);
                Transform statsUIParent = _statsPanel.GetUnitStatsParent();
                unitStatsInfo.transform.SetParent(statsUIParent);
                unitStatsInfo.transform.ResetLocals();
                unitStatsInfo.Init(unit.DynamicStatsProvider);
                if (withScale)
                {
                    unitStatsInfo.transform.DOScale(Vector3.one, .1f).From(Vector3.one / 1.4f);
                }
                _currentStats.Add(unit, unitStatsInfo);
            }
        }

        public void HideStatsInfo(BaseUnit unit, bool withScale)
        {
            if (_currentStats.ContainsKey(unit) && _currentStats[unit] != null)
            {
                GameObject objToDestroy = _currentStats[unit].gameObject;
                if (withScale)
                {
                    _currentStats[unit].transform.DOScale(Vector3.one / 1.4f, .1f).From(Vector3.one).OnComplete(
                        () => Object.Destroy(objToDestroy));
                }
                else
                {
                    Object.Destroy(objToDestroy);
                }
                _currentStats.Remove(unit);
            }
        }

        private void UpdateStatPositions()
        {
            foreach (var statData in _currentStats)
            {
                if (statData.Value != null && statData.Key != null)
                {
                    statData.Value.transform.position = Camera.main.WorldToScreenPoint(statData.Key.GetUpperHeadPoint().position);
                }
            }
        }
    }

    public interface IUnitsUIStatsController
    {
        void Init();
        void HighlightStatsInfo(BaseUnit unit, bool withScale);
        void HideStatsInfo(BaseUnit unit, bool withScale);
        void Tick();
    }
}