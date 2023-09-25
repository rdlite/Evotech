using Hexnav.Core;
using UnityEngine;

namespace Core.Data
{
    public interface IMapDataProvider
    {
        (Vector3, Vector3) GetBorders();
        NodeBase GetNearestNodeOfWorldPoint(Vector3 point);
        float GetHeightOfWorldPoint(Vector3 point);
        float GetHeightDistanceMultiplier();
    }
}