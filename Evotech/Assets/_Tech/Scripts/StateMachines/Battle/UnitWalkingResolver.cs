using Core.Battle;
using Core.Cameras;
using Core.Units;
using Extensions;
using Utils;
using Hexnav.Core;
using UnityEngine;
using System.Collections.Generic;

public class UnitWalkingResolver
{
    private List<NodeBase> _nodesToWalk;
    private BaseUnit _currentUnit;
    private IWalkFieldVisualizer _walkFieldVisualizer;
    private IRaycaster _raycaster;
    private CameraController _camera;
    private Vector3 _lastRaycastPos;
    private Vector3 _lastEndPoint;

    public UnitWalkingResolver(
        IRaycaster raycaster, CameraController camera, IWalkFieldVisualizer walkFieldVisualizer)
    {
        _walkFieldVisualizer = walkFieldVisualizer;
        _raycaster = raycaster;
        _camera = camera;
    }

    public void SetCurrentWalkingUnit(BaseUnit unit, List<NodeBase> nodesToWalk)
    {
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
            _walkFieldVisualizer.ProcessPathScale(path);
        }

        if (_currentUnit != null)
        {
            RotateUnitToPointer(_lastEndPoint);
        }
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

    private void RotateUnitToPointer(Vector3 endPoint)
    {
        Vector3 lookDirection = (endPoint - _currentUnit.transform.position).FlatY().normalized;

        if (lookDirection != Vector3.zero)
        {
            float yRotation = Quaternion.LookRotation(lookDirection, Vector3.up).eulerAngles.y;
            yRotation = (int)yRotation / 60;
            yRotation = (int)yRotation * 60;

            _currentUnit.transform.rotation =
                Quaternion.Slerp(
                    _currentUnit.transform.rotation,
                    Quaternion.Euler(_currentUnit.transform.rotation.eulerAngles.x, yRotation, _currentUnit.transform.rotation.eulerAngles.z),
                    10f * Time.deltaTime);
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