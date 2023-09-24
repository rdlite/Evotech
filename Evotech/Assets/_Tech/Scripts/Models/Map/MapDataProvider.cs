using Core.Settings;
using Hexnav.Core;
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
            _pathfindingGrid = new HexPathfindingGrid();
            _pathfindingGrid.LoadNodesArray(MapData.Nodes.ToArray());
        }

        public (Vector3, Vector3) GetBorders()
        {
            return _borders;
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
    }
}