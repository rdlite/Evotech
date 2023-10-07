using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "New unit settings", menuName = "Add/Settings/Unit settings")]
    public class UnitSettings : ScriptableObject
    {
        public Enums.WeaponType WeaponID;
        public float WalkDistance = 5f;
    }
}