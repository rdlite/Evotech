using Utils;
using Core.Data;
using Core.Units;
using Extensions;
using Hexnav.Core;
using UnityEngine;
using Core.Battle;
using Core.Cameras;
using Utils.Battle;
using System.Collections.Generic;

public class UnitWalkingResolver
{
    private List<NodeBase> _nodesToWalk;
    private BaseUnit _currentUnit;
    private BaseUnit _hoverUnit;
    private IBattleLinesFactory _battleLinesFactory;
    private IWalkFieldVisualizer _walkFieldVisualizer;
    private IRaycaster _raycaster;
    private CameraController _camera;
    private Vector3 _lastRaycastPos;
    private Vector3 _lastEndPoint;
    private GhostCopy _currentWalkingUnitGhost;
    private bool _isCurrentlyPointingOnEnemy;
    private bool _isCreatedLines;

    public UnitWalkingResolver(
        IRaycaster raycaster, CameraController camera, IWalkFieldVisualizer walkFieldVisualizer,
        IBattleLinesFactory battleLinesFactory)
    {
        _walkFieldVisualizer = walkFieldVisualizer;
        _raycaster = raycaster;
        _camera = camera;
        _battleLinesFactory = battleLinesFactory;
    }

    public void SetCurrentWalkingUnit(BaseUnit unit, List<NodeBase> nodesToWalk)
    {
        if (unit == null && _currentUnit != null)
        {
            SetUnitRotationTarget(Vector3.zero, true);
            ClearGhost();
            ClearAllLines();
        }

        _currentUnit = unit;
        _nodesToWalk = nodesToWalk;
    }

    public void Update()
    {
        if (_currentUnit != null && _nodesToWalk != null && _nodesToWalk.Count > 0)
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

            if (_hoverUnit != null && _currentUnit != _hoverUnit)
            {
                int distanceBetweenUtinsInNodes = HexPathfindingGrid.GetDistanceBetweenPointsInNodes(_currentUnit.transform.position, _hoverUnit.transform.position);

                if (distanceBetweenUtinsInNodes < 2)
                {
                    _isCurrentlyPointingOnEnemy = BattleUtils.GetRelationForUnits(Enums.UnitType.Player, _hoverUnit.UnitType) == Enums.UnitRelation.Enemy;
                    _lastEndPoint = _hoverUnit.transform.position;
                    _walkFieldVisualizer.ProcessPathScale(null);

                    ShowAttackLine(_currentUnit.transform.position, _hoverUnit.transform.position);
                    ClearGhost();
                }
                else
                {
                    CreateGhost();

                    _currentWalkingUnitGhost.transform.position = path[path.Count - 1].WorldPos + path[path.Count - 1].SurfaceOffset;

                    Vector3 targetLook = (_hoverUnit.transform.position - _currentWalkingUnitGhost.transform.position).FlatY().normalized;

                    if (targetLook != Vector3.zero)
                    {
                        _currentWalkingUnitGhost.transform.rotation =
                            Quaternion.LookRotation(targetLook, Vector3.up);
                    }

                    _walkFieldVisualizer.ProcessPathScale(path);

                    ClearAllLines();
                    ShowAttackLine(_currentWalkingUnitGhost.transform.position, _hoverUnit.transform.position);
                }
            }
            else
            {
                _isCurrentlyPointingOnEnemy = false;
                _walkFieldVisualizer.ProcessPathScale(path);

                ClearAllLines();
                ClearGhost();
            }
        }

        if (_currentUnit != null)
        {
            SetUnitRotationTarget(_lastEndPoint, _isCurrentlyPointingOnEnemy);
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

    private void SetUnitRotationTarget(Vector3 endPoint, bool isRotateWithChildFigure)
    {
        _currentUnit.SetTargetRotation(endPoint, isRotateWithChildFigure);
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

    public BaseUnit GetCurrenUnit()
    {
        return _currentUnit;
    }

    public Vector3 GetLastWalkPoint()
    {
        return _lastEndPoint;
    }
}