using Utils;
using Qnject;
using System;
using UnityEngine;
using Core.Settings;
using System.Collections.Generic;

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
            List<Node> nodes = new List<Node>();
            GameObject mapParent = new GameObject("MapParent");
            mapParent.transform.position = worldPos;

            HexesWrapper hexesWrapper = JsonUtility.FromJson<HexesWrapper>(jsonData);

            for (int i = 0; i < hexesWrapper.Places.Length; i++)
            {
                Vector2Int point = new Vector2Int(hexesWrapper.Places[i].HexX, hexesWrapper.Places[i].HexY);

                HexagonNode hex = CreateMainHex(point, mapParent.transform);
                SetHexagonHeight(hex, hexesWrapper.Places[i].Height);

                //if (hexesWrapper.Places[i].ObstacleNames != null && hexesWrapper.Places[i].ObstacleNames.Strings.Count != 0)
                //{
                //    for (int j = 0; j < hexesWrapper.Places[i].ObstacleNames.Strings.Count; j++)
                //    {
                //        int paletteID = FindAssetIndexByName(hexesWrapper.Places[i].ObstacleNames.Strings[j]);
                //        GetSceneEditor().PinCurrentPrefab(_totalPalette[paletteID]);
                //        GetSceneEditor().AddObstacle(point, true);
                //    }

                //    GetSceneEditor().SetObstaclesNames(point, hexesWrapper.Places[i].ObstacleNames.Strings);
                //    GetSceneEditor().SetObstaclesOffset(point, hexesWrapper.Places[i].ObstaclesOffsets.Vectors);
                //}

                //if (hexesWrapper.Places[i].Tags != null && hexesWrapper.Places[i].Tags.Strings.Count != 0)
                //{
                //    for (int j = 0; j < hexesWrapper.Places[i].Tags.Strings.Count; j++)
                //    {
                //        GetSceneEditor().AddTag(point, hexesWrapper.Places[i].Tags.Strings[j]);
                //    }
                //}
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