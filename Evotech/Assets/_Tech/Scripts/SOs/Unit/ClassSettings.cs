using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "New class settings", menuName = "Add/Settings/Class settings")]
    public class ClassSettings : ScriptableObject
    {
        public Enums.WeaponType WeaponID;
        public float HealthMultiplier = 1f;
        public float ArmorMultiplier = 1f;
        public float MoveRangeMultiplier = 1f;
        public float InitiativeMultiplier = 1f;
        public float AttackDamage = 10f;
        public float AttackRandom = 2f;
    }
}