using UnityEngine;

namespace Extensions
{
    public static class VectorsExtensions
    {
        public static Vector3 FlatX(this Vector3 value)
        {
            value.x = 0f;

            return value;
        }

        public static Vector3 FlatY(this Vector3 value)
        {
            value.y = 0f;

            return value;
        }

        public static Vector3 FlatZ(this Vector3 value)
        {
            value.z = 0f;

            return value;
        }
    }
}