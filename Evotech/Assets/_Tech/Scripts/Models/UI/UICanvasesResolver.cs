using Core.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI
{
    public class UICanvasesResolver : IUICanvasesResolver
    {
        private Dictionary<Enums.UICanvasType, AbstractCanvas> _canvasPrefabs;
        private Dictionary<Enums.UICanvasType, AbstractCanvas> _currentCanvases;

        private const string BATTLE_CANVAS_PATH = "UI/Canvases/BattleCanvas";

        public UICanvasesResolver()
        {
            _canvasPrefabs = new Dictionary<Enums.UICanvasType, AbstractCanvas>();
            _canvasPrefabs.Add(Enums.UICanvasType.Battle, Resources.Load<BattleCanvas>(BATTLE_CANVAS_PATH));

            _currentCanvases = new Dictionary<Enums.UICanvasType, AbstractCanvas>();
        }

        public void OpenCanvas(Enums.UICanvasType canvasType, bool withDontDestroy)
        {
            if (_currentCanvases.ContainsKey(canvasType)) return;

            AbstractCanvas newCanvas = Object.Instantiate(_canvasPrefabs[canvasType]);
            _currentCanvases.Add(canvasType, newCanvas);
            newCanvas.FirstShow();

            if (withDontDestroy)
            {
                Object.DontDestroyOnLoad(newCanvas.gameObject);
            }
        }

        public void Clear()
        {
            foreach (var canvas in _currentCanvases)
            {
                Object.Destroy(canvas.Value.gameObject);
            }

            _currentCanvases.Clear();
        }
    }

    public interface IUICanvasesResolver
    {
        void OpenCanvas(Enums.UICanvasType canvasType, bool withDontDestroy);
        void Clear();
    }
}