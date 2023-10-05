using Utils;
using Qnject;
using Extensions;
using UnityEngine;
using Core.Settings;
using Core.InputSystem;
using Core.Data;

namespace Core.Cameras
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _cameraRotationContainer;

        private CameraSettings _cameraSettings;
        private ICursorController _cursorController;
        private IUpdateProvider _updateProvider;
        private IMapDataProvider _mapDataProvider;
        private Quaternion _targetRotation;
        private IInput _input;
        private Vector3 _targetVelocityMovement;
        private Vector3 _targetMovement;
        private Vector3 _smoothLookupPoint;
        private float _currentZooming;
        private float _currentZoomingVelocity;
        private bool _hasSmoothBreakableLookupPoint;

        [Inject]
        private void Contruct(
            GameSettings gameSettigns, IUpdateProvider updateProvider, IInput input,
            ICursorController cursorController, IMapDataProvider mapDataProvider)
        {
            _cameraSettings = gameSettigns.CameraSettings;
            _cursorController = cursorController;
            _updateProvider = updateProvider;
            _mapDataProvider = mapDataProvider;

            _cameraRotationContainer.localRotation = Quaternion.Euler(_cameraSettings.RotationOffset);
            _camera.fieldOfView = _cameraSettings.DefaultFOV;
            _updateProvider.AddLateUpdate(LateTick);
            _input = input;
            _currentZoomingVelocity = 1f;
            _currentZooming = 1f;
            _targetRotation = transform.rotation;
        }

        private void OnEnable()
        {
            _updateProvider?.AddLateUpdate(LateTick);
        }

        private void OnDisable()
        {
            _updateProvider.RemoveLateUpdate(LateTick);
        }

        private void LateTick()
        {
            HandleZoom();
            HandleMovement();
            HandleRotation();
            if (_cameraSettings.IsChangeHeight)
            {
                HandleHeight();
            }
            ClampPosition();
        }

        public void SetInstanLookupPoint(Vector3 point)
        {
            transform.position = point;
        }

        public void SetSmoothLookupPoint(Vector3 point)
        {
            _hasSmoothBreakableLookupPoint = true;
            _smoothLookupPoint = point;
        }

        private void HandleZoom()
        {
            _currentZooming -= _input.GetMouseWheel();
            _currentZooming = Mathf.Clamp(_currentZooming, _cameraSettings.MinZoom, _cameraSettings.MaxZoom);
            _currentZoomingVelocity = Mathf.Lerp(_currentZoomingVelocity, _currentZooming, _cameraSettings.ZoomSmooth * Time.deltaTime);

            _cameraRotationContainer.localPosition = _cameraSettings.PositionOffset * _currentZoomingVelocity;
            _camera.transform.localRotation = Quaternion.Lerp(
                _camera.transform.localRotation,
                Quaternion.Euler(
                    Mathf.Lerp(
                        _cameraSettings.MaxRotationCameraOnZoom, 
                        -_cameraSettings.MaxRotationCameraOnZoom, 
                        Mathf.InverseLerp(
                            _cameraSettings.MinZoom, 
                            _cameraSettings.MaxZoom, 
                            _currentZoomingVelocity)),
                    0f,
                    0f),
                Time.deltaTime * _cameraSettings.RotationSpeed);
        }

        private void HandleMovement()
        {
            Vector3 inputAxis = new Vector3(_input.GetAxis().x, 0f, _input.GetAxis().y);

            _targetMovement = Quaternion.Euler(_camera.transform.eulerAngles.FlatX()) * inputAxis;

            if (inputAxis != Vector3.zero && _hasSmoothBreakableLookupPoint)
            {
                BreakSmoothLookup();
            }
            
            if (inputAxis == Vector3.zero && _hasSmoothBreakableLookupPoint)
            {
                HandleSmoothMovementToPoint();
            }
            else if (!_hasSmoothBreakableLookupPoint)
            {
                _targetVelocityMovement = Vector3.Lerp(_targetVelocityMovement, _targetMovement, _cameraSettings.MovementSmooth * Time.deltaTime);
                transform.position += _targetVelocityMovement.FlatY() * _cameraSettings.MovementSpeed * _currentZoomingVelocity * Time.deltaTime;
            }
        }

        private void HandleSmoothMovementToPoint()
        {
            Vector3 movemetnDirection = (_smoothLookupPoint - transform.position).FlatY().normalized;
            _targetVelocityMovement = Vector3.Lerp(_targetVelocityMovement, movemetnDirection, _cameraSettings.MovementSmooth * Time.deltaTime);
            transform.position += _targetVelocityMovement.FlatY() * _cameraSettings.MovementSpeed * _currentZoomingVelocity * Time.deltaTime * 2f;

            if (Vector3.Distance(transform.position.FlatY(), _smoothLookupPoint.FlatY()) < .5f)
            {
                BreakSmoothLookup();
            }
        }

        private void HandleRotation()
        {
            if (_input.IsWheelPressed())
            {
                BreakSmoothLookup();

                _cursorController.SetLockedCursor(true);

                _targetRotation *= Quaternion.Euler(
                    0f,
                    _input.GetMouseVelocity().x * Time.deltaTime * _cameraSettings.RotationSmooth,
                    0f);
            }
            else
            {
                _cursorController.SetLockedCursor(false);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _cameraSettings.RotationSpeed * Time.deltaTime);
        }

        private void HandleHeight()
        {
            transform.position = Vector3.Lerp(
                transform.position,
                new Vector3(
                    transform.position.x,
                    _mapDataProvider.GetHeightOfWorldPoint(transform.position),
                    transform.position.z),
                _cameraSettings.HeightChangeSmooth * Time.deltaTime);
        }

        private void ClampPosition()
        {
            Vector3 ldPos = _mapDataProvider.GetBorders().Item1;
            Vector3 ruPos = _mapDataProvider.GetBorders().Item2;

            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, ldPos.x, ruPos.x),
                transform.position.y,
                Mathf.Clamp(transform.position.z, ldPos.z, ruPos.z));
        }

        private void BreakSmoothLookup()
        {
            _hasSmoothBreakableLookupPoint = false;
        }

        public Camera GetCamera()
        {
            return _camera;
        }
    }
}