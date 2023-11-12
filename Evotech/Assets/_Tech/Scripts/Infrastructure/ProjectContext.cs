using Qnject;
using UnityEngine;

namespace Core.Infrastructure 
{
    public class ProjectContext : ProjectInstaller
    {
        public override void Install()
        {
            GameStartup startup = FindObjectOfType<GameStartup>();
            startup.Init(_container);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}