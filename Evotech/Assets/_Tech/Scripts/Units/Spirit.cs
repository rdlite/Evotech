using Extensions;
using Qnject;
using UnityEngine;

namespace Core.Units
{
    [RequireComponent(typeof(SpiritAnimator))]
    public class Spirit : MonoBehaviour
    {
        [HideInInspector] private OneWeaponLeftHandPoint _oneWeaponLeftHandPoint;

        private SpiritAnimator _animator;
        private GameObject _weapon;

        public void Init(int animatorWeaponID)
        {
            _animator = GetComponent<SpiritAnimator>();
            _animator.Init(animatorWeaponID);

            _oneWeaponLeftHandPoint = GetComponentInChildren<OneWeaponLeftHandPoint>();
        }

        public void CreateWeapon(GameObject weapon)
        {
            if (_weapon != null)
            {
                Destroy(_weapon);
            }

            _weapon = QnjectPrefabsFactory.CreatePrefab(weapon);
            _weapon.transform.SetParent(_oneWeaponLeftHandPoint.transform);
            _weapon.transform.ResetLocals();
        }
    }
}