using UnityEngine;

namespace Core.Units
{
    public class GhostCreator : MonoBehaviour
    {
        [SerializeField] private Material _ghostMaterial;

        public GhostCopy CreateGhostCopy()
        {
            GhostCopy newGhost = Instantiate(gameObject).AddComponent<GhostCopy>();
            newGhost.Init(_ghostMaterial);

            return newGhost;
        }
    }
}