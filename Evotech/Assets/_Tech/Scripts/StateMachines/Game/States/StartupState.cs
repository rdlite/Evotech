using Core.StateMachines;
using Qnject;

namespace Core.Infrastructure
{
    public class StartupState : IState
    {
        private Container _projectInstaller;
        private GameStateMachine _gameSM;

        public StartupState(
            Container projectInstaller, GameStateMachine gameSM)
        {
            _projectInstaller = projectInstaller;
            _gameSM = gameSM;
        }

        public void Enter()
        {
            Install();

            _gameSM.Enter<LoadBattleState>();
        }

        private void Install()
        {

        }

        public void Exit() { }
    }
}