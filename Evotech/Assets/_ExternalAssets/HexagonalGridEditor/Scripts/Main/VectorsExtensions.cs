using UnityEngine;

namespace HexEditor
{
    public static class VectorsExtensions
    {
        public static Vector3 FlatY(this Vector3 value)
        {
            value.y = 0f;

            return value;
        }
    }
}