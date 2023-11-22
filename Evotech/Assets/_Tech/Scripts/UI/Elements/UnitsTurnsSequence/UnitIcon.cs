using Core.Units;
using Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.UI.Elements
{
    public class UnitIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform _healthRect;
        [SerializeField] private Image _highlightImage;

        private BaseUnit _correspondingUnit;
        private float _maxHealthRectHeight;
        private float _healthPercentageTarget;
        private float _highlightTimer;
        private bool _isPulsateHighlight, _isSelectedHighlight;

        public void Init(BaseUnit unit)
        {
            _correspondingUnit = unit;
            _maxHealthRectHeight = _healthRect.sizeDelta.y;
            _healthRect.sizeDelta = new Vector2(_healthRect.sizeDelta.x, 0f);
            unit.DynamicStatsProvider.OnModelChanged += OnStatsModelChange;
            _healthPercentageTarget = _correspondingUnit.DynamicStatsProvider.GetHealthPercentage();
        }

        private void OnDestroy()
        {
            if (_correspondingUnit != null)
            {
                _correspondingUnit.DynamicStatsProvider.OnModelChanged -= OnStatsModelChange;
            }
        }

        public void Tick()
        {
            _healthRect.sizeDelta = Vector3.Lerp(
                _healthRect.sizeDelta,
                new Vector2(
                    _healthRect.sizeDelta.x,
                    Mathf.Lerp(0f, _maxHealthRectHeight, 1f - _healthPercentageTarget)),
                10f * Time.deltaTime);

            float targetAlpha = 0f;

            if (_isSelectedHighlight)
            {
                targetAlpha = .6f;
            }
            else
            {
                if (_isPulsateHighlight)
                {
                    _highlightTimer += Time.deltaTime * 5f;
                    targetAlpha = (Mathf.Sin(_highlightTimer) + 1f) / 2f;
                }
                else
                {
                    targetAlpha = 0f;
                }
            }

            _highlightImage.SetAlpha(Mathf.Lerp(
                _highlightImage.color.a,
                targetAlpha,
                10f * Time.deltaTime));
        }

        public void SetHighlightedPulsate(bool value)
        {
            _isPulsateHighlight = value;

            if (value)
            {
                _highlightTimer = 0f;
            }
        }

        public void SetHighlightedSelected(bool value)
        {
            _isSelectedHighlight = value;
        }

        private void OnStatsModelChange(Data.UnitStatsModel statsModel)
        {
            _healthPercentageTarget = _correspondingUnit.DynamicStatsProvider.GetHealthPercentage();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _correspondingUnit.SetActiveOutline(true, false);
            SetHighlightedSelected(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _correspondingUnit.SetActiveOutline(false, false);
            SetHighlightedSelected(false);
        }
    }
}