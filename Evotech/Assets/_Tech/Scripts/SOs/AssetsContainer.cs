using Core.Cameras;
using Core.Curtains;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "New assets container", menuName = "Add/Containers/Assets container")]
    public class AssetsContainer : ScriptableObject
    {
        public CameraController CameraPrefab;
        public HexagonsContainer HexagonsContainer;
        public MapTextsContainer MapTextsContainer;
        public Curtain CurtainPrefab;
    }
}