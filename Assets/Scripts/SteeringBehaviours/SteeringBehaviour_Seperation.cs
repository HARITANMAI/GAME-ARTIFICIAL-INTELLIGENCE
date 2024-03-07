using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Seperation : SteeringBehaviour
{
    public float m_SeperationRange;
    Vector2 accumulatedSeperationForce = Vector2.zero;
    
    [Range(1,-1)]
    public float m_FOV;
    public override Vector2 CalculateForce()
    {
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_SeperationRange);
        accumulatedSeperationForce = Vector2.zero;

        for(int i = 0; i < entities.Length; i++)
        {
            Vector2 differenceVec = transform.position - entities[i].transform.position;
            if (m_FOV < Maths.Dot(Maths.Normalise(differenceVec), Maths.Normalise(m_Manager.m_Entity.m_Velocity))) continue;

            float distance = differenceVec.magnitude;
            if (distance < m_SeperationRange)
            {
                accumulatedSeperationForce += differenceVec;
            }
        }

        m_DesiredVelocity = accumulatedSeperationForce;
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

        return m_Steering * m_Weight;
    }
}