using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathFinding
{
    protected GridNode[] m_GridNodes;

    [Header("Settings")]
	[Tooltip("Sets whether the agent can move diagonally, can only be set in the editor during runtime")]
	protected bool m_AllowDiagonal;
	[Tooltip("Sets whether the agent can cut corners when moving diagonally, can only be set in the editor during runtime")]
	protected bool m_CanCutCorners;

    /// <summary>
    /// Caps the max iteration count when calculating paths. Useful to stop infinate loops
    /// </summary>
    protected int m_MaxPathCount = 10000;

    [Header("Debug")]

    [Tooltip("Changes the colour of the grid to show pathing, can only be set during runtime")]
    [SerializeField]
    public bool m_Debug_ChangeTileColours;

	public List<Vector2> m_Path { get; protected set; }

    public PathFinding(bool allowDiagonal, bool cutCorners)
    {
        m_Path = new List<Vector2>();
        m_AllowDiagonal = allowDiagonal;
        m_CanCutCorners = cutCorners;
        m_GridNodes = Grid.GridNodes;
    }

    public abstract void GeneratePath(GridNode start, GridNode end);

    public Vector2 GetClosestPointOnPath(Vector2 position)
    {
        float distance = float.MaxValue;
        int closestPoint = int.MaxValue;

        for (int i = 0; i < m_Path.Count; ++i)
        {
            float tempDistance = Maths.Magnitude(m_Path[i] - position);
            if (tempDistance < distance)
            {
                closestPoint = i;
                distance = tempDistance;
            }
        }

        for (int j = 0; j < closestPoint - 1; ++j)
        {
            m_Path.RemoveAt(0);
        }

        return m_Path[0];
    }

    public Vector2 GetNextPointOnPath(Vector2 position)
    {
        Vector2 pos = position;
        if (m_Path.Count > 0)
        {
            m_Path.RemoveAt(0);

            if (m_Path.Count > 0)
                pos = m_Path[0];
        }

        return pos;
    }

    protected float Heuristic_Manhattan(GridNode start, GridNode end)
    {
        Vector2 delta = start.transform.position - end.transform.position;

        float manhattan = Mathf.Abs(delta.x + delta.y);


        return manhattan;
    }

    protected float Heuristic_Euclidean(GridNode start, GridNode end)
    {
        Vector2 delta = start.transform.position - end.transform.position;

        float euclidean = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y);   


        return euclidean;
    }

    protected float Heuristic_Octile(GridNode start, GridNode end)
    {
		Vector2 delta = start.transform.position - end.transform.position;

        float octile = Mathf.Max(delta.x, delta.y) + (0.41f * Mathf.Min(delta.x, delta.y));

		return octile;
	}

    public float Heuristic_Chebyshev(GridNode start, GridNode end)
    {
        Vector2 delta = start.transform.position - end.transform.position;

        float chebyshev = Mathf.Max(delta.x, delta.y);

        return chebyshev;
	}
}
