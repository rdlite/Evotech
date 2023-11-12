using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Elements
{
    public class NoiseIntensitySetter : MonoBehaviour
    {
        private void Awake()
        {
#if !UNITY_EDITOR
            Image currentImage = GetComponent<Image>();
            currentImage.color = currentImage.color.SetAlpha(currentImage.color.a / 2f);
#elif UNITY_EDITOR
            gameObject.SetActive(false);
#endif
        }
    }
}