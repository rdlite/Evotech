using Core.Data;
using Qnject;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Battle
{
    public class BattleLinesFactory : IBattleLinesFactory
    {
        private List<BattlePointedLine> _createdLines;
        private AssetsContainer _assetsContainer;
        private StylesContainer _stylesContainer;

        public BattleLinesFactory(
            StylesContainer stylesContainer, AssetsContainer assetsContainer)
        {
            _stylesContainer = stylesContainer;
            _assetsContainer = assetsContainer;
            _createdLines = new List<BattlePointedLine>();
        }

        public BattlePointedLine CreateLine(Vector3 start, Vector3 end, Enums.PointedLineType pointedLineType)
        {
            BattlePointedLine newLine = QnjectPrefabsFactory.CreatePrefab(_assetsContainer.BattlePrefabs.Line);
            newLine.Create(start, end, _stylesContainer.GetStyleOfLineType(pointedLineType));
            _createdLines.Add(newLine);
            return newLine;
        }

        public void ClearLines()
        {
            for (int i = 0; i < _createdLines.Count; i++)
            {
                _createdLines[i].Destroy();
            }
            _createdLines.Clear();
        }
    }

    public interface IBattleLinesFactory
    {
        BattlePointedLine CreateLine(Vector3 start, Vector3 end, Enums.PointedLineType pointedLineType);
        void ClearLines();
    }
}