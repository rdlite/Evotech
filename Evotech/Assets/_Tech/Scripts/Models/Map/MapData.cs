using System.Collections.Generic;
using UnityEngine;

namespace Core.Data
{
    public struct MapData
    {
        public List<Node> Nodes;

        public MapData(List<Node> nodes)
        {
            Nodes = nodes;
        }
    }

    public struct Node
    {
        public Vector3 WorldPos;
        public Vector2Int Point;
    }
}