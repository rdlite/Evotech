using UnityEditor;
using UnityEngine;

namespace HexEditor
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SceneHexGridEditor))]
    public class SceneGridView : MonoBehaviour
    {
        [SerializeField] private float _gridDrawCameraDistance = 30f;
        [SerializeField] private AnimationCurve _distanceCurveAlpha;
        [SerializeField] private Color _gridColor;

        private float _hexelSize = 1f;
        private float _halfSqrt = 0f;
        private bool _isDraw;

        public void SetView(bool value)
        {
            _isDraw = value;
        }

        private void OnDrawGizmos()
        {
            if (_halfSqrt == 0f)
            {
                _halfSqrt = Mathf.Sqrt(3) / 2f;
            }

            if (SceneView.currentDrawingSceneView == null || SceneView.currentDrawingSceneView.camera == null)
            {
                return;
            }

            Vector3 sceneCameraPos = SceneView.currentDrawingSceneView.camera.transform.position;

            if (_isDraw)
            {
                int gridSize = GetComponent<SceneHexGridEditor>().RectangularGridSize;

                for (int x = -gridSize; x <= gridSize; x++)
                {
                    for (int y = -gridSize; y <= gridSize; y++)
                    {
                        Vector3 hexPos = new Vector3(
                            HexGridUtility.ConvertXToWorldPos(x, y, _hexelSize),
                            0f,
                            HexGridUtility.ConvertYToWorldPos(y, _hexelSize));

                        float distance = Vector3.Distance(sceneCameraPos.FlatY(), hexPos.FlatY());

                        if (distance <= _gridDrawCameraDistance)
                        {
                            Color gizmoColor = _gridColor;
                            gizmoColor.a = _distanceCurveAlpha.Evaluate(distance / _gridDrawCameraDistance) * gizmoColor.a;
                            Gizmos.color = gizmoColor;
                            DrawHexel(hexPos);
                        }
                    }
                }
            }

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(Vector3.zero, .2f);
        }

        public void DrawHexel(Vector3 point)
        {
            Gizmos.DrawLineList(
                new System.ReadOnlySpan<Vector3>(
                    new Vector3[] {
                        point + new Vector3(-_halfSqrt, 0f, -.5f) * _hexelSize,
                        point + new Vector3(-_halfSqrt, 0f, .5f) * _hexelSize,

                        point + new Vector3(-_halfSqrt, 0f, .5f) * _hexelSize,
                        point + new Vector3(0f, 0f, 1f) * _hexelSize,

                        point + new Vector3(0f, 0f, 1f) * _hexelSize,
                        point + new Vector3(_halfSqrt, 0f, .5f) * _hexelSize,

                        point + new Vector3(_halfSqrt, 0f, .5f) * _hexelSize,
                        point + new Vector3(_halfSqrt, 0f, -.5f) * _hexelSize,

                        point + new Vector3(_halfSqrt, 0f, -.5f) * _hexelSize,
                        point + new Vector3(0f, 0f, -1f) * _hexelSize,

                        point + new Vector3(0f, 0f, -1f) * _hexelSize,
                        point + new Vector3(-_halfSqrt, 0f, -.5f) * _hexelSize,
                    }));
        }
    }
}