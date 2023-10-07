using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;

namespace Qnject
{
    public static class MonoObjectsResolver
    {
        private const BindingFlags bindingAttrs = BindingFlags.Instance |
                                                  BindingFlags.Public |
                                                  BindingFlags.NonPublic;

        private static List<Container> _containers = new List<Container>();
        private static List<MonoBehaviour> _monosInInstallers = new List<MonoBehaviour>();

        public static void ResolveCurrentScene()
        {
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.GetActiveScene().rootCount; i++)
            {
                foreach (var item in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()[i].transform.GetComponentsInChildren<MonoBehaviour>())
                {
                    MonoBehaviour[] monos = item.GetComponents<MonoBehaviour>();

                    if (monos != null && monos.Length > 0)
                    {
                        foreach (var mono in monos)
                        {
                            if (!_monosInInstallers.Contains(mono))
                            {
                                ResolveObject(mono);
                            }
                        }
                    }
                }
            }
        }

        public static void ResolveObject(MonoBehaviour mono)
        {
            ResolveFields(mono);
            ResolveMethods(mono);
        }

        private static void ResolveMethods(MonoBehaviour target)
        {
            Type currentCheckType = target.GetType();

            while (currentCheckType != typeof(MonoBehaviour))
            {
                Type targetType = currentCheckType;
                MethodInfo[] methods = targetType.GetMethods(bindingAttrs);

                foreach (MethodInfo method in methods)
                {
                    foreach (Attribute attribute in method.GetCustomAttributes())
                    {
                        if (attribute is not Inject)
                        {
                            continue;
                        }

                        int it = 0;
                        object[] parameters = new object[method.GetParameters().Length];

                        foreach (var parameter in method.GetParameters())
                        {
                            parameters[it] = TryGetObjFromContainers(parameter.ParameterType);
                            it++;
                        }

                        method.Invoke(target, parameters);
                    }
                }

                currentCheckType = currentCheckType.BaseType;
            }
        }

        private static void ResolveFields(MonoBehaviour target)
        {
            Type currentCheckType = target.GetType();

            while (currentCheckType != typeof(MonoBehaviour))
            {
                Type targetType = currentCheckType;
                FieldInfo[] fields = targetType.GetFields(bindingAttrs);

                foreach (FieldInfo fi in fields)
                {
                    foreach (Attribute attribute in fi.GetCustomAttributes())
                    {
                        if (attribute is not Inject)
                        {
                            continue;
                        }

                        fi.SetValue(target, TryGetObjFromContainers(fi.FieldType));
                    }
                }

                currentCheckType = currentCheckType.BaseType;
            }
        }

        public static void RegisterContainer(Container container)
        {
            _containers.Add(container);
        }

        public static void UnregisterContainer(Container container)
        {
            _containers.Remove(container);
        }
        
        public static void RegisterInstallerBehaviours(MonoBehaviour[] monos)
        {
            _monosInInstallers.AddRange(monos.ToList());
        }

        public static void UnregisterInstallerBehaviours(MonoBehaviour[] monos)
        {
            foreach (var mono in monos)
            {
                _monosInInstallers.Remove(mono);
            }
        }

        private static object TryGetObjFromContainers(Type fieldType)
        {
            foreach (var container in _containers)
            {
                if (container.Get(fieldType) != null)
                {
                    return container.Get(fieldType);
                }
            }

            return null;
        }
    }
}