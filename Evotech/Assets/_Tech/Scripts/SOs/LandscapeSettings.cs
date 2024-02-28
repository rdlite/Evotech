using Core.Map;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "New landscape", menuName = "Add/Landscape/Default landscape")]
    public class LandscapeSettings : ScriptableObject
    {
        public HexagonNode[] HexagonsDatabase;
        public MapObstacle[] NonwalkableObstacles;
        public MapObstacle[] WalkableObstacles;

        public HexagonNode GetHexPrefabByName(string hexagonName)
        {
            foreach (HexagonNode hex in HexagonsDatabase)
            {
                if (hex.transform.name == hexagonName)
                {
                    return hex;
                }
            }

            return null;
        }
    }
}

public enum LandscapeTypes
{
    None = -1,
    DefaultGrass = 1,
}