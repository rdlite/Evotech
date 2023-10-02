using Core.Data;
using Core.Units;
using Hexnav.Core;
using UnityEngine;

namespace Core.Factories
{
    public class UnitsFactory : IUnitsFactory
    {
        private AssetsContainer _assetsContainer;

        public UnitsFactory(AssetsContainer assetsContainer)
        {
            _assetsContainer = assetsContainer;
        }

        public BaseUnit Create(NodeBase spawnNode)
        {
            BaseUnit newUnit = Object.Instantiate(_assetsContainer.UnitAssets.TestUnit);
            newUnit.transform.position = spawnNode.WorldPos + spawnNode.SurfaceOffset;
            newUnit.transform.forward = spawnNode.WorldObject.forward;
            return newUnit;
        }
    }

    public interface IUnitsFactory
    {
        BaseUnit Create(NodeBase spawnNode);
    }
}