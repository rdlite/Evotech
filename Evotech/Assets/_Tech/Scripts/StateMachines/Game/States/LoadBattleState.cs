using Utils;
using Core.Data;
using UnityEngine;
using Core.StateMachines;

namespace Core.Infrastructure
{
    public class LoadBattleState : IState
    {
        private ICurtain _curtain;

        public LoadBattleState(ICurtain curtain)
        {
            _curtain = curtain;
        }

        public void Enter()
        {
            _curtain.TriggerCurtain(true, false, () => SceneLoader.LoadAsync(SceneNames.BATTLE_SCENE));
        }

        public void Exit() { }
    }
}