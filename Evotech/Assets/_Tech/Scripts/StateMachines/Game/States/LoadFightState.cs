using Utils;
using Core.Data;
using UnityEngine;
using Core.StateMachine;
using Core.Settings;

namespace Core.Infrastructure
{
    public class LoadFightState : IState
    {
        private readonly AssetsContainer _assetsContainer;
        private readonly GameSettings _gameSettings;
        private readonly MapTextsContainer _mapsContainer;

        public LoadFightState(AssetsContainer assetsContainer, GameSettings gameSettings, MapTextsContainer mapsContainer)
        {
            _assetsContainer = assetsContainer;
            _gameSettings = gameSettings;
            _mapsContainer = mapsContainer;
        }

        public void Enter()
        {
            SceneLoader.LoadAsync(SceneNames.FIGHT_SCENE, InitFight);
        }

        private void InitFight()
        {
            LandscapeSettings landscapeSettings = _assetsContainer.HexagonsContainer.GetSettingsOfType(LandscapeTypes.DefaultGrass);

            MapLoader mapLoader = new HexagonalMapLoader(_gameSettings.MapSettings, landscapeSettings);
            MapData mapData = mapLoader.LoadMap(_mapsContainer.GetMapData("TestFight"), Vector3.zero);

            MapDataProvider mapDataProvider = new MapDataProvider();
            mapDataProvider.Init(mapData);
        }

        public void Exit() { }
    }
}