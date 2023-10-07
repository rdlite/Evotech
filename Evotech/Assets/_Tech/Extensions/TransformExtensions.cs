using UnityEngine;

namespace Extensions
{
    public static class TransformExtensions
    {
        public static void ResetLocals(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.transform.localScale = Vector3.one;
        }
    }
}