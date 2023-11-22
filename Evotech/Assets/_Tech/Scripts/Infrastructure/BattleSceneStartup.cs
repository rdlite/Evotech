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
using Core.UI;
using Core.UI.Models;

namespace Core.Infrastructure
{
    public class BattleSceneStartup : MonoBehaviour
    {
        private IUnitsUIStatsController _uiStatsController;
        private IWalkFieldVisualizer _walkFieldVisualizer;
        private IBattleLinesFactory _battleLinesFactory;
        private IUICanvasesResolver _canvasesResolver;
        private IMapDataProvider _mapDataProvider;
        private StylesContainer _stylesContainer;
        private AssetsContainer _assetsContainer;
        private MapTextsContainer _mapsContainer;
        private IUpdateProvider _updateProvider;
        private BattleObserver _battleObserver;
        private BattleStateMachine _battleSM;
        private ICameraShaker _cameraShaker;
        private ITurnsSequencer _turnsSequencer;
        private GameSettings _gameSettings;
        private Container _sceneInstaller;
        private IGameFactory _gameFactory;
        private IRaycaster _raycaster;
        private ICurtain _curtain;
        private IInput _input;

        [Inject]
        private void Construct(
            AssetsContainer assetsContainer, GameSettings gameSettings, MapTextsContainer mapsContainer,
            IGameFactory gameFactory, ICurtain curtain, IRaycaster raycaster,
            IUpdateProvider updateProvider, IInput input, StylesContainer stylesContainer,
            IUICanvasesResolver canvasesResolver, ICameraShaker cameraShaker)
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
            _canvasesResolver = canvasesResolver;
            _cameraShaker = cameraShaker;

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
            _canvasesResolver.CreateCanvas(Enums.UICanvasType.Battle, false);

            LandscapeSettings landscapeSettings = _assetsContainer.HexagonsContainer.GetSettingsOfType(LandscapeTypes.DefaultGrass);

            IUnitsFactory unitsFactory = new UnitsFactory(_assetsContainer);

            _mapDataProvider = CreateMap(landscapeSettings);
            _uiStatsController = CreateUIStatsController();

            _walkFieldVisualizer = new WalkFieldVisualizer(_assetsContainer);

            CameraController camera = CreateCamera(Vector3.zero);

            _battleObserver = new BattleObserver(_uiStatsController, _mapDataProvider);
            _canvasesResolver.GetCanvas<BattleCanvas>().GetPanelOfType<MainBattlePanel>().InitBattleObserver(_battleObserver);

            _battleLinesFactory = CreateBattleLinesFactory(_stylesContainer, _assetsContainer);
            _turnsSequencer = CreateTurnsSequencer(_battleObserver);

            _battleSM = CreateStateMachine(
                unitsFactory, camera, _battleObserver,
                _battleLinesFactory, _cameraShaker, _turnsSequencer);

            _uiStatsController.Init();

            _battleSM.Enter<StartBattleState>();

            await UniTask.Delay(100);

            _curtain.TriggerCurtain(false, false);

            await UniTask.Delay(500);

            _canvasesResolver.OpenCanvas(Enums.UICanvasType.Battle);
        }

        private void Tick()
        {
            _battleSM.UpdateState();
            _battleObserver?.Tick();
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
            IBattleLinesFactory battleLinesFactory, ICameraShaker cameraShaker, ITurnsSequencer turnsSequencer)
        {
            return new BattleStateMachine(
                unitsFactory, _mapDataProvider, camera,
                battleObserver, _raycaster, _input,
                _walkFieldVisualizer, _gameSettings.BattleSettings, _battleLinesFactory,
                _canvasesResolver, cameraShaker, _assetsContainer,
                turnsSequencer);
        }

        private IBattleLinesFactory CreateBattleLinesFactory(StylesContainer stylesContainer, AssetsContainer assetsContainer)
        {
            BattleLinesFactory battleLinesFactory = new BattleLinesFactory(stylesContainer, assetsContainer);
            return battleLinesFactory;
        }

        private ITurnsSequencer CreateTurnsSequencer(BattleObserver battleObserver)
        {
            ITurnsSequencer turnSequencer = new TurnsSequencer(
                battleObserver,
                _canvasesResolver.GetCanvas<BattleCanvas>().GetPanelOfType<MainBattlePanel>().GetUnitsSequencePanel());
            return turnSequencer;
        }

        private IUnitsUIStatsController CreateUIStatsController()
        {
            IUnitsUIStatsController unitsUIStatsController = new UnitUIStatsController(_canvasesResolver, _assetsContainer);
            return unitsUIStatsController;
        }

        private CameraController CreateCamera(Vector3 position)
        {
            return _gameFactory.CreateCamera(position);
        }
    }
}