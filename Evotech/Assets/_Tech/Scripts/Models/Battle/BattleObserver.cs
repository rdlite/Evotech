using Core.Units;
using System.Collections.Generic;

namespace Core.Battle
{
    public class BattleObserver
    {
        private List<BaseUnit> _unitsOnMap = new List<BaseUnit>();

        public void AddUnit(BaseUnit newUnit)
        {
            _unitsOnMap.Add(newUnit);
        }
    }
}