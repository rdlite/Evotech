using UnityEngine;

namespace Core.Settings
{
    [CreateAssetMenu(fileName = "New game settings", menuName = "Add/Settings/Game settings")]
    public class GameSettings : ScriptableObject
    {
        public CameraSettings CameraSettings;
        public MapSettings MapSettings;
        public LayersSettings LayersSettings;
        public BattleSettings BattleSettings;
    }

    [System.Serializable]
    public class MapSettings
    {
        public float HeightOffset = .2f;
        public float HeightDistanceMultiplier = 2f;
    }

    [System.Serializable]
    public class CameraSettings
    {
        public float MovementSpeed = 10f;
        public float MovementSmooth = 10f;
        public bool IsChangeHeight = true;
        public float HeightChangeSmooth = 2f;
        public float ZoomSmooth = 2f;
        public float RotationSmooth = 5f;
        public float RotationSpeed = 20f;
        public float MinZoom = .5f;
        public float MaxZoom = 1.5f;
        public float DefaultFOV = 40f;
        public float MaxRotationCameraOnZoom = 30f;
        public Vector3 PositionOffset;
        public Vector3 ParentStartRotationOffset, RotationOffset;
    }

    [System.Serializable]
    public class LayersSettings
    {
        public LayerMask UnitsMask;
        public LayerMask GroundMask;
    }

    [System.Serializable]
    public class BattleSettings
    {
        public float MovementDuration = .6f;
        public float MaxUpMovementHeight = 3f;
        public AnimationCurve MovementSmoothToUp;
        public AnimationCurve ArcMovementCurve;
        public AnimationCurve MovementSmoothToDown;
        public ParticleSystem PlacingParticle;
    }
}