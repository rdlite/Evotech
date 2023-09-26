using Qnject;

namespace Core.Infrastructure
{
    public class BattleSceneInstaller : SceneInstaller
    {
        public override void Install()
        {
            BattleSceneStartup sceneStartup = FindObjectOfType<BattleSceneStartup>();
            sceneStartup.CreateBattleScene(_container);
        }
    }
}