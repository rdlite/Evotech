using UnityEngine;

namespace Qnject
{
    public abstract class Installer : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] _monoToInstall;
        [SerializeField] private ScriptableObject[] _scriptableObjectsToInstall;

        protected Container _container;

        protected virtual void Awake()
        {
            _container = new Container();
            MonoObjectsResolver.RegisterContainer(_container);

            if (_monoToInstall != null && _monoToInstall.Length != 0)
            {
                _container.BindRange(_monoToInstall);
            }

            if (_scriptableObjectsToInstall != null && _scriptableObjectsToInstall.Length != 0)
            {
                _container.BindRange(_scriptableObjectsToInstall);
            }

            Bind();
        }

        public abstract void Bind();

        private void OnDestroy()
        {
            UnloadLocalDependencies();
        }

        protected void UnloadLocalDependencies()
        {
            MonoObjectsResolver.UnregisterContainer(_container);
            _container.Clear();
        }
    }
}