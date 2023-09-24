using Qnject;
using UnityEngine;

namespace Core.Infrastructure
{
    public class FightSceneInstaller : SceneInstaller
    {
        public override void Install()
        {
            FightSceneStartup sceneStartup = FindObjectOfType<FightSceneStartup>();
            sceneStartup.CreateFightScene(_container);
        }
    }
}