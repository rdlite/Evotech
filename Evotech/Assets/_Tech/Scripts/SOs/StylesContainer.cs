using System.Collections.Generic;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "New styles container", menuName = "Add/Containers/Styles container")]
    public class StylesContainer : ScriptableObject
    {
        public List<WeaponStyle> WeaponStyles;
        public List<UnitSpiritStyle> SpiritStyles;
        public List<PointedLineStyle> PointedLineStyles;

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

        public UnitSpiritStyle GetStyleOfUnitSpirit(Enums.UnitType unitType)
        {
            foreach (UnitSpiritStyle style in SpiritStyles)
            {
                if (style.Type == unitType)
                {
                    return style;
                }
            }

            return null;
        }

        public PointedLineStyle GetStyleOfLineType(Enums.PointedLineType lineType)
        {
            foreach (PointedLineStyle style in PointedLineStyles)
            {
                if (style.Type == lineType)
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
        public int AnimationsAmount = 1;
        public GameObject WeaponPrefab;
    }

    [System.Serializable]
    public class UnitSpiritStyle
    {
        public Enums.UnitType Type;
        public Color UnitColor;
        public Color WavesColor;
    }

    [System.Serializable]
    public class PointedLineStyle
    {
        public Enums.PointedLineType Type;
        public Color Color;
        public bool IsMoving = true;
    }
}