using Utils;
using Core.Units;
using Core.Battle;
using Core.Cameras;
using Core.InputSystem;
using UnityEngine;
using Core.Data;
using Hexnav.Core;
using System.Collections.Generic;

namespace Core.StateMachines.Battle
{
    public class WaitingForTurnState : IUpdateState, IState
    {
        private readonly BattleObserver _battleObserver;
        private readonly IRaycaster _raycaster;
        private readonly CameraController _camera;
        private readonly IInput _input;
        private readonly StateMachine _battleSM;
        private readonly IWalkFieldVisualizer _walkFieldVisualizer;
        private readonly IMapDataProvider _mapDataProvider;

        private BaseUnit _currentHoverUnit;
        private UnitWalkingResolver _unitWalkingResolver;
        private float _timeForClick;
        private bool _isClicked;

        public WaitingForTurnState(
            BattleObserver battleObserver, IRaycaster raycaster, CameraController camera, 
            IInput input, StateMachine battleSM, IWalkFieldVisualizer walkFieldVisualizer,
            IMapDataProvider mapDataProvider)
        {
            _battleObserver = battleObserver;
            _raycaster = raycaster;
            _camera = camera;
            _input = input;
            _battleSM = battleSM;
            _walkFieldVisualizer = walkFieldVisualizer;
            _mapDataProvider = mapDataProvider;
            _unitWalkingResolver = new UnitWalkingResolver(
                raycaster, camera, walkFieldVisualizer);
        }

        public void Enter()
        {
            _input.OnLMBDown += LMBDown;
            _input.OnLMBUp += LMBUp;
            _input.OnRMBUp += ResetWalkSelection;
            _input.OnMMBDown += ResetWalkSelection;
        }

        public void Update()
        {
            if (_isClicked)
            {
                _timeForClick += Time.deltaTime;
            }

            HandleHover();

            _unitWalkingResolver.Update();
        }

        public void Exit()
        {
            _input.OnLMBDown -= LMBDown;
            _input.OnLMBUp -= LMBUp;
            _input.OnRMBUp -= ResetWalkSelection;
            _input.OnMMBDown -= ResetWalkSelection;
        }

        private void LMBDown()
        {
            _isClicked = true;
            _timeForClick = 0f;
        }

        private void HandleHover()
        {
            UnitRaycastTrigger unitUnderPointer = _raycaster.GetUnitTrigger(_camera.GetCamera());

            if (unitUnderPointer != null && !_input.IsWheelPressed())
            {
                _currentHoverUnit = unitUnderPointer.ParentUnit;
            }
            else
            {
                _currentHoverUnit = null;
            }
        }

        private void LMBUp()
        {
            _isClicked = false;

            if (!_unitWalkingResolver.IsHaveUnitToWalk() && _currentHoverUnit != null && _timeForClick <= 1f)
            {
                _walkFieldVisualizer.Hide();

                NodeBase unitNode = _mapDataProvider.GetNearestNodeOfWorldPoint(_currentHoverUnit.transform.position);
                List<NodeBase> nodesWalkingRange = HexPathfindingGrid.GetWalkRange(unitNode, _currentHoverUnit.GetWalkRange());
                _walkFieldVisualizer.Show(
                    unitNode,
                    nodesWalkingRange);

                _unitWalkingResolver.SetCurrentWalkingUnit(_currentHoverUnit, nodesWalkingRange);
            }
            else if (_unitWalkingResolver.IsHaveUnitToWalk() && _timeForClick <= 1f)
            {
                UnitMovementState.MovementData movementData = new UnitMovementState.MovementData(
                    _unitWalkingResolver.GetCurrenUnit(),
                    _mapDataProvider.GetNearestNodeOfWorldPoint(_unitWalkingResolver.GetCurrenUnit().transform.position),
                    _mapDataProvider.GetNearestNodeOfWorldPoint(_unitWalkingResolver.GetLastWalkPoint()));

                _battleSM.Enter<UnitMovementState, UnitMovementState.MovementData>(movementData);
             
                ResetWalkSelection();
            }
        }

        private void ResetWalkSelection()
        {
            _unitWalkingResolver.SetCurrentWalkingUnit(null, null);
            _walkFieldVisualizer.Hide();
        }
    }
}