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
        }

        private void ResolveScene()
        {
            MonoObjectsResolver.ResolveCurrentScene();
        }
    }
}