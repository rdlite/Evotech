using Core.Units;
using UnityEngine;

namespace Utils
{
    public interface IRaycaster
    {
        UnitRaycastTrigger GetUnitTrigger(Camera camera);
        bool GetGroundPos(Camera camera, out Vector3 groundPos);
        bool IsPointerOverUI();
    }
}