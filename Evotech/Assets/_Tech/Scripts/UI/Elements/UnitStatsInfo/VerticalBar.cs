using UnityEngine;

namespace Core.UI.Elements
{
    public class VerticalBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _barRect;
        [SerializeField] private RectTransform _barBackground;
        [SerializeField] private float _backgroundBarSlideSpeed = 5f;

        private float _currentPercentage;
        private float _maxHeight;

        public void Init(float startPercentage)
        {
            _maxHeight = _barRect.sizeDelta.y;
            _currentPercentage = startPercentage;
            UpdateRectHeight();
            _barBackground.sizeDelta = _barRect.sizeDelta;
        }

        private void Update()
        {
            _barBackground.sizeDelta = Vector2.Lerp(_barBackground.sizeDelta, _barRect.sizeDelta, _backgroundBarSlideSpeed * Time.deltaTime);
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