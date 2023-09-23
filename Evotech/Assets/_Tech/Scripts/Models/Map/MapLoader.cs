using UnityEngine;

namespace Core.Data
{
    public abstract class MapLoader
    {
        public abstract MapData LoadMap(string jsonData, Vector3 point);
    }
}