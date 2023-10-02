using UnityEngine;

namespace Core.Units
{
    public abstract class BaseUnit : MonoBehaviour 
    { 
        public float GetWalkRange()
        {
            return Random.Range(5f, 10f);
        }
    }
}