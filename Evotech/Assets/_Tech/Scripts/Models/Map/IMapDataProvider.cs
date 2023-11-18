using Hexnav.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Data
{
    public interface IMapDataProvider
    {
        (Vector3, Vector3) GetBorders();
        NodeBase GetNearestNodeOfWorldPoint(Vector3 point);
        float GetHeightOfWorldPoint(Vector3 point);
        float GetHeightDistanceMultiplier();
        List<NodeBase> GetNodes();
        NodeBase GetNodeOfPosition(Vector3 pos);
    }
}