using Core.UI;
using Core.Data;
using Core.Battle;
using Core.Cameras;

namespace Core.StateMachines.Battle
{
    public class UnitsActionState : IUpdateState, IPayloadState<ActionInfo>
    {
        private readonly BattleObserver _battleObserver;
        private readonly StateMachine _battleSM;
        private readonly IUICanvasesResolver _canvasesResolver;
        private UnitActionResolver _actionResolver;

        public UnitsActionState(
            BattleObserver battleObserver, StateMachine battleSM, ICameraShaker cameraShaker,
            AssetsContainer assetsContainer, IUICanvasesResolver canvasesResolver)
        {
            _battleObserver = battleObserver;
            _battleSM = battleSM;
            _canvasesResolver = canvasesResolver;
            _actionResolver = new UnitActionResolver(cameraShaker, assetsContainer);
            _actionResolver.OnFinished += ActionFinished;
        }

        public void Enter(ActionInfo actionDesc)
        {
            _canvasesResolver.BlockAllUIInteraction();
            _actionResolver.Resolve(actionDesc);
        }

        public void Update() { }

        public void Exit() { }

        private void ActionFinished()
        {
            _canvasesResolver.ReturnUIInteraction();
            _battleSM.Enter<WaitingForTurnState>();
        }
    }
}