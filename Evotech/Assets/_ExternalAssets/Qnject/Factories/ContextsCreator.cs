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
                ProjectInstaller pIPrefab = Resources.Load<ProjectInstaller>("ProjectContext");

                if (pIPrefab == null)
                {
                    Debug.LogError("There is no ProjectContext prefab in Resources folder!.. Maybe you forgot to create one?");
                }
                else
                {
                    Object.Instantiate(pIPrefab);
                }
            }
        }
    }
}