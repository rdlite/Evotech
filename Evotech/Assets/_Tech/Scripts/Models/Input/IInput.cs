using UnityEngine;

namespace Core.InputSystem
{
    public interface IInput
    {
        Vector2 GetAxis();
        Vector2 GetMouseVelocity();
        Vector2 GetMousePos();
        float GetMouseWheel();
        bool IsWheelPressed();
    }
}