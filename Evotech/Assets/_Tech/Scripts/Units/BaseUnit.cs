using Utils;
using Qnject;
using Core.Data;
using Extensions;
using UnityEngine;
using QOutline.Configs;
using Core.Battle.Outline;

namespace Core.Units
{
    [RequireComponent(typeof(GhostCreator))]
    [RequireComponent(typeof(BaseUnitAnimator))]
    public abstract class BaseUnit : MonoBehaviour
    {
        public Enums.UnitType UnitType { get; private set; }
        public Enums.UnitClass UnitClass { get; private set; }
        public Enums.OutlineType OutlineType { get; private set; }
        public IUnitDynamicStatsProvider DynamicStatsProvider { get; private set; }
        public IUnitStaticStatsProvider StaticStatsProvider { get; private set; }

        [SerializeField] private Enums.UnitGeneralType _unitGeneralType;

        protected ClassSettings _unitSettings;
        protected BaseUnitAnimator _baseAnimator;
        protected StylesContainer _stylesContainer;
        protected IUpdateProvider _updatesProvider;
        protected GhostCreator _ghostCreator;
        protected UnitOutlineController _unitOutlineController;
        protected Vector3 _targetToRotate;
        private UnitAnimationEventsCatcher _animationEventsCatcher;
        private UnitSettingsContainer _unitSettingsContainer;
        private float _lastRotationTarget;
        protected bool _isRotateToTarget;
        protected bool _isRotateWithChildFigure;

        [Inject]
        private void Construct(
            StylesContainer stylesContainer, IUpdateProvider updatesProvider, OutlinesContainer outlinesContainer,
            UnitSettingsContainer unitSettingsContainer, IUnitStaticStatsProvider statsProvider)
        {
            _stylesContainer = stylesContainer;
            _updatesProvider = updatesProvider;
            _unitSettingsContainer = unitSettingsContainer;
            StaticStatsProvider = statsProvider;

            _baseAnimator = GetComponent<BaseUnitAnimator>();
            _ghostCreator = GetComponent<GhostCreator>();
            _animationEventsCatcher = GetComponentInChildren<UnitAnimationEventsCatcher>();
            _unitOutlineController = GetComponentInChildren<UnitOutlineController>();

            _unitOutlineController.Init(outlinesContainer.GetOutlineOfType(OutlineType));

            _updatesProvider.AddUpdate(Tick);
        }

        public virtual void Init(
            Enums.UnitType unitType, Enums.UnitClass unitClass, Enums.OutlineType outlineType)
        {
            UnitType = unitType;
            UnitClass = unitClass;
            OutlineType = outlineType;

            _unitSettings = _unitSettingsContainer.GetUnitSettingsOfClassType(unitClass);

            _lastRotationTarget = transform.eulerAngles.y;

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
            if (_isRotateToTarget)
            {
                Vector3 lookDirection = (_targetToRotate - transform.position).FlatY().normalized;

                if (lookDirection != Vector3.zero)
                {
                    float yRotation = Quaternion.LookRotation(lookDirection, Vector3.up).eulerAngles.y;
                    yRotation = (int)yRotation / 60;
                    yRotation = (int)yRotation * 60;
                    _lastRotationTarget = yRotation;
                }
            }

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.Euler(transform.rotation.eulerAngles.x, _lastRotationTarget, transform.rotation.eulerAngles.z),
                    10f * Time.deltaTime);
        }

        public void SetTargetRotation(Vector3 targetPoint, bool isRotateWithChildFigure)
        {
            _isRotateWithChildFigure = isRotateWithChildFigure;

            if (targetPoint != Vector3.zero)
            {
                _isRotateToTarget = true;
                _targetToRotate = targetPoint;
            }
            else
            {
                _isRotateToTarget = false;
            }
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
            Destroy(gameObject);
        }

        public abstract void PerformMeleeAttack();

        public abstract void PerformAttackedImpact();

        private void CreateStatsModel()
        {
            UnitStatsModel statsModel = new UnitStatsModel
            {
                MaxHealth = StaticStatsProvider.GetMaxHealth(_unitGeneralType, UnitClass),
                CurrentHealth = StaticStatsProvider.GetMaxHealth(_unitGeneralType, UnitClass),
                MaxArmor = 10f,
                CurrentArmor = 10f,
                WalkRange = StaticStatsProvider.GetWalkRange(_unitGeneralType, UnitClass)
            };

            DynamicStatsProvider = new UnitDynamicStatsProvider(statsModel);
            DynamicStatsProvider.OnHealthZeroed += KillUnit;
        }

        public float GetWalkRange()
        {
            return StaticStatsProvider.GetWalkRange(_unitGeneralType, UnitClass);
        }

        public float GetAtackDamage()
        {
            return StaticStatsProvider.GetAtackDamage(_unitGeneralType, UnitClass);
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
    }
}