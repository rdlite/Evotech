using Qnject;

namespace Core.Infrastructure 
{
    public class ProjectContext : ProjectInstaller
    {
        public override void Install()
        {
            Startup startup = FindObjectOfType<Startup>();
            startup.Init(_container);
        }
    }
}