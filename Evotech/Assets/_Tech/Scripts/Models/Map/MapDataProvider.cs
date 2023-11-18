using Core.Settings;
using Extensions;
using Hexnav.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Data
{
    public class MapDataProvider : IMapDataProvider
    {
        public MapData MapData { get; private set; }

        private (Vector3 LD, Vector3 RU) _borders;
        private MapSettings _mapSettings;
        private HexPathfindingGrid _pathfindingGrid;

        public void Init(MapData mapData, MapSettings mapSettings)
        {
            MapData = mapData;
            _borders = mapData.CalculateMapBorders();
            _mapSettings = mapSettings;
            _pathfindingGrid = new HexPathfindingGrid(this);
            _pathfindingGrid.LoadNodesArray(MapData.Nodes.ToArray());
        }

        public (Vector3, Vector3) GetBorders()
        {
            return _borders;
        }

        public NodeBase GetNearestNodeOfWorldPoint(Vector3 point)
        {
            NodeBase node = MapData.GetNodeByWPos(point);

            if (node != null)
            {
                return node;
            }
            else
            {
                float nearestDistance = Mathf.Infinity;
                NodeBase nearestNode = null;

                for (int i = 0; i < MapData.Nodes.Count; i++)
                {
                    float distance = Vector3.Distance(point.FlatY(), MapData.Nodes[i].WorldPos.FlatY());

                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestNode = MapData.Nodes[i];
                    }
                }

                return nearestNode;
            }
        }

        public float GetHeightOfWorldPoint(Vector3 point)
        {
            NodeBase node = MapData.GetNodeByWPos(point);

            if (node != null)
            {
                return node.Height * _mapSettings.HeightOffset;
            }
            else
            {
                return 0f;
            }
        }

        public NodeBase GetNodeOfPosition(Vector3 point)
        {
            return MapData.GetNodeByWPos(point);
        }

        public float GetHeightDistanceMultiplier()
        {
            return _mapSettings.HeightDistanceMultiplier;
        }

        public List<NodeBase> GetNodes()
        {
            return new List<NodeBase>(MapData.Nodes);
        }
    }
}