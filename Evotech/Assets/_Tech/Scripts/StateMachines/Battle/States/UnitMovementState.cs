using System;
using Core.Units;
using Hexnav.Core;
using UnityEngine;
using Core.Cameras;
using Core.Settings;

namespace Core.StateMachines.Battle
{
    public class UnitMovementState : IUpdateState, IPayloadState<UnitMovementState.MovementData>
    {
        private readonly StateMachine _battleSM;
        private readonly BattleSettings _battleSettings;
        private readonly CameraController _camera;
        private MovementData _movementData;
        private BaseUnit _unitToAnimate;
        private NodeBase _startNode, _endNode;
        private float _t;
        private bool _isCameraMoved;
        private bool _isPlayedShakeAnimation;

        public UnitMovementState(
            StateMachine battleSM, BattleSettings battleSettings, CameraController camera)
        {
            _battleSM = battleSM;
            _battleSettings = battleSettings;
            _camera = camera;
        }

        public void Enter(MovementData data)
        {
            _movementData = data;
            _unitToAnimate = _movementData.Unit;
            _startNode = _movementData.Start;
            _endNode = _movementData.End;

            _startNode.NonwalkableFactors--;
            _endNode.NonwalkableFactors++;

            _t = 0f;
            _isCameraMoved = false;
            _isPlayedShakeAnimation = false;
        }

        public void Update()
        {
            _t += Time.deltaTime / _battleSettings.MovementDuration;

            //MoveWithGap();
            MoveByArc();
        }

        public void Exit() { }

        private void MoveByArc()
        {
            _unitToAnimate.transform.position = Vector3.Lerp(
                new Vector3(
                        _startNode.WorldPos.x,
                        (_startNode.WorldPos + _startNode.SurfaceOffset).y,
                        _startNode.WorldPos.z), 
                new Vector3(
                        _endNode.WorldPos.x,
                        (_endNode.WorldPos + _endNode.SurfaceOffset).y,
                        _endNode.WorldPos.z),
                _t) + Vector3.up * 3f * _battleSettings.ArcMovementCurve.Evaluate(_t);

            if (!_isCameraMoved)
            {
                _isCameraMoved = true;
                _camera.SetSmoothLookupPoint(
                    new Vector3(
                        _endNode.WorldPos.x,
                        (_endNode.WorldPos + _endNode.SurfaceOffset).y,
                        _endNode.WorldPos.z));
            }

            if (_t >= 1f)
            {
                FinishMovement();
            }
            else if (_t >= .95f && !_isPlayedShakeAnimation)
            {
                _isPlayedShakeAnimation = true;
                _unitToAnimate.GetBaseAnimator().PlayPlacedShake();
            }
        }

        private void MoveWithGap()
        {
            if (_t < .3f)
            {
                _unitToAnimate.transform.position = Vector3.Lerp(
                    new Vector3(
                        _unitToAnimate.transform.position.x,
                        (_startNode.WorldPos + _startNode.SurfaceOffset).y,
                        _unitToAnimate.transform.position.z),
                    new Vector3(
                        _unitToAnimate.transform.position.x,
                        (_startNode.WorldPos + _startNode.SurfaceOffset).y + _battleSettings.MaxUpMovementHeight,
                        _unitToAnimate.transform.position.z),
                    _battleSettings.MovementSmoothToUp.Evaluate(Mathf.InverseLerp(0f, .3f, _t)));
            }
            else if (_t >= .3f && _t < .7f)
            {
                _unitToAnimate.gameObject.SetActive(false);

                if (!_isCameraMoved)
                {
                    _isCameraMoved = true;

                    _camera.SetSmoothLookupPoint(_endNode.WorldPos + _endNode.SurfaceOffset);
                }
            }
            else
            {
                _unitToAnimate.gameObject.SetActive(true);

                _unitToAnimate.transform.position = Vector3.Lerp(
                    new Vector3(
                        _endNode.WorldPos.x,
                        (_endNode.WorldPos + _endNode.SurfaceOffset).y + _battleSettings.MaxUpMovementHeight,
                        _endNode.WorldPos.z),
                    new Vector3(
                        _endNode.WorldPos.x,
                        (_endNode.WorldPos + _endNode.SurfaceOffset).y,
                        _endNode.WorldPos.z),
                    _battleSettings.MovementSmoothToDown.Evaluate(Mathf.InverseLerp(.7f, 1f, _t)));

                if (_t >= .76f && !_isPlayedShakeAnimation)
                {
                    _isPlayedShakeAnimation = true;

                    _unitToAnimate.GetBaseAnimator().PlayPlacedShake();
                }

                if (_t >= 1f)
                {
                    FinishMovement();
                }
            }
        }

        private void FinishMovement()
        {
            _battleSM.Enter<WaitingForTurnState>();
            ParticleSystem dustFX = UnityEngine.Object.Instantiate(_battleSettings.PlacingParticle);
            dustFX.transform.position = _unitToAnimate.transform.position;
            _movementData.Callback?.Invoke();
        }

        public class MovementData
        {
            public BaseUnit Unit;
            public NodeBase Start, End;
            public Action Callback;

            public MovementData(BaseUnit unit, NodeBase start, NodeBase end, Action callback)
            {
                Unit = unit;
                Start = start;
                End = end;
                Callback = callback;
            }
        }
    }
}