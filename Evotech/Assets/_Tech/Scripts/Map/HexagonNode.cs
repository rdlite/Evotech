using UnityEngine;

namespace Core.Map
{
    public class HexagonNode : MonoBehaviour
    {
        [SerializeField] private Transform _obstaclesContainer;

        public Transform GetObstaclesContainer()
        {
            return _obstaclesContainer;
        }
    }
}