using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[System.Serializable]
public class Pathfinding_JPS : PathFinding
{
    [System.Serializable]
    class NodeInformation
    {
        public GridNode node;
        public NodeInformation parent;
        public float gCost;
        public float hCost;
        public float fCost;

        public NodeInformation(GridNode node, NodeInformation parent, float gCost, float hCost)
        {
            this.node = node;
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
            fCost = gCost + hCost;
        }

        public void UpdateNodeInformation(NodeInformation parent, float gCost, float hCost)
        {
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
            fCost = gCost + hCost;
        }
    }

    public Pathfinding_JPS(bool allowDiagonal, bool cutCorners) : base(allowDiagonal, cutCorners) { }

    public override void GeneratePath(GridNode start, GridNode end)
    {
        //clears current path
        m_Path.Clear();

        //list to track visted and unvisited nodes
        List<NodeInformation> openList = new List<NodeInformation>();
        List<NodeInformation> closedList = new List<NodeInformation>();

        
        NodeInformation current = new NodeInformation(start, null, 0, 0);

        int maxIterations = 0;

        while (current != null)
        {
            maxIterations++;
            if (maxIterations > m_MaxPathCount)
            {
                Debug.LogError("Max Iteration Reached");
                break;
            }

            //NodeInformation currentNode = GetCheapestNode(openList);

            if (current.node == end)
            {
                Debug.Log("Path found, start pos = " + start.transform.position + " - end pos = " + end.transform.position);
                SetPath(current);
                DrawPath(closedList, openList);
                return;
            }

            for(int i = 0; i < 8; i++)
            {
                GridNode neighbour = current.node.Neighbours[i];

                if (neighbour == null || DoesListContainNode(closedList, neighbour) || neighbour.m_Walkable == false)
                {
                    continue;
                }

                ExploreNeighbors(current, end, openList, closedList);
            }

            openList.Remove(current);
            closedList.Add(current);

            if(openList.Count > 0)
            {
                current = GetCheapestNode(openList);
            }
            else
            {
                break;
            }


        }

        Debug.LogError("No path found, start pos = " + start.transform.position + " - end pos = " + end.transform.position);
    }

    private void ExploreNeighbors(NodeInformation currentNode, GridNode end, List<NodeInformation> openList, List<NodeInformation> closedList)
    {
        // Iterate through all neighbors
        for (int i = 0; i < 8; ++i)
        {
            GridNode neighbor = currentNode.node.Neighbours[i];

            // Check if the neighbor is valid and walkable
            if (neighbor == null || DoesListContainNode(closedList, neighbor) || neighbor.m_Walkable == false)
                continue;

            // Identify jump points from the current node
            List<GridNode> jumpPoints = IdentifyJumpPoints(neighbor, currentNode.node, end);

            // Add jump points to open list
            foreach (GridNode jumpPoint in jumpPoints)
            {
                float hCost = Heuristic_Euclidean(jumpPoint, end);
                float gCost = currentNode.gCost + Vector2.Distance(currentNode.node.transform.position, jumpPoint.transform.position);
                float fCost = gCost + hCost;

                NodeInformation existingNode = GetNodeInformationFromList(openList, jumpPoint);
                if (existingNode != null)
                {
                    if (existingNode.fCost > fCost)
                        existingNode.UpdateNodeInformation(currentNode, gCost, hCost);
                }
                else
                {
                    NodeInformation newNode = new NodeInformation(jumpPoint, currentNode, gCost, hCost);
                    openList.Add(newNode);
                }
            }
        }
    }


    private List<GridNode> IdentifyJumpPoints(GridNode currentNode, GridNode parentNode, GridNode endNode)
    {
        List<GridNode> jumpPoints = new List<GridNode>();

        // Calculate the direction of movement from the parent node to the current node
        float dx = currentNode.transform.position.x.CompareTo(parentNode.transform.position.x);
        float dy = currentNode.transform.position.y.CompareTo(parentNode.transform.position.y);

        // Check for jump points in horizontal and vertical directions
        if (dx != 0 && dy == 0)
        {
            // Horizontal movement
            if ((dx < 0 && currentNode.Neighbours[6] != null && currentNode.Neighbours[6].m_Walkable) ||
                (dx > 0 && currentNode.Neighbours[2] != null && currentNode.Neighbours[2].m_Walkable))
            {
                jumpPoints.Add(currentNode);
            }
        }
        else if (dx == 0 && dy != 0)
        {
            // Vertical movement
            if ((dy < 0 && currentNode.Neighbours[4] != null && currentNode.Neighbours[4].m_Walkable) ||
                (dy > 0 && currentNode.Neighbours[0] != null && currentNode.Neighbours[0].m_Walkable))
            {
                jumpPoints.Add(currentNode);
            }
        }
        else if (dx != 0 && dy != 0)
        {
            // Diagonal movement
            if ((dx < 0 && dy < 0 && currentNode.Neighbours[5] != null && currentNode.Neighbours[5].m_Walkable) ||
                (dx > 0 && dy < 0 && currentNode.Neighbours[3] != null && currentNode.Neighbours[3].m_Walkable) ||
                (dx < 0 && dy > 0 && currentNode.Neighbours[7] != null && currentNode.Neighbours[7].m_Walkable) ||
                (dx > 0 && dy > 0 && currentNode.Neighbours[1] != null && currentNode.Neighbours[1].m_Walkable))
            {
                jumpPoints.Add(currentNode);
            }
        }

        return jumpPoints;
    }
    private void SetPath(NodeInformation end)
    {
        NodeInformation current = end;
        while (current != null)
        {
            m_Path.Add(current.node.transform.position);
            current = current.parent;
        }
        m_Path.Reverse();
    }

    private NodeInformation GetCheapestNode(List<NodeInformation> nodes)
    {
        return nodes.OrderBy(n => n.fCost).First();
    }

    private bool DoesListContainNode(List<NodeInformation> nodeInformation, GridNode gridNode)
    {
        return nodeInformation.Any(x => x.node == gridNode);
    }

    private NodeInformation GetNodeInformationFromList(List<NodeInformation> nodeInformation, GridNode gridNode)
    {
        return nodeInformation.Find(x => x.node == gridNode);
    }

    private void DrawPath(List<NodeInformation> closed, List<NodeInformation> open)
    {
        if (m_Debug_ChangeTileColours)
        {
            Grid.ResetGridNodeColours();

            foreach (NodeInformation node in closed)
            {
                node.node.SetOpenInPathFinding();
            }

            foreach (NodeInformation node in open)
            {
                node.node.SetClosedInPathFinding();
            }
        }
    }
}

