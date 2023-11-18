using Utils;
using Core.Factories;
using Core.Data.Camera;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Cameras
{
    public class CameraShaker : ICameraShaker
    {
        private List<ShakeEvent> _currentEvents = new List<ShakeEvent>();
        private CameraShakesContainer _shakesContainer;
        private IUpdateProvider _updateProvider;
        private IGameFactory _gameFactory;

        public CameraShaker(CameraShakesContainer shakesContainer, IUpdateProvider updateProvider, IGameFactory gameFactory)
        {
            _shakesContainer = shakesContainer;
            _updateProvider = updateProvider;
            _gameFactory = gameFactory;
            updateProvider.AddUpdate(Tick);
        }

        public void Shake(string tag, float overridePower = 1f)
        {
            CameraShakeConfigs shakeConfig = _shakesContainer.GetShakeOfTag(tag);

            (Vector3, AnimationCurve[]) posCurves = new(shakeConfig.CameraPosAxis, new AnimationCurve[3]);
            (Vector3, AnimationCurve[]) rotCurves = new(shakeConfig.CameraRotationAxis, new AnimationCurve[3]);

            posCurves.Item2[0] = shakeConfig.GetRandomCurve();
            posCurves.Item2[1] = shakeConfig.GetRandomCurve();
            posCurves.Item2[2] = shakeConfig.GetRandomCurve();

            rotCurves.Item2[0] = shakeConfig.GetRandomCurve();
            rotCurves.Item2[1] = shakeConfig.GetRandomCurve();
            rotCurves.Item2[2] = shakeConfig.GetRandomCurve();

            ShakeEvent newShakeEvent = new ShakeEvent(shakeConfig, posCurves, rotCurves);
            _currentEvents.Add(newShakeEvent);
        }

        private void Tick()
        {
            if (_currentEvents.Count != 0 && _gameFactory == null || _gameFactory.CameraBaseContainer == null || _gameFactory.CameraBaseContainer.CameraRotationParent == null) return;

            Transform cameraRotation = _gameFactory.CameraBaseContainer.CameraRotationParent;

            for (int i = _currentEvents.Count - 1; i >= 0; i--)
            {
                _currentEvents[i].Time += (_currentEvents[i].ShakeConfig.IsUnscaleTime ? Time.unscaledDeltaTime : Time.deltaTime) / _currentEvents[i].ShakeConfig.Duration;
                float t = _currentEvents[i].Time;

                (Vector3, AnimationCurve[]) posCurves = _currentEvents[i].PositionCurves;
                (Vector3, AnimationCurve[]) rotCurves = _currentEvents[i].RotationCurves;

                float currentPower = _currentEvents[i].ShakeConfig.Power * _currentEvents[i].ShakeConfig.PowerCurve.Evaluate(t);

                Vector3 currentPos = new Vector3(
                    posCurves.Item1.x * posCurves.Item2[0].Evaluate(t),
                    posCurves.Item1.y * posCurves.Item2[1].Evaluate(t),
                    posCurves.Item1.z * posCurves.Item2[2].Evaluate(t));
                currentPos *= currentPower;
                Vector3 currentRot = new Vector3(
                    rotCurves.Item1.x * rotCurves.Item2[0].Evaluate(t),
                    rotCurves.Item1.y * rotCurves.Item2[1].Evaluate(t),
                    rotCurves.Item1.z * rotCurves.Item2[2].Evaluate(t));
                currentRot *= currentPower;

                cameraRotation.localPosition = currentPos;
                cameraRotation.localRotation = Quaternion.Euler(currentRot);

                if (_currentEvents[i].Time >= 1f)
                {
                    _currentEvents.RemoveAt(i);
                }
            }
        }

        private class ShakeEvent
        {
            public float Time;
            public CameraShakeConfigs ShakeConfig;
            public (Vector3, AnimationCurve[]) PositionCurves, RotationCurves;

            public ShakeEvent(CameraShakeConfigs shakeConfig, (Vector3, AnimationCurve[]) positionCurves, (Vector3, AnimationCurve[]) rotationCurves)
            {
                ShakeConfig = shakeConfig;
                PositionCurves = positionCurves;
                RotationCurves = rotationCurves;
            }
        }
    }

    public interface ICameraShaker
    {
        void Shake(string tag, float overridePower = 1f);
    }
}