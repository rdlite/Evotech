using Core.Cameras;
using Core.Curtains;
using Core.Units;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "New assets container", menuName = "Add/Containers/Assets container")]
    public class AssetsContainer : ScriptableObject
    {
        public CameraController CameraPrefab;
        public HexagonsContainer HexagonsContainer;
        public MapTextsContainer MapTextsContainer;
        public Curtain CurtainPrefab;
        public UnitAssets UnitAssets;
    }

    [System.Serializable]
    public class UnitAssets
    {
        public BaseUnit TestUnit;
    }
}