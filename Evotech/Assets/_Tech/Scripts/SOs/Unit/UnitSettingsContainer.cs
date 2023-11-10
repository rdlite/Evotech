using UnityEngine;
using System.Collections.Generic;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "New unit settings container", menuName = "Add/Containers/Unit settings container")]
    public class UnitSettingsContainer : ScriptableObject
    {
        [SerializeField] private List<ClassSettingsMap> _classSettingsMap;
        [SerializeField] private List<UnitSettingsMap> _unitGeneralSettingsMap;

        public ClassSettings GetUnitSettingsOfClassType(Enums.UnitClass classType)
        {
            for (int i = 0; i < _classSettingsMap.Count; i++)
            {
                if (_classSettingsMap[i].Class == classType)
                {
                    return _classSettingsMap[i].Settings;
                }
            }

            return null;
        }

        public UnitSettings GetUnitGeneralSettingsType(Enums.UnitGeneralType generalType)
        {
            for (int i = 0; i < _unitGeneralSettingsMap.Count; i++)
            {
                if (_unitGeneralSettingsMap[i].Type == generalType)
                {
                    return _unitGeneralSettingsMap[i].Settings;
                }
            }

            return null;
        }

        [System.Serializable]
        public class UnitSettingsMap
        {
            public Enums.UnitGeneralType Type;
            public UnitSettings Settings;
        }

        [System.Serializable]
        public class ClassSettingsMap
        {
            public Enums.UnitClass Class;
            public ClassSettings Settings;
        }
    }
}