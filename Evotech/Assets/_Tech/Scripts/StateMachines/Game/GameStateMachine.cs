using System;
using Core.StateMachines;
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
            MapTextsContainer mapsContainer, IGameFactory gameFactory, ICurtain curtain)
        {
            _states = new Dictionary<Type, IExitableState>();
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(StartupState)] = new StartupState(
                    projectInstaller, this),
                [typeof(LoadBattleState)] = new LoadBattleState(
                    curtain),
            };
        }
    }
}