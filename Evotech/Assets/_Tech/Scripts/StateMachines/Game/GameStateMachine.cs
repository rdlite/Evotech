using System;
using Core.StateMachine;
using System.Collections.Generic;
using Qnject;
using Core.Data;
using Core.Settings;
using Core.Factories;

namespace Core.Infrastructure
{
    public class GameStateMachine : UpdateStateMachine
    {
        public GameStateMachine(
            Container projectInstaller, AssetsContainer assetsContainer, GameSettings gameSettings,
            MapTextsContainer mapsContainer, IGameFactory gameFactory)
        {
            _states = new Dictionary<Type, IExitableState>();
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(StartupState)] = new StartupState(
                    projectInstaller, this),
                [typeof(LoadFightState)] = new LoadFightState(
                    assetsContainer, gameSettings, mapsContainer,
                    gameFactory, projectInstaller),
            };
        }
    }
}