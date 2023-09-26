using System;

namespace Core.Units
{
    public class TestUnit : BaseUnit
    {
        public event Action OnUnitClicked;

        private UnitRaycastTrigger _raycastTrigger;

        public void Init()
        {
            _raycastTrigger = GetComponentInChildren<UnitRaycastTrigger>();
            _raycastTrigger.OnClicked += () => OnUnitClicked?.Invoke();
        }
    }
}