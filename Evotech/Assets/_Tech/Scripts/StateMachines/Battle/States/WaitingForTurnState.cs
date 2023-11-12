using Utils;
using Core.Units;
using Core.Battle;
using Core.Cameras;
using Core.InputSystem;
using UnityEngine;
using Core.Data;
using Hexnav.Core;
using System.Collections.Generic;
using Core.UI;

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
        private readonly IBattleLinesFactory _battleLinesFactory;

        private BaseUnit _currentHoverUnit;
        private UnitWalkingResolver _unitWalkingResolver;
        private IUICanvasesResolver _uICanvasesResolver;
        private float _timeForClick;
        private bool _isClicked;

        public WaitingForTurnState(
            BattleObserver battleObserver, IRaycaster raycaster, CameraController camera, 
            IInput input, StateMachine battleSM, IWalkFieldVisualizer walkFieldVisualizer,
            IMapDataProvider mapDataProvider, IBattleLinesFactory battleLinesFactory, IUICanvasesResolver uICanvasesResolver)
        {
            _battleObserver = battleObserver;
            _raycaster = raycaster;
            _camera = camera;
            _input = input;
            _battleSM = battleSM;
            _walkFieldVisualizer = walkFieldVisualizer;
            _mapDataProvider = mapDataProvider;
            _battleLinesFactory = battleLinesFactory;
            _unitWalkingResolver = new UnitWalkingResolver(
                raycaster, camera, walkFieldVisualizer,
                battleLinesFactory, mapDataProvider);
            _uICanvasesResolver = uICanvasesResolver;
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
            _battleLinesFactory.ClearLines();
            _currentHoverUnit?.SetActiveOutline(false, false);
        }

        private void LMBDown()
        {
            _isClicked = true;
            _timeForClick = 0f;
        }

        private void HandleHover()
        {
            UnitRaycastTrigger unitUnderPointer = _raycaster.GetUnitTrigger(_camera.GetCamera());
               
            if (unitUnderPointer != null && 
                !_input.IsWheelPressed())
            {
                _currentHoverUnit = unitUnderPointer.ParentUnit;
                _currentHoverUnit.SetActiveOutline(true, false);
            }
            else if (_currentHoverUnit != null)
            {
                _currentHoverUnit.SetActiveOutline(false, false);
                _currentHoverUnit = null;
            }

            if (_unitWalkingResolver.IsHaveUnitToWalk())
            {
                _unitWalkingResolver.SetHoverUnit(_currentHoverUnit);
            }
            else
            {
                _unitWalkingResolver.ReleaseHoveredUnit();
            }
        }

        private void LMBUp()
        {
            _isClicked = false;

            if (IsWalkingAndClickedOnEnemy())
            {
                BaseUnit currentWalkingUnit = _unitWalkingResolver.GetCurrenUnit();
                BaseUnit currentHoverUnit = _currentHoverUnit;

                System.Action attackCallback = () => 
                {
                    ActionMeleeAttack actionInfo = new ActionMeleeAttack();
                    actionInfo.Actor = currentWalkingUnit;
                    actionInfo.Damage = currentWalkingUnit.GetAtackDamage();
                    actionInfo.SubjectUnits = new List<BaseUnit>() { currentHoverUnit };

                    _battleSM.Enter<UnitsActionState, ActionInfo>(actionInfo);
                };

                bool isNearEnemy = IsPointsAreNeighbours(
                    currentWalkingUnit.transform.position, 
                    _currentHoverUnit.transform.position);

                if (!isNearEnemy)
                {
                    if (IsLastWalkPointNearEnemy())
                    {
                        MoveUnit(attackCallback);
                    }
                    else
                    {
                        MoveUnit(null);
                    }
                }
                else
                {
                    ResetWalkSelection();
                    attackCallback?.Invoke();
                }
            }
            else
            {
                if (IsClickedOnPossibleWalkinUnit())
                {
                    _walkFieldVisualizer.Hide();

                    NodeBase unitNode = _mapDataProvider.GetNearestNodeOfWorldPoint(_currentHoverUnit.transform.position);
                    List<NodeBase> nodesWalkingRange = HexPathfindingGrid.GetWalkRange(unitNode, _currentHoverUnit.GetWalkRange());
                    _walkFieldVisualizer.Show(
                        unitNode,
                        nodesWalkingRange);

                    _unitWalkingResolver.SetCurrentWalkingUnit(_currentHoverUnit, nodesWalkingRange);
                }
                else if (IsClickedOnFieldWithSelectedWalkingUnit())
                {
                    MoveUnit(null);
                }
            }
        }

        private void MoveUnit(System.Action callback)
        {
            UnitMovementState.MovementData movementData = new UnitMovementState.MovementData(
                _unitWalkingResolver.GetCurrenUnit(),
                _mapDataProvider.GetNearestNodeOfWorldPoint(_unitWalkingResolver.GetCurrenUnit().transform.position),
                _mapDataProvider.GetNearestNodeOfWorldPoint(_unitWalkingResolver.GetLastWalkPoint()),
                callback);

            ResetWalkSelection();

            _battleSM.Enter<UnitMovementState, UnitMovementState.MovementData>(movementData);
        }

        private bool IsWalkingAndClickedOnEnemy()
        {
            return 
                _unitWalkingResolver.IsHaveUnitToWalk() && _currentHoverUnit != null &&
                Utils.Battle.BattleUtils.GetRelationForUnits(Enums.UnitType.Player, _currentHoverUnit.UnitType) == Enums.UnitRelation.Enemy &&
                _timeForClick <= 1f;
        }

        private bool IsClickedOnPossibleWalkinUnit()
        {
            return
                !_unitWalkingResolver.IsHaveUnitToWalk() &&
                _currentHoverUnit != null && _currentHoverUnit.UnitType == Enums.UnitType.Player &&
                _timeForClick <= 1f;
        }

        private bool IsClickedOnFieldWithSelectedWalkingUnit()
        {
            return 
                _unitWalkingResolver.IsHaveUnitToWalk() && 
                _currentHoverUnit == null && 
                _timeForClick <= 1f;
        }

        private bool IsLastWalkPointNearEnemy()
        {
            return IsPointsAreNeighbours(_currentHoverUnit.transform.position, _unitWalkingResolver.GetLastWalkPoint());
        }

        private bool IsPointsAreNeighbours(Vector3 point1, Vector3 point2)
        {
            return _mapDataProvider.GetNearestNodeOfWorldPoint(point1).Neighbours.Contains(
                _mapDataProvider.GetNearestNodeOfWorldPoint(point2));
        }

        private void ResetWalkSelection()
        {
            _unitWalkingResolver.SetCurrentWalkingUnit(null, null);
            _walkFieldVisualizer.Hide();
        }
    }
}