using UnityEngine;
using UnityEngine.UI;

namespace Extensions
{
    public static class ColorExtensions
    {
        public static Color SetAlpha(this Color value, float alpha)
        {
            value.a = alpha;

            return value;
        }

        public static void SetAlpha(this Image value, float alpha)
        {
            value.color = new Color(value.color.r, value.color.g, value.color.b, alpha);
        }
    }
}