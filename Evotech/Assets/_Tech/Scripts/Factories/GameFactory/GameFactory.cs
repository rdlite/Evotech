using Core.Cameras;
using Core.Data;
using Qnject;
using UnityEngine;

namespace Core.Factories
{
    public class GameFactory : IGameFactory
    {
        public CameraController MainCamera { get; private set; }
        public CameraBaseContainer CameraBaseContainer { get; private set; }

        private AssetsContainer _assetsContainer;

        public GameFactory(AssetsContainer assetsContainer)
        {
            _assetsContainer = assetsContainer;
        }

        public CameraController CreateCamera(Vector3 position)
        {
            CameraController camera = QnjectPrefabsFactory.Instantiate(_assetsContainer.CameraPrefab, position);
            CameraBaseContainer = camera.GetComponent<CameraBaseContainer>();
            MainCamera = camera;
            return camera;
        }
    }
}