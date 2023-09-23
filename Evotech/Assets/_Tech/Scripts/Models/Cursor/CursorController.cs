using UnityEngine;
using Utils;

namespace Core.InputSystem
{
    public class CursorController : ICursorController
    {
        private IInput _input;

        public CursorController(IInput input)
        {
            _input = input;
        }

        public void SetLockedCursor(bool isLocked)
        {
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}