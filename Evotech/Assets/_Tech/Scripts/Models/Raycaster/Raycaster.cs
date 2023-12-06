using Core.InputSystem;
using Core.Settings;
using Core.Units;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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

        public bool IsPointerOverUI()
        {
            return GetEventSystemRaycastResults().Count > 0;
        }

        private List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = _input.GetMousePos();
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
    }
}