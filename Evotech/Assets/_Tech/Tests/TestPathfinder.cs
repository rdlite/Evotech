using Core.Data;
using Extensions;
using Hexnav.Core;
using Qnject;
using System.Collections.Generic;
using UnityEngine;

public class TestPathfinder : MonoBehaviour
{
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;

    [Inject] private IMapDataProvider _mapProvider;

    private List<NodeBase> path;

    private void Start()
    {
        path = new List<NodeBase>();
    }

    private void Update()
    {
        if (_mapProvider != null)
        {
            NodeBase startNode = _mapProvider.GetNodeOfWorldPoint(_startPoint.position);
            NodeBase targetNode = _mapProvider.GetNodeOfWorldPoint(_endPoint.position);

            if (startNode != null && targetNode != null)
            {
                _startPoint.transform.position = _startPoint.transform.position.FlatY() + Vector3.up * _mapProvider.GetHeightOfWorldPoint(_startPoint.position) + Vector3.up;
                _endPoint.transform.position = _endPoint.transform.position.FlatY() + Vector3.up * _mapProvider.GetHeightOfWorldPoint(_endPoint.position) + Vector3.up;

                path = HexPathfinfing.FindPath(startNode, targetNode);
                //Debug.Log(path.Count);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null && path.Count != 0)
        {
            Gizmos.color = Color.green;
            foreach (var pathPoint in path)
            {
                Gizmos.DrawWireSphere(pathPoint.WorldPos + Vector3.up, .4f);
            }
        }
    }
}
