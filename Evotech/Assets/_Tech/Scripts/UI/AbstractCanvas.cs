using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI
{
    public abstract class AbstractCanvas : MonoBehaviour
    {
        private Dictionary<Type, Panel> _childPanels;

        protected virtual void Awake()
        {
            GatherPanels();
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

        private void GatherPanels()
        {
            _childPanels = new Dictionary<Type, Panel>();

            foreach (var childPanel in GetComponentsInChildren<Panel>(true))
            {
                _childPanels.Add(childPanel.GetType(), childPanel);
            }
        }
    }
}