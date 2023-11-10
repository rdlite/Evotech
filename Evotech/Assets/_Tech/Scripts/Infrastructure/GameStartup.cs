using Utils;
using Qnject;
using Core.Data;
using UnityEngine;
using Core.Settings;
using Core.Curtains;
using Core.Factories;
using Core.InputSystem;
using Core.UI;

namespace Core.Infrastructure
{
    public class GameStartup : MonoBehaviour
    {
        private AssetsContainer _assetsContainer;
        private GameSettings _gameSettings;
        private MapTextsContainer _mapsContainer;
        private UnitSettingsContainer _unitSettingsContainer;
        private GameStateMachineLoader _gameStateMachine;
        private UpdatesProvider _updateProvider;
        private Container _projectInstaller;

        [Inject]
        private void Contruct(
            AssetsContainer assetsContainer, GameSettings gameSettings, MapTextsContainer mapsContainer,
            UnitSettingsContainer unitSettingsContainer)
        {
            _assetsContainer = assetsContainer;
            _gameSettings = gameSettings;
            _mapsContainer = mapsContainer;
            _unitSettingsContainer = unitSettingsContainer;
        }

        public void Init(Container projectInstaller)
        {
            _projectInstaller = projectInstaller;

            SetBuildSettings();
            IUpdateProvider updateProvider = CreateUpdateProvider();
            IGameFactory gameFactory = CreateGameFactory();
            IInput input = CreateInput();
            IRaycaster raycaster = CreateRaycaster(input);
            ICursorController cursorController = CreateCursorController(input);
            ICurtain curtain = CreateCurtain();
            IUnitStaticStatsProvider statsProvider = CreateStatsProvider();
            IUICanvasesResolver canvasesResolver = CreateCanvasesResolver();
            LoadGlobalStateMachine(gameFactory, curtain);
        }

        private void Update()
        {
            _updateProvider.Update();
            _gameStateMachine.GameUpdate();
        }

        private void FixedUpdate()
        {
            _updateProvider.FixedUpdate();
            _gameStateMachine.GameFixedUpdate();
        }

        private void LateUpdate()
        {
            _updateProvider.LateUpdate();
            _gameStateMachine.GameLateUpdate();
        }

        private void SetBuildSettings()
        {
            BuildSettings buildSettings = new BuildSettings();
            buildSettings.Set();
        }

        private IUpdateProvider CreateUpdateProvider()
        {
            _updateProvider = new UpdatesProvider();
            _projectInstaller.BindAsSingle<IUpdateProvider>(_updateProvider);
            return _updateProvider;
        }

        private IGameFactory CreateGameFactory()
        {
            GameFactory gameFactory = new GameFactory(_assetsContainer);
            _projectInstaller.BindAsSingle<IGameFactory>(gameFactory);
            return gameFactory;
        }

        private IInput CreateInput()
        {
            InputController input = new InputController(_updateProvider);
            _projectInstaller.BindAsSingle<IInput>(input);
            return input;
        }

        private ICurtain CreateCurtain()
        {
            Curtain curtainPrefab = Instantiate(_assetsContainer.CurtainPrefab);
            curtainPrefab.transform.SetParent(transform);

            ICurtain curtain = curtainPrefab;
            curtain.TriggerCurtain(true, true);
            _projectInstaller.BindAsSingle<ICurtain>(curtain);

            return curtain;
        }

        private ICursorController CreateCursorController(IInput input)
        {
            CursorController cursorController = new CursorController(input);
            _projectInstaller.BindAsSingle<ICursorController>(cursorController);
            return cursorController;
        }

        private IUnitStaticStatsProvider CreateStatsProvider()
        {
            UnitStaticStatsProvider statsProvider = new UnitStaticStatsProvider(_unitSettingsContainer);
            _projectInstaller.BindAsSingle<IUnitStaticStatsProvider>(statsProvider);
            return statsProvider;
        }
        
        private IUICanvasesResolver CreateCanvasesResolver()
        {
            UICanvasesResolver canvasesResolver = new UICanvasesResolver();
            _projectInstaller.BindAsSingle<IUICanvasesResolver>(canvasesResolver);
            return canvasesResolver;
        }

        private void LoadGlobalStateMachine(IGameFactory gameFactory, ICurtain curtain)
        {
            _gameStateMachine = new GameStateMachineLoader(
                _projectInstaller, _assetsContainer, _gameSettings,
                _mapsContainer, gameFactory, curtain);
        }

        private IRaycaster CreateRaycaster(IInput input)
        {
            Raycaster newRaycaster = new Raycaster(_gameSettings.LayersSettings, input);
            _projectInstaller.BindAsSingle<IRaycaster>(newRaycaster);
            return newRaycaster;
        }
    }
}