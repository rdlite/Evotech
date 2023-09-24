using Utils;
using Qnject;
using Core.Data;
using UnityEngine;
using Core.Settings;
using Core.Curtains;
using Core.Factories;
using Core.InputSystem;

namespace Core.Infrastructure
{
    public class Startup : MonoBehaviour
    {
        private AssetsContainer _assetsContainer;
        private GameSettings _gameSettings;
        private MapTextsContainer _mapsContainer;
        private GameStateMachineLoader _gameStateMachine;
        private UpdatesProvider _updateProvider;
        private Container _projectInstaller;

        [Inject]
        private void Contruct(
            AssetsContainer assetsContainer, GameSettings gameSettings, MapTextsContainer mapsContainer)
        {
            _assetsContainer = assetsContainer;
            _gameSettings = gameSettings;
            _mapsContainer = mapsContainer;
        }

        public void Init(Container projectInstaller)
        {
            _projectInstaller = projectInstaller;

            SetBuildSettings();
            IUpdateProvider updateProvider = CreateUpdateProvider();
            IGameFactory gameFactory = CreateGameFactory();
            IInput input = CreateInput();
            ICursorController cursorController = CreateCursorController(input);
            ICurtain curtain = CreateCurtain();
            _projectInstaller.BindAsSingle<ICurtain>(curtain);
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

            return curtain;
        }

        private ICursorController CreateCursorController(IInput input)
        {
            CursorController cursorController = new CursorController(input);
            _projectInstaller.BindAsSingle<ICursorController>(cursorController);
            return cursorController;
        }

        private void LoadGlobalStateMachine(IGameFactory gameFactory, ICurtain curtain)
        {
            _gameStateMachine = new GameStateMachineLoader(
                _projectInstaller, _assetsContainer, _gameSettings,
                _mapsContainer, gameFactory, curtain);
        }
    }
}