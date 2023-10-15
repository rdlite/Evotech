using Core.Data;
using UnityEngine;

namespace Core.Battle
{
    [RequireComponent(typeof(LineRenderer))]
    public class BattlePointedLine : MonoBehaviour
    {
        [SerializeField] private int _linePointsAmount = 25;

        private LineRenderer _line;

        public void Create(
            Vector3 startPoint, Vector3 endPoint, PointedLineStyle lineStyle)
        {
            _line = GetComponent<LineRenderer>();
            _line.positionCount = _linePointsAmount;
            Vector3[] positions = new Vector3[_linePointsAmount];

            float defaultHeight = .4f * Vector3.Distance(startPoint, endPoint);

            for (int i = 0; i < _linePointsAmount; i++)
            {
                float interpolator = (float)i / _linePointsAmount;

                positions[i] = Vector3.Lerp(startPoint, endPoint, interpolator) + Vector3.up * Mathf.Sin(interpolator * 3.14f) * defaultHeight;
            }

            _line.SetPositions(positions);

            _line.material.SetFloat("_MoveSpeed", lineStyle.IsMoving ? 1f : 0f);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}