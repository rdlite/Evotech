using System.Collections.Generic;
using UnityEngine;

namespace Hexnav.Core
{
    public class NodeBase
    {
        public List<NodeBase> Neighbours { get; private set; }
        public NodeBase Connection { get; private set; }
        public List<string> Tags { get; private set; }
        public List<string> ObstacleNames { get; private set; }
        public Transform WorldObject { get; }
        public Vector3 SurfaceOffset { get; }

        public float G;
        public float H;
        public float F;
        public Vector3 WorldPos;
        public Vector2Int Point;
        public float Height;
        public bool IsWalkable;

        private float _g, _h;

        public NodeBase(
            Vector3 worldPos, Vector2Int point, float height,
            Transform worldRepresent, Vector3 surfaceOffset, List<string> tags, 
            List<string> obstacleNames, bool isWalkable)
        {
            WorldPos = worldPos;
            Point = point;
            Height = height;
            Neighbours = new List<NodeBase>();
            IsWalkable = isWalkable;
            WorldObject = worldRepresent;
            SurfaceOffset = surfaceOffset;
            Tags = tags;
            ObstacleNames = obstacleNames;
        }

        public void SetConnection(NodeBase nodeBase)
        {
            Connection = nodeBase;
        }

        public void AddNeightbour(NodeBase neighbour)
        {
            Neighbours.Add(neighbour);
        }

        public void SetG(float g)
        {
            G = g;
            _g = g;
            F = g + _h;
        }

        public void SetH(float h)
        {
            H = h;
            _h = h;
            F = h + _g;
        }

        public float GetDistance(NodeBase node)
        {
            return Vector3.Distance(node.WorldPos, WorldPos);
        }
    }
}