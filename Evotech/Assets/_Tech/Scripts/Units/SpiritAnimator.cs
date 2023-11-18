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
        private int ATTACKED_ID_HASH = Animator.StringToHash("AttackedID");
        private int DEATH_ID_HASH = Animator.StringToHash("DeathID");
        private int DEATH_TRIGGER_HASH = Animator.StringToHash("DeathTrigger");
        private int ATTACKED_TRIGGER_HASH = Animator.StringToHash("AttackedTrigger");

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

        public void PlayAttackedImpact()
        {
            _animator.SetFloat(ATTACKED_ID_HASH, Random.Range(0, 2));
            _animator.SetTrigger(ATTACKED_TRIGGER_HASH);
        }

        public void PlayDead()
        {
            _animator.SetFloat(DEATH_ID_HASH, Random.Range(0, 3));
            _animator.SetTrigger(DEATH_TRIGGER_HASH);
        }
    }
}