using Utils;
using Qnject;
using System;
using Core.Data;
using UnityEngine;
using Core.Data.Skills;
using QOutline.Configs;
using Core.Battle.Outline;
using System.Collections.Generic;

namespace Core.Units
{
    [RequireComponent(typeof(GhostCreator))]
    [RequireComponent(typeof(BaseUnitAnimator))]
    public abstract class BaseUnit : MonoBehaviour
    {
        public event Action<BaseUnit> OnDead;

        public bool IsDead { get; private set; }
        public Enums.UnitType UnitType { get; private set; }
        public Enums.UnitClass UnitClass { get; private set; }
        public Enums.OutlineType OutlineType { get; private set; }
        public IUnitDynamicStatsProvider DynamicStatsProvider { get; private set; }
        public IUnitStaticStatsProvider StaticStatsProvider { get; private set; }

        [SerializeField] protected Enums.UnitGeneralType _unitGeneralType;
        [SerializeField] protected Transform _upperHeadPoint, _impactVFXPoint;
        [SerializeField] protected Transform _stand;

        protected ClassSettings _classSettings;
        protected BaseUnitAnimator _baseAnimator;
        protected UnitAssets _unitAssets;
        protected StylesContainer _stylesContainer;
        protected IUpdateProvider _updatesProvider;
        protected GhostCreator _ghostCreator;
        protected UnitOutlineController _unitOutlineController;
        protected Vector3 _targetToRotate;
        protected UnitAnimationEventsCatcher _animationEventsCatcher;
        protected UnitSettingsContainer _unitSettingsContainer;
        protected OutlinesContainer _outlinesContainer;
        protected bool _isRotateChildFigure;

        [Inject]
        private void Construct(
            StylesContainer stylesContainer, IUpdateProvider updatesProvider, OutlinesContainer outlinesContainer,
            UnitSettingsContainer unitSettingsContainer, IUnitStaticStatsProvider statsProvider, AssetsContainer assetsContainer)
        {
            _unitAssets = assetsContainer.UnitAssets;
            _stylesContainer = stylesContainer;
            _updatesProvider = updatesProvider;
            _unitSettingsContainer = unitSettingsContainer;
            _outlinesContainer = outlinesContainer;
            StaticStatsProvider = statsProvider;

            _baseAnimator = GetComponent<BaseUnitAnimator>();
            _ghostCreator = GetComponent<GhostCreator>();
            _animationEventsCatcher = GetComponentInChildren<UnitAnimationEventsCatcher>();
            _unitOutlineController = GetComponentInChildren<UnitOutlineController>();

            _updatesProvider.AddUpdate(Tick);
        }

        public virtual void Init(
            Enums.UnitType unitType, Enums.UnitClass unitClass, Enums.OutlineType outlineType)
        {
            UnitType = unitType;
            UnitClass = unitClass;
            OutlineType = outlineType;

            _classSettings = _unitSettingsContainer.GetSettingsOfClassType(unitClass);

            CreateStatsModel();
        }

        private void OnEnable()
        {
            _updatesProvider?.AddUpdate(Tick);
        }

        private void OnDisable()
        {
            _updatesProvider?.RemoveUpdate(Tick);
        }

        protected virtual void Tick()
        {
            //if (_isRotateToTarget)
            //{
            //    Vector3 lookDirection = (_targetToRotate - transform.position).FlatY().normalized;

            //    if (lookDirection != Vector3.zero)
            //    {
            //        float yRotation = Quaternion.LookRotation(lookDirection, Vector3.up).eulerAngles.y;
            //        yRotation = (int)yRotation / 60;
            //        yRotation = (int)yRotation * 60;
            //        _lastRotationTarget = yRotation;
            //    }
            //}

            //transform.rotation =
            //    Quaternion.Slerp(
            //        transform.rotation,
            //        Quaternion.Euler(transform.rotation.eulerAngles.x, _lastRotationTarget, transform.rotation.eulerAngles.z),
            //        10f * Time.deltaTime);
        }

        public void SetTargetRotation(Vector3 targetPoint)
        {
            _isRotateChildFigure = targetPoint != Vector3.zero;
            _targetToRotate = targetPoint;
        }

        public void SetActiveOutline(bool value, bool interactWithSnap)
        {
            if (interactWithSnap && !value)
            {
                _unitOutlineController.SnapOutline(false);
            }

            if (value)
            {
                _unitOutlineController.AddObjectsToBatch();
            }
            else
            {
                _unitOutlineController.RemoveObjectsFromBatch();
            }

            if (interactWithSnap && value)
            {
                _unitOutlineController.SnapOutline(true);
            }
        }

        public virtual void KillUnit()
        {
            OnDead?.Invoke(this);
            IsDead = true;
        }

        public abstract void PerformMeleeAttack();

        public abstract void SetAttackPreparationAnimation(bool isActive);

        public virtual void PerformAttackedImpact(ParticleSystem impactVFX)
        {
            ParticleSystem newImpactVFX = Instantiate(impactVFX);
            newImpactVFX.transform.position = _impactVFXPoint.transform.position;
        }

        private void CreateStatsModel()
        {
            UnitStatsModel statsModel = new UnitStatsModel
            {
                MaxHealth = StaticStatsProvider.GetMaxHealth(_unitGeneralType, UnitClass),
                CurrentHealth = StaticStatsProvider.GetMaxHealth(_unitGeneralType, UnitClass),
                MaxArmor = StaticStatsProvider.GetMaxArmor(_unitGeneralType, UnitClass),
                CurrentArmor = StaticStatsProvider.GetMaxArmor(_unitGeneralType, UnitClass),
                WalkRange = StaticStatsProvider.GetWalkRange(_unitGeneralType, UnitClass),
                MaxInitiative = StaticStatsProvider.GetMaxInitiative(_unitGeneralType, UnitClass),
                CurrentInitiative = StaticStatsProvider.GetMaxInitiative(_unitGeneralType, UnitClass),
            };

            if (UnitType == Enums.UnitType.Player)
            {
                statsModel.MaxInitiative *= 2f;
                statsModel.CurrentInitiative = statsModel.MaxInitiative;
            }
            else if (UnitType == Enums.UnitType.EnemyRed || UnitType == Enums.UnitType.EnemyNeutral)
            {
                statsModel.MaxInitiative *= 1.5f * UnityEngine.Random.Range(.8f, 1.2f);
                statsModel.CurrentInitiative = statsModel.MaxInitiative;
            }

            DynamicStatsProvider = new UnitDynamicStatsProvider(statsModel);
            DynamicStatsProvider.OnHealthZeroed += KillUnit;
        }

        public float GetWalkRange()
        {
            return StaticStatsProvider.GetWalkRange(_unitGeneralType, UnitClass);
        }

        public float GetAttackDamage()
        {
            return StaticStatsProvider.GetRandomizedAttackDamage(_unitGeneralType, UnitClass);
        }

        public (float, float) GetDecomposedAttackDamage()
        {
            return StaticStatsProvider.GetDecomposedDamage(_unitGeneralType, UnitClass);
        }

        public BaseUnitAnimator GetBaseAnimator()
        {
            return _baseAnimator;
        }

        public GhostCopy CreateGhostCopy()
        {
            return _ghostCreator.CreateGhostCopy();
        }

        public UnitAnimationEventsCatcher GetEventsCatcher()
        {
            return _animationEventsCatcher;
        }

        public Transform GetUpperHeadPoint()
        {
            return _upperHeadPoint;
        }

        public List<UnitBaseSkill> GetCurrentSkills()
        {
            List<UnitBaseSkill> skills = new List<UnitBaseSkill>();
            skills.AddRange(_unitSettingsContainer.GetUnitGeneralSettingsType(_unitGeneralType).UnitSkills);

            foreach (var skill in _unitSettingsContainer.GetSettingsOfClassType(UnitClass).UnitSkills)
            {
                if (!skills.Contains(skill))
                {
                    skills.Add(skill);
                }
            }

            skills.Sort((s1, s2) => s1.SortingOrder > s2.SortingOrder ? 1 : (s1.SortingOrder < s2.SortingOrder ? -1 : 0));

            return skills;
        }
    }
}