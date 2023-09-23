using System;
using Core.StateMachine;
using System.Collections.Generic;
using Qnject;

namespace Core.Infrastructure
{
    public class GameStateMachine : UpdateStateMachine
    {
        public GameStateMachine(Container projectInstaller)
        {
            _states = new Dictionary<Type, IExitableState>();
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(StartupState)] = new StartupState(
                    projectInstaller, this),
                [typeof(LoadFightState)] = new LoadFightState(),
            };
        }
    }
}