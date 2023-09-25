using Core.Data;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

namespace Hexnav.Core
{
    public class HexPathfinfing
    {
        private static IMapDataProvider _mapProvider;

        public HexPathfinfing(IMapDataProvider mapProvider)
        {
            _mapProvider = mapProvider;
        }

        public static List<NodeBase> SetDesination(Vector3 start, Vector3 target)
        {
            NodeBase startNode = _mapProvider.GetNearestNodeOfWorldPoint(start);
            NodeBase targetNode = _mapProvider.GetNearestNodeOfWorldPoint(target);

            if (startNode != null && targetNode != null)
            {
                return FindPath(startNode, targetNode);
            }

            return null;
        }

        public static List<NodeBase> FindPath(NodeBase startNode, NodeBase targetNode)
        {
            List<NodeBase> toSearch = new List<NodeBase>() { startNode };
            List<NodeBase> processed = new List<NodeBase>();

            while (toSearch.Count > 0)
            {
                NodeBase current = toSearch[0];

                foreach (NodeBase t in toSearch)
                {
                    if (t.F < current.F || t.F == current.F && t.H < current.H)
                    {
                        current = t;
                    }
                }

                processed.Add(current);
                toSearch.Remove(current);

                if (current == targetNode)
                {
                    NodeBase currentPathTile = targetNode;
                    List<NodeBase> path = new List<NodeBase>();
                    while (currentPathTile != startNode)
                    {
                        path.Add(currentPathTile);
                        currentPathTile = currentPathTile.Connection;
                    }

                    return path;
                }

                //foreach (NodeBase neighbour in current.Neighbours.Where(t => t.IsWalkable && !processed.Contains(t)))
                foreach (NodeBase neighbour in current.Neighbours)
                {
                    if (neighbour.IsWalkable && !processed.Contains(neighbour))
                    {
                        bool inSearch = toSearch.Contains(neighbour);
                        float costToNeighbour = current.G + current.GetDistance(neighbour);

                        if (!inSearch || costToNeighbour < neighbour.G)
                        {
                            neighbour.SetG(costToNeighbour);
                            neighbour.SetConnection(current);

                            if (!inSearch)
                            {
                                neighbour.SetH(neighbour.GetDistance(targetNode));
                                toSearch.Add(neighbour);
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static List<NodeBase> GetWalkRange(NodeBase startNode, float walkRange)
        {
            walkRange -= 1f;

            List<NodeBase> toSearch = new List<NodeBase>() { startNode };
            List<NodeBase> resultNodes = new List<NodeBase>();

            while (toSearch.Count > 0)
            {
                NodeBase current = toSearch[0];

                resultNodes.Add(current);
                toSearch.Remove(current);

                foreach (var neighbour in current.Neighbours)
                {
                    if (neighbour.IsWalkable && !resultNodes.Contains(neighbour))
                    {
                        bool inSearch = toSearch.Contains(neighbour);

                        if (!inSearch && GetPathLength(FindPath(startNode, neighbour)) <= walkRange)
                        {
                            toSearch.Add(neighbour);
                        }
                    }
                }
            }

            return resultNodes;
        }

        public static float GetPathLength(List<NodeBase> pathToCalculate)
        {
            float result = 0f;

            for (int i = 0; i < pathToCalculate.Count - 1; i++)
            {
                result += GetDistanceBetweenNodes(pathToCalculate[i], pathToCalculate[i + 1]);
            }

            return result;
        }

        public static float GetDistanceBetweenNodes(NodeBase startNode, NodeBase endNode)
        {
            if (startNode.Height > endNode.Height)
            {
                return 1f * _mapProvider.GetHeightDistanceMultiplier();
            }
            else if (startNode.Height == endNode.Height)
            {
                return 1f;
            }
            else
            {
                return 1f / _mapProvider.GetHeightDistanceMultiplier();
            }
        }
    }
}