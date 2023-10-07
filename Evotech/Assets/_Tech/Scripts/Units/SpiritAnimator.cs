using UnityEngine;

namespace Core.Units
{
    [RequireComponent(typeof(Animator))]
    public class SpiritAnimator : MonoBehaviour
    {
        private int WEAPON_ID_HASH = Animator.StringToHash("WeaponID");

        private Animator _animator;

        public void Init(int weaponID)
        {
            _animator = GetComponent<Animator>();
            _animator.keepAnimatorStateOnDisable = true;
            _animator.SetFloat(WEAPON_ID_HASH, weaponID);
        }
    }
}