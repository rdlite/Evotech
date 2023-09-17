using UnityEditor;
using UnityEngine;

namespace HexEditor
{
    public class SceneHexGridEditor : MonoBehaviour
    {
        [field: SerializeField] public int RectangularGridSize { get; private set; } = 100;

        [SerializeField] private Transform _hexesPrefabsContainer;
        [HideInInspector, SerializeField] private HexPlaceInfo[,] _hexPlaces;
        [HideInInspector, SerializeField] private GameObject _currentPrefab;

        public void PinCurrentPrefab(GameObject prefab)
        {
            _currentPrefab = prefab;
        }

        public void ClearAndGenerateGrid(Vector2Int size)
        {
            ClearField();

            for (int x = -size.x / 2; x <= size.x / 2; x++)
            {
                for (int y = -size.y / 2; y <= size.y / 2; y++)
                {
                    GameObject createdPrefab = PlaceHexGroundAtPoint(new Vector2Int(x, y), false);
                }
            }
        }

        public GameObject PlaceHexGroundAtPoint(Vector2Int point, bool withUndoRegister)
        {
            if (GetHexInfoByAbsoleteCoord(point).HexObject == null)
            {
                GameObject createdPrefab = PrefabUtility.InstantiatePrefab(_currentPrefab) as GameObject;
                createdPrefab.transform.SetParent(_hexesPrefabsContainer);
                createdPrefab.transform.position = HexGridUtility.ConvertHexCoordToWorldPoint(point);

                if (withUndoRegister)
                {
                    Undo.RegisterCreatedObjectUndo(createdPrefab, "Create object");
                }

                GetHexInfoByAbsoleteCoord(point).HexObject = createdPrefab;

                return createdPrefab;
            }
            else
            {
                return GetHexInfoByAbsoleteCoord(point).HexObject;
            }
        }


        public void TryRemoveObjectFromPoint(Vector2Int point, bool withUndoRegister)
        {
            if (GetHexInfoByAbsoleteCoord(point).HexObject != null)
            {
                if (withUndoRegister)
                {
                    Undo.DestroyObjectImmediate(GetHexInfoByAbsoleteCoord(point).HexObject);
                }
                else
                {
                    DestroyImmediate(GetHexInfoByAbsoleteCoord(point).HexObject);
                }

                GetHexInfoByAbsoleteCoord(point).HexObject = null;
            }
            else
            {
                return;
            }
        }

        public void ClearField()
        {
            int childCount = _hexesPrefabsContainer.childCount;
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(_hexesPrefabsContainer.GetChild(0).gameObject);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_hexesPrefabsContainer.transform.position != Vector3.zero)
            {
                _hexesPrefabsContainer.transform.position = Vector3.zero;
            }

            if (_hexPlaces == null || (_hexPlaces.GetLength(0) != RectangularGridSize))
            {
                ClearField();
                _hexPlaces = new HexPlaceInfo[RectangularGridSize, RectangularGridSize];
                for (int x = 0; x < RectangularGridSize; x++)
                {
                    for (int y = 0; y < RectangularGridSize; y++)
                    {
                        _hexPlaces[x, y] = new HexPlaceInfo();
                    }
                }
            }
        }
#endif

        private HexPlaceInfo GetHexInfoByAbsoleteCoord(Vector2Int notAbsoleteCoord)
        {
            return _hexPlaces[notAbsoleteCoord.x + RectangularGridSize / 2, notAbsoleteCoord.y + RectangularGridSize / 2];
        }

        [System.Serializable]
        private class HexPlaceInfo
        {
            public GameObject HexObject;
        }
    }
}