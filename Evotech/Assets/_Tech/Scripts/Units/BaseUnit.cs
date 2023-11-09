using Core.Data;
using Utils;
using Qnject;
using Extensions;
using UnityEngine;
using Core.Battle.Outline;
using QOutline.Configs;

namespace Core.Units
{
    [RequireComponent(typeof(GhostCreator))]
    [RequireComponent(typeof(BaseUnitAnimator))]
    public abstract class BaseUnit : MonoBehaviour
    {
        public Enums.UnitType UnitType { get; private set; }
        public Enums.OutlineType OutlineType { get; private set; }

        [SerializeField] protected UnitSettings _unitSettings;

        protected BaseUnitAnimator _baseAnimator;
        protected StylesContainer _stylesContainer;
        protected IUpdateProvider _updatesProvider;
        protected GhostCreator _ghostCreator;
        protected UnitOutlineController _unitOutlineController;
        protected Vector3 _targetToRotate;
        private UnitAnimationEventsCatcher _animationEventsCatcher;
        private float _lastRotationTarget;
        protected bool _isRotateToTarget;
        protected bool _isRotateWithChildFigure;

        [Inject]
        private void Construct(
            StylesContainer stylesContainer, IUpdateProvider updatesProvider, OutlinesContainer outlinesContainer)
        {
            _stylesContainer = stylesContainer;
            _updatesProvider = updatesProvider;

            _baseAnimator = GetComponent<BaseUnitAnimator>();
            _ghostCreator = GetComponent<GhostCreator>();
            _animationEventsCatcher = GetComponentInChildren<UnitAnimationEventsCatcher>();
            _unitOutlineController = GetComponentInChildren<UnitOutlineController>();

            _unitOutlineController.Init(outlinesContainer.GetOutlineOfType(OutlineType));

            _updatesProvider.AddUpdate(Tick);
        }

        public virtual void Init(
            Enums.UnitType unitType, Enums.OutlineType outlineType)
        {
            UnitType = unitType;
            OutlineType = outlineType;
            _lastRotationTarget = transform.eulerAngles.y;
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

        public abstract void PerformMeleeAttack();

        public float GetWalkRange()
        {
            return _unitSettings.WalkDistance;
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
    }
}