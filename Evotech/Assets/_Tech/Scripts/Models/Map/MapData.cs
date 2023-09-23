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

        public (Vector3 LD, Vector3 RU) CalculateMapBorders()
        {
            (Vector3 LD, Vector3 RU) borders;

            float minX = Mathf.Infinity, minZ = Mathf.Infinity, maxX = -Mathf.Infinity, maxZ = -Mathf.Infinity;

            for (int i = 0; i < Nodes.Count; i++)
            {
                Vector3 nodeWPos = Nodes[i].WorldPos;

                if (minX > nodeWPos.x)
                {
                    minX = nodeWPos.x;
                }

                if (minZ > nodeWPos.z)
                {
                    minZ = nodeWPos.z;
                }

                if (maxX < nodeWPos.x)
                {
                    maxX = nodeWPos.x;
                }

                if (maxZ < nodeWPos.z)
                {
                    maxZ = nodeWPos.z;
                }
            }

            borders.LD = new Vector3(minX, 0f, minZ);
            borders.RU = new Vector3(maxX, 0f, maxZ);

            return borders;
        }
    }

    public struct Node
    {
        public Vector3 WorldPos;
        public Vector2Int Point;

        public Node(Vector3 worldPos, Vector2Int point)
        {
            WorldPos = worldPos;
            Point = point;
        }
    }
}