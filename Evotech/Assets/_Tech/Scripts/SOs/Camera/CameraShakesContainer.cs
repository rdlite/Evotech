using UnityEngine;
using System.Collections.Generic;

namespace Core.Data.Camera
{
    [CreateAssetMenu(fileName = "Camera shakes container", menuName = "Add/Containers/Camera shakes container")]
    public class CameraShakesContainer : ScriptableObject
    {
        [SerializeField] private List<CameraShakeConfigs> _configs;

        public CameraShakeConfigs GetShakeOfTag(string tag)
        {
            for (int i = 0; i < _configs.Count; i++)
            {
                if (_configs[i].Tag == tag)
                {
                    return _configs[i];
                }
            }

            Debug.LogError($"There is no shake with {tag} tag!..");

            return null;
        }
    }
}