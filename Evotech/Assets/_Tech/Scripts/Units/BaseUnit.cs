using Core.Data;
using Qnject;
using UnityEngine;

namespace Core.Units
{
    [RequireComponent(typeof(BaseUnitAnimator))]
    public abstract class BaseUnit : MonoBehaviour
    {
        [SerializeField] protected UnitSettings _unitSettings;

        protected BaseUnitAnimator _baseAnimator;
        protected StylesContainer _stylesContainer;

        [Inject]
        private void Construct(StylesContainer stylesContainer)
        {
            _stylesContainer = stylesContainer;
        }

        public virtual void Init()
        {
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