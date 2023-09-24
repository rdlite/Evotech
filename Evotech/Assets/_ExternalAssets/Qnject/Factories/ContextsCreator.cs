using UnityEngine;

namespace Qnject
{
    public class ContextsCreator
    {
        public static bool IsProjectContextExists()
        {
            return Object.FindObjectOfType<ProjectInstaller>() != null;
        }

        public static void TryCreateProjectContextFromResources()
        {
            if (!IsProjectContextExists())
            {
                ProjectInstaller[] projectInstallers = Resources.LoadAll<ProjectInstaller>("");

                if (projectInstallers.Length == 0)
                {
                    Debug.LogError("There is no ProjectContext prefab in Resources folder!.. Maybe you forgot to create one?");
                }
                else
                {
                    Object.Instantiate(projectInstallers[0]);
                }
            }
        }
    }
}