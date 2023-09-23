namespace Core.Infrastructure
{
    public class GameStateMachineLoader
    {
        private GameStateMachine _gameStateMachine;

        public GameStateMachineLoader()
        {
            _gameStateMachine = new GameStateMachine();

            //_gameStateMachine.Enter<BootstrapState>();
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