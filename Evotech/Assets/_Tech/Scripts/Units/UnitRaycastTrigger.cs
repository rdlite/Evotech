using System;
using UnityEngine;

namespace Core.Units
{
    public class UnitRaycastTrigger : MonoBehaviour
    {
        public event Action OnClicked;

        public void RaiseUnitClickEvent()
        {
            OnClicked?.Invoke();
        }
    }
}