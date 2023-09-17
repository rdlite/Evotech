using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace HexEditor
{
    public class HexEditorWindow : EditorWindow
    {
        [SerializeField] private List<GameObject> _totalPalette = new List<GameObject>();
        [SerializeField] private int _paletteIndex;

        [SerializeField] private DefaultAsset _hexagonsFolder = null;
        [SerializeField] private DefaultAsset _obstaclesFolder = null;

        private Vector2Int _selectedSize;
        private SceneGridView _sceneGridView;
        private SceneHexGridEditor _sceneHexGridEditor;
        private Vector2Int _lastSelectedHexID;
        private Vector2Int _selectedHexCoord;
        private Vector2 scrollPos;
        private bool _isShowGrid;
        private bool _isChangedGridView;
        private bool _isHeightEditMode = false;
        private bool _isHexagonsPaintMode = false;
        private bool _isObstaclesPaintMode = false;
        private bool _isDrawingPrefabs = false;
        private bool _isChangingHeight = false;
        private int _targetHeight;

        [MenuItem("Window/HexEditor")]
        public static void Open()
        {
            GetWindow<HexEditorWindow>("Hex map editor");
        }

        private void OnGUI()
        {
            scrollPos =
                EditorGUILayout.BeginScrollView(scrollPos, false, false);

            DefaultSpace();

            EditorGUILayout.LabelField("-------------Global grid settings-------------");

            _selectedSize = EditorGUILayout.Vector2IntField("Size for generation", _selectedSize, GUILayout.Width(200f));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Regenerate grid"))
            {
                RegenerateGrid();
            }
            
            if (GUILayout.Button("Clear grid"))
            {
                ClearGrid();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Switch grid view"))
            {
                SwitchGridView();
            }

            DefaultSpace();
            EditorGUILayout.LabelField("-------------Palette-------------");

            bool hexagonsPaintOld = _isHexagonsPaintMode;
            bool obstaclesPaintOld = _isObstaclesPaintMode;
            GUILayout.BeginHorizontal();
            _isHexagonsPaintMode = GUILayout.Toggle(_isHexagonsPaintMode, "Hexagons paint mode", "Button", GUILayout.Height(60f));
            _isObstaclesPaintMode = GUILayout.Toggle(_isObstaclesPaintMode, "Obstacles paint mode", "Button", GUILayout.Height(60f));
            GUILayout.EndHorizontal();

            if (_isHexagonsPaintMode != hexagonsPaintOld && _isHexagonsPaintMode)
            {
                ToggleModes();
                _isHexagonsPaintMode = true;
            }

            if (_isObstaclesPaintMode != obstaclesPaintOld && _isObstaclesPaintMode)
            {
                ToggleModes();
                _isObstaclesPaintMode = true;
            }

            HandleTargetFolder();
            HandlePaletteGrid();

            DefaultSpace();
            EditorGUILayout.LabelField("-------------Height edit-------------");

            bool oldHeightEditMode = _isHeightEditMode;
            _isHeightEditMode = GUILayout.Toggle(_isHeightEditMode, "Height edit mode", "Button", GUILayout.Height(60f));

            if (oldHeightEditMode != _isHeightEditMode && _isHeightEditMode)
            {
                ToggleModes();
                _isHeightEditMode = true;
            }

            DefaultSpace();
            DefaultSpace();
            DefaultSpace();
            EditorGUILayout.EndScrollView();

            HandleGridView();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (_isHexagonsPaintMode || _isHeightEditMode || _isObstaclesPaintMode)
            {
                CalculateSelectedHex();
                HandleSceneViewInputs();

                if (_isDrawingPrefabs)
                {
                    if (Event.current.shift)
                    {
                        GetSceneEditor().TryRemoveHexFromPoint(_selectedHexCoord, true);
                    }
                    else
                    {
                        GetSceneEditor().TryPlaceHexGroundAtPoint(_selectedHexCoord, true);
                    }
                }

                if (_lastSelectedHexID != _selectedHexCoord)
                {
                    _lastSelectedHexID = _selectedHexCoord;

                    if (_isChangingHeight)
                    {
                        if (Event.current.control)
                        {
                            GetSceneEditor().FlatHeight(_selectedHexCoord);
                        }
                        else
                        {
                            GetSceneEditor().ChangeHeight(_selectedHexCoord, Event.current.shift ? -1 : 1);
                        }
                    }
                }

                sceneView.Repaint();
            }
        }

        void OnFocus()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            RefreshPalette();
        }

        void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void CalculateSelectedHex()
        {
            Ray guiRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 mousePosition = guiRay.origin - guiRay.direction * (guiRay.origin.y / guiRay.direction.y);

            _selectedHexCoord = HexGridUtility.ConvertWorldPointToGridID(mousePosition);
            DrawSelectedHexel(_selectedHexCoord);
        }

        private void DrawSelectedHexel(Vector2Int hexID)
        {
            float hexSize = 1f;
            float halfSqrt = Mathf.Sqrt(3) / 2f;

            Vector3 hexPos = new Vector3(
                HexGridUtility.ConvertXToWorldPos(hexID.x, hexID.y),
                0f,
                HexGridUtility.ConvertYToWorldPos(hexID.y));

            Handles.color = Color.yellow;

            Vector3[] lines = new Vector3[] {
                // bottom
                        hexPos + new Vector3(-halfSqrt, -1f, -.5f) * hexSize,
                        hexPos + new Vector3(-halfSqrt, -1f, .5f) * hexSize,

                        hexPos + new Vector3(-halfSqrt, -1f, .5f) * hexSize,
                        hexPos + new Vector3(0f, -1f, 1f) * hexSize,

                        hexPos + new Vector3(0f, -1f, 1f) * hexSize,
                        hexPos + new Vector3(halfSqrt, -1f, .5f) * hexSize,

                        hexPos + new Vector3(halfSqrt, -1f, .5f) * hexSize,
                        hexPos + new Vector3(halfSqrt, -1f, -.5f) * hexSize,

                        hexPos + new Vector3(halfSqrt, -1f, -.5f) * hexSize,
                        hexPos + new Vector3(0f, -1f, -1f) * hexSize,

                        hexPos + new Vector3(0f, -1f, -1f) * hexSize,
                        hexPos + new Vector3(-halfSqrt, -1f, -.5f) * hexSize,
                        
                // top
                        hexPos + new Vector3(-halfSqrt, 1f, -.5f) * hexSize,
                        hexPos + new Vector3(-halfSqrt, 1f, .5f) * hexSize,

                        hexPos + new Vector3(-halfSqrt, 1f, .5f) * hexSize,
                        hexPos + new Vector3(0f, 1f, 1f) * hexSize,

                        hexPos + new Vector3(0f, 1f, 1f) * hexSize,
                        hexPos + new Vector3(halfSqrt, 1f, .5f) * hexSize,

                        hexPos + new Vector3(halfSqrt, 1f, .5f) * hexSize,
                        hexPos + new Vector3(halfSqrt, 1f, -.5f) * hexSize,

                        hexPos + new Vector3(halfSqrt, 1f, -.5f) * hexSize,
                        hexPos + new Vector3(0f, 1f, -1f) * hexSize,

                        hexPos + new Vector3(0f, 1f, -1f) * hexSize,
                        hexPos + new Vector3(-halfSqrt, 1f, -.5f) * hexSize,
                        
                // edges
                        hexPos + new Vector3(-halfSqrt, -1f, -.5f) * hexSize,
                        hexPos + new Vector3(-halfSqrt, 1f, -.5f) * hexSize,

                        hexPos + new Vector3(-halfSqrt, -1f, .5f) * hexSize,
                        hexPos + new Vector3(-halfSqrt, 1f, .5f) * hexSize,

                        hexPos + new Vector3(0f, -1f, 1f) * hexSize,
                        hexPos + new Vector3(0f, 1f, 1f) * hexSize,

                        hexPos + new Vector3(halfSqrt, -1f, .5f) * hexSize,
                        hexPos + new Vector3(halfSqrt, 1f, .5f) * hexSize,

                        hexPos + new Vector3(halfSqrt, -1f, -.5f) * hexSize,
                        hexPos + new Vector3(halfSqrt, 1f, -.5f) * hexSize,

                        hexPos + new Vector3(0f, -1f, -1f) * hexSize,
                        hexPos + new Vector3(0f, 1f, -1f) * hexSize,
                    };

            Handles.DrawLines(lines);
        }

        private void RegenerateGrid()
        {
            GetSceneEditor().ClearAndGenerateGrid(_selectedSize);
        }

        private void ClearGrid()
        {
            GetSceneEditor().ClearField();
        }

        private void HandleSceneViewInputs()
        {
            if (Event.current.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(0);
            }

            if (_isHexagonsPaintMode)
            {
                if (!_isDrawingPrefabs && _paletteIndex < _totalPalette.Count && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    _isDrawingPrefabs = true;
                }
                else if (_isDrawingPrefabs && _paletteIndex < _totalPalette.Count && Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    _isDrawingPrefabs = false;
                }
            }
            else if (_isObstaclesPaintMode)
            {
                if (_paletteIndex < _totalPalette.Count && Event.current.type == EventType.MouseDown && Event.current.button == 0 && !Event.current.shift)
                {
                    GetSceneEditor().AddObstacle(_selectedHexCoord, true);
                }
                else if (_paletteIndex < _totalPalette.Count && Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.shift)
                {
                    GetSceneEditor().RemoveObstacle(_selectedHexCoord, true);
                }
            }
            else if (_isHeightEditMode)
            {
                if (!_isChangingHeight && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    _isChangingHeight = true;

                    if (Event.current.control)
                    {
                        GetSceneEditor().FlatHeight(_selectedHexCoord);
                    }
                    else
                    {
                        _targetHeight = Event.current.shift ? -1 : 1;
                        GetSceneEditor().ChangeHeight(_selectedHexCoord, _targetHeight);
                    }
                }
                else if (_isChangingHeight && Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    _isChangingHeight = false;
                }
            }
        }

        static void DeselectAll()
        {
            Selection.objects = new Object[0];
        }

        private void SwitchGridView()
        {
            _isShowGrid = !_isShowGrid;
            _isChangedGridView = true;
        }

        private void HandleGridView()
        {
            if (_isChangedGridView)
            {
                _isChangedGridView = false;

                GetGridView().SetView(_isShowGrid);
            }
        }

        private void HandleTargetFolder()
        {
            _hexagonsFolder = (DefaultAsset)EditorGUILayout.ObjectField(
                "Hexagons palette folder",
                _hexagonsFolder,
                typeof(DefaultAsset),
                false);

            _obstaclesFolder = (DefaultAsset)EditorGUILayout.ObjectField(
                "Obstacles palette folder",
                _obstaclesFolder,
                typeof(DefaultAsset),
                false);

            if (_hexagonsFolder == null || _obstaclesFolder == null)
            {
                EditorGUILayout.HelpBox(
                    "Not valid!",
                    MessageType.Warning,
                    true);
            }
        }

        private void HandlePaletteGrid()
        {
            List<GUIContent> paletteIcons = new List<GUIContent>();
            foreach (GameObject prefab in _totalPalette)
            {
                Texture2D texture = AssetPreview.GetAssetPreview(prefab);
                paletteIcons.Add(new GUIContent(texture));
            }

            _paletteIndex = GUILayout.SelectionGrid(_paletteIndex, paletteIcons.ToArray(), 6, GUILayout.Width(position.width * .9f));

            GetSceneEditor().PinCurrentPrefab(_totalPalette[_paletteIndex]);
        }

        private void DefaultSpace()
        {
            GUILayout.Space(20);
        }

        private void RefreshPalette()
        {
            _totalPalette.Clear();

            if (_hexagonsFolder != null)
            {
                string[] prefabFiles = Directory.GetFiles(AssetDatabase.GetAssetPath(_hexagonsFolder), "*.prefab");
                foreach (string prefabFile in prefabFiles)
                    _totalPalette.Add(AssetDatabase.LoadAssetAtPath(prefabFile, typeof(GameObject)) as GameObject);
            }
            if (_obstaclesFolder != null)
            {
                string[] prefabFiles = Directory.GetFiles(AssetDatabase.GetAssetPath(_obstaclesFolder), "*.prefab");
                foreach (string prefabFile in prefabFiles)
                    _totalPalette.Add(AssetDatabase.LoadAssetAtPath(prefabFile, typeof(GameObject)) as GameObject);
            }
        }

        private void ToggleModes()
        {
            DeselectAll();
            _isHexagonsPaintMode = false;
            _isHeightEditMode = false;
            _isObstaclesPaintMode = false;
        }

        private SceneHexGridEditor GetSceneEditor()
        {
            if (_sceneHexGridEditor == null)
            {
                _sceneHexGridEditor = FindObjectOfType<SceneHexGridEditor>();
            }

            return _sceneHexGridEditor;
        }

        private SceneGridView GetGridView()
        {
            if (_sceneGridView == null)
            {
                _sceneGridView = FindObjectOfType<SceneGridView>();
            }

            return _sceneGridView;
        }
    }
}