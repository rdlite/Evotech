using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace Qnject
{
    public class QnjectPrefabsFactory
    {
        public static GameObject CreatePrefab(
            GameObject instance, 
            Vector3 position = new Vector3(), 
            Quaternion rotation = new Quaternion(), 
            Transform parent = null)
        {
            GameObject newObject = Object.Instantiate(instance);
            newObject.transform.position = position;
            newObject.transform.rotation = rotation;
            newObject.transform.SetParent(parent);
            ResolveMonosOnObject(newObject.GetComponent<MonoBehaviour>());
            return newObject;
        }

        public static T CreatePrefab<T>(
            T instance, 
            Vector3 position = new Vector3(), 
            Quaternion rotation = new Quaternion(), 
            Transform parent = null) where T : MonoBehaviour
        {
            T newObject = Object.Instantiate(instance);
            newObject.transform.position = position;
            newObject.transform.rotation = rotation;
            newObject.transform.SetParent(parent);
            ResolveMonosOnObject(newObject);
            return newObject;
        }

        private static void ResolveMonosOnObject(MonoBehaviour obj)
        {
            List<MonoBehaviour> monosToResolve = obj.GetComponentsInChildren<MonoBehaviour>(true).ToList();
            monosToResolve.AddRange(obj.GetComponents<MonoBehaviour>());

            foreach (MonoBehaviour item in obj.GetComponentsInChildren<MonoBehaviour>(true))
            {
                MonoObjectsResolver.ResolveObject(item);
            }
        }
    }
}