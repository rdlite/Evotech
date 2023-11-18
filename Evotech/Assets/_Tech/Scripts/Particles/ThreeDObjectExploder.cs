using Cysharp.Threading.Tasks;
using DG.Tweening;
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

        public void Explode(Vector3 mainDirection, bool withRotation, float destroyTimer)
        {
            transform.SetParent(null);

            foreach (var rb in _rbs)
            {
                rb.mass = 10f;
                rb.drag = .5f;
                rb.isKinematic = false;
                rb.constraints = RigidbodyConstraints.None;
                rb.velocity = Vector3.zero;

                rb.AddForce(mainDirection.normalized * Random.Range(50f, 70f), ForceMode.Impulse);
                
                if (withRotation)
                {
                    rb.AddTorque(Random.insideUnitSphere * Random.Range(1f, 5f), ForceMode.Impulse);
                }
            }

            DestroyTimer(destroyTimer);
        }

        public void SmoothGravityFalling(bool withRotation, float destroyTimer)
        {
            transform.SetParent(null);
            
            foreach (var rb in _rbs)
            {
                rb.mass = 10f;
                rb.drag = .5f;
                rb.isKinematic = false;
                rb.constraints = RigidbodyConstraints.None;
                rb.velocity = Vector3.zero;

                if (withRotation)
                {
                    rb.AddTorque(Random.insideUnitSphere * Random.Range(1f, 5f), ForceMode.VelocityChange);
                }
            }

            DestroyTimer(destroyTimer);
        }

        private async void DestroyTimer(float timeInSeconds)
        {
            await UniTask.Delay((int)(timeInSeconds * 1000));
            foreach (var rb in _rbs)
            {
                rb.isKinematic = true;
                rb.transform.DOScale(Vector3.zero, Random.Range(.8f, 1.2f)).SetDelay(Random.Range(0f, .2f)).OnComplete
                (
                    () =>
                    {
                        Destroy(rb.gameObject);
                    }
                );
            }

            await UniTask.Delay(3000);

            Destroy(gameObject);
        }
    }
}