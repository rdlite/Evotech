using Utils;
using Qnject;
using Core.Data;
using UnityEngine;
using Core.Battle;
using Core.Cameras;
using Core.Settings;
using Core.Factories;
using Core.InputSystem;
using Core.StateMachines;
using Cysharp.Threading.Tasks;
using Core.StateMachines.Battle;

namespace Core.Infrastructure
{
    public class BattleSceneStartup : MonoBehaviour
    {
        private StylesContainer _stylesContainer;
        private AssetsContainer _assetsContainer;
        private GameSettings _gameSettings;
        private MapTextsContainer _mapsContainer;
        private IGameFactory _gameFactory;
        private ICurtain _curtain;
        private IRaycaster _raycaster;
        private IUpdateProvider _updateProvider;
        private IInput _input;
        private Container _sceneInstaller;
        private IMapDataProvider _mapDataProvider;
        private IWalkFieldVisualizer _walkFieldVisualizer;
        private IBattleLinesFactory _battleLinesFactory;
        private BattleStateMachine _battleSM;

        [Inject]
        private void Construct(
            AssetsContainer assetsContainer, GameSettings gameSettings, MapTextsContainer mapsContainer,
            IGameFactory gameFactory, ICurtain curtain, IRaycaster raycaster,
            IUpdateProvider updateProvider, IInput input, StylesContainer stylesContainer)
        {
            _stylesContainer = stylesContainer;
            _assetsContainer = assetsContainer;
            _gameSettings = gameSettings;
            _mapsContainer = mapsContainer;
            _gameFactory = gameFactory;
            _curtain = curtain;
            _raycaster = raycaster;
            _updateProvider = updateProvider;
            _input = input;

            _updateProvider.AddUpdate(Tick);
            _updateProvider.AddFixedUpdate(FixedTick);
            _updateProvider.AddLateUpdate(LateTick);
        }

        private void OnEnable()
        {
            _updateProvider.AddUpdate(Tick);
            _updateProvider.AddFixedUpdate(FixedTick);
            _updateProvider.AddLateUpdate(LateTick);
        }

        private void OnDisable()
        {
            _updateProvider.RemoveUpdate(Tick);
            _updateProvider.RemoveFixedUpdate(FixedTick);
            _updateProvider.RemoveLateUpdate(LateTick);
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

            _walkFieldVisualizer = new WalkFieldVisualizer(_assetsContainer);

            CameraController camera = CreateCamera(Vector3.zero);

            BattleObserver battleObserver = new BattleObserver();

            _battleLinesFactory = CreateBattleLinesFactory(_stylesContainer, _assetsContainer);

            _battleSM = CreateStateMachine(
                unitsFactory, camera, battleObserver,
                _battleLinesFactory);

            _battleSM.Enter<StartBattleState>();

            await UniTask.Delay(100);

            _curtain.TriggerCurtain(false, false);
        }

        private void Tick()
        {
            _battleSM.UpdateState();
        }

        private void FixedTick()
        {
            _battleSM.FixedUpdateState();
        }

        private void LateTick()
        {
            _battleSM.LateUpdateState();
        }

        private IMapDataProvider CreateMap(LandscapeSettings landscapeSettings)
        {
            MapLoader mapLoader = new HexagonalMapLoader(_gameSettings.MapSettings, landscapeSettings);
            MapData mapData = mapLoader.LoadMap(_mapsContainer.GetMapData("GradientHeight"), Vector3.zero);

            MapDataProvider mapDataProvider = new MapDataProvider();
            mapDataProvider.Init(mapData, _gameSettings.MapSettings);
            _sceneInstaller.BindAsSingle<IMapDataProvider>(mapDataProvider);

            return mapDataProvider;
        }

        private BattleStateMachine CreateStateMachine(
            IUnitsFactory unitsFactory, CameraController camera, BattleObserver battleObserver,
            IBattleLinesFactory battleLinesFactory)
        {
            return new BattleStateMachine(
                unitsFactory, _mapDataProvider, camera,
                battleObserver, _raycaster, _input,
                _walkFieldVisualizer, _gameSettings.BattleSettings, _battleLinesFactory);
        }

        private IBattleLinesFactory CreateBattleLinesFactory(StylesContainer stylesContainer, AssetsContainer assetsContainer)
        {
            BattleLinesFactory battleLinesFactory = new BattleLinesFactory(stylesContainer, assetsContainer);
            return battleLinesFactory;
        }

        private CameraController CreateCamera(Vector3 position)
        {
            return _gameFactory.CreateCamera(position);
        }
    }
}