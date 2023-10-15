using Qnject;

namespace Core.Infrastructure 
{
    public class ProjectContext : ProjectInstaller
    {
        public override void Install()
        {
            GameStartup startup = FindObjectOfType<GameStartup>();
            startup.Init(_container);
        }
    }
}