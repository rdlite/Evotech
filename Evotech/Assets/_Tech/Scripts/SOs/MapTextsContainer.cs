using System.Collections.Generic;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "New maps container", menuName = "Add/Containers/Maps container")]
    public class MapTextsContainer : ScriptableObject
    {
        public List<MapData> Maps;

        public string GetMapData(string mapName)
        {
            for (int i = 0; i < Maps.Count; i++)
            {
                if (Maps[i].MapName == mapName)
                {
                    return Maps[i].JsonData.text;
                }
            }

            Debug.LogError($"There's no map with name {mapName}!..");

            return "";
        }

        [System.Serializable]
        public class MapData
        {
            public string MapName = "New map";
            public TextAsset JsonData;
        }
    }
}