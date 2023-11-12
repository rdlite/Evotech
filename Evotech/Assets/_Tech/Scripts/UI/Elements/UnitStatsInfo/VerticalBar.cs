using UnityEngine;

namespace Core.UI.Elements
{
    public class VerticalBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _barRect;

        private float _currentPercentage;
        private float _maxHeight;

        public void Init(float startPercentage)
        {
            _maxHeight = _barRect.sizeDelta.y;
            _currentPercentage = startPercentage;

            UpdateRectHeight();
        }

        public void SetValue(float percentage)
        {
            _currentPercentage = percentage;

            UpdateRectHeight();
        }

        private void UpdateRectHeight()
        {
            _barRect.sizeDelta = new Vector2(_barRect.sizeDelta.x, _maxHeight * _currentPercentage);
        }
    }
}