using Utils;
using Qnject;
using System;
using UnityEngine;
using Core.Settings;
using System.Collections.Generic;
using Hexnav.Core;
using Core.Map;

namespace Core.Data
{
    public class HexagonalMapLoader : MapLoader
    {
        private MapSettings _mapSettings;
        private LandscapeSettings _landscapeSettings;

        public HexagonalMapLoader(
            MapSettings mapSettings, LandscapeSettings landscapeSettings)
        {
            _mapSettings = mapSettings;
            _landscapeSettings = landscapeSettings;
        }

        public override MapData LoadMap(string jsonData, Vector3 worldPos)
        {
            List<NodeBase> nodes = new List<NodeBase>();
            GameObject mapParent = new GameObject("MapParent");
            mapParent.transform.position = worldPos;

            HexesWrapper hexesWrapper = JsonUtility.FromJson<HexesWrapper>(jsonData);

            for (int i = 0; i < hexesWrapper.Places.Length; i++)
            {
                Vector2Int point = new Vector2Int(hexesWrapper.Places[i].HexX, hexesWrapper.Places[i].HexY);

                HexagonNode hex = CreateMainHex(point, mapParent.transform);
                SetHexagonHeight(hex, hexesWrapper.Places[i].Height);

                bool hasNonwalkableObstacles = false;

                if (hexesWrapper.Places[i].ObstacleNames != null && hexesWrapper.Places[i].ObstacleNames.Strings.Count != 0)
                {
                    hasNonwalkableObstacles = CreateObstaclesOnHex(
                        hex, hexesWrapper.Places[i].ObstacleNames.Strings, hexesWrapper.Places[i].ObstaclesOffsets.Vectors);
                }

                nodes.Add(new NodeBase(
                    hex.transform.position,
                    point,
                    hexesWrapper.Places[i].Height,
                    hex.transform,
                    hex.GetSurfaceOffset(),
                    hexesWrapper.Places[i].Tags.Strings,
                    hexesWrapper.Places[i].ObstacleNames.Strings));

                if (hasNonwalkableObstacles)
                {
                    nodes[nodes.Count - 1].NonwalkableFactors++;
                }
            }

            MapData map = new MapData(nodes);

            return map;
        }

        private HexagonNode CreateMainHex(Vector2Int point, Transform parent)
        {
            HexagonNode newHex = QnjectPrefabsFactory.CreatePrefab<HexagonNode>(_landscapeSettings.MainHex);
            newHex.transform.SetParent(parent);
            newHex.transform.localPosition = HexGridUtility.ConvertHexCoordToWorldPoint(point);
            return newHex;
        }

        private bool CreateObstaclesOnHex(HexagonNode hex, List<string> obstacles, List<Vector3> localOffsets)
        {
            bool hasNonwalkableObstacles = false;

            for (int i = 0; i < obstacles.Count; i++)
            {
                foreach (var nonwalkableObstacle in _landscapeSettings.NonwalkableObstacles)
                {
                    if (obstacles[i].Contains(nonwalkableObstacle.name))
                    {
                        hasNonwalkableObstacles = true;
                        CreatePrefabOnHex(hex, nonwalkableObstacle.gameObject, localOffsets[i]);
                    }
                }

                foreach (var walkableObstacle in _landscapeSettings.WalkableObstacles)
                {
                    if (obstacles[i].Contains(walkableObstacle.name))
                    {
                        CreatePrefabOnHex(hex, walkableObstacle.gameObject, localOffsets[i]);
                    }
                }
            }

            return hasNonwalkableObstacles;
        }

        private GameObject CreatePrefabOnHex(HexagonNode hex, GameObject prefab, Vector3 localOffsets)
        {
            GameObject newPrefab = UnityEngine.Object.Instantiate(prefab);
            newPrefab.transform.SetParent(hex.GetObstaclesContainer());
            newPrefab.transform.localPosition = localOffsets;
            return newPrefab;
        }

        private void SetHexagonHeight(HexagonNode hex, int height)
        {
            hex.transform.position += Vector3.up * height * _mapSettings.HeightOffset;
        }

        [Serializable]
        public class HexesWrapper
        {
            public HexPlaceInfo[] Places;
            public string[] GlobalTags;
        }

        [Serializable]
        public class HexPlaceInfo
        {
            public string HexagonName;
            public int HexX;
            public int HexY;
            public int Height = 0;
            public Vector3Container ObstaclesOffsets = new Vector3Container();
            public StringsContainer Tags = new StringsContainer();
            public StringsContainer ObstacleNames = new StringsContainer();
        }

        [Serializable]
        public class Vector3Container
        {
            public List<Vector3> Vectors = new List<Vector3>();
        }

        [Serializable]
        public class StringsContainer
        {
            public List<string> Strings = new List<string>();
        }
    }
}