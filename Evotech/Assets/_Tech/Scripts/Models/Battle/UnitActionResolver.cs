using System;
using UnityEngine;

namespace Core.Battle
{
    public class UnitActionResolver
    {
        public event Action OnFinished;

        private ActionInfo _actionDesc;

        public void Resolve(ActionInfo actionDesc)
        {
            _actionDesc = actionDesc;
            SetActorRotationSnap();
            _actionDesc.Actor.GetEventsCatcher().OnAttacked += ActionEvent;
            _actionDesc.Actor.GetEventsCatcher().OnAnimationFinished += FinishAction;
            PerformAction();
        }

        private void PerformAction()
        {
            if (_actionDesc is ActionMeleeAttack)
            {
                _actionDesc.Actor.PerformMeleeAttack();
            }
        }

        private void FinishAction()
        {
            UnsnapActorRotation();
            _actionDesc.Actor.GetEventsCatcher().OnAttacked -= ActionEvent;
            _actionDesc.Actor.GetEventsCatcher().OnAnimationFinished -= FinishAction;
            OnFinished?.Invoke();
        }

        private void SetActorRotationSnap()
        {
            if (_actionDesc.SubjectUnits.Count == 1)
            {
                _actionDesc.Actor.SetTargetRotation(_actionDesc.SubjectUnits[0].transform.position, true);
            }
        }

        private void UnsnapActorRotation()
        {
            _actionDesc.Actor.SetTargetRotation(Vector3.zero, false);
        }

        private void ActionEvent()
        {
        }
    }
}