using Core.Cameras;
using Core.Data;
using Qnject;
using UnityEngine;

namespace Core.Factories
{
    public class GameFactory : IGameFactory
    {
        public CameraController MainCamera { get; private set; }

        private AssetsContainer _assetsContainer;

        public GameFactory(AssetsContainer assetsContainer)
        {
            _assetsContainer = assetsContainer;
        }

        public CameraController CreateCamera(Vector3 position)
        {
            CameraController camera = QnjectPrefabsFactory.CreatePrefab(_assetsContainer.CameraPrefab, position);
            MainCamera = camera;
            return camera;
        }
    }
}