using Core.InputSystem;
using Core.Settings;
using Core.Units;
using UnityEngine;

namespace Utils
{
    public class Raycaster : IRaycaster
    {
        private readonly LayersSettings _layersSettings;
        private readonly IInput _input;

        public Raycaster(LayersSettings layersSettings, IInput input)
        {
            _layersSettings = layersSettings;
            _input = input;
        }

        public UnitRaycastTrigger GetUnitTrigger(Camera camera)
        {
            RaycastHit hitInfo;
            Physics.Raycast(camera.ScreenPointToRay(_input.GetMousePos()), out hitInfo, Mathf.Infinity, _layersSettings.UnitsMask);

            if (hitInfo.transform != null)
            {
                return hitInfo.transform.GetComponent<UnitRaycastTrigger>();
            }

            return null;
        }

        public bool GetGroundPos(Camera camera, out Vector3 groundPos)
        {
            RaycastHit hitInfo;
            Physics.Raycast(camera.ScreenPointToRay(_input.GetMousePos()), out hitInfo, Mathf.Infinity, _layersSettings.GroundMask);

            groundPos = Vector3.zero;

            if (hitInfo.transform != null)
            {
                groundPos = hitInfo.point;
            }

            return hitInfo.transform != null;
        }
    }
}