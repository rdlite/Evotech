using Core.Data;
using Extensions;
using Hexnav.Core;
using Qnject;
using System.Collections.Generic;
using UnityEngine;

public class WalkFieldCreationTest : MonoBehaviour
{
    [SerializeField] private Transform _startPoint;

    [Inject] private IMapDataProvider _mapProvider;

    private List<NodeBase> possibleWalkNodes;

    private void Start()
    {
        possibleWalkNodes = new List<NodeBase>();
    }

    private void Update()
    {
        if (_mapProvider != null)
        {
            _startPoint.transform.position = _startPoint.transform.position.FlatY() + Vector3.up * _mapProvider.GetHeightOfWorldPoint(_startPoint.position) + Vector3.up;
        }
    }

    private void OnDrawGizmos()
    {
        if (possibleWalkNodes != null && possibleWalkNodes.Count != 0)
        {
            Gizmos.color = Color.green;
            foreach (var pathPoint in possibleWalkNodes)
            {
                Gizmos.DrawWireSphere(pathPoint.WorldPos + Vector3.up, .4f);
            }
        }
    }
}
