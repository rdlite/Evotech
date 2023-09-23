using UnityEngine;

namespace Core.Settings
{
    public class BuildSettings : MonoBehaviour
    {
        public void Set()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}