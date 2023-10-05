using UnityEngine;

namespace Core.Units
{
    public class BaseUnitAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private int SHAKE_TRIGGER_HASH = Animator.StringToHash("ShakeTrigger");
        private int SHAKE_ID_HASH = Animator.StringToHash("ShakeID");

        public void PlayPlacedShake()
        {
            _animator.SetFloat(SHAKE_ID_HASH, Random.Range(0, 2));
            _animator.SetTrigger(SHAKE_TRIGGER_HASH);
        }
    }
}