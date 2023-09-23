using UnityEngine;

namespace Core.Data
{
    public interface IMapDataProvider
    {
        (Vector3, Vector3) GetBorders();
    }
}