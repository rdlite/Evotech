using System.Collections.Generic;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "New landscape", menuName = "Add/Landscape/Default landscape")]
    public class LandscapeSettings : ScriptableObject
    {
        public LandscapeTypes LandType;
        public HexagonNode MainHex;
    }

    [System.Serializable]
    public class HexagonsContainer
    {
        public List<LandscapeSettings> Landscapes;

        public LandscapeSettings GetSettingsOfType(LandscapeTypes type)
        {
            for (int i = 0; i < Landscapes.Count; i++)
            {
                if (Landscapes[i].LandType == type)
                {
                    return Landscapes[i];
                }
            }

            Debug.LogError($"There's no landscape of type {type}!..");

            return null;
        }
    }
}

public enum LandscapeTypes
{
    None = -1,
    DefaultGrass = 1,
}