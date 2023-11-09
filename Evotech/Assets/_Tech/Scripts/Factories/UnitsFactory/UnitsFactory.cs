using Core.Data;
using Core.Units;
using Hexnav.Core;
using Qnject;

namespace Core.Factories
{
    public class UnitsFactory : IUnitsFactory
    {
        private AssetsContainer _assetsContainer;

        public UnitsFactory(AssetsContainer assetsContainer)
        {
            _assetsContainer = assetsContainer;
        }

        public BaseUnit Create(NodeBase spawnNode, Enums.UnitType unitType, Enums.OutlineType outlineType)
        {
            BaseUnit newUnit = QnjectPrefabsFactory.CreatePrefab(_assetsContainer.UnitAssets.TestUnit);
            newUnit.transform.position = spawnNode.WorldPos + spawnNode.SurfaceOffset;
            newUnit.transform.forward = spawnNode.WorldObject.forward;
            newUnit.Init(unitType, outlineType);
            return newUnit;
        }
    }

    public interface IUnitsFactory
    {
        BaseUnit Create(NodeBase spawnNode, Enums.UnitType unitType, Enums.OutlineType outlineType);
    }
}