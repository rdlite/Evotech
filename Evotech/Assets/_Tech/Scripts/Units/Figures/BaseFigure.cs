using UnityEngine;

namespace Core.Units
{
    public class BaseFigure : MonoBehaviour
    {
        [SerializeField] protected Renderer _mainSkinRenderer;

        protected SpiritAnimator _animator;

        public SpiritAnimator GetAnimator()
        {
            return _animator;
        }
    }
}