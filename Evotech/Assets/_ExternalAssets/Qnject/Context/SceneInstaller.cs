using UnityEngine;

namespace Qnject
{
    [DefaultExecutionOrder(-120)]
    public abstract class SceneInstaller : Installer
    {
        protected override void Awake()
        {
            ContextsCreator.TryCreateProjectContextFromResources();

            base.Awake();

            ResolveScene();
            InstallScene();
        }

        public override void Install() {}

        protected abstract void InstallScene();

        private void ResolveScene()
        {
            MonoObjectsResolver.ResolveCurrentScene();
        }
    }
}