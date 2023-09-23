using Core.Data;
using Core.Settings;
using Qnject;
using UnityEngine;

namespace Core.Infrastructure
{
    public class Startup : MonoBehaviour
    {
        [Inject] private AssetsContainer _assetsContainer;
        [Inject] private GameSettings _gameSettings;
        [Inject] private MapTextsContainer _mapsContainer;

        private GameStateMachineLoader _gameStateMachine;

        public void Init(Container projectInstaller)
        {
            BuildSettings buildSettings = new BuildSettings();
            buildSettings.Set();

            _gameStateMachine = new GameStateMachineLoader(
                projectInstaller, _assetsContainer, _gameSettings,
                _mapsContainer);
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