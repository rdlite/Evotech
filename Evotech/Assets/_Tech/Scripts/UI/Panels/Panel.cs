using UnityEngine;

namespace Core.UI
{
    public abstract class Panel : MonoBehaviour
    {
        protected virtual void Awake()
        {

        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Freeze() { }

        public virtual void Unfreeze() { }
    }
}