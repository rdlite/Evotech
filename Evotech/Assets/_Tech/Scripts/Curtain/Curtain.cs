using Cysharp.Threading.Tasks;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Curtains
{
    public class Curtain : MonoBehaviour, ICurtain
    {
        [SerializeField] private Image _bgImage;
        [SerializeField] private float _duration = 1f;

        public async void TriggerCurtain(bool activate, bool instantly, System.Action callback = null)
        {
            if (activate)
            {
                await ShowCurtain(instantly);
                callback?.Invoke();
            }
            else
            {
                await HideCurtain(instantly);
                callback?.Invoke();
            }
        }

        private async UniTask ShowCurtain(bool instantly)
        {
            if (gameObject.activeSelf && _bgImage.color.a == 1f)
            {
                return;
            }

            gameObject.SetActive(true);

            if (instantly)
            {
                _bgImage.SetAlpha(1f);

                return;
            }

            float t = 0f;

            while (t <= 1f)
            {
                t += Time.deltaTime / _duration;

                _bgImage.SetAlpha(t);

                await UniTask.DelayFrame(1);
            }

            _bgImage.SetAlpha(1f);
        }

        private async UniTask HideCurtain(bool instantly)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (instantly)
            {
                gameObject.SetActive(false);
                _bgImage.SetAlpha(0f);

                return;
            }

            _bgImage.SetAlpha(1f);

            float t = 0f;

            while (t <= 1f)
            {
                t += Time.deltaTime / _duration;

                _bgImage.SetAlpha(1f - t);

                await UniTask.DelayFrame(1);
            }

            gameObject.SetActive(false);
            _bgImage.SetAlpha(0f);
        }
    }
}