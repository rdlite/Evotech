using System;

namespace Utils
{
    public interface IUpdateProvider
    {
        void AddUpdate(Action callback);
        void RemoveUpdate(Action callback);

        void AddFixedUpdate(Action callback);
        void RemoveFixedUpdate(Action callback);

        void AddLateUpdate(Action callback);
        void RemoveLateUpdate(Action callback);

        void Update();
        void FixedUpdate();
        void LateUpdate();
    }
}