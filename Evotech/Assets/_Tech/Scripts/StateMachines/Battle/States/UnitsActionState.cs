using Core.Battle;
using Core.Cameras;
using Core.Data;

namespace Core.StateMachines.Battle
{
    public class UnitsActionState : IUpdateState, IPayloadState<ActionInfo>
    {
        private readonly BattleObserver _battleObserver;
        private readonly StateMachine _battleSM;

        private UnitActionResolver _actionResolver;

        public UnitsActionState(
            BattleObserver battleObserver, StateMachine battleSM, ICameraShaker cameraShaker,
            AssetsContainer assetsContainer)
        {
            _battleObserver = battleObserver;
            _battleSM = battleSM;
            _actionResolver = new UnitActionResolver(cameraShaker, assetsContainer);
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