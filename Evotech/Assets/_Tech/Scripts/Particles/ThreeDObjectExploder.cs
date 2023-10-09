using UnityEngine;

namespace Core.Particles
{
    public class ThreeDObjectExploder : MonoBehaviour
    {
        private Rigidbody[] _rbs;

        private void Awake()
        {
            _rbs = GetComponentsInChildren<Rigidbody>();

            foreach (var rb in _rbs)
            {
                rb.isKinematic = true;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        public void Explode()
        {
            for (int i = 0; i < _rbs.Length; i++)
            {
                Destroy(_rbs[i].gameObject);
            }
        }
    }
}