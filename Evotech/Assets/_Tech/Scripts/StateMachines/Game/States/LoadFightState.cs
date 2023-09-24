using Utils;
using Core.Data;
using UnityEngine;
using Core.StateMachine;

namespace Core.Infrastructure
{
    public class LoadFightState : IState
    {
        private ICurtain _curtain;

        public LoadFightState(ICurtain curtain)
        {
            _curtain = curtain;
        }

        public void Enter()
        {
            _curtain.TriggerCurtain(true, false, () => SceneLoader.LoadAsync(SceneNames.FIGHT_SCENE));
        }

        public void Exit() { }
    }
}