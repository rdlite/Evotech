using Core.Units;
using UnityEngine;
using Utils.Decal;
using Core.Cameras;
using Core.Curtains;
using Core.Particles;
using Core.Battle;
using Core.UI.Elements;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "New assets container", menuName = "Add/Containers/Assets container")]
    public class AssetsContainer : ScriptableObject
    {
        public CameraController CameraPrefab;
        public MapTextsContainer MapTextsContainer;
        public Curtain CurtainPrefab;
        public UnitAssets UnitAssets;
        public BattlePrefabs BattlePrefabs;
        public ArmorPrebafs ArmorPrebafs;
        public UIElements UIElements;
    }

    [System.Serializable]
    public class UnitAssets
    {
        public BaseUnit TestUnit;
        public ParticleSystem SpiritEvaporationParticle;
        public ParticleSystem SimpleHitParticle;
        public ParticleSystem SimpleAttackPrepareParticle;
    }

    [System.Serializable]
    public class BattlePrefabs
    {
        public DecalWrapper HexagonalDefaultDecal;
        public GameObject SelectionHexagon;
        public BattlePointedLine Line;
    }

    [System.Serializable]
    public class ArmorPrebafs
    {
        [SerializeField] private PrefabsByType[] _maps;

        public ThreeDObjectExploder GetRandomObjectByType(Enums.SpiritArmorPointType armorType)
        {
            for (int i = 0; i < _maps.Length; i++)
            {
                if (_maps[i].ArmorType == armorType)
                {
                    return _maps[i].Prefabs[Random.Range(0, _maps[i].Prefabs.Length)];
                }
            }

            return null;
        }

        [System.Serializable]
        public class PrefabsByType
        {
            public Enums.SpiritArmorPointType ArmorType;
            public ThreeDObjectExploder[] Prefabs;
        }
    }

    [System.Serializable]
    public class UIElements
    {
        public UnitStatsInfo UnitStatsInfo;
    }
}