using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Cohesion : SteeringBehaviour
{
    public float m_CohesionRange;

    [Range(1, -1)]
    public float m_FOV;
    public override Vector2 CalculateForce()
    {
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_CohesionRange);
        Vector2 AccumulatedPosition = Vector2.zero;

        for (int i = 0; i < entities.Length; i++)
        {
            Vector2 distance = transform.position - entities[i].transform.position;
            if (m_FOV < Maths.Dot(Maths.Normalise(distance), m_Manager.m_Entity.m_Velocity.normalized)) continue;

            foreach (Collider2D entity in entities)
            {
                if (m_FOV <= Maths.Dot(Maths.Normalise(distance), Maths.Normalise(m_Manager.m_Entity.m_Velocity)))
                {
                    AccumulatedPosition += (Vector2)entity.transform.position;
                }
            }
        }

        Vector2 CohesionForce = AccumulatedPosition / entities.Length;

        m_DesiredVelocity = CohesionForce;
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

        return m_Steering * m_Weight;
    }
}
