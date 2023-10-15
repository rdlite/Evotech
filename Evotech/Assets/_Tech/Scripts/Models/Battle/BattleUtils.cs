using static Core.Data.Enums;

namespace Utils.Battle
{
    public static class BattleUtils
    {
        public static UnitRelation GetRelationForUnits(UnitType type1, UnitType type2)
        {
            // cover enemies
            if ((type1 == UnitType.EnemyRed || type1 == UnitType.EnemyNeutral ||
                type2 == UnitType.EnemyRed || type2 == UnitType.EnemyNeutral) && 
                type1 != type2)
            {
                return UnitRelation.Enemy;
            }
            else if (
                (type1 == UnitType.EnemyRed || type1 == UnitType.EnemyNeutral ||
                type2 == UnitType.EnemyRed || type2 == UnitType.EnemyNeutral) &&
                type1 == type2)
            {
                return UnitRelation.Ally;
            }

            // cover players
            if (type1 == UnitType.PlayerAlly && type2 == UnitType.PlayerAlly)
            {
                return UnitRelation.Ally;
            }
            else if (type1 == UnitType.PlayerAlly && type2 == UnitType.EnemyRed)
            {
                return UnitRelation.Enemy;
            }
            else if (type1 == UnitType.PlayerAlly && type2 == UnitType.EnemyNeutral)
            {
                return UnitRelation.Enemy;
            }
            else if (type1 == UnitType.PlayerAlly && type2 == UnitType.Player)
            {
                return UnitRelation.Ally;
            }
            else if (type1 == UnitType.Player && type2 == UnitType.PlayerAlly)
            {
                return UnitRelation.Ally;
            }
            else if (type1 == UnitType.Player && type2 == UnitType.EnemyRed)
            {
                return UnitRelation.Enemy;
            }
            else if (type1 == UnitType.Player && type2 == UnitType.EnemyNeutral)
            {
                return UnitRelation.Enemy;
            }
            else if (type1 == UnitType.Player && type2 == UnitType.Player)
            {
                return UnitRelation.Ally;
            }

            return UnitRelation.None;
        }
    }
}