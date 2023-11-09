using Core.Data;
using System.Collections.Generic;
using UnityEngine;

namespace QOutline.Configs
{
    [CreateAssetMenu(fileName = "New outlines container", menuName = "Add/Containers/OutlineContainer")]
    public class OutlinesContainer : ScriptableObject
    {
        public List<OutlineSettings> Outlines;
        public LayerMask DisposeBatchMask;

        public OutlineConfigs GetOutlineOfType(Enums.OutlineType type)
        {
            for (int i = 0; i < Outlines.Count; i++)
            {
                if (Outlines[i].OutlineType == type)
                {
                    return Outlines[i].OutlineConfig;
                }
            }

            return null;
        }

        [System.Serializable]
        public class OutlineSettings
        {
            public Enums.OutlineType OutlineType;
            public OutlineConfigs OutlineConfig;
        }
    }
}