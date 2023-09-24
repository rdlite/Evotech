using Core.Cameras;
using Core.Data;
using Core.Factories;
using Core.Settings;
using Qnject;
using UnityEngine;

public class FightSceneStartup : MonoBehaviour
{
    private AssetsContainer _assetsContainer;
    private GameSettings _gameSettings;
    private MapTextsContainer _mapsContainer;
    private IGameFactory _gameFactory;
    private Container _sceneInstaller;

    [Inject]
    private void Construct(
        AssetsContainer assetsContainer, GameSettings gameSettings, MapTextsContainer mapsContainer,
        IGameFactory gameFactory)
    {
        _assetsContainer = assetsContainer;
        _gameSettings = gameSettings;
        _mapsContainer = mapsContainer;
        _gameFactory = gameFactory;
    }

    public void CreateFightScene(Container sceneInstaller)
    {
        _sceneInstaller = sceneInstaller;
        Init();
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
        mapDataProvider.Init(mapData, _gameSettings.MapSettings);
        _sceneInstaller.BindAsSingle<IMapDataProvider>(mapDataProvider);
    }

    private CameraController CreateCamera(Vector3 position)
    {
        return _gameFactory.CreateCamera(position);
    }
}