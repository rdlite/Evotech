using Core.Units;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Core.UI.Elements
{
    public class UnitIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform _healthRect;
        [SerializeField] private Image _highlightImage;
        [SerializeField] private CanvasGroup _canvasGroup;

        private BaseUnit _correspondingUnit;
        private float _maxHealthRectHeight;
        private float _healthPercentageTarget;
        private float _highlightTimer;
        private bool _isPulsateHighlight, _isSelectedHighlight;
        private bool _isDestroying;
        private float _startHeight;
        private float _targetHeight;
        private float _destroyingTimer;

        public void Init(BaseUnit unit)
        {
            _correspondingUnit = unit;
            _maxHealthRectHeight = _healthRect.sizeDelta.y;
            _healthRect.sizeDelta = new Vector2(_healthRect.sizeDelta.x, 0f);
            unit.DynamicStatsProvider.OnModelChanged += OnStatsModelChange;
            _healthPercentageTarget = _correspondingUnit.DynamicStatsProvider.GetHealthPercentage();
        }

        public void Tick()
        {
            if (_isDestroying)
            {
                _destroyingTimer += Time.deltaTime / .3f;

                transform.position = Vector3.Lerp(transform.position.SetY(_startHeight), transform.position.SetY(_targetHeight), _destroyingTimer);
                _canvasGroup.alpha = 1f - _destroyingTimer;

                if (_destroyingTimer >= 1f)
                {
                    Destroy(gameObject);
                }
            }
            else
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
        }

        public void SetHighlightedPulsate(bool value)
        {
            if (_isDestroying) return;

            _isPulsateHighlight = value;

            if (value)
            {
                _highlightTimer = 0f;
            }
        }

        public void SetHighlightedSelected(bool value)
        {
            if (_isDestroying) return;

            _isSelectedHighlight = value;
        }

        private void OnStatsModelChange(Data.UnitStatsModel statsModel)
        {
            if (_isDestroying) return;

            _healthPercentageTarget = _correspondingUnit.DynamicStatsProvider.GetHealthPercentage();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isDestroying) return;

            _correspondingUnit.SetActiveOutline(true, false);
            SetHighlightedSelected(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isDestroying) return;

            _correspondingUnit.SetActiveOutline(false, false);
            SetHighlightedSelected(false);
        }

        public void Remove()
        {
            if (_isDestroying) return;

            _isDestroying = true;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            _startHeight = transform.position.y;
            _targetHeight = transform.position.y - GetComponent<RectTransform>().sizeDelta.y / 2f;

            _highlightImage.gameObject.SetActive(false);

            transform.SetParent(transform.parent.parent);

            if (_correspondingUnit != null)
            {
                _correspondingUnit.DynamicStatsProvider.OnModelChanged -= OnStatsModelChange;
            }
        }
    }
}