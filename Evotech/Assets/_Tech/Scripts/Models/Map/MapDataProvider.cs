namespace Core.Data
{
    public class MapDataProvider : IMapProvider
    {
        public MapData MapData { get; private set; }

        public void Init(MapData mapData)
        {
            MapData = mapData;
        }
    }
}