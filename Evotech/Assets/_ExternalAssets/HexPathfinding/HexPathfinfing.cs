using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hexnav.Core
{
    public static class HexPathfinfing
    {
        public static List<NodeBase> FindPath(NodeBase startNode, NodeBase targetNode)
        {
            List<NodeBase> toSearch = new List<NodeBase>() { startNode };
            List<NodeBase> processed = new List<NodeBase>();

            while (toSearch.Any())
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

                foreach (NodeBase neighbour in current.Neighbours.Where(t => t.IsWalkable && !processed.Contains(t)))
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

            return null;
        }
    }
}