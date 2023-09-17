using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace HexEditor
{
    public class HexEditorWindow : EditorWindow
    {
        [SerializeField] private List<GameObject> _palette = new List<GameObject>();
        [SerializeField] private int _paletteIndex;

        [SerializeField] private DefaultAsset _targetFolder = null;
        [SerializeField] private Vector2Int _selectedSize;
        [SerializeField] private SceneGridView _sceneGridView;
        [SerializeField] private SceneHexGridEditor _sceneHexGridEditor;
        [SerializeField] private Vector2Int _selectedHexID;
        [SerializeField] private Vector2 scrollPos;
        [SerializeField] private bool _isShowGrid;
        [SerializeField] private bool _isChangedGridView;
        [SerializeField] private bool _isHeightEditMode = false;
        [SerializeField] private bool _isPaintMode = false;
        [SerializeField] private bool _isDrawingPrefabs = false;
        [SerializeField] private bool _isGrowingHeight = false;
        [SerializeField] private bool _isLoweringHeight = false;
        [SerializeField] private Vector3 _mousePosWhenPressed;

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

            _selectedSize = EditorGUILayout.Vector2IntField("Size for generation", _selectedSize);

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

            DefaultSpace();
            EditorGUILayout.LabelField("-------------Palette-------------");

            bool paintOld = _isPaintMode;
            _isPaintMode = GUILayout.Toggle(_isPaintMode, "Paint mode", "Button", GUILayout.Height(60f));

            if (_isPaintMode != paintOld && _isPaintMode)
            {
                ToggleModes();
                _isPaintMode = true;
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
            if (_isPaintMode || _isHeightEditMode)
            {
                CalculateSelectedHex();
                HandleSceneViewInputs(_selectedHexID);

                if (_isDrawingPrefabs)
                {
                    GetSceneEditor().PlaceHexGroundAtPoint(_selectedHexID, true);
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

            _selectedHexID = HexGridUtility.ConvertWorldPointToGridID(mousePosition);
            DrawSelectedHexel(_selectedHexID);
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

        private void HandleSceneViewInputs(Vector2Int hexCoord)
        {
            if (Event.current.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(0);
            }

            if (_isPaintMode)
            {
                if (!_isDrawingPrefabs && _paletteIndex < _palette.Count && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    _isDrawingPrefabs = true;
                }
                else if (_isDrawingPrefabs && _paletteIndex < _palette.Count && Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    _isDrawingPrefabs = false;
                }
                else if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    _mousePosWhenPressed = Event.current.mousePosition;
                }
                else if (Event.current.type == EventType.MouseUp && Event.current.button == 1)
                {
                    if (Vector3.Distance(_mousePosWhenPressed, Event.current.mousePosition) < 10f)
                    {
                        GetSceneEditor().TryRemoveObjectFromPoint(hexCoord, true);
                    }
                }
            }
            else if (_isHeightEditMode)
            {
                if (!_isGrowingHeight && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    _isGrowingHeight = true;
                }
                else if (_isGrowingHeight && Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    _isGrowingHeight = false;
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
            _targetFolder = (DefaultAsset)EditorGUILayout.ObjectField(
                "Palette Folder",
                _targetFolder,
                typeof(DefaultAsset),
                false);

            if (_targetFolder == null)
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
            foreach (GameObject prefab in _palette)
            {
                Texture2D texture = AssetPreview.GetAssetPreview(prefab);
                paletteIcons.Add(new GUIContent(texture));
            }

            _paletteIndex = GUILayout.SelectionGrid(_paletteIndex, paletteIcons.ToArray(), 6, GUILayout.Width(position.width * .9f));

            GetSceneEditor().PinCurrentPrefab(_palette[_paletteIndex]);
        }

        private void DefaultSpace()
        {
            GUILayout.Space(20);
        }

        private void RefreshPalette()
        {
            _palette.Clear();

            if (_targetFolder != null)
            {
                string[] prefabFiles = Directory.GetFiles(AssetDatabase.GetAssetPath(_targetFolder), "*.prefab");
                foreach (string prefabFile in prefabFiles)
                    _palette.Add(AssetDatabase.LoadAssetAtPath(prefabFile, typeof(GameObject)) as GameObject);
            }
        }

        private void ToggleModes()
        {
            DeselectAll();
            _isPaintMode = false;
            _isHeightEditMode = false;
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