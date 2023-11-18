using Qnject;

namespace Core.Infrastructure
{
    public class BattleSceneContext : SceneInstaller
    {
        protected override void InstallScene()
        {
            BattleSceneStartup sceneStartup = FindObjectOfType<BattleSceneStartup>();
            sceneStartup.CreateBattleScene(_container);
        }
    }
}