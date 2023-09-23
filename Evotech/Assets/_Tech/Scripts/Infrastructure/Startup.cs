using Core.Settings;
using Qnject;
using UnityEngine;

namespace Core.Infrastructure
{
    public class Startup : MonoBehaviour
    {
        private GameStateMachineLoader _gameStateMachine;

        public void Init(Container projectInstaller)
        {
            BuildSettings buildSettings = new BuildSettings();
            buildSettings.Set();

            _gameStateMachine = new GameStateMachineLoader(projectInstaller);
        }

        private void Update()
        {
            _gameStateMachine.GameUpdate();
        }

        private void FixedUpdate()
        {
            _gameStateMachine.GameFixedUpdate();
        }

        private void LateUpdate()
        {
            _gameStateMachine.GameLateUpdate();
        }
    }
}