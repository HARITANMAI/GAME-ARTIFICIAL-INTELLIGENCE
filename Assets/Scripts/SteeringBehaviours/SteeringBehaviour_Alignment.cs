using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Alignment : SteeringBehaviour
{
    public float m_AlignmentRange;
    Vector2 accumulatedAlignmentForce = Vector2.zero;

    [Range(1,-1)]
    public float m_FOV;
    public override Vector2 CalculateForce()
    {
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_AlignmentRange);
        accumulatedAlignmentForce = Vector2.zero;
        
        for(int i = 0; i < entities.Length; i++)
        {
            Vector2 differenceVec = transform.position - entities[i].transform.position;
            if (m_FOV < Maths.Dot(Maths.Normalise(differenceVec), Maths.Normalise(m_Manager.m_Entity.m_Velocity))) continue;

            foreach(Collider2D entity in entities)
            {
                accumulatedAlignmentForce += (Vector2)entity.transform.forward;
            }
        }

        Vector2 alignmentForce = accumulatedAlignmentForce / entities.Length;
        return alignmentForce * m_Weight;
    }
}
