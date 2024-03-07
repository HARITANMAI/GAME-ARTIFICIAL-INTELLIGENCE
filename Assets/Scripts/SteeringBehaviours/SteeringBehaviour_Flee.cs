using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Flee : SteeringBehaviour
{
    [Header("Flee Properties")]
    [Header("Settings")]
    public Transform m_FleeTarget;
    public float m_FleeRadius;

    [Space(10)]

    [Header("Debugs")]
    [SerializeField]
    protected Color m_Debug_RadiusColour = Color.yellow;

    public override Vector2 CalculateForce()
    {
        if (m_FleeTarget != null)
        {
            Vector2 distance = m_FleeTarget.position - transform.position;

            if(Maths.Magnitude(distance) <= m_FleeRadius)
            {
                m_DesiredVelocity = Maths.Normalise(distance) * m_Manager.m_Entity.m_MaxSpeed * -1;
                m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
                return m_Steering * Mathf.Lerp(m_Weight, 0, Mathf.Min(Maths.Magnitude(distance), m_FleeRadius) / m_FleeRadius);
            }
        }

        return Vector2.zero;
    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                Gizmos.color = m_Debug_RadiusColour;
                Gizmos.DrawWireSphere(transform.position, m_FleeRadius);

                base.OnDrawGizmosSelected();
            }
        }
    }
}
