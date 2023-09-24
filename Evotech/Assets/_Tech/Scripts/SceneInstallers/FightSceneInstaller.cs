using Qnject;
using UnityEngine;

public class FightSceneInstaller : SceneInstaller
{
    [SerializeField] private FightSceneStartup _sceneStartup;

    public override void Install()
    {
        _sceneStartup.CreateFightScene(_container);
    }
}
