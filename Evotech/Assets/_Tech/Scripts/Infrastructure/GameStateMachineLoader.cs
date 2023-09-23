using Qnject;

namespace Core.Infrastructure
{
    public class GameStateMachineLoader
    {
        private GameStateMachine _gameStateMachine;

        public GameStateMachineLoader(Container projectInstaller)
        {
            _gameStateMachine = new GameStateMachine(projectInstaller);

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