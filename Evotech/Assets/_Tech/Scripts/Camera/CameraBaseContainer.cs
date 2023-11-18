using UnityEngine;

namespace Core.Cameras
{
    public class CameraBaseContainer : MonoBehaviour
    {
        [field: SerializeField] public Transform CameraRotationParent { get; private set; }
    }
}