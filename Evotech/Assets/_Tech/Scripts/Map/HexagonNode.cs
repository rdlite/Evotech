using UnityEditor;
using UnityEngine;

namespace Core.Map
{
    public class HexagonNode : MonoBehaviour
    {
        [SerializeField] private Transform _obstaclesContainer;
        [SerializeField] private GameObject _tagPoint;

        public Vector3 GetSurfaceOffset()
        {
            return _obstaclesContainer.transform.position - transform.position;
        }

        public Transform GetObstaclesContainer()
        {
            return _obstaclesContainer;
        }

        public void AddObstacle(Transform obstacle, bool withRandom)
        {
            obstacle.SetParent(_obstaclesContainer);
            if (withRandom)
            {
                obstacle.transform.localPosition = Vector3.zero + new Vector3(Random.insideUnitCircle.x * .5f, 0f, Random.insideUnitCircle.y * .5f);
            }
        }

        public void RemoveLastObstacle(bool withUndoRegister)
        {
            if (!withUndoRegister)
            {
                DestroyImmediate(_obstaclesContainer.GetChild(_obstaclesContainer.childCount - 1).gameObject);
            }
            else
            {
#if UNITY_EDITOR
                Undo.DestroyObjectImmediate(_obstaclesContainer.GetChild(_obstaclesContainer.childCount - 1).gameObject);
#endif
            }
        }

        public void SetTag(string tagName, Texture2D texture)
        {
#if UNITY_EDITOR
            EditorGUIUtility.SetIconForObject(_tagPoint.gameObject, texture);
            _tagPoint.gameObject.name = tagName;
#endif
        }
    }
}