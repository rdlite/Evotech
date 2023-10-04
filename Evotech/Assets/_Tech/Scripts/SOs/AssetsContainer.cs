using Core.Cameras;
using Core.Curtains;
using Core.Units;
using UnityEngine;
using Utils.Decal;

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
        public BattlePrefabs BattlePrefabs;
    }

    [System.Serializable]
    public class UnitAssets
    {
        public BaseUnit TestUnit;
    }

    [System.Serializable]
    public class BattlePrefabs
    {
        public DecalWrapper HexagonalDefaultDecal;
        public GameObject SelectionHexagon;
    }
}