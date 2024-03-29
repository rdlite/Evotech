using Utils;
using Core.UI;
using Core.Data;
using Core.Units;
using Hexnav.Core;
using Core.Battle;
using UnityEngine;
using Core.Cameras;
using Utils.Battle;
using Core.UI.Elements;
using Core.Data.Skills;
using Core.InputSystem;
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
        private readonly IBattleLinesFactory _battleLinesFactory;
        private readonly UnitsSequencePanel _unitsSequencePanel;
        private readonly IUICanvasesResolver _canvasesResolver;
        private BaseUnit _currentHoverUnit;
        private UnitWalkingResolver _unitWalkingResolver;
        private float _timeForClick;
        private bool _isClicked;

        public WaitingForTurnState(
            BattleObserver battleObserver, IRaycaster raycaster, CameraController camera, 
            IInput input, StateMachine battleSM, IWalkFieldVisualizer walkFieldVisualizer,
            IMapDataProvider mapDataProvider, IBattleLinesFactory battleLinesFactory, UnitsSequencePanel unitSequencePanel,
            IUICanvasesResolver canvasesResolver)
        {
            _battleObserver = battleObserver;
            _raycaster = raycaster;
            _camera = camera;
            _input = input;
            _battleSM = battleSM;
            _walkFieldVisualizer = walkFieldVisualizer;
            _mapDataProvider = mapDataProvider;
            _battleLinesFactory = battleLinesFactory;
            _unitsSequencePanel = unitSequencePanel;
            _unitWalkingResolver = new UnitWalkingResolver(
                raycaster, camera, walkFieldVisualizer,
                battleLinesFactory, mapDataProvider, unitSequencePanel);
            _canvasesResolver = canvasesResolver;
        }

        public void Enter()
        {
            _input.OnLMBDown += LMBDown;
            _input.OnLMBUp += LMBUp;
            _input.OnRMBUp += RMBUp;
            _canvasesResolver.GetCanvas<BattleCanvas>().GetPanelOfType<UnitManagementPanel>().OnSkillButtonPressed += OnSkillPressed;

            if (_unitWalkingResolver.GetCurrenUnit() != null)
            {
                List<NodeBase> nodesWalkingRange = ShowMovementRange();
                _unitWalkingResolver.SetCurrentUnit(_unitWalkingResolver.GetCurrenUnit(), nodesWalkingRange);
            }
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
            _input.OnRMBUp -= RMBUp;
            _battleLinesFactory.ClearLines();
            _battleObserver.ClearAdditionalInfo();
            _canvasesResolver.GetCanvas<BattleCanvas>().GetPanelOfType<UnitManagementPanel>().OnSkillButtonPressed -= OnSkillPressed;
            _canvasesResolver.GetCanvas<BattleCanvas>().GetPanelOfType<UnitManagementPanel>().RemoveSelection();
        }

        private void LMBDown()
        {
            _isClicked = true;
            _timeForClick = 0f;
        }

        private void HandleHover()
        {
            UnitRaycastTrigger unitUnderPointer = _raycaster.GetUnitTrigger(_camera.GetCamera());

            if (_currentHoverUnit != null && (unitUnderPointer != null && unitUnderPointer.ParentUnit != _currentHoverUnit || unitUnderPointer == null))
            {
                _currentHoverUnit.SetActiveOutline(false, false);
                _unitsSequencePanel.SetHighlightedSelected(_currentHoverUnit, false);
                _battleObserver.HideStatsInfo(_currentHoverUnit, false, false);
                _battleObserver.DeactivateDamageInfo(_currentHoverUnit);
                _currentHoverUnit = null;
            }
            else if (unitUnderPointer != null && _currentHoverUnit != unitUnderPointer.ParentUnit && !_input.IsWheelPressed())
            {
                _currentHoverUnit = unitUnderPointer.ParentUnit;
                _currentHoverUnit.SetActiveOutline(true, false);
                _unitsSequencePanel.SetHighlightedSelected(_currentHoverUnit, true);
                _battleObserver.HighlightUIStatsInfo(_currentHoverUnit, true);

                if (_unitWalkingResolver.GetCurrenUnit() != null && _unitWalkingResolver.IsChoosingAttackTarget())
                {
                    BaseUnit currentWalkingUnit = _unitWalkingResolver.GetCurrenUnit();
                    (float, float) damage = currentWalkingUnit.GetDecomposedAttackDamage();

                    if (BattleUtils.GetRelationForUnits(_currentHoverUnit.UnitType, Enums.UnitType.Player) == Enums.UnitRelation.Enemy)
                    {
                        _battleObserver.ActivateDamageInfo(_currentHoverUnit, damage);
                    }
                }
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

            if (_raycaster.IsPointerOverUI())
            {
                return;
            }

            if (_unitWalkingResolver.IsChoosingAttackTarget() && IsWalkingAndClickedOnEnemy() && IsPointerNearTarget())
            {
                bool isAttacking = _unitWalkingResolver.IsChoosingAttackTarget();
                bool isMoving = _unitWalkingResolver.IsChoosingWalkPoint();

                BaseUnit currentWalkingUnit = _unitWalkingResolver.GetCurrenUnit();
                BaseUnit currentHoverUnit = _currentHoverUnit;

                System.Action attackCallback = () => 
                {
                    ActionMeleeAttack actionInfo = new ActionMeleeAttack();
                    actionInfo.Actor = currentWalkingUnit;
                    actionInfo.Damage = currentWalkingUnit.GetAttackDamage();
                    actionInfo.SubjectUnits = new List<BaseUnit>() { currentHoverUnit };

                    _battleSM.Enter<UnitsActionState, ActionInfo>(actionInfo);
                };

                bool isNearEnemy = IsPointsAreNeighbours(
                    currentWalkingUnit.transform.position, 
                    _currentHoverUnit.transform.position);

                if (!isNearEnemy)
                {
                    if (IsLastWalkPointNearEnemy() && isAttacking)
                    {
                        MoveUnit(attackCallback);
                    }
                    else if (isMoving || isAttacking)
                    {
                        MoveUnit(null);
                    }
                }
                else
                {
                    if (isAttacking)
                    {
                        ResetWalkSelection();
                        attackCallback?.Invoke();
                    }
                }
            }
            else if (!_unitWalkingResolver.IsChoosingAttackTarget())
            {
                if (IsClickedOnFieldWithSelectedWalkingUnit() && _unitWalkingResolver.IsChoosingWalkPoint())
                {
                    if (IsPointingOnPossiblePointToMove())
                    {
                        MoveUnit(null);
                    }
                }
                else if (_currentHoverUnit != null && _unitWalkingResolver.GetCurrenUnit() != _currentHoverUnit)
                {
                    _walkFieldVisualizer.Hide();
                    _unitWalkingResolver.SetCurrentUnit(null, null);
                    _unitWalkingResolver.ClearOldHighlights();

                    List<NodeBase> nodesWalkingRange = ShowMovementRange();

                    bool isCurrentWalkingUnit = IsClickedOnPossibleWalkingUnit();

                    _canvasesResolver.GetCanvas<BattleCanvas>().GetPanelOfType<UnitManagementPanel>().FillInfo(_currentHoverUnit, isCurrentWalkingUnit);
                    _canvasesResolver.GetCanvas<BattleCanvas>().SetActivePanel<UnitManagementPanel>(true);
                    _unitWalkingResolver.SetCurrentUnit(_currentHoverUnit, nodesWalkingRange);
                }
            }
        }

        private void RMBUp()
        {
            StopCurrentAction();
        }

        private void StopCurrentAction()
        {
            if (_unitWalkingResolver.IsHaveSomeAction())
            {
                _unitWalkingResolver.StopCurrentAction();
                _canvasesResolver.GetCanvas<BattleCanvas>().GetPanelOfType<UnitManagementPanel>().RemoveSelection();
            }
            else
            {
                ResetWalkSelection();
                UnselectUnit();
            }
        }

        private void MoveUnit(System.Action callback)
        {
            UnitsMovementState.MovementData movementData = new UnitsMovementState.MovementData(
                _unitWalkingResolver.GetCurrenUnit(),
                _mapDataProvider.GetNearestNodeOfWorldPoint(_unitWalkingResolver.GetCurrenUnit().transform.position),
                _mapDataProvider.GetNearestNodeOfWorldPoint(_unitWalkingResolver.GetLastWalkPoint()),
                callback);

            ResetWalkSelection();

            _battleSM.Enter<UnitsMovementState, UnitsMovementState.MovementData>(movementData);
        }

        private List<NodeBase> ShowMovementRange()
        {
            BaseUnit unit = _currentHoverUnit;

            if (unit == null)
            {
                unit = _unitWalkingResolver.GetCurrenUnit();
            }

            NodeBase unitNode = _mapDataProvider.GetNearestNodeOfWorldPoint(unit.transform.position);
            List<NodeBase> nodesWalkingRange = HexPathfindingGrid.GetWalkRange(unitNode, unit.GetWalkRange());
            _walkFieldVisualizer.Show(
                unitNode,
                nodesWalkingRange);
            return nodesWalkingRange;
        }

        private bool IsPointingOnPossiblePointToMove()
        {
            return _mapDataProvider.GetNearestNodeOfWorldPoint(_unitWalkingResolver.GetCurrenUnit().transform.position) != _mapDataProvider.GetNearestNodeOfWorldPoint(_unitWalkingResolver.GetLastWalkPoint());
        }

        private bool IsPointerNearTarget()
        {
            return
                _currentHoverUnit != null &&
                BattleUtils.GetRelationForUnits(Enums.UnitType.Player, _currentHoverUnit.UnitType) == Enums.UnitRelation.Enemy &&
                (_mapDataProvider.GetNearestNodeOfWorldPoint(_currentHoverUnit.transform.position).Neighbours.Contains(
                    _mapDataProvider.GetNearestNodeOfWorldPoint(_unitWalkingResolver.GetLastWalkPoint())) ||
                _mapDataProvider.GetNearestNodeOfWorldPoint(_currentHoverUnit.transform.position) ==
                    _mapDataProvider.GetNearestNodeOfWorldPoint(_unitWalkingResolver.GetLastWalkPoint()));
        }

        private bool IsWalkingAndClickedOnEnemy()
        {
            return 
                _unitWalkingResolver.IsHaveUnitToWalk() && _currentHoverUnit != null &&
                BattleUtils.GetRelationForUnits(Enums.UnitType.Player, _currentHoverUnit.UnitType) == Enums.UnitRelation.Enemy &&
                _timeForClick <= 1f;
        }

        private bool IsClickedOnPossibleWalkingUnit()
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
                (_currentHoverUnit == null ||
                BattleUtils.GetRelationForUnits(Enums.UnitType.Player, _currentHoverUnit.UnitType) != Enums.UnitRelation.Enemy) && 
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
            _walkFieldVisualizer.Hide();
            _unitWalkingResolver.ResetMoving();
        }

        private void UnselectUnit()
        {
            _unitWalkingResolver.SetCurrentUnit(null, null);
            _canvasesResolver.GetCanvas<BattleCanvas>().SetActivePanel<UnitManagementPanel>(false);
        }

        private void OnSkillPressed(UnitBaseSkill skill)
        {
            if (skill is UnitDefaultWalkSkill)
            {
                _unitWalkingResolver.SwitchCurrentUnitWalk();
            }
            else if ((skill as UnitAttackBaseSkill) != null && (skill as UnitAttackBaseSkill).IsAttackOnOneUnit)
            {
                _unitWalkingResolver.SwitchAttackOnTarget();
            }
        }
    }
}