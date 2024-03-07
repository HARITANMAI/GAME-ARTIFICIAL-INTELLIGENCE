using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Pursuit : SteeringBehaviour
{
    [Header("Pursuit Properties")]
    [Header("Settings")]
    public MovingEntity m_PursuingEntity;

    public override Vector2 CalculateForce()
    {
        Vector2 distance = transform.position - m_PursuingEntity.transform.position;
        float m_CombinedSpeed =  distance.magnitude + m_PursuingEntity.m_MaxSpeed;
        float preTime = distance.magnitude / m_CombinedSpeed;

        Vector2 SeekTarget = (Vector2)m_PursuingEntity.transform.position + (m_PursuingEntity.m_Velocity * preTime);

        m_DesiredVelocity = SeekTarget -(Vector2)transform.position;
        m_DesiredVelocity = Maths.Normalise(m_DesiredVelocity) * m_Manager.m_Entity.m_MaxSpeed;
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

        return m_Steering * m_Weight;
    }
}
