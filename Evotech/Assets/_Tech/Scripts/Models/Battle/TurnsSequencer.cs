using Core.Units;
using Core.UI.Elements;
using System.Collections.Generic;

namespace Core.Battle
{
    public class TurnsSequencer : ITurnsSequencer
    {
        private List<BaseUnit> _currentWalkingSequence;
        private BattleObserver _battleObserver;
        private UnitsSequencePanel _unitsSequencePanel;

        public TurnsSequencer(BattleObserver battleObserver, UnitsSequencePanel unitsSequencePanel)
        {
            _battleObserver = battleObserver;
            _unitsSequencePanel = unitsSequencePanel;

            _battleObserver.OnUnitAdded += NewUnitArived;
            _battleObserver.OnUnitRemoved += UnitRemoved;
        }

        private void NewUnitArived(BaseUnit unit)
        {
            _unitsSequencePanel.AddUnitsIcon(unit);
        }

        private void UnitRemoved(BaseUnit unit)
        {
            _unitsSequencePanel.RemoveIconOfUnit(unit);
        }

        public void GenerateNewSequence()
        {
            _currentWalkingSequence = new List<BaseUnit>(_battleObserver.GetUnitsOnMap());
            _currentWalkingSequence.Sort((unitA, unitB) => (int)(unitB.DynamicStatsProvider.GetCurrentInitiative() - unitA.DynamicStatsProvider.GetCurrentInitiative()));
            _unitsSequencePanel.SortIconsAccordingly(new List<BaseUnit>(_currentWalkingSequence));
        }
    }

    public interface ITurnsSequencer
    {
        void GenerateNewSequence();
    }
}