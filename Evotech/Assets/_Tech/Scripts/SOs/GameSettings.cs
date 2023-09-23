using UnityEngine;

namespace Core.Settings
{
    [CreateAssetMenu(fileName = "New game settings", menuName = "Add/Settings/Game settings")]
    public class GameSettings : ScriptableObject
    {
        public MapSettings MapSettings;
    }

    [System.Serializable]
    public class MapSettings
    {
        public float HeightOffset = .2f;
    }
}