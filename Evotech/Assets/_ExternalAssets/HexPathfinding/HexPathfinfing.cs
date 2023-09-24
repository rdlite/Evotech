using System.Collections.Generic;
using System.Linq;

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
                toSearch.Add(current);

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
            }

            return null;
        }
    }
}