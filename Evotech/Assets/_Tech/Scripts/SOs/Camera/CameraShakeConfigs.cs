using UnityEngine;

namespace Core.Data.Camera
{
    [CreateAssetMenu(fileName = "New camera shake configs", menuName = "Add/Configs/Camera/Camera shake config")]
    public class CameraShakeConfigs : ScriptableObject
    {
        public string Tag;
        public Vector3 CameraPosAxis;
        public Vector3 CameraRotationAxis;
        public AnimationCurve[] Curves;
        public AnimationCurve PowerCurve;
        public float Power = 1f;
        public float Duration = .3f;
        public bool IsUnscaleTime;

        public AnimationCurve GetRandomCurve()
        {
            return Curves[Random.Range(0, Curves.Length)];
        }
    }
}