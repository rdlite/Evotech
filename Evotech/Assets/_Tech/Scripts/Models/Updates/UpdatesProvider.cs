using System;
using System.Collections.Generic;

namespace Utils
{
    public class UpdatesProvider : IUpdateProvider
    {
        private List<Action> _updates = new List<Action>();
        private List<Action> _fixedUpdates = new List<Action>();
        private List<Action> _lateUpdates = new List<Action>();

        public void AddUpdate(Action callback)
        {
            _updates.Add(callback);
        }

        public void AddFixedUpdate(Action callback)
        {
            _fixedUpdates.Add(callback);
        }

        public void AddLateUpdate(Action callback)
        {
            _lateUpdates.Add(callback);
        }

        public void RemoveUpdate(Action callback)
        {
            _updates.Remove(callback);
        }

        public void RemoveFixedUpdate(Action callback)
        {
            _fixedUpdates.Remove(callback);
        }

        public void RemoveLateUpdate(Action callback)
        {
            _lateUpdates.Remove(callback);
        }

        public void Update()
        {
            for (int i = 0; i < _updates.Count; i++)
            {
                _updates[i]?.Invoke();
            }
        }

        public void FixedUpdate()
        {
            for (int i = 0; i < _fixedUpdates.Count; i++)
            {
                _fixedUpdates[i]?.Invoke();
            }
        }

        public void LateUpdate()
        {
            for (int i = 0; i < _lateUpdates.Count; i++)
            {
                _lateUpdates[i]?.Invoke();
            }
        }
    }
}