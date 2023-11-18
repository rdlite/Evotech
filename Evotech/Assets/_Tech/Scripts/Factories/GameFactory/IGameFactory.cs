using Core.Cameras;
using UnityEngine;

namespace Core.Factories
{
    public interface IGameFactory
    {
        CameraController MainCamera { get; }
        CameraBaseContainer CameraBaseContainer { get; }
        CameraController CreateCamera(Vector3 position);
    }
}