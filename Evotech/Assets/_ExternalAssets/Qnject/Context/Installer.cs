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
            MonoObjectsResolver.RegisterInstallerBehaviours(_monoToInstall);

            if (_monoToInstall != null && _monoToInstall.Length != 0)
            {
                _container.BindRange(_monoToInstall);
            }

            if (_scriptableObjectsToInstall != null && _scriptableObjectsToInstall.Length != 0)
            {
                _container.BindRange(_scriptableObjectsToInstall);
            }

            for (int i = 0; i < _monoToInstall.Length; i++)
            {
                MonoObjectsResolver.ResolveObject(_monoToInstall[i]);
            }

            Install();
        }

        public abstract void Install();

        private void OnDestroy()
        {
            UnloadLocalDependencies();
        }

        protected void UnloadLocalDependencies()
        {
            MonoObjectsResolver.UnregisterContainer(_container);
            MonoObjectsResolver.UnregisterInstallerBehaviours(_monoToInstall);
            _container.Clear();
        }
    }
}