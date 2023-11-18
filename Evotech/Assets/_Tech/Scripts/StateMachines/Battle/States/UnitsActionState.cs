using Core.Battle;
using Core.Cameras;

namespace Core.StateMachines.Battle
{
    public class UnitsActionState : IUpdateState, IPayloadState<ActionInfo>
    {
        private readonly BattleObserver _battleObserver;
        private readonly StateMachine _battleSM;

        private UnitActionResolver _actionResolver;

        public UnitsActionState(
            BattleObserver battleObserver, StateMachine battleSM, ICameraShaker cameraShaker)
        {
            _battleObserver = battleObserver;
            _battleSM = battleSM;
            _actionResolver = new UnitActionResolver(cameraShaker);
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