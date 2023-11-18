using System;
using UnityEngine;

namespace Core.Units
{
    public class UnitAnimationEventsCatcher : MonoBehaviour
    {
        public event Action OnAttacked;
        public event Action OnActivateSlashEffect;
        public event Action OnAnimationFinished;
        public event Action OnDeactivateSlashEffect;

        public void RaiseOnAttackEvent()
        {
            OnAttacked?.Invoke();
        }

        public void RaiseFinishAnimationEvent()
        {
            OnAnimationFinished?.Invoke();
        }

        public void RaiseActivateSlashEffect()
        {
            OnActivateSlashEffect?.Invoke();
        }

        public void RaiseDeactivateSlashEffect()
        {
            OnDeactivateSlashEffect?.Invoke();
        }
    }
}