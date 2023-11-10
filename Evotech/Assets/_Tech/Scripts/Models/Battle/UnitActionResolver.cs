using Core.Data;
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
            _actionDesc.Actor.GetEventsCatcher().OnAnimationFinished += FinishAction;
            PerformAction();
        }

        private void PerformAction()
        {
            if (_actionDesc is ActionMeleeAttack)
            {
                _actionDesc.Actor.GetEventsCatcher().OnAttacked += ActionEvent;
                _actionDesc.Actor.PerformMeleeAttack();
                if (_actionDesc.SubjectUnits.Count == 1)
                {
                    _actionDesc.SubjectUnits[0].SetTargetRotation(_actionDesc.Actor.transform.position, false);
                }
            }
        }

        private void FinishAction()
        {
            UnsnapActorRotation();

            if (_actionDesc is ActionMeleeAttack)
            {
                _actionDesc.Actor.GetEventsCatcher().OnAttacked -= ActionEvent;
                if (_actionDesc.SubjectUnits.Count == 1)
                {
                    _actionDesc.SubjectUnits[0].SetTargetRotation(Vector3.zero, false);
                }
            }

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
            if (_actionDesc is ActionMeleeAttack)
            {
                foreach (var unitsInAttack in _actionDesc.SubjectUnits)
                {
                    unitsInAttack.DynamicStatsProvider.TakeDamage(new InstantDamageInfo
                    {
                        Damage = (_actionDesc as ActionMeleeAttack).Damage
                    });
                    unitsInAttack.PerformAttackedImpact();
                }
            }
        }
    }
}