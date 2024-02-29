using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Core.Map;

namespace HexEditor
{
    [ExecuteInEditMode]
    public class SceneHexGridEditor : MonoBehaviour
    {
        [field: SerializeField] public int RectangularGridSize { get; private set; } = 100;

        [SerializeField] private Transform _hexesPrefabsContainer;
        [SerializeField] private float _heightStep = .3f;
        [HideInInspector, SerializeField] private HexPlaceInfo[] _hexPlaces;
        [HideInInspector, SerializeField] private GameObject _currentPrefab;

        public void PinCurrentPrefab(GameObject prefab)
        {
            _currentPrefab = prefab;

            if (_hexPlaces == null || (_hexPlaces.Length != (RectangularGridSize * RectangularGridSize)))
            {
                ClearField();
            }
        }

        public void ClearAndGenerateGrid(Vector2Int size)
        {
            ClearField();

            for (int x = -size.x / 2; x <= size.x / 2; x++)
            {
                for (int y = -size.y / 2; y <= size.y / 2; y++)
                {
                    TryPlaceHexGroundAtPoint(new Vector2Int(x, y), false);
                }
            }
        }

        public HexagonNode TryPlaceHexGroundAtPoint(Vector2Int point, bool withUndoRegister)
        {
            if (GetHexInfoByAbsoleteCoord(point) != null && GetHexInfoByAbsoleteCoord(point).HexagonName == _currentPrefab.transform.name)
            {
                return null;
            }

            StringsContainer tags = new StringsContainer();
            List<GameObject> obstacles = new List<GameObject>();
            int prevHexHeight = 0;
            int prevHexRotation = 0;

            if (GetHexInfoByAbsoleteCoord(point).HexObject != null)
            {
                prevHexHeight = GetHexInfoByAbsoleteCoord(point).Height;
                prevHexRotation = GetHexInfoByAbsoleteCoord(point).Rotation;

                foreach (var tag in GetHexInfoByAbsoleteCoord(point).Tags.Strings)
                {
                    tags.Strings.Add(tag);
                }

                foreach (var obstacle in GetHexInfoByAbsoleteCoord(point).Obstacles)
                {
                    obstacle.transform.SetParent(null);
                    obstacles.Add(obstacle);
                }

                TryRemoveHexFromPoint(point, withUndoRegister);
            }

            if (_currentPrefab.GetComponent<HexagonNode>() == null)
            {
                Debug.LogWarning("Not valid prefab!!..");
                return null;
            }

            HexagonNode createdHex = (PrefabUtility.InstantiatePrefab(_currentPrefab) as GameObject).GetComponent<HexagonNode>();

            createdHex.transform.SetParent(_hexesPrefabsContainer);
            createdHex.transform.position = HexGridUtility.ConvertHexCoordToWorldPoint(point);

            if (withUndoRegister)
            {
                Undo.RegisterCreatedObjectUndo(createdHex, "Create object");
            }

            GetHexInfoByAbsoleteCoord(point).HexObject = createdHex;
            GetHexInfoByAbsoleteCoord(point).HexagonName = createdHex.transform.name;
            GetHexInfoByAbsoleteCoord(point).HexagonPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(createdHex);
            GetHexInfoByAbsoleteCoord(point).HexX = point.x;
            GetHexInfoByAbsoleteCoord(point).HexY = point.y;
            SetHeight(point, prevHexHeight);
            SetRotation(point, prevHexRotation);
            GetHexInfoByAbsoleteCoord(point).Obstacles = new List<GameObject>();
            GetHexInfoByAbsoleteCoord(point).ObstaclesOffsets = new Vector3Container();
            GetHexInfoByAbsoleteCoord(point).ObstacleNames = new StringsContainer();

            foreach (var obstacle in obstacles)
            {
                AddObstacle(obstacle, point, true);
            }

            foreach (var tag in tags.Strings)
            {
                AddTag(point, tag);
            }

            return createdHex;
        }

        public void TryRemoveHexFromPoint(Vector2Int point, bool withUndoRegister)
        {
            if (GetHexInfoByAbsoleteCoord(point) != null && GetHexInfoByAbsoleteCoord(point).HexObject != null)
            {
                if (withUndoRegister)
                {
                    Undo.DestroyObjectImmediate(GetHexInfoByAbsoleteCoord(point).HexObject.gameObject);
                }
                else
                {
                    DestroyImmediate(GetHexInfoByAbsoleteCoord(point).HexObject.gameObject);
                }

                GetHexInfoByAbsoleteCoord(point).HexObject = null;
                GetHexInfoByAbsoleteCoord(point).Obstacles.Clear();
                GetHexInfoByAbsoleteCoord(point).ObstaclesOffsets.Vectors.Clear();
                GetHexInfoByAbsoleteCoord(point).ObstacleNames.Strings.Clear();
                GetHexInfoByAbsoleteCoord(point).Tags.Strings.Clear();
            }
            else
            {
                return;
            }
        }

        public GameObject AddObstacle(Vector2Int point, bool withUndoRegister)
        {
            if (GetHexInfoByAbsoleteCoord(point) != null && GetHexInfoByAbsoleteCoord(point).HexObject != null)
            {
                GameObject createdObstacle = PrefabUtility.InstantiatePrefab(_currentPrefab) as GameObject;
                GetHexInfoByAbsoleteCoord(point).HexObject.AddObstacle(createdObstacle.transform, true);

                if (withUndoRegister)
                {
                    Undo.RegisterCreatedObjectUndo(createdObstacle, "Create obstacle");
                }

                GetHexInfoByAbsoleteCoord(point).Obstacles.Add(createdObstacle);
                GetHexInfoByAbsoleteCoord(point).ObstaclesOffsets.Vectors.Add(createdObstacle.transform.localPosition);
                GetHexInfoByAbsoleteCoord(point).ObstacleNames.Strings.Add(createdObstacle.name);

                return createdObstacle;
            }

            return null;
        }

        public GameObject AddObstacle(GameObject preciseObstacle, Vector2Int point, bool withUndoRegister)
        {
            if (GetHexInfoByAbsoleteCoord(point).HexObject != null)
            {
                GetHexInfoByAbsoleteCoord(point).HexObject.AddObstacle(preciseObstacle.transform, false);

                if (withUndoRegister)
                {
                    Undo.RegisterCreatedObjectUndo(preciseObstacle, "Create obstacle");
                }

                GetHexInfoByAbsoleteCoord(point).Obstacles.Add(preciseObstacle);
                GetHexInfoByAbsoleteCoord(point).ObstaclesOffsets.Vectors.Add(preciseObstacle.transform.localPosition);
                GetHexInfoByAbsoleteCoord(point).ObstacleNames.Strings.Add(preciseObstacle.name);

                return preciseObstacle;
            }

            return null;
        }

        public void RemoveObstacle(Vector2Int point, bool withUndoRegister)
        {
            if (GetHexInfoByAbsoleteCoord(point).HexObject != null && GetHexInfoByAbsoleteCoord(point).Obstacles.Count != 0)
            {
                GetHexInfoByAbsoleteCoord(point).HexObject.RemoveLastObstacle(withUndoRegister);
                GetHexInfoByAbsoleteCoord(point).Obstacles.RemoveAt(GetHexInfoByAbsoleteCoord(point).Obstacles.Count - 1);
                GetHexInfoByAbsoleteCoord(point).ObstaclesOffsets.Vectors.RemoveAt(GetHexInfoByAbsoleteCoord(point).ObstaclesOffsets.Vectors.Count - 1);
                GetHexInfoByAbsoleteCoord(point).ObstacleNames.Strings.RemoveAt(GetHexInfoByAbsoleteCoord(point).ObstacleNames.Strings.Count - 1);
            }

            return;
        }

        public void ClearField()
        {
            int childCount = _hexesPrefabsContainer.childCount;
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(_hexesPrefabsContainer.GetChild(0).gameObject);
            }

            int iterator = 0;
            _hexPlaces = new HexPlaceInfo[RectangularGridSize * RectangularGridSize];
            for (int x = 0; x < RectangularGridSize; x++)
            {
                for (int y = 0; y < RectangularGridSize; y++)
                {
                    _hexPlaces[iterator] = new HexPlaceInfo();
                    iterator++;
                }
            }
        }

        public void ChangeHeight(Vector2Int point, int direction)
        {
            if (GetHexInfoByAbsoleteCoord(point).HexObject != null)
            {
                GetHexInfoByAbsoleteCoord(point).Height += direction;
                GetHexInfoByAbsoleteCoord(point).HexObject.transform.position += Vector3.up * _heightStep * direction;
                Undo.RegisterCompleteObjectUndo(GetHexInfoByAbsoleteCoord(point).HexObject, "Hex change height");
            }
        }

        public void ChangeRotation(Vector2Int point, int direction)
        {
            if (GetHexInfoByAbsoleteCoord(point).HexObject != null)
            {
                GetHexInfoByAbsoleteCoord(point).Rotation += direction;
                GetHexInfoByAbsoleteCoord(point).HexObject.transform.rotation *= Quaternion.Euler(0f, 60f * direction, 0f);
                Undo.RegisterCompleteObjectUndo(GetHexInfoByAbsoleteCoord(point).HexObject, "Hex change height");
            }
        }

        public void SetHeight(Vector2Int point, int height)
        {
            if (GetHexInfoByAbsoleteCoord(point).HexObject != null)
            {
                GetHexInfoByAbsoleteCoord(point).Height = height;
                GetHexInfoByAbsoleteCoord(point).HexObject.transform.position += Vector3.up * _heightStep * height;
                Undo.RegisterCompleteObjectUndo(GetHexInfoByAbsoleteCoord(point).HexObject, "Hex set height");
            }
        }

        public void SetRotation(Vector2Int point, int rotation)
        {
            if (GetHexInfoByAbsoleteCoord(point).HexObject != null)
            {
                GetHexInfoByAbsoleteCoord(point).Rotation = rotation;
                GetHexInfoByAbsoleteCoord(point).HexObject.transform.rotation *= Quaternion.Euler(0f, 60f * rotation, 0f);
                Undo.RegisterCompleteObjectUndo(GetHexInfoByAbsoleteCoord(point).HexObject, "Hex set height");
            }
        }

        public void FlatHeight(Vector2Int point)
        {
            if (GetHexInfoByAbsoleteCoord(point).HexObject != null)
            {
                GetHexInfoByAbsoleteCoord(point).Height = 0;
                GetHexInfoByAbsoleteCoord(point).HexObject.transform.position = GetHexInfoByAbsoleteCoord(point).HexObject.transform.position.FlatY();
                Undo.RegisterCompleteObjectUndo(GetHexInfoByAbsoleteCoord(point).HexObject, "Hex flat height");
            }
        }

        public void SetTagsMode(bool value)
        {
            for (int i = 0; i < _hexPlaces.Length; i++)
            {
                if (_hexPlaces[i] != null && _hexPlaces[i].HexObject != null)
                {
                    string tagName = "";

                    foreach (var item in _hexPlaces[i].Tags.Strings)
                    {
                        tagName += item;
                        tagName += "_";
                    }

                    SetTagIconForHex(
                        _hexPlaces[i].HexObject,
                        tagName,
                        _hexPlaces[i].Tags.Strings.Count > 0 ? 6 : -1,
                        value);
                }
            }
        }

        public void AddTag(Vector2Int point, string tag)
        {
            if (GetHexInfoByAbsoleteCoord(point).HexObject != null)
            {
                if (!GetHexInfoByAbsoleteCoord(point).Tags.Strings.Contains(tag))
                {
                    GetHexInfoByAbsoleteCoord(point).Tags.Strings.Add(tag);
                }
            }

            SetTagsMode(true);
        }

        public void RemoveTag(Vector2Int point, string tag)
        {
            if (GetHexInfoByAbsoleteCoord(point).HexObject != null)
            {
                if (GetHexInfoByAbsoleteCoord(point).Tags.Strings.Contains(tag))
                {
                    GetHexInfoByAbsoleteCoord(point).Tags.Strings.Remove(tag);
                }
            }

            SetTagsMode(true);
        }

        private void SetTagIconForHex(HexagonNode hex, string tagName, int idx, bool isBigIcon)
        {
            if (idx == -1)
            {
                hex.SetTag(tagName, null);
            }
            else
            {
                GUIContent[] largeIcons = GetTextures(isBigIcon ? "sv_label_" : "sv_icon_name", string.Empty, 0, 8);
                GUIContent icon = largeIcons[idx];
                hex.SetTag(tagName, icon.image as Texture2D);
            }
        }

        private GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
        {
            GUIContent[] array = new GUIContent[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);
            }
            
            return array;
        }

        public void ClearAllTags(bool isInTagsMode)
        {
            for (int i = 0; i < _hexPlaces.Length; i++)
            {
                if (_hexPlaces[i] != null && _hexPlaces[i].HexObject != null)
                {
                    _hexPlaces[i].Tags.Strings.Clear();
                    SetTagIconForHex(
                        _hexPlaces[i].HexObject,
                        "",
                        -1,
                        isInTagsMode);
                }
            }
        }

        public void SetObstaclesNames(Vector2Int point, List<string> strings)
        {
            if (GetHexInfoByAbsoleteCoord(point).HexObject != null)
            {
                GetHexInfoByAbsoleteCoord(point).ObstacleNames.Strings = strings;
            }
        }
        
        public void SetObstaclesOffset(Vector2Int point, List<Vector3> offsets)
        {
            if (GetHexInfoByAbsoleteCoord(point).HexObject != null)
            {
                GetHexInfoByAbsoleteCoord(point).ObstaclesOffsets.Vectors = offsets;

                for (int i = 0; i < offsets.Count; i++)
                {
                    GetHexInfoByAbsoleteCoord(point).Obstacles[i].transform.localPosition = offsets[i];
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_hexesPrefabsContainer.transform.position != Vector3.zero)
            {
                _hexesPrefabsContainer.transform.position = Vector3.zero;
            }
        }
#endif

        public float GetHeightStep()
        {
            return _heightStep;
        }

        public bool HaveHexInCoord(Vector2Int coord)
        {
            if (GetHexInfoByAbsoleteCoord(coord) != null)
            {
                return GetHexInfoByAbsoleteCoord(coord).HexObject != null;
            }
            else
            {
                return false;
            }
        }

        public HexPlaceInfo GetHexByCoord(Vector2Int coord)
        {
            return GetHexInfoByAbsoleteCoord(coord);
        }

        private HexPlaceInfo GetHexInfoByAbsoleteCoord(Vector2Int notAbsoleteCoord)
        {
            int xID = notAbsoleteCoord.x + RectangularGridSize / 2;
            int yID = notAbsoleteCoord.y + RectangularGridSize / 2;
             
            if (xID >= 0 && yID >= 0 && _hexPlaces.Length > (xID * RectangularGridSize + yID))
            { 
                return _hexPlaces[xID * RectangularGridSize + yID];
            }
            else
            {
                return null;
            }
        }

        public HexPlaceInfo[] GetAllHexes()
        {
            return _hexPlaces;
        }

        [System.Serializable]
        public class HexesWrapper
        {
            public HexPlaceInfo[] Places;
            public string[] GlobalTags;
        }

        [System.Serializable]
        public class HexPlaceInfo
        {
            [NonSerialized] public HexagonNode HexObject;
            public string HexagonName;
            public string HexagonPath;
            public int HexX;
            public int HexY;
            [NonSerialized] public List<GameObject> Obstacles = new List<GameObject>();
            public int Height = 0;
            public int Rotation = 0;
            public Vector3Container ObstaclesOffsets = new Vector3Container();
            public StringsContainer Tags = new StringsContainer();
            public StringsContainer ObstacleNames = new StringsContainer();
        }

        [System.Serializable]
        public class Vector3Container
        {
            public List<Vector3> Vectors = new List<Vector3>();
        }

        [System.Serializable]
        public class StringsContainer
        {
            public List<string> Strings = new List<string>();
        }
    }
}