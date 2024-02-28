using System;
using Core.Data;
using Extensions;
using UnityEngine;
using Core.Particles;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

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

        public override void Init(
            Enums.UnitType unitType, Enums.UnitClass unitClass, Enums.OutlineType outlineType)
        {
            base.Init(unitType, unitClass, outlineType);

            _raycastTrigger = GetComponentInChildren<UnitRaycastTrigger>();
            _raycastTrigger.OnClicked += () => OnUnitClicked?.Invoke();

            _weaponStyle = _stylesContainer.GetStyleOfWeaponType(_classSettings.WeaponID);

            _spirit = GetComponentInChildren<Spirit>();
            _spirit.Init(_weaponStyle, unitType, this);
            _spirit.CreateWeapon(_weaponStyle.WeaponPrefab);

            _armor = _spirit.CreateArmor();
            _lastLocalRotationTarget = _spirit.transform.localRotation.eulerAngles.y;

            _unitOutlineController.Init(_outlinesContainer.GetOutlineOfType(OutlineType));
        }

        protected override void Tick()
        {
            base.Tick();
            
            if (_isRotateChildFigure)
            {
                Vector3 lookDirection = (_targetToRotate - _spirit.transform.position).FlatY().normalized;

                if (lookDirection != Vector3.zero)
                {
                    _lastLocalRotationTarget = Quaternion.LookRotation(lookDirection, Vector3.up).eulerAngles.y;
                }
            }
            //else
            //{
            //    _lastLocalRotationTarget = transform.eulerAngles.y;
            //}

            _spirit.transform.rotation =
                Quaternion.Slerp(
                    _spirit.transform.rotation,
                    Quaternion.Euler(_spirit.transform.rotation.eulerAngles.x, _lastLocalRotationTarget, _spirit.transform.rotation.eulerAngles.z),
                    7f * Time.deltaTime);
        }

        public override void PerformMeleeAttack()
        {
            _spirit.GetAnimator().PlayMeleeAttack();
        }

        public override void SetAttackPreparationAnimation(bool isActive)
        {
            if (isActive)
            {
                ParticleSystem prepareParticle = Instantiate(_unitAssets.SimpleAttackPrepareParticle);
                prepareParticle.transform.position = _impactVFXPoint.transform.position;
            }

            _spirit.GetAnimator().SetAttackPreparationAnimation(isActive);
        }
        
        public override void PerformAttackedImpact(ParticleSystem impactVFX)
        {
            base.PerformAttackedImpact(impactVFX);

            _spirit.GetAnimator().PlayAttackedImpact();
        }

        public override void KillUnit()
        {
            base.KillUnit();

            DestructionRoutine(false);
        }

        private async void DestructionRoutine(bool isExplodeDeath)
        {
            Destroy(_raycastTrigger);

            _spirit.SetDead();

            await UniTask.Delay(400);

            _spirit.PlayDeathParticle();

            float t = 0f;

            List<Vector3> _defaultArmorPositions = new List<Vector3>();

            foreach (ThreeDObjectExploder armorItem in _armor)
            {
                _defaultArmorPositions.Add(armorItem.transform.localPosition);
            }

            while (t <= 1f)
            {
                t += Time.deltaTime;

                _spirit.SetSpiritTransparency(1f - t);

                if (isExplodeDeath)
                {
                    ShakeArmor(_defaultArmorPositions, t / 2f);
                }

                await UniTask.DelayFrame(1);
            }

            t = 0f;

            while (t <= 1f)
            {
                t += Time.deltaTime * 2f;

                if (isExplodeDeath)
                {
                    ShakeArmor(_defaultArmorPositions, t / 2f + .5f);
                }

                await UniTask.DelayFrame(1);
            }

            foreach (ThreeDObjectExploder armorItem in _armor)
            {
                if (isExplodeDeath)
                {
                    armorItem.Explode(UnityEngine.Random.insideUnitSphere.normalized.FlatY(), true, 5f);
                }
                else
                {
                    armorItem.SmoothGravityFalling(true, 5f);
                    await UniTask.Delay(UnityEngine.Random.Range(0, 200));
                }
            }

            _spirit.DestroyWeapon();

            await UniTask.Delay(1000);

            _stand.transform.SetParent(null);

            t = 0f;

            Vector3 defaultStandScale = _stand.transform.localScale;

            while (t <= 1f)
            {
                t += Time.deltaTime * 2f;

                _stand.transform.position -= Vector3.up * Time.deltaTime / 2f;
                _stand.transform.localScale = defaultStandScale * (1f - t);

                await UniTask.DelayFrame(1);
            }

            Destroy(gameObject);
        }

        private void ShakeArmor(List<Vector3> defaultPoses, float t)
        {
            float shakeIntencity = .0025f;
            float shadeSpeed = 20f;

            int counter = 0;
            float pos = 0;
            foreach (ThreeDObjectExploder armorItem in _armor)
            {
                float time = Time.time * shadeSpeed;
                armorItem.transform.localPosition = defaultPoses[counter] + new Vector3
                    (
                        Mathf.PerlinNoise(pos + time, pos + time) * shakeIntencity,
                        Mathf.PerlinNoise((pos + 100f) + time, (pos + 100f) + time) * shakeIntencity,
                        Mathf.PerlinNoise((pos + 200f) + time, (pos + 200f) + time) * shakeIntencity
                    ) * (t / 2f);

                pos += .4f;
                counter++;
            }
        }
    }
}