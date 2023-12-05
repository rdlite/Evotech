using UnityEngine;

namespace Core.UI
{
    public class UnitsInfoPanel : Panel
    {
        [SerializeField] private Transform _statsParent;

        public Transform GetUnitStatsParent()
        {
            return _statsParent;
        }
    }
}