using UnityEngine;

namespace Core.Units
{
    [RequireComponent(typeof(BaseUnitAnimator))]
    public abstract class BaseUnit : MonoBehaviour
    {
        protected BaseUnitAnimator _baseAnimator;

        protected virtual void Awake()
        {
            _baseAnimator = GetComponent<BaseUnitAnimator>();
        }

        public float GetWalkRange()
        {
            return Random.Range(5f, 10f);
        }

        public BaseUnitAnimator GetBaseAnimator()
        {
            return _baseAnimator;
        }
    }
}