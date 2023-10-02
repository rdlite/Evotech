using System;
using UnityEngine;

namespace Core.Units
{
    public class UnitRaycastTrigger : MonoBehaviour
    {
        public BaseUnit ParentUnit;

        public event Action OnClicked;

        private void Awake()
        {
            ParentUnit = GetComponentInParent<BaseUnit>();
        }

        public void RaiseUnitClickEvent()
        {
            OnClicked?.Invoke();
        }
    }
}