using System;
using UnityEngine;
using Utils;

namespace Core.InputSystem
{
    public class InputController : IInput
    {
        public event Action OnLMBDown;
        public event Action OnLMBUp;
        public event Action OnRMBDown;
        public event Action OnRMBUp;

        private const string HORIZONTAL_AXIS = "Horizontal";
        private const string VERTICAL_AXIS = "Vertical";
        private const string MOUSE_X_OFFSET = "Mouse X";
        private const string MOUSE_Y_OFFSET = "Mouse Y";
        private const string MOUSE_SCROLLWHEEL = "Mouse ScrollWheel";
        private const float MOUSE_VELOCITY_ACCELERATION = 10f;

        private Vector2 _mouseVelocity;
        private Vector2 _mousePos;
        private Vector2 _lastMousePos;
        private Vector2 _axisMove;
        private float _scrollValue;
        private bool _isWheelPressed;

        public InputController(IUpdateProvider updateProvider)
        {
            updateProvider.AddUpdate(Update);
        }

        private void Update()
        {
            _axisMove = new Vector2(Input.GetAxis(HORIZONTAL_AXIS), Input.GetAxis(VERTICAL_AXIS));
            _scrollValue = Input.GetAxis(MOUSE_SCROLLWHEEL);
            _isWheelPressed = Input.GetMouseButton(2);
            _mouseVelocity = new Vector2(Input.GetAxis(MOUSE_X_OFFSET), Input.GetAxis(MOUSE_Y_OFFSET)) * MOUSE_VELOCITY_ACCELERATION;
            _mousePos = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                OnLMBDown?.Invoke();
            }

            if (Input.GetMouseButtonUp(0))
            {
                OnLMBUp?.Invoke();
            }

            if (Input.GetMouseButtonDown(1))
            {
                OnRMBDown?.Invoke();
            }

            if (Input.GetMouseButtonUp(1))
            {
                OnRMBUp?.Invoke();
            }
        }

        public Vector2 GetMouseVelocity()
        {
            return _mouseVelocity;
        }

        public Vector2 GetAxis()
        {
            return _axisMove;
        }

        public bool IsWheelPressed()
        {
            return _isWheelPressed;
        }

        public float GetMouseWheel()
        {
            return _scrollValue;
        }

        public Vector2 GetMousePos()
        {
            return _mousePos;
        }
    }
}