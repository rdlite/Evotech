using Core.Battle;
using Core.Cameras;
using Core.Units;
using Extensions;
using Hexnav.Core;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class UnitWalkingResolver
{
    private List<NodeBase> _nodesToWalk;
    private BaseUnit _currentUnit;
    private IWalkFieldVisualizer _walkFieldVisualizer;
    private IRaycaster _raycaster;
    private CameraController _camera;
    private Vector3 _lastRaycastPos;

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

            List<NodeBase> path = HexPathfindingGrid.SetDesination(startPoint, endPoint);
            _walkFieldVisualizer.ProcessPathScale(path);
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
}