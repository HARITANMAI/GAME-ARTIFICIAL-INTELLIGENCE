using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAvoidanceState : FuzzyStates
{
    Transform m_WallLocation;

    SteeringBehaviour_Manager m_SteeringBehavioursManager;
    SteeringBehaviour_Flee m_Flee;

    private void Awake()
    {
        m_SteeringBehavioursManager = GetComponent<SteeringBehaviour_Manager>();
        if (!m_SteeringBehavioursManager)
        {
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
        }

        m_Flee = gameObject.AddComponent<SteeringBehaviour_Flee>();
        if (!m_Flee)
        {
            Debug.LogError("Object doesn't have this Steering Behaviour attached", this);
        }

        m_Flee.m_FleeRadius = 2f;
    }

    private void FixedUpdate()
    {
        m_WallLocation = CheckForWalls();
        m_Flee.m_FleeTarget = m_WallLocation;
    }

    public override float CalculateActivation()
    {
        if (!m_WallLocation)
        {
            float dist = (transform.position - m_WallLocation.transform.position).magnitude;
            m_DegreeOfActivation = 1 - Mathf.Clamp01(dist / 2f);

            return m_DegreeOfActivation;
        }
        m_DegreeOfActivation = 0f;
        return m_DegreeOfActivation;
    }

    public override void Enter()
    {
        m_SteeringBehavioursManager.AddSteeringBehaviour(m_Flee);
    }

    public override void Exit()
    {
        m_SteeringBehavioursManager.RemoveSteeringBehaviour(m_Flee);
    }

    public override void Run()
    {
        m_Flee.m_Weight = Mathf.Lerp(5, 30, m_DegreeOfActivation);
    }

    Transform CheckForWalls()
    {
        //float m_ShortestWallDistance = float.MaxValue;
        //float[] m_Distances;
        //int m_Index = -1;

        //RaycastHit2D walls = Physics2D.Raycast(transform.position, transform.up, 10.0f, 13);
        //Debug.Log(walls);

        //if(walls != null)
        //{
        //    m_Active = true;
            
        //    Transform wallTrasform = walls.collider.transform;
        //    return wallTrasform;
        //}

        //Collider2D[] walls = Physics2D.OverlapCircleAll(transform.position, 1.0f, 13);
        //Debug.Log($"NUMBER OF CURRENT WALLS = {walls.Length}");

        //if(walls.Length > 0)
        //{
        //    m_Active = true;
        //    m_Distances = new float[walls.Length];
        //    for (int i = 0; i < walls.Length; i++)
        //    {
        //        Vector2 hitVec = walls[i].transform.position - transform.position;
        //        m_Distances[i] = hitVec.magnitude;

        //        if (m_Distances[i] < m_ShortestWallDistance)
        //        {
        //            m_ShortestWallDistance = m_Distances[i];
        //            m_Index = i;
        //        }
        //    }

        //    Transform wallTransform = walls[m_Index].transform;
        //    for(int i = 0; i < walls.Length; i++)
        //    {
        //        Array.Clear(walls, i, walls.Length);
        //    }

        //    Debug.Log($"NUMBER OF CURRENT WALLS AFTER CLEARING = {walls.Length}");
        //    return wallTransform;
        //}

        m_Active = false;
        return null;
    }
}

//HOW TO SET THE ARRAY TO IT HAVING 0 VARIABLES IN IT