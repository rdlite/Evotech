using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class AbstractCanvas : MonoBehaviour
    {
        protected CanvasGroup _canvasGroup;

        private Dictionary<Type, Panel> _childPanels;

        public virtual void Init()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public abstract void Show();

        public void SetActivePanel<TPanel>(bool value)
        {
            if (value)
            {
                _childPanels[typeof(TPanel)].Show();
            }
            else
            {
                _childPanels[typeof(TPanel)].Hide();
            }
        }

        public void HideAll()
        {
            foreach (var panel in _childPanels)
            {
                panel.Value.Hide();
            }
        }

        public TPanel GetPanelOfType<TPanel>() where TPanel : class
        {
            return _childPanels[typeof(TPanel)] as TPanel;
        }

        public void GatherPanels()
        {
            _childPanels = new Dictionary<Type, Panel>();

            foreach (var childPanel in GetComponentsInChildren<Panel>(true))
            {
                _childPanels.Add(childPanel.GetType(), childPanel);
            }
        }

        public virtual void Freeze()
        {
            _canvasGroup.interactable = false;

            foreach (Panel panel in _childPanels.Values)
            {
                panel.Freeze();
            }
        }

        public virtual void Unfreeze()
        {
            _canvasGroup.interactable = true;

            foreach (Panel panel in _childPanels.Values)
            {
                panel.Unfreeze();
            }
        }
    }
}