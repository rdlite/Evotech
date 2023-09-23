using Core.StateMachine;
using UnityEngine;
using Utils;

namespace Core.Infrastructure
{
    public class LoadFightState : IState
    {
        public void Enter()
        {
            SceneLoader.LoadAsync(SceneNames.FIGHT_SCENE, InitFight);
        }

        private void InitFight()
        {

        }

        public void Exit() { }
    }
}