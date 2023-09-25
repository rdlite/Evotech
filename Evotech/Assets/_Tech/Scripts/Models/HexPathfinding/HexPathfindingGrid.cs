using UnityEngine;

namespace Hexnav.Core
{
    public class HexPathfindingGrid
    {
        private static NodeBase[,] _nodesGrid;
        private static Vector2Int[,] _neighbourOffsets;

        public HexPathfindingGrid()
        {
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
    }
}