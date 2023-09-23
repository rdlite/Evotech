using Utils;
using Core.Data;
using UnityEngine;
using Core.StateMachine;
using Core.Settings;
using Core.Factories;
using Core.Cameras;
using Qnject;

namespace Core.Infrastructure
{
    public class LoadFightState : IState
    {
        private readonly AssetsContainer _assetsContainer;
        private readonly GameSettings _gameSettings;
        private readonly MapTextsContainer _mapsContainer;
        private readonly IGameFactory _gameFactory;
        private readonly Container _projectInstaller;

        public LoadFightState(
            AssetsContainer assetsContainer, GameSettings gameSettings, MapTextsContainer mapsContainer,
            IGameFactory gameFactory, Container projectInstaller)
        {
            _assetsContainer = assetsContainer;
            _gameSettings = gameSettings;
            _mapsContainer = mapsContainer;
            _gameFactory = gameFactory;
            _projectInstaller = projectInstaller;
        }

        public void Enter()
        {
            SceneLoader.LoadAsync(SceneNames.FIGHT_SCENE, Init);
        }

        private void Init()
        {
            LandscapeSettings landscapeSettings = _assetsContainer.HexagonsContainer.GetSettingsOfType(LandscapeTypes.DefaultGrass);

            CreateMap(landscapeSettings);

            CameraController camera = CreateCamera(Vector3.zero);
        }

        private void CreateMap(LandscapeSettings landscapeSettings)
        {
            MapLoader mapLoader = new HexagonalMapLoader(_gameSettings.MapSettings, landscapeSettings);
            MapData mapData = mapLoader.LoadMap(_mapsContainer.GetMapData("TestFight"), Vector3.zero);

            MapDataProvider mapDataProvider = new MapDataProvider();
            mapDataProvider.Init(mapData);
            _projectInstaller.BindAsSingle<IMapDataProvider>(mapDataProvider);
        }
        
        private CameraController CreateCamera(Vector3 position)
        {
            return _gameFactory.CreateCamera(position);
        }

        public void Exit() { }
    }
}