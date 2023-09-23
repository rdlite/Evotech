using UnityEngine;

namespace Utils
{
    public class HexGridUtility
    {
        public static Vector2Int ConvertWorldPointToGridID(Vector3 point, float radius = 1f)
        {
            float x = Mathf.Sqrt(3f) / 3f * point.x + -1f / 3f * point.z;
            float y = (1f - 1f / 3f) * point.z;
            Vector2Int offsetCoord = AxialToOddR(new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y)));
            return offsetCoord;
        }

        private static Vector2Int AxialToOddR(Vector2Int axialCoord)
        {
            return new Vector2Int(
                axialCoord.x + (axialCoord.y - Mathf.Abs(axialCoord.y % 2)) / 2,
                axialCoord.y
                );
        }

        public static Vector3 ConvertHexCoordToWorldPoint(Vector2Int hexCoord)
        {
            return new Vector3(
                ConvertXToWorldPos(hexCoord.x, hexCoord.y),
                0f,
                ConvertYToWorldPos(hexCoord.y)
                );
        }

        public static float ConvertXToWorldPos(int x, int y, float hexSize = 1f)
        {
            return x * Mathf.Sqrt(3f) * hexSize + ((y % 2 != 0) ? (Mathf.Sqrt(3f) / 2f) : 0f);
        }

        public static float ConvertYToWorldPos(int y, float hexSize = 1f)
        {
            float vert = 3f / 2f * hexSize;
            return y * vert;
        }
    }
}