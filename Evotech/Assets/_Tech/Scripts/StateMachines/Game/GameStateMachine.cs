using System;
using Core.StateMachine;
using System.Collections.Generic;

namespace Core.Infrastructure
{
    public class GameStateMachine : UpdateStateMachine
    {
        public GameStateMachine()
        {
            _states = new Dictionary<Type, IExitableState>();
            //_states = new Dictionary<Type, IExitableState>()
            //{
            //    [typeof(BootstrapState)] = new BootstrapState(
            //        this, coroutineService, uiRoot,
            //        assetsContainer),
            //    [typeof(LoadLevelState)] = new LoadLevelState(
            //        ServicesContainer.Instance.Get<LevelsLoadingService>(), this, configsContainer,
            //        assetsContainer, coroutineService, ServicesContainer.Instance.Get<InputService>()),
            //    [typeof(WordWalkingState)] = new WordWalkingState(
            //        this, uiRoot),
            //    [typeof(BattleState)] = new BattleState(
            //        uiRoot, assetsContainer, coroutineService,
            //        ServicesContainer.Instance.Get<InputService>())
            //};
        }
    }
}