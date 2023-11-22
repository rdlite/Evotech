using Core.Data;
using Qnject;
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

        public void CreateCanvas(Enums.UICanvasType canvasType, bool withDontDestroy)
        {
            AbstractCanvas newCanvas = QnjectPrefabsFactory.Instantiate(_canvasPrefabs[canvasType]);
            _currentCanvases.Add(canvasType, newCanvas);
            _canvasPrefabs[canvasType].gameObject.SetActive(false);
            if (withDontDestroy)
            {
                Object.DontDestroyOnLoad(_canvasPrefabs[canvasType].gameObject);
            }
        }

        public void OpenCanvas(Enums.UICanvasType canvasType)
        {
            if (_currentCanvases.ContainsKey(canvasType)) return;

            _canvasPrefabs[canvasType].Show();
        }

        public AbstractCanvas GetCanvas<TCanvas>() where TCanvas : AbstractCanvas
        {
            foreach (var canvas in _currentCanvases)
            {
                if (canvas.Value.GetType() == typeof(TCanvas))
                {
                    return canvas.Value as TCanvas;
                }
            }

            return null;
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
        void OpenCanvas(Enums.UICanvasType canvasType);
        void CreateCanvas(Enums.UICanvasType canvasType, bool withDontDestroy);
        AbstractCanvas GetCanvas<TCanvas>() where TCanvas : AbstractCanvas;
        void Clear();
    }
}