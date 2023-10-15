using System;
using UnityEngine;

namespace Core.Units
{
    public class UnitAnimationEventsCatcher : MonoBehaviour
    {
        public event Action OnAttacked;
        public event Action OnAnimationFinished;

        public void RaiseOnAttackEvent()
        {
            OnAttacked?.Invoke();
        }

        public void RaiseFinishAnimationEvent()
        {
            OnAnimationFinished?.Invoke();
        }
    }
}