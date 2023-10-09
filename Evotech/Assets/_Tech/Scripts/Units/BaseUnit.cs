using Core.Data;
using Qnject;
using UnityEngine;

namespace Core.Units
{
    [RequireComponent(typeof(BaseUnitAnimator))]
    public abstract class BaseUnit : MonoBehaviour
    {
        public Enums.UnitType UnitType { get; private set; }

        [SerializeField] protected UnitSettings _unitSettings;

        protected BaseUnitAnimator _baseAnimator;
        protected StylesContainer _stylesContainer;

        [Inject]
        private void Construct(StylesContainer stylesContainer)
        {
            _stylesContainer = stylesContainer;
        }

        public virtual void Init(Enums.UnitType unitType)
        {
            UnitType = unitType;
            _baseAnimator = GetComponent<BaseUnitAnimator>();
        }

        public float GetWalkRange()
        {
            return _unitSettings.WalkDistance;
        }

        public BaseUnitAnimator GetBaseAnimator()
        {
            return _baseAnimator;
        }
    }
}