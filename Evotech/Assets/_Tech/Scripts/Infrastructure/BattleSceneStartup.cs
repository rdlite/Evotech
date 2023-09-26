using Qnject;
using Core.Data;
using UnityEngine;
using Core.Cameras;
using Core.Settings;
using Core.Factories;
using Cysharp.Threading.Tasks;
using Hexnav.Core;
using Core.StateMachines;
using Core.StateMachines.Battle;

namespace Core.Infrastructure
{
    public class BattleSceneStartup : MonoBehaviour
    {
        private AssetsContainer _assetsContainer;
        private GameSettings _gameSettings;
        private MapTextsContainer _mapsContainer;
        private IGameFactory _gameFactory;
        private ICurtain _curtain;
        private Container _sceneInstaller;
        private IMapDataProvider _mapDataProvider;

        [Inject]
        private void Construct(
            AssetsContainer assetsContainer, GameSettings gameSettings, MapTextsContainer mapsContainer,
            IGameFactory gameFactory, ICurtain curtain)
        {
            _assetsContainer = assetsContainer;
            _gameSettings = gameSettings;
            _mapsContainer = mapsContainer;
            _gameFactory = gameFactory;
            _curtain = curtain;
        }

        public void CreateBattleScene(Container sceneInstaller)
        {
            _sceneInstaller = sceneInstaller;
            Init();
        }

        private async void Init()
        {
            LandscapeSettings landscapeSettings = _assetsContainer.HexagonsContainer.GetSettingsOfType(LandscapeTypes.DefaultGrass);

            IUnitsFactory unitsFactory = new UnitsFactory(_assetsContainer);

            _mapDataProvider = CreateMap(landscapeSettings);

            CameraController camera = CreateCamera(Vector3.zero);

            BattleStateMachine battleSM = CreateStateMachine(unitsFactory, camera);

            battleSM.Enter<StartBattleState>();

            await UniTask.Delay(100);

            _curtain.TriggerCurtain(false, false);
        }

        private IMapDataProvider CreateMap(LandscapeSettings landscapeSettings)
        {
            MapLoader mapLoader = new HexagonalMapLoader(_gameSettings.MapSettings, landscapeSettings);
            MapData mapData = mapLoader.LoadMap(_mapsContainer.GetMapData("GradientHeight"), Vector3.zero);

            MapDataProvider mapDataProvider = new MapDataProvider();
            mapDataProvider.Init(mapData, _gameSettings.MapSettings);
            _sceneInstaller.BindAsSingle<IMapDataProvider>(mapDataProvider);

            HexPathfinfing hexPathfinding = new HexPathfinfing(_mapDataProvider);

            return mapDataProvider;
        }

        private BattleStateMachine CreateStateMachine(IUnitsFactory unitsFactory, CameraController camera)
        {
            return new BattleStateMachine(
                unitsFactory, _mapDataProvider, camera);
        }

        private CameraController CreateCamera(Vector3 position)
        {
            return _gameFactory.CreateCamera(position);
        }
    }
}