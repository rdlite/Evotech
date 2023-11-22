using Core.Battle;

namespace Core.StateMachines.Battle
{
    public class StartRoundState : IState
    {
        private StateMachine _battleSM;
        private BattleObserver _battleObserver;
        private ITurnsSequencer _turnsSequencer;

        public StartRoundState(
            StateMachine battleStateMachine, BattleObserver battleObserver, ITurnsSequencer turnsSequencer)
        {
            _battleSM = battleStateMachine;
            _battleObserver = battleObserver;
            _turnsSequencer = turnsSequencer;
        }

        public void Enter()
        {
            _turnsSequencer.GenerateNewSequence();
            _battleSM.Enter<WaitingForTurnState>();
        }

        public void Exit() { }
    }
}