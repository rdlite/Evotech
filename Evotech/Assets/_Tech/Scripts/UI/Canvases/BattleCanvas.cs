using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.UI
{
    public class BattleCanvas : AbstractCanvas
    {
        public override void Show()
        {
            AppearAnimation();
        }

        private async void AppearAnimation()
        {
            float t = 0f;

            while (t <= 1f)
            {
                t += Time.deltaTime;

                _canvasGroup.alpha = Mathf.Clamp01(t);

                await UniTask.DelayFrame(1);
            }

            _canvasGroup.alpha = 1f;
        }
    }
}