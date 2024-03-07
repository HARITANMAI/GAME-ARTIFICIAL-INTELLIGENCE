using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Wander : SteeringBehaviour
{
    [Header("Wander Properties")]
    [Header("Settings")]
    public float m_WanderRadius = 2; 
    public float m_WanderOffset = 2;
    public float m_AngleDisplacement = 20;

    Vector2 m_CirclePosition;
    Vector2 m_PointOnCircle;
    float m_Angle = 0.0f;

    [Space(10)]

    [Header("Debugs")]
    [SerializeField]
    protected Color m_Debug_RadiusColour = Color.yellow;
    [SerializeField]
    protected Color m_Debug_TargetColour = Color.cyan;


    public override Vector2 CalculateForce()
    {
        m_Angle += Random.Range(-m_AngleDisplacement, m_AngleDisplacement);
        m_Angle = Mathf.Repeat(m_Angle, 360);

        //Turning the angle into radians
        float rad = Mathf.Deg2Rad * m_Angle;

        //Getting a point on the circle with a radius of 2, at a distance of 2 units infront of the player
        m_CirclePosition = (Vector2)transform.position + (Maths.Normalise(m_Manager.m_Entity.m_Velocity) * m_WanderOffset);
        m_PointOnCircle = m_CirclePosition + (new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * m_WanderRadius);

        //Seek Behaviour
        m_DesiredVelocity = m_PointOnCircle - (Vector2)transform.position;
        m_DesiredVelocity = Maths.Normalise(m_DesiredVelocity) * m_Manager.m_Entity.m_MaxSpeed;
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

        return m_Steering * m_Weight;
    }

	protected override void OnDrawGizmosSelected()
	{
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(m_CirclePosition, m_WanderRadius);

				Gizmos.color = Color.blue;
				Gizmos.DrawLine(transform.position, m_CirclePosition);

				Gizmos.color = Color.green;
				Gizmos.DrawLine(m_CirclePosition, m_PointOnCircle);

				Gizmos.color = Color.red;
				Gizmos.DrawLine(transform.position, m_PointOnCircle);

                base.OnDrawGizmosSelected();
			}
        }
	}
}
