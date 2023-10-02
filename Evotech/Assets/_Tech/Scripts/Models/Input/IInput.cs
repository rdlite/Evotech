using System;
using UnityEngine;

namespace Core.InputSystem
{
    public interface IInput
    {
        event Action OnLMBDown;
        event Action OnLMBUp;
        event Action OnRMBDown;
        event Action OnRMBUp;
        Vector2 GetAxis();
        Vector2 GetMouseVelocity();
        Vector2 GetMousePos();
        float GetMouseWheel();
        bool IsWheelPressed();
    }
}