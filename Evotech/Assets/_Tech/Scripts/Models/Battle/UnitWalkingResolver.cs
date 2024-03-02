using Utils;
using System;
using Core.Data;
using Core.Units;
using Extensions;
using Hexnav.Core;
using UnityEngine;
using Core.Battle;
using Core.Cameras;
using Utils.Battle;
using Core.UI.Elements;
using System.Collections.Generic;

public class UnitWalkingResolver
{
    private List<NodeBase> _nodesToWalk;
    private BaseUnit _currentUnit;
    private BaseUnit _currentHighlightedUnit;
    private BaseUnit _hoverUnit;
    private IBattleLinesFactory _battleLinesFactory;
    private IMapDataProvider _mapDataProvider;
    private IWalkFieldVisualizer _walkFieldVisualizer;
    private UnitsSequencePanel _unitsSequencePanel;
    private IRaycaster _raycaster;
    private CameraController _camera;
    private Vector3 _lastRaycastPos;
    private Vector3 _lastEndPoint;
    private GhostCopy _currentWalkingUnitGhost;
    private bool _isCreatedLines;
    private bool _isChoosingWalkPoint;
    private bool _isChoosingAttackTarget;

    public UnitWalkingResolver(
        IRaycaster raycaster, CameraController camera, IWalkFieldVisualizer walkFieldVisualizer,
        IBattleLinesFactory battleLinesFactory, IMapDataProvider mapDataProvider, UnitsSequencePanel unitsSequencePanel)
    {
        _walkFieldVisualizer = walkFieldVisualizer;
        _unitsSequencePanel = unitsSequencePanel;
        _raycaster = raycaster;
        _camera = camera;
        _battleLinesFactory = battleLinesFactory;
        _mapDataProvider = mapDataProvider;
    }

    public void SetCurrentUnit(BaseUnit unit, List<NodeBase> nodesToWalk)
    {
        if (unit == null && _currentUnit != null)
        {
            _isChoosingWalkPoint = false;
            _isChoosingAttackTarget = false;

            _currentUnit.SetActiveOutline(false, true);
            _currentUnit.SetAttackPreparationAnimation(false);
            _unitsSequencePanel.SetHighlightedPulsate(_currentUnit, false);

            ClearOldHighlights();
            SetUnitRotationTarget(Vector3.zero);
            ClearGhost();
            ClearAllLines();
        }

        _currentUnit = unit;
        _nodesToWalk = nodesToWalk;

        if (_currentUnit != null)
        {
            _unitsSequencePanel.SetHighlightedPulsate(_currentUnit, true);
            _currentUnit.SetActiveOutline(true, true);
        }
    }

    public void ResetMoving()
    {
        if (_currentUnit != null)
        {
            _isChoosingWalkPoint = false;
            _isChoosingAttackTarget = false;

            _currentUnit.SetAttackPreparationAnimation(false);

            ClearOldHighlights();
            SetUnitRotationTarget(Vector3.zero);
            ClearGhost();
            ClearAllLines();
        }
    }

    public void ClearOldHighlights()
    {
        if (_currentHighlightedUnit != null)
        {
            _currentHighlightedUnit.SetActiveOutline(false, true);
        }
    }

    public void SetHighlightedUnit(BaseUnit unit)
    {
        _currentHighlightedUnit = unit;
        unit.SetActiveOutline(true, true);
    }

    public void SwitchCurrentUnitWalk()
    {
        _isChoosingWalkPoint = !_isChoosingWalkPoint;
        _isChoosingAttackTarget = false;
        _currentUnit.SetAttackPreparationAnimation(false);
    }

    public void SwitchAttackOnTarget()
    {
        _isChoosingWalkPoint = false;
        _isChoosingAttackTarget = !_isChoosingAttackTarget;
        _currentUnit.SetAttackPreparationAnimation(_isChoosingAttackTarget);
    }

    public void StopCurrentAction()
    {
        _walkFieldVisualizer.ProcessPathScale(null);

        if (_isChoosingWalkPoint)
        {
            SwitchCurrentUnitWalk();
        }
        else if (_isChoosingAttackTarget)
        {
            SwitchAttackOnTarget();
        }
    }

    public void Update()
    {
        int pathCount = 0;

        if (IsHaveSomeAction() && _currentUnit != null && _nodesToWalk != null && _nodesToWalk.Count > 0)
        {
            Vector3 pointToSearch = Vector3.zero;

            if (_raycaster.GetGroundPos(_camera.GetCamera(), out Vector3 groundPos))
            {
                _lastRaycastPos = groundPos;
                pointToSearch = groundPos;
            }
            else
            {
                pointToSearch = _lastRaycastPos;
            }

            Vector3 startPoint = _currentUnit.transform.position;
            Vector3 endPoint = GetNearestPointOfWalkableField(pointToSearch);

            if (startPoint != endPoint)
            {
                _lastEndPoint = endPoint;
            }

            List<NodeBase> path = HexPathfindingGrid.SetDesination(startPoint, endPoint);

            bool isPointerOverUI = _raycaster.IsPointerOverUI();

            if (path != null)
            {
                if (isPointerOverUI)
                {
                    path.Clear();
                }

                pathCount = path.Count;
            }

            if (!isPointerOverUI && _hoverUnit != null && _currentUnit != _hoverUnit && BattleUtils.GetRelationForUnits(_currentUnit.UnitType, _hoverUnit.UnitType) == Enums.UnitRelation.Enemy)
            {
                int distanceBetweenUnitsInNodes = HexPathfindingGrid.GetDistanceBetweenPointsInNodes(_currentUnit.transform.position, _hoverUnit.transform.position);

                if (_isChoosingAttackTarget && distanceBetweenUnitsInNodes < 2)
                {
                    _lastEndPoint = _hoverUnit.transform.position;
                    _walkFieldVisualizer.ProcessPathScale(null);

                    ShowAttackLine(_currentUnit.transform.position, _hoverUnit.transform.position);
                    ClearGhost();
                }
                else if (path[path.Count - 1].Neighbours.Contains(_mapDataProvider.GetNearestNodeOfWorldPoint(_hoverUnit.transform.position)))
                {
                    if (_isChoosingAttackTarget)
                    {
                        CreateGhost();

                        _currentWalkingUnitGhost.transform.position = path[path.Count - 1].WorldPos + path[path.Count - 1].SurfaceOffset;

                        Vector3 targetLook = (_hoverUnit.transform.position - _currentWalkingUnitGhost.transform.position).FlatY().normalized;

                        if (targetLook != Vector3.zero)
                        {
                            _currentWalkingUnitGhost.transform.rotation =
                                Quaternion.LookRotation(targetLook, Vector3.up);
                        }
                    }

                    _walkFieldVisualizer.ProcessPathScale(path);

                    if (_isChoosingAttackTarget)
                    {
                        ClearAllLines();
                        ShowAttackLine(_currentWalkingUnitGhost.transform.position, _hoverUnit.transform.position);
                    }
                }
            }
            else if (_isChoosingWalkPoint)
            {
                _walkFieldVisualizer.ProcessPathScale(path);

                ClearAllLines();
                ClearGhost();
            }
            else
            {
                if (path != null)
                {
                    path.Clear();
                }
                pathCount = 0;
                _walkFieldVisualizer.ProcessPathScale(null);
                ClearAllLines();
                ClearGhost();
            }
        }

        if (_currentUnit != null && pathCount != 0)
        {
            SetUnitRotationTarget(_lastEndPoint);
        }
    }

    public void SetHoverUnit(BaseUnit currentHoverUnit)
    {
        _hoverUnit = currentHoverUnit;
    }

    public void ReleaseHoveredUnit()
    {
        _hoverUnit = null;
    }

    private Vector3 GetNearestPointOfWalkableField(Vector3 point)
    {
        Vector3 nearestPoint = _nodesToWalk[0].WorldPos;
        float nearestDist = Vector3.Distance(_nodesToWalk[0].WorldPos.FlatY(), point.FlatY());

        for (int i = 1; i < _nodesToWalk.Count; i++)
        {
            float distToPoint = Vector3.Distance(_nodesToWalk[i].WorldPos.FlatY(), point.FlatY());

            if (distToPoint < nearestDist)
            {
                nearestDist = distToPoint;
                nearestPoint = _nodesToWalk[i].WorldPos;
            }
        }

        return nearestPoint;
    }

    private void SetUnitRotationTarget(Vector3 endPoint)
    {
        _currentUnit.SetTargetRotation(endPoint);
    }

    private void CreateGhost()
    {
        if (_currentWalkingUnitGhost == null)
        {
            _currentWalkingUnitGhost = _currentUnit.CreateGhostCopy();
        }
    }

    private void ClearGhost()
    {
        if (_currentWalkingUnitGhost != null)
        {
            _currentWalkingUnitGhost.Destroy();
            _currentWalkingUnitGhost = null;
        }
    }

    private void ShowAttackLine(Vector3 start, Vector3 end)
    {
        if (!_isCreatedLines)
        {
            _isCreatedLines = true;
            _battleLinesFactory.CreateLine(start + Vector3.up * 2.4f, end + Vector3.up * 2.4f, Enums.PointedLineType.Attack);
        }
    }

    private void ClearAllLines()
    {
        if (_isCreatedLines)
        {
            _isCreatedLines = false;
            _battleLinesFactory.ClearLines();
        }
    }

    public bool IsHaveUnitToWalk()
    {
        return _currentUnit != null;
    }

    public bool IsChoosingWalkPoint()
    {
        return _isChoosingWalkPoint || _isChoosingAttackTarget;
    }

    public bool IsChoosingAttackTarget()
    {
        return _isChoosingAttackTarget;
    }

    public bool IsHaveSomeAction()
    {
        return _isChoosingAttackTarget || _isChoosingWalkPoint;
    }

    public BaseUnit GetCurrenUnit()
    {
        return _currentUnit;
    }

    public bool IsUnitHighlighted(BaseUnit unit)
    {
        return _currentHighlightedUnit == unit;
    }

    public Vector3 GetLastWalkPoint()
    {
        return _lastEndPoint;
    }
}