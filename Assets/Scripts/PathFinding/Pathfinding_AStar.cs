using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Pathfinding_AStar : PathFinding
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

    public Pathfinding_AStar(bool allowDiagonal, bool cutCorners) : base(allowDiagonal, cutCorners) { }

    public override void GeneratePath(GridNode start, GridNode end)
    {
        //clears the current path
        m_Path.Clear();

        //lists to track the open and closed nodes
        List<NodeInformation> openList = new List<NodeInformation>();
        List<NodeInformation> closedList = new List<NodeInformation>();

        //replace with start
        NodeInformation current = new NodeInformation(start, null, 0, 0);

        int maxIternation = 0;

        //loop while there is a node selected
        while (current != null)
        {
            maxIternation++;
            if (maxIternation > m_MaxPathCount)
            {
                Debug.LogError("Max Iteration Reached");
                break;
            }

            //if the current node is the end node, a path has been found.
            if (current.node == end)
            {
                Debug.Log("Path found, start pos = " + start.transform.position + " - end pos = " + end.transform.position);
                SetPath(current);
                DrawPath(closedList, openList);
                return;
            }
            //F = the path generated, G = total cost travelled, H estimated cost from end node to current node
            for (int i = 0; i < 8; ++i)
            {
                GridNode HelloNeighbour = current.node.Neighbours[i];
                if (HelloNeighbour == null || DoesListContainNode(closedList, HelloNeighbour) || HelloNeighbour.m_Walkable == false)
                {
                    continue;
                }

                float HCost = Heuristic_Euclidean(HelloNeighbour, end);
                Vector2 GDist = current.node.transform.position - HelloNeighbour.transform.position;
                float GCost = current.gCost + GDist.magnitude;
                float FCost = GCost + HCost;
                NodeInformation Check = GetNodeInformationFromList(openList, HelloNeighbour);
                if (DoesListContainNode(openList, HelloNeighbour))
                {
                    if (Check.fCost > FCost)
                    {
                        NodeInformation NewNeighbour = GetNodeInformationFromList(openList, HelloNeighbour);
                        NewNeighbour.UpdateNodeInformation(current, GCost, HCost);
                    }
                }
                else
                {
                    NodeInformation NewNode = new NodeInformation(HelloNeighbour, current, GCost, HCost);
                    openList.Add(NewNode);
                }

            }

            closedList.Add(current);
            openList.Remove(current);

            if (openList.Count > 0)
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

    /// <summary>
    /// pass in the final node information and sets m_Path
    /// </summary>
    private void SetPath(NodeInformation end)
    {
        NodeInformation CurrentNode = end;

        while (CurrentNode != null)
        {
            m_Path.Add(CurrentNode.node.transform.position);
            CurrentNode = CurrentNode.parent;
        }
        m_Path.Reverse();

    }

    /// <summary>
    /// Returns the cheapest node in the list calculated by cost
    /// </summary>
    private NodeInformation GetCheapestNode(List<NodeInformation> nodes)
    {
        return nodes.OrderBy(n => n.fCost).First();
    }

    /// <summary>
    /// checks if a grid node reference is held within a list of Node Informations
    /// </summary>
    bool DoesListContainNode(List<NodeInformation> nodeInformation, GridNode gridNode)
    {
        return nodeInformation.Any(x => x.node == gridNode);
    }

    /// <summary>
    /// Returns a Node Information if a grid node reference is within the list
    /// </summary>
    NodeInformation GetNodeInformationFromList(List<NodeInformation> nodeInformation, GridNode gridNode)
    {
        return nodeInformation.Find(x => x.node == gridNode);
    }

    /// <summary>
    /// Changest the colour of the grid based on the values passed in
    /// </summary>
    void DrawPath(List<NodeInformation> open, List<NodeInformation> closed)
    {
        //drawPath
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

