using Utils;
using System;
using Core.Data;
using Core.Battle;
using Core.Cameras;
using Core.Factories;
using Core.InputSystem;
using Core.StateMachines.Battle;
using System.Collections.Generic;
using Core.Settings;

namespace Core.StateMachines
{
    public class BattleStateMachine : UpdateStateMachine
    {
        public BattleStateMachine(
            IUnitsFactory unitsFactory, IMapDataProvider mapDataProvider, CameraController camera,
            BattleObserver battleObserver, IRaycaster raycaster, IInput input,
            IWalkFieldVisualizer walkFieldVisualizer, BattleSettings battleSettings)
        {
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(StartBattleState)] = new StartBattleState(
                    this, unitsFactory, mapDataProvider,
                    camera, battleObserver),
                [typeof(WaitingForTurnState)] = new WaitingForTurnState(
                    battleObserver, raycaster, camera,
                    input, this, walkFieldVisualizer,
                    mapDataProvider),
                [typeof(UnitMovementState)] = new UnitMovementState(
                    this, battleSettings, camera),
            };
        }
    }
}