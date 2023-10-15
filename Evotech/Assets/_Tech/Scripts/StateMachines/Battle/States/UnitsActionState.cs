using Core.Battle;
namespace Core.StateMachines.Battle
{
    public class UnitsActionState : IUpdateState, IPayloadState<ActionInfo>
    {
        private readonly BattleObserver _battleObserver;
        private readonly StateMachine _battleSM;

        private UnitActionResolver _actionResolver;

        public UnitsActionState(
            BattleObserver battleObserver, StateMachine battleSM)
        {
            _battleObserver = battleObserver;
            _battleSM = battleSM;
            _actionResolver = new UnitActionResolver();
            _actionResolver.OnFinished += ActionFinish;
        }

        public void Enter(ActionInfo actionDesc)
        {
            _actionResolver.Resolve(actionDesc);
        }

        public void Update() { }

        public void Exit() { }

        private void ActionFinish()
        {
            _battleSM.Enter<WaitingForTurnState>();
        }
    }
}