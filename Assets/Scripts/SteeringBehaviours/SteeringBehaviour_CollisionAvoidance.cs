using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SteeringBehaviour_CollisionAvoidance : SteeringBehaviour
{
    [System.Serializable]
    public struct Feeler
	{
        [Range(0, 360)]
        public float m_Angle;
        public float m_MaxLength;
        public Color m_Colour;
    }

    public Feeler[] m_Feelers;
    Vector2[] m_FeelerVectors;
    float[] m_FeelersLength;
    [SerializeField] LayerMask m_FeelerLayerMask;

    float[] distances;
    float shortestDistance = float.MaxValue;
    Vector2 closestVec = Vector2.zero;
    int indexFeeler = 0;

    private void Start()
    {
        distances = new float[m_Feelers.Length];
        m_FeelersLength = new float[m_Feelers.Length];
        m_FeelerVectors = new Vector2[m_Feelers.Length];
    }

    public override Vector2 CalculateForce()
    {
        UpdateFeelers();
        shortestDistance = float.MaxValue;
        closestVec = Vector2.zero;

        for (int i = 0; i < m_Feelers.Length; i++)
        {
            RaycastHit2D tempHit = Physics2D.Raycast(transform.position, m_FeelerVectors[i], m_FeelersLength[i], m_FeelerLayerMask.value);

            if (tempHit.collider == null) continue;

            Vector2 hitVec = tempHit.collider.transform.position - transform.position;
            distances[i] = Maths.Magnitude(hitVec);

            if (distances[i] < shortestDistance)
            {
                shortestDistance = distances[i];
                closestVec = hitVec;
                indexFeeler = i;
            }
        }

        m_DesiredVelocity = Maths.Normalise(closestVec) * m_Manager.m_MaxForce * -1;
        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

        if (closestVec == Vector2.zero) { return Vector2.zero; }
        return m_Steering * Mathf.Lerp(m_Weight, 0, Mathf.Min(Maths.Magnitude(closestVec), m_FeelersLength[indexFeeler]) / m_FeelersLength[indexFeeler]);
    }

    void UpdateFeelers()
    {
        for (int i = 0; i < m_Feelers.Length; ++i)
        {
            m_FeelersLength[i] = Mathf.Lerp(1, m_Feelers[i].m_MaxLength, Maths.Magnitude(m_Manager.m_Entity.m_Velocity) / m_Manager.m_Entity.m_MaxSpeed);
            m_FeelerVectors[i] = Maths.RotateVector(Maths.Normalise(m_Manager.m_Entity.m_Velocity), m_Feelers[i].m_Angle) * m_FeelersLength[i];
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                for (int i = 0; i < m_Feelers.Length; ++i)
                {
                    Gizmos.color = m_Feelers[i].m_Colour;
                    Gizmos.DrawLine(transform.position, (Vector2)transform.position + m_FeelerVectors[i]);
                }

                base.OnDrawGizmosSelected();
            }
        }
    }
}

