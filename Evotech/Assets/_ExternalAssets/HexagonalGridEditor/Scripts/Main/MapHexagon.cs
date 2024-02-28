using UnityEditor;
using UnityEngine;

namespace HexEditor 
{
    public class MapHexagon : MonoBehaviour
    {
        [SerializeField] private Transform _obstaclesContainer;
        [SerializeField] private GameObject _tagPoint;

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
                Undo.DestroyObjectImmediate(_obstaclesContainer.GetChild(_obstaclesContainer.childCount - 1).gameObject);
            }
        }

        public void SetTag(string tagName, Texture2D texture)
        {
            EditorGUIUtility.SetIconForObject(_tagPoint.gameObject, texture);
            _tagPoint.gameObject.name = tagName;
        }
    }
}