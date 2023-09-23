using UnityEngine;

namespace Core.Data
{
    public class MapDataProvider : IMapDataProvider
    {
        public MapData MapData { get; private set; }

        private (Vector3 LD, Vector3 RU) _borders;

        public void Init(MapData mapData)
        {
            MapData = mapData;
            _borders = mapData.CalculateMapBorders();
        }

        public (Vector3, Vector3) GetBorders()
        {
            return _borders;
        }
    }
}