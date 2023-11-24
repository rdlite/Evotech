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

        public static Vector3 SetX(this Vector3 value, float target)
        {
            value.x = target;

            return value;
        }

        public static Vector3 SetY(this Vector3 value, float target)
        {
            value.y = target;

            return value;
        }

        public static Vector3 SetZ(this Vector3 value, float target)
        {
            value.z = target;

            return value;
        }
    }
}