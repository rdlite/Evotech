using System;
using Core.Data;
using Core.Particles;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace Core.Units
{
    public class HumanoidUnit : BaseUnit
    {
        public event Action OnUnitClicked;

        private List<ThreeDObjectExploder> _armor;
        private UnitRaycastTrigger _raycastTrigger;
        private Spirit _spirit;
        private WeaponStyle _weaponStyle;
        private float _lastLocalRotationTarget;

        public override void Init(Enums.UnitType unitType, Enums.OutlineType outlineType)
        {
            base.Init(unitType, outlineType);

            _raycastTrigger = GetComponentInChildren<UnitRaycastTrigger>();
            _raycastTrigger.OnClicked += () => OnUnitClicked?.Invoke();

            _weaponStyle = _stylesContainer.GetStyleOfWeaponType(_unitSettings.WeaponID);

            _spirit = GetComponentInChildren<Spirit>();
            _spirit.Init(_weaponStyle, unitType);
            _spirit.CreateWeapon(_weaponStyle.WeaponPrefab);

            _armor = _spirit.CreateArmor();
            _lastLocalRotationTarget = _spirit.transform.localRotation.eulerAngles.y;
        }

        protected override void Tick()
        {
            base.Tick();
            
            if (_isRotateToTarget && _isRotateWithChildFigure)
            {
                Vector3 lookDirection = (_targetToRotate - _spirit.transform.position).FlatY().normalized;

                if (lookDirection != Vector3.zero)
                {
                    _lastLocalRotationTarget = Quaternion.LookRotation(lookDirection, Vector3.up).eulerAngles.y;
                }
            }
            else
            {
                _lastLocalRotationTarget = transform.eulerAngles.y;
            }

            _spirit.transform.rotation =
                Quaternion.Slerp(
                    _spirit.transform.rotation,
                    Quaternion.Euler(_spirit.transform.rotation.eulerAngles.x, _lastLocalRotationTarget, _spirit.transform.rotation.eulerAngles.z),
                    15f * Time.deltaTime);
        }

        public override void PerformMeleeAttack()
        {
            _spirit.GetAnimator().PlayMeleeAttack();
        }
    }
}