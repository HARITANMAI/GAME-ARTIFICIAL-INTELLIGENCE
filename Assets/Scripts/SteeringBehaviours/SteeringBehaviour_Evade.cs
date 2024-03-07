using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Evade : SteeringBehaviour
{
    [Header("Evade Properties")]
    [Header("Settings")]
    public MovingEntity m_EvadingEntity;
    public float m_EvadeRadius;

    [Space(10)]

    [Header("Debugs")]
    [SerializeField]
    protected Color m_Debug_RadiusColour = Color.yellow;

    public override Vector2 CalculateForce()
    {
        Vector2 distance =  transform.position - m_EvadingEntity.transform.position;
        float m_CombinedSpeed = distance.magnitude + m_EvadingEntity.m_MaxSpeed;
        float preTime = distance.magnitude / m_CombinedSpeed;

        if (distance.magnitude <= m_EvadeRadius)
        {
            Vector2 FleeTarget = (Vector2)m_EvadingEntity.transform.position + (m_EvadingEntity.m_Velocity * preTime);

            m_DesiredVelocity = FleeTarget - (Vector2)transform.position;
            m_DesiredVelocity = Maths.Normalise(m_DesiredVelocity) * m_Manager.m_Entity.m_MaxSpeed * -1;
            m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

            return m_Steering * Mathf.Lerp(m_Weight, 0, Mathf.Min(Maths.Magnitude(distance), m_EvadeRadius) / m_EvadeRadius);
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
                Gizmos.DrawWireSphere(transform.position, m_EvadeRadius);

                base.OnDrawGizmosSelected();
            }
        }
    }
}
