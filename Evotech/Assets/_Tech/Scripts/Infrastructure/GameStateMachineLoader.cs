using Core.Data;
using Core.Settings;
using Qnject;

namespace Core.Infrastructure
{
    public class GameStateMachineLoader
    {
        private GameStateMachine _gameStateMachine;

        public GameStateMachineLoader(
            Container projectInstaller, AssetsContainer assetsContainer, GameSettings gameSettings,
            MapTextsContainer mapsContainer)
        {
            _gameStateMachine = new GameStateMachine(
                projectInstaller, assetsContainer, gameSettings,
                mapsContainer);

            _gameStateMachine.Enter<StartupState>();
        }

        public void GameUpdate()
        {
            _gameStateMachine.UpdateState();
        }

        public void GameFixedUpdate()
        {
            _gameStateMachine.FixedUpdateState();
        }

        public void GameLateUpdate()
        {
            _gameStateMachine.LateUpdateState();
        }
    }
}