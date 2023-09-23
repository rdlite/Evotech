using Core.Settings;
using UnityEngine;

namespace Core.Infrastructure
{
    public class Startup : MonoBehaviour
    {
        private GameStateMachineLoader _gameStateMachine;

        private void Awake()
        {
            BuildSettings buildSettings = new BuildSettings();
            buildSettings.Set();

            _gameStateMachine = new GameStateMachineLoader();
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