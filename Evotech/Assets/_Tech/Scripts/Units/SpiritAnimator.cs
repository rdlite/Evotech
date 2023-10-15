using Core.Data;
using UnityEngine;

namespace Core.Units
{
    [RequireComponent(typeof(Animator))]
    public class SpiritAnimator : MonoBehaviour
    {
        private int WEAPON_ID_HASH = Animator.StringToHash("WeaponID");
        private int ATTACK_ID_HASH = Animator.StringToHash("AttackID");
        private int ATTACK_TRIGGER_HASH = Animator.StringToHash("AttackTrigger");

        private WeaponStyle _weaponStyle;
        private Animator _animator;

        public void Init(WeaponStyle weaponStyle)
        {
            _weaponStyle = weaponStyle;

            _animator = GetComponent<Animator>();
            _animator.keepAnimatorStateOnDisable = true;
            _animator.SetFloat(WEAPON_ID_HASH, weaponStyle.AnimatorID);
        }

        public void PlayMeleeAttack()
        {
            _animator.SetFloat(ATTACK_ID_HASH, Random.Range(0, _weaponStyle.AnimationsAmount));
            _animator.SetTrigger(ATTACK_TRIGGER_HASH);
        }
    }
}