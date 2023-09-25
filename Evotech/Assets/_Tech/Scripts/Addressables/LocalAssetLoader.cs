using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

namespace Core.Data
{
    public class LocalAssetsLoader
    {
        private Dictionary<int, Object> _cachedObjects = new Dictionary<int, Object>();

        protected async UniTask<T> LoadInternal<T>(string assetID) where T : Object
        {
            var handle = Addressables.InstantiateAsync(assetID);
            Object cahcedAsset = await handle.Task;

            _cachedObjects.Add(cahcedAsset.GetInstanceID(), cahcedAsset);

            if (cahcedAsset is GameObject)
            {
                return cahcedAsset as T;
            }
            else
            {
                T returnType = (cahcedAsset as GameObject).GetComponent<T>();

                if (returnType == null)
                    throw new NullReferenceException($"Object of type {typeof(T)} is null of attempt to load it from addressables");

                return returnType;
            }
        }

        protected void UnloadAll()
        {
            foreach (var item in _cachedObjects)
            {
                UnloadInternal(item.Value);
            }
        }

        protected void UnloadInternal(Object asset)
        {
            GameObject cachedAsset = GetAssetByInstanceID(asset.GetInstanceID()) as GameObject;

            if (cachedAsset != null)
            {
                Addressables.Release(cachedAsset);
                Object.Destroy(cachedAsset);
                _cachedObjects.Remove(asset.GetInstanceID());
            }
        }

        private Object GetAssetByInstanceID(int id)
        {
            if (_cachedObjects.ContainsKey(id))
            {
                return _cachedObjects[id];
            }
            else
            {
                return null;
            }
        }
    }

}