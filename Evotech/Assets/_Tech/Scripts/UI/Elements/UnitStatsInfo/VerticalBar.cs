using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Elements
{
    public class VerticalBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _barRect;
        [SerializeField] private RectTransform _damageDiffBar;
        [SerializeField] private RectTransform _possibleDamageAmount;
        [SerializeField] private Image _possibleDamageImage;
        [SerializeField] private Gradient _possibleDamageColorGradient;
        [SerializeField] private float _changePossibleDamageGradientSpeed = 1f;
        [SerializeField] private float _backgroundBarSlideSpeed = 5f;

        private float _currentPercentage;
        private float _maxHeight;

        public void Init(float startPercentage)
        {
            _maxHeight = _barRect.sizeDelta.y;
            _currentPercentage = startPercentage;
            _possibleDamageAmount.gameObject.SetActive(false);
            UpdateRectHeight();
            _damageDiffBar.sizeDelta = _barRect.sizeDelta;
        }

        private void Update()
        {
            if (_damageDiffBar.gameObject.activeSelf)
            {
                _damageDiffBar.sizeDelta = Vector2.Lerp(_damageDiffBar.sizeDelta, _barRect.sizeDelta, _backgroundBarSlideSpeed * Time.deltaTime);
            }

            if (_possibleDamageAmount.gameObject.activeSelf)
            {
                _possibleDamageImage.color = _possibleDamageColorGradient.Evaluate((Time.time * _changePossibleDamageGradientSpeed) % 1);
            }
        }

        public void SetPossibleDamageValue(float targetPercentage)
        {
            _damageDiffBar.gameObject.SetActive(false);
            _possibleDamageAmount.gameObject.SetActive(true);

            _possibleDamageAmount.sizeDelta = new Vector2(_barRect.sizeDelta.x, _maxHeight * _currentPercentage);
            _currentPercentage = targetPercentage;
            UpdateRectHeight();
        }

        public void HideDamageInfo(float percentage)
        {
            SetValue(percentage);
        }

        public void SetValue(float percentage)
        {
            _currentPercentage = percentage;
            _damageDiffBar.gameObject.SetActive(true);
            _possibleDamageAmount.gameObject.SetActive(false);

            UpdateRectHeight();
        }

        private void UpdateRectHeight()
        {
            _barRect.sizeDelta = new Vector2(_barRect.sizeDelta.x, _maxHeight * _currentPercentage);
        }
    }
}