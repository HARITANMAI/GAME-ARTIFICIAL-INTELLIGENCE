using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class EvadeState : FuzzyStates
{
    public SimpleEnemy m_Enemy;
    public bool m_EnemyExists = false;

    float[] m_Distances;
    float m_shortestDistance = float.MaxValue;
    int m_Index = -1;

    SteeringBehaviour_Manager m_SteeringBehavioursManager;
    SteeringBehaviour_Evade m_Evade;

    float m_EvadeMembership;

    public DecisionMakingEntity m_Entity;
    List<WeaponImpl> m_Weapons = new List<WeaponImpl>();
    WeaponImpl m_currentWeapon;

    //Vector2[] directions;
    //Vector2 m_AverageEnemyDirection = Vector2.zero;

    private void Awake()
    {
        m_Entity = GetComponent<DecisionMakingEntity>();
        m_Weapons = GetComponentsInChildren<WeaponImpl>(true).ToList();
        m_currentWeapon = m_Entity.m_current;

        m_SteeringBehavioursManager = GetComponent<SteeringBehaviour_Manager>();
        if (!m_SteeringBehavioursManager)
        {
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
        }

        m_Evade = gameObject.AddComponent<SteeringBehaviour_Evade>();
        if (!m_Evade)
        {
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
        }

        m_Evade.m_EvadeRadius = 5f;
    }

    private void FixedUpdate()
    {
        m_Enemy = CalculateClosestEnemy();
        m_Evade.m_EvadingEntity = m_Enemy;

        if (m_Enemy)
        {
            m_Active = true;
            float dist = (transform.position - m_Enemy.transform.position).magnitude;
            m_EvadeMembership = 1 - Mathf.Clamp01(dist / 5f);
        }
        else
        {
            m_Active = false;
        }
    }

    public override float CalculateActivation()
    {
        if (m_Enemy)
        {
            m_DegreeOfActivation = m_EvadeMembership;
            return m_DegreeOfActivation;
        }

        m_DegreeOfActivation = 0f;
        return m_DegreeOfActivation;
    }

    public override void Enter()
    {
        m_SteeringBehavioursManager.AddSteeringBehaviour(m_Evade);
    }

    public override void Exit()
    {
        if(m_SteeringBehavioursManager.m_SteeringBehaviours.Contains(m_Evade))
        {
            m_SteeringBehavioursManager.RemoveSteeringBehaviour(m_Evade);
        }
    }

    public override void Run()
    {
        m_Evade.m_Weight = Mathf.Lerp(5, 30, m_DegreeOfActivation);
    }

    SimpleEnemy CalculateClosestEnemy()
    {
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, 5.0f);
        m_shortestDistance = float.MaxValue;

        if (entities.Length > 1) 
        {
            //Debug.Log($"Evading Entities = {entities.Length}");

            m_Distances = new float[entities.Length];
            for(int i = 1; i < entities.Length; i++)
            {
                Vector2 hitVec = entities[i].transform.position - transform.position;
                m_Distances[i] = hitVec.magnitude;

                if (m_Distances[i] < m_shortestDistance)
                {
                    m_shortestDistance = m_Distances[i];
                    m_Index = i;
                }
            }

            SimpleEnemy simpleEnemy = entities[m_Index].GetComponent<SimpleEnemy>();

            SwitchWeapons();

            return simpleEnemy;
        }

        return null;
    }

    void SwitchWeapons()
    {
        if (m_shortestDistance <= 5f && m_shortestDistance > 3f)
        {
            EquipWeaponOfType(WeaponType.Handgun);
        }
        else if (m_shortestDistance <= 3f && m_shortestDistance > 1.5f)
        {
            EquipWeaponOfType(WeaponType.Machinegun);
        }
        else if (m_shortestDistance < 1.5f)
        {
            EquipWeaponOfType(WeaponType.Shotgun);
        }
    }

    private void EquipWeaponOfType(WeaponType type)
    {
        m_currentWeapon.gameObject.SetActive(false);

        foreach (WeaponImpl w in m_Weapons)
        {
            if (w.GetWeaponType() == type)
            {
                m_currentWeapon = w;
                m_currentWeapon.gameObject.SetActive(true);
                break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 5.0f);
    }

    //public void Directions(Collider2D[] entities)
    //{
    //    entities = Physics2D.OverlapCircleAll(transform.position, 5.0f);

    //    if(entities.Length > 1)
    //    {
    //        for(int i = 0; i < entities.Length; i++)
    //        {
    //            directions[i] = entities[i].gameObject.transform.forward;
    //        }
    //    }
    //}

    //public void AverageDirection(Vector2[] DirectionsToSum)
    //{
    //    Vector2 enemyDirections = Vector2.zero;

    //    for (int i = 0; i < DirectionsToSum.Length; i++)
    //    {
    //        enemyDirections += DirectionsToSum[i];
    //    }

    //    m_AverageEnemyDirection = enemyDirections / DirectionsToSum.Length;
    //}
}
