using Core.Units;
using System.Collections.Generic;

namespace Core.Battle
{
    public class ActionInfo
    {
        public BaseUnit Actor;
        public List<BaseUnit> SubjectUnits = new List<BaseUnit>();
    }

    public class ActionMeleeAttack : ActionInfo
    {
        public float Damage = 0f;
    }
}