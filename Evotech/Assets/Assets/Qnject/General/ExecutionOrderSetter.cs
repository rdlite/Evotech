using UnityEditor;

namespace Qnject
{
    [InitializeOnLoad]
    public class ExecutionOrderSetter : Editor
    {
        static ExecutionOrderSetter()
        {
            string projectInstaller = typeof(ProjectInstaller).Name;
            string sceneInstaller = typeof(SceneInstaller).Name;

            foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
            {
                if (monoScript.name == projectInstaller)
                {
                    if (MonoImporter.GetExecutionOrder(monoScript) != -130)
                    {
                        MonoImporter.SetExecutionOrder(monoScript, -130);
                    }

                    break;
                }
                else if (monoScript.name == sceneInstaller)
                {
                    if (MonoImporter.GetExecutionOrder(monoScript) != -120)
                    {
                        MonoImporter.SetExecutionOrder(monoScript, -120);
                    }

                    break;
                }
            }
        }
    }
}