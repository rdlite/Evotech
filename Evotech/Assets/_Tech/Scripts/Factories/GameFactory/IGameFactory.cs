using Core.Cameras;
using UnityEngine;

namespace Core.Factories
{
    public interface IGameFactory
    {
        public CameraController CreateCamera(Vector3 position);
    }
}