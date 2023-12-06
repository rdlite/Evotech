using UnityEngine;

namespace Core.Data.Skills
{
    public class UnitBaseSkill : ScriptableObject
    {
        public string SkillName;
        public string SkillDescription;
        public Sprite Icon;
        public int SortingOrder;
    }
}