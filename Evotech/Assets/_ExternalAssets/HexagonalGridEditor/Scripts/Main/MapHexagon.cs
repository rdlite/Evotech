using UnityEditor;
using UnityEngine;

namespace HexEditor 
{
    public class MapHexagon : MonoBehaviour
    {
        [SerializeField] private Transform _obstaclesContainer;

        public void AddObstacle(Transform obstacle)
        {
            obstacle.SetParent(_obstaclesContainer);
            obstacle.transform.localPosition = Vector3.zero + new Vector3(Random.insideUnitCircle.x * .5f, 0f, Random.insideUnitCircle.y * .5f);
        }

        public void RemoveLastObstacle(bool withUndoRegister)
        {
            if (!withUndoRegister)
            {
                DestroyImmediate(_obstaclesContainer.GetChild(_obstaclesContainer.childCount - 1).gameObject);
            }
            else
            {
                Undo.DestroyObjectImmediate(_obstaclesContainer.GetChild(_obstaclesContainer.childCount - 1).gameObject);
            }
        }
    }
}