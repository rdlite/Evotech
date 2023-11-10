using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "New unit settings", menuName = "Add/Settings/Unit settings")]
    public class UnitSettings : ScriptableObject
    {
        public float DefaultHealth = 0;
        public float DefaultWalkRange = 0;
    }
}