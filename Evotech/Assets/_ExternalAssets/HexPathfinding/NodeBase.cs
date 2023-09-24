using System.Collections.Generic;
using UnityEngine;

namespace Hexnav.Core
{
    public class NodeBase
    {
        public List<NodeBase> Neighbours { get; private set; }
        public NodeBase Connection { get; private set; }
        public float G { get; private set; }
        public float H { get; private set; }
        public float F => G + H;
        public Vector3 WorldPos;
        public Vector2Int Point;
        public float Height;

        public NodeBase(Vector3 worldPos, Vector2Int point, float height)
        {
            WorldPos = worldPos;
            Point = point;
            Height = height;
            Neighbours = new List<NodeBase>();
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
        }

        public void SetH(float h)
        {
            H = h;
        }
    }
}