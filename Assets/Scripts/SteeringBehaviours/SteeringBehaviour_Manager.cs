using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Manager : MonoBehaviour
{
    public MovingEntity m_Entity { get; private set; }
    public float m_MaxForce = 100;
    public float m_RemainingForce;
    public List<SteeringBehaviour> m_SteeringBehaviours;

	private void Awake()
	{
        m_Entity = GetComponent<MovingEntity>();

        if(!m_Entity)
            Debug.LogError("Steering Behaviours only working on type moving entity", this);
    }

	public Vector2 GenerateSteeringForce()
    {
        if (m_MaxForce <= 0) return Vector2.zero;

        Vector2 force = Vector2.zero;
        Vector2 tempForce = Vector2.zero;
        m_RemainingForce = m_MaxForce;

        for(int i = 0; i <  m_SteeringBehaviours.Count; i++)
        {
            if (m_RemainingForce <= 0) break;
            if (!m_SteeringBehaviours[i].m_Active) continue;

            tempForce += m_SteeringBehaviours[i].CalculateForce();

            if (tempForce.magnitude > m_RemainingForce)
            {
                tempForce = tempForce.normalized * m_RemainingForce;
            }

            force += tempForce;
            m_RemainingForce -= Maths.Magnitude(tempForce);
        }

        return force;
    }

    public void AddSteeringBehaviour(SteeringBehaviour behaviours)
    {
        m_SteeringBehaviours.Add(behaviours);
    }

    public void RemoveSteeringBehaviour(SteeringBehaviour behaviours)
    {
        m_SteeringBehaviours.Remove(behaviours);
    }

    public void EnableExclusive(SteeringBehaviour behaviour)
    {
        if (m_SteeringBehaviours.Contains(behaviour))
        {
            foreach (SteeringBehaviour sb in m_SteeringBehaviours)
            {
                sb.m_Active = false;
            }

            behaviour.m_Active = true;
        }
        else
        {
            Debug.Log(behaviour + " doesn't not exist on object", this);
        }
    }

    public void DisableAllSteeringBehaviours()
    {
        foreach (SteeringBehaviour sb in m_SteeringBehaviours)
        {
            sb.m_Active = false;
        }
    }
}
