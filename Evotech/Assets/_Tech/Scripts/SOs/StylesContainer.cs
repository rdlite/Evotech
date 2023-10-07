using System.Collections.Generic;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "New styles container", menuName = "Add/Containers/Styles container")]
    public class StylesContainer : ScriptableObject
    {
        public List<WeaponStyle> WeaponStyles;

        public WeaponStyle GetStyleOfWeaponType(Enums.WeaponType weaponType)
        {
            foreach (WeaponStyle style in WeaponStyles)
            {
                if (style.WeaponType == weaponType)
                {
                    return style;
                }
            }

            return null;
        }
    }

    [System.Serializable]
    public class WeaponStyle
    {
        public Enums.WeaponType WeaponType;
        public int AnimatorID = 0;
        public GameObject WeaponPrefab;
    }
}