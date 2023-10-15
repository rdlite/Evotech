using Core.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Hexnav.Core
{
    public class HexPathfindingGrid
    {
        public static float DistanceBetweenNodes = 1f;

        private static NodeBase[,] _nodesGrid;
        private static Vector2Int[,] _neighbourOffsets;
        private static IMapDataProvider _mapProvider;

        public HexPathfindingGrid(IMapDataProvider mapProvider)
        {
            _mapProvider = mapProvider;

            _neighbourOffsets = new Vector2Int[2, 6]
            {
                {
                    new Vector2Int(1, 0),
                    new Vector2Int(0, -1),
                    new Vector2Int(-1, -1),
                    new Vector2Int(-1, 0),
                    new Vector2Int(-1, 1),
                    new Vector2Int(0, 1),
                },
                {
                    new Vector2Int(1, 0),
                    new Vector2Int(1, -1),
                    new Vector2Int(0, -1),
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                }
            };
        }

        public void LoadNodesArray(NodeBase[] nodes)
        {
            _nodesGrid = ConvertArrayToNodesGrid(nodes);
            FillNeighbours(_nodesGrid);
        }

        private NodeBase[,] ConvertArrayToNodesGrid(NodeBase[] nodes)
        {
            int minXNode = nodes.Length, minYNode = nodes.Length, maxXNode = -nodes.Length, maxYNode = -nodes.Length;

            for (int i = 0; i < nodes.Length; i++)
            {
                if (minXNode > nodes[i].Point.x)
                {
                    minXNode = nodes[i].Point.x;
                }

                if (minYNode > nodes[i].Point.y)
                {
                    minYNode = nodes[i].Point.y;
                }

                if (maxXNode < nodes[i].Point.x)
                {
                    maxXNode = nodes[i].Point.x;
                }

                if (maxYNode < nodes[i].Point.y)
                {
                    maxYNode = nodes[i].Point.y;
                }
            }

            maxXNode++;
            maxYNode++;

            NodeBase[,] grid = new NodeBase[Mathf.Abs(minXNode) + Mathf.Abs(maxXNode), Mathf.Abs(minYNode) + Mathf.Abs(maxYNode)];

            for (int x = minXNode; x < maxXNode; x++)
            {
                for (int y = minYNode; y < maxYNode; y++)
                {
                    NodeBase node = null;

                    for (int i = 0; i < nodes.Length; i++)
                    {
                        if (nodes[i].Point.x == x && nodes[i].Point.y == y)
                        {
                            node = nodes[i];
                            break;
                        }
                    }

                    grid[Mathf.Abs(minXNode) + x, Mathf.Abs(minYNode) + y] = node;
                }
            }

            return grid;
        }

        private void FillNeighbours(NodeBase[,] nodesGrid)
        {
            for (int x = 0; x < nodesGrid.GetLength(0); x++)
            {
                for (int y = 0; y < nodesGrid.GetLength(1); y++)
                {
                    if (nodesGrid[x, y] != null)
                    {
                        int rowEven = y % 2;

                        for (int i = 0; i < _neighbourOffsets.GetLength(1); i++)
                        {
                            if (x + _neighbourOffsets[rowEven, i].x >= 0 && y + _neighbourOffsets[rowEven, i].y >= 0 &&
                                x + _neighbourOffsets[rowEven, i].x < nodesGrid.GetLength(0) && y + _neighbourOffsets[rowEven, i].y < nodesGrid.GetLength(1))
                            {
                                NodeBase neighbour = nodesGrid[x + _neighbourOffsets[rowEven, i].x, y + _neighbourOffsets[rowEven, i].y];

                                if (neighbour != null && (Mathf.Abs(nodesGrid[x, y].Height - neighbour.Height)) <= 1)
                                {
                                    nodesGrid[x, y].AddNeightbour(neighbour);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static NodeBase[,] GetNodesGrid()
        {
            return _nodesGrid;
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
                    path.Reverse();

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

        public static int GetDistanceBetweenPointsInNodes(Vector3 point1, Vector3 point2)
        {
            point1.y = 0f;
            point2.y = 0f;

            return (int)Vector3.Distance(point1, point2);
        }

        public static float GetDistanceBetweenNodes(NodeBase startNode, NodeBase endNode)
        {
            if (startNode.Height < endNode.Height)
            {
                return DistanceBetweenNodes * _mapProvider.GetHeightDistanceMultiplier();
            }
            else if (startNode.Height == endNode.Height)
            {
                return DistanceBetweenNodes;
            }
            else
            {
                return DistanceBetweenNodes / _mapProvider.GetHeightDistanceMultiplier();
            }
        }
    }
}