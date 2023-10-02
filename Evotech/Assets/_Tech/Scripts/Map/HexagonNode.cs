using UnityEngine;

namespace Core.Map
{
    public class HexagonNode : MonoBehaviour
    {
        [SerializeField] private Transform _obstaclesContainer;

        public Vector3 GetSurfaceOffset()
        {
            return _obstaclesContainer.transform.position - transform.position;
        }

        public Transform GetObstaclesContainer()
        {
            return _obstaclesContainer;
        }
    }
}