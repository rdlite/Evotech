using UnityEngine;

namespace Core.Settings
{
    public class BuildSettings 
    {
        public void Set()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}