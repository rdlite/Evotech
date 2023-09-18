using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace HexEditor
{
    public class HexEditorWindow : EditorWindow
    {
        [SerializeField] private List<GameObject> _totalPalette = new List<GameObject>();
        [SerializeField] private int _paletteIndex;

        [SerializeField] private DefaultAsset _hexagonsFolder = null;
        [SerializeField] private DefaultAsset _obstaclesFolder = null;

        private List<string> _tags = new List<string>();
        private Vector2Int _selectedSize;
        private SceneGridView _sceneGridView;
        private SceneHexGridEditor _sceneHexGridEditor;
        private Vector2Int _lastSelectedHexID;
        private Vector2Int _selectedHexCoord;
        private Vector2 _scrollPos;
        private string _currentTextTag;
        private int _tagIndex;
        private bool _isShowTags;
        private bool _isShowGrid;
        private bool _isChangedGridView;
        private bool _isHeightEditMode = false;
        private bool _isHexagonsPaintMode = false;
        private bool _isTagsEditorMode = false;
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
            _scrollPos =
                EditorGUILayout.BeginScrollView(_scrollPos, false, false);

            DefaultSpace();

            GUIStyle headerStyle = new GUIStyle();
            headerStyle.fontSize = 20;
            headerStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.LabelField("Global grid editor", headerStyle);
            HalfDefaultSpace();
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
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

                if (GUILayout.Button("Switch grid view"))
                {
                    SwitchGridView();
                }
                GUILayout.EndHorizontal();
            }

            DefaultSpace();
            EditorGUILayout.LabelField("Palette", headerStyle);
            HalfDefaultSpace();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
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
            }

            DefaultSpace();
            EditorGUILayout.LabelField("Height editor", headerStyle);
            HalfDefaultSpace();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                bool oldHeightEditMode = _isHeightEditMode;
                _isHeightEditMode = GUILayout.Toggle(_isHeightEditMode, "Height edit mode", "Button", GUILayout.Height(60f));

                if (oldHeightEditMode != _isHeightEditMode && _isHeightEditMode)
                {
                    ToggleModes();
                    _isHeightEditMode = true;
                }
            }

            DefaultSpace();
            EditorGUILayout.LabelField("Tags editor", headerStyle);
            HalfDefaultSpace();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                HandleTagsEditor();
            }

            DefaultSpace();
            DefaultSpace();
            DefaultSpace();
            EditorGUILayout.EndScrollView();

            HandleGridView();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (_isHexagonsPaintMode || _isHeightEditMode || _isObstaclesPaintMode || _isTagsEditorMode)
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

            float height = 0f;

            if (GetSceneEditor().HaveHexInCoord(hexID))
            {
                height = GetSceneEditor().GetHexByCoord(hexID).Height * GetSceneEditor().GetHeightStep();
                Handles.color = Color.green;
            }
            else
            {
                Handles.color = Color.yellow;
            }

            Vector3[] lines = new Vector3[] {
                // bottom
                        hexPos + new Vector3(-halfSqrt, height + -1f, -.5f) * hexSize,
                        hexPos + new Vector3(-halfSqrt, height + -1f, .5f) * hexSize,

                        hexPos + new Vector3(-halfSqrt, height + -1f, .5f) * hexSize,
                        hexPos + new Vector3(0f, height + -1f, 1f) * hexSize,

                        hexPos + new Vector3(0f, height + -1f, 1f) * hexSize,
                        hexPos + new Vector3(halfSqrt, height + -1f, .5f) * hexSize,

                        hexPos + new Vector3(halfSqrt, height + -1f, .5f) * hexSize,
                        hexPos + new Vector3(halfSqrt, height + -1f, -.5f) * hexSize,

                        hexPos + new Vector3(halfSqrt, height + -1f, -.5f) * hexSize,
                        hexPos + new Vector3(0f, height + -1f, -1f) * hexSize,

                        hexPos + new Vector3(0f, height + -1f, -1f) * hexSize,
                        hexPos + new Vector3(-halfSqrt, height + -1f, -.5f) * hexSize,
                        
                // top
                        hexPos + new Vector3(-halfSqrt, height + 1f, -.5f) * hexSize,
                        hexPos + new Vector3(-halfSqrt, height + 1f, .5f) * hexSize,

                        hexPos + new Vector3(-halfSqrt, height + 1f, .5f) * hexSize,
                        hexPos + new Vector3(0f, height + 1f, 1f) * hexSize,

                        hexPos + new Vector3(0f, height + 1f, 1f) * hexSize,
                        hexPos + new Vector3(halfSqrt, height + 1f, .5f) * hexSize,

                        hexPos + new Vector3(halfSqrt, height + 1f, .5f) * hexSize,
                        hexPos + new Vector3(halfSqrt, height + 1f, -.5f) * hexSize,

                        hexPos + new Vector3(halfSqrt, height + 1f, -.5f) * hexSize,
                        hexPos + new Vector3(0f, height + 1f, -1f) * hexSize,

                        hexPos + new Vector3(0f, height + 1f, -1f) * hexSize,
                        hexPos + new Vector3(-halfSqrt, height + 1f, -.5f) * hexSize,
                        
                // edges
                        hexPos + new Vector3(-halfSqrt, height + -1f, -.5f) * hexSize,
                        hexPos + new Vector3(-halfSqrt, height + 1f, -.5f) * hexSize,

                        hexPos + new Vector3(-halfSqrt, height + -1f, .5f) * hexSize,
                        hexPos + new Vector3(-halfSqrt, height + 1f, .5f) * hexSize,

                        hexPos + new Vector3(0f, height + -1f, 1f) * hexSize,
                        hexPos + new Vector3(0f, height + 1f, 1f) * hexSize,

                        hexPos + new Vector3(halfSqrt, height + -1f, .5f) * hexSize,
                        hexPos + new Vector3(halfSqrt, height + 1f, .5f) * hexSize,

                        hexPos + new Vector3(halfSqrt, height + -1f, -.5f) * hexSize,
                        hexPos + new Vector3(halfSqrt, height + 1f, -.5f) * hexSize,

                        hexPos + new Vector3(0f, height + -1f, -1f) * hexSize,
                        hexPos + new Vector3(0f, height + 1f, -1f) * hexSize,
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
            else if (_isTagsEditorMode)
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.shift)
                {
                    GetSceneEditor().RemoveTag(_selectedHexCoord, _tags[_tagIndex]);
                }
                else if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !Event.current.shift)
                {
                    GetSceneEditor().AddTag(_selectedHexCoord, _tags[_tagIndex]);
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

        private void HalfDefaultSpace()
        {
            GUILayout.Space(10);
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

        private void HandleTagsEditor()
        {
            RefillDefaultTags();

            _currentTextTag = EditorGUILayout.TextField(_currentTextTag);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add new tag"))
            {
                AddNewTag(_currentTextTag);
            }
            if (GUILayout.Button("Remove existing tag"))
            {
                RemoveTag(_currentTextTag);
            }
            if (GUILayout.Button("Reset"))
            {
                ResetTags();
            }
            EditorGUILayout.EndHorizontal();

            bool oldTagsEditing = _isTagsEditorMode;
            _isTagsEditorMode = GUILayout.Toggle(_isTagsEditorMode, "Tags editor mode", "Button", GUILayout.Height(60f));
            if (_isTagsEditorMode != oldTagsEditing && _isTagsEditorMode)
            {
                ToggleModes();
                _isTagsEditorMode = true;
                GetSceneEditor().SetTagsMode(true);
            }
            else if (_isTagsEditorMode != oldTagsEditing && !_isTagsEditorMode)
            {
                GetSceneEditor().SetTagsMode(false);
            }

            if (GUILayout.Button("Clear all tags"))
            {
                ClearObjectsTag();
            }

            _isShowTags = EditorGUILayout.Foldout(_isShowTags, new GUIContent("Tags"));

            if (_isShowTags)
            {
                _tagIndex = GUILayout.SelectionGrid(_tagIndex, _tags.ToArray(), 3, GUILayout.Width(position.width * .93f));
                //for (int i = 0; i < _tags.Count; i++)
                //{
                //    EditorGUILayout.LabelField("    - " + _tags[i]);
                //}
            }
        }

        private void RefillDefaultTags()
        {
            if (!_tags.Contains("playerHex"))
            {
                _tags.Add("playerHex");
            }

            if (!_tags.Contains("enemyHex"))
            {
                _tags.Add("enemyHex");
            }
        }

        private void AddNewTag(string tag)
        {
            if (!_tags.Contains(tag))
            {
                _tags.Add(tag);
            }
        }

        private void RemoveTag(string tag)
        {
            if (_tags.Contains(tag))
            {
                _tags.Remove(tag);
            }
        }
        
        private void ResetTags()
        {
            _currentTextTag = "";
            _tags.Clear();
        }
        
        private void ClearObjectsTag()
        {
            GetSceneEditor().ClearAllTags(_isTagsEditorMode);
        }

        private void ToggleModes()
        {
            DeselectAll();
            _isTagsEditorMode = false;
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