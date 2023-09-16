using UnityEngine;

namespace Qnject
{
    [DefaultExecutionOrder(-130)]
    public abstract class ProjectInstaller : Installer
    {
        protected override void Awake()
        {
            base.Awake();

            gameObject.name = "[ProjectContext]";

            DontDestroyOnLoad(gameObject);
        }

        public override void Bind() {}
    }
}