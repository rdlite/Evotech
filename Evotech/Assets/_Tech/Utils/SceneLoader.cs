using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public static class SceneLoader
    {
        public static void LoadAsync(string sceneName, Action onLoaded = null)
        {
            if (SceneManager.GetActiveScene().name == sceneName)
            {
                onLoaded?.Invoke();
                return;
            }

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            asyncOperation.completed += asyncOperation =>
            {
                onLoaded?.Invoke();
            };
        }
    }
}