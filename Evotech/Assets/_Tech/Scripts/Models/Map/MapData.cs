using Hexnav.Core;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Core.Data
{
    public struct MapData
    {
        public List<NodeBase> Nodes;

        public MapData(List<NodeBase> nodes)
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

        public NodeBase GetNodeByWPos(Vector3 pos)
        {
            Vector2Int hexID = HexGridUtility.ConvertWorldPointToGridID(pos);
            return GetNodeByID(hexID);
        }

        public NodeBase GetNodeByID(Vector2Int id)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Point == id)
                {
                    return Nodes[i];
                }
            }

            return null;
        }
    }
}