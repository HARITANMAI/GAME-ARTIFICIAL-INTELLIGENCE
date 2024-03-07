using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Arrive : SteeringBehaviour
{
    [Header("Arrive Properties")]
    [Header("Settings")]
    public Vector2 m_TargetPosition;
    public float m_SlowingRadius; 

    [Space(10)]

    [Header("Debugs")]
    [SerializeField]
    protected Color m_Debug_RadiusColour = Color.yellow;
    [SerializeField]
    protected Color m_Debug_TargetColour = Color.cyan;


    public override Vector2 CalculateForce()
    {
        Vector2 distance = m_TargetPosition - (Vector2)transform.position;

        if (Maths.Magnitude(distance) >= m_SlowingRadius)
        {
            m_DesiredVelocity = Maths.Normalise(distance) * m_Manager.m_Entity.m_MaxSpeed;
            m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
            return m_Steering * m_Weight;
        }
        else if (Maths.Magnitude(distance) <= m_SlowingRadius)
        {
            m_DesiredVelocity = Maths.Normalise(distance) * m_Manager.m_Entity.m_MaxSpeed;
            m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
            //Vector2 slowfactor = m_Steering * Mathf.Lerp(0, 1, Mathf.Min(Maths.Magnitude(distance), m_SlowingRadius) / m_SlowingRadius);
            return m_Steering * Mathf.Lerp(0, Maths.Magnitude(m_DesiredVelocity), Mathf.Min(Maths.Magnitude(distance), m_SlowingRadius) / m_SlowingRadius);
        }

        return Vector2.zero;
    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                Gizmos.color = m_Debug_TargetColour;
                Gizmos.DrawSphere(m_TargetPosition, 0.5f);

                Gizmos.color = m_Debug_RadiusColour;
                Gizmos.DrawWireSphere(transform.position, m_SlowingRadius);

                base.OnDrawGizmosSelected();
            }
        }
    }
}
