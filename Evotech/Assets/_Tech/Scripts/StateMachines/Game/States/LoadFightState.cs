using Utils;
using Core.Data;
using UnityEngine;
using Core.StateMachine;

namespace Core.Infrastructure
{
    public class LoadFightState : IState
    {
        public void Enter()
        {
            SceneLoader.LoadAsync(SceneNames.FIGHT_SCENE);
        }

        public void Exit() { }
    }
}