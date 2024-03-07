using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class WanderState : FuzzyStates
{
    public SimpleEnemy m_Enemy;
    HealthPickupState m_HealthPickupState;
    AttackState m_AttackState;
    SteeringBehaviour_Manager m_SteeringBehavioursManager;
    SteeringBehaviour_Wander m_Wander;

    private void Awake()
    {
        PickupManager.OnPickUpSpawned += RecieveOnPickUpSpawned;
        Pickup.PickUpCollected += RecieveOnPickUpCollected;

        m_HealthPickupState = GetComponent<HealthPickupState>();
        if (!m_HealthPickupState)
        {
            Debug.LogError("Object doesn't have a this component attached", this);
        }

        m_SteeringBehavioursManager = GetComponent<SteeringBehaviour_Manager>();
        if (!m_SteeringBehavioursManager)
        {
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
        }

        m_Wander = gameObject.AddComponent<SteeringBehaviour_Wander>();
        if (!m_Wander)
        {
            Debug.LogError("Object doesn't have this Steering Behaviour attached", this);
        }

        m_AttackState = GetComponent<AttackState>();
        if (!m_AttackState)
        {
            Debug.LogError("Object doesn't have this State attached", this);
        }

        m_Active = true;
    }

    private void Update()
    {
        m_Enemy = CheckForEnemies();
    }

    public override float CalculateActivation()
    {
        if(!m_Enemy)
        {
            m_DegreeOfActivation = 1f;
            return m_DegreeOfActivation;
        }

        m_DegreeOfActivation = 0.4f;
        return m_DegreeOfActivation;
    }

    public override void Enter()
    {
        m_Wander.m_WanderRadius = 2f;
        m_Wander.m_WanderOffset = 2f;
        m_Wander.m_AngleDisplacement = 20f;
        m_SteeringBehavioursManager.AddSteeringBehaviour(m_Wander);
    }

    public override void Exit()
    {
        if (m_SteeringBehavioursManager.m_SteeringBehaviours.Contains(m_Wander))
        {
            m_SteeringBehavioursManager.RemoveSteeringBehaviour(m_Wander);
        }
    }

    public override void Run()
    {
        m_Wander.m_Weight = Mathf.Lerp(5, 30, m_DegreeOfActivation);
    }

    SimpleEnemy CheckForEnemies()
    {
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, 5.0f);

        if(entities.Length > 1)
        {
            //Debug.Log($"Wander Entites = {entities.Length}");

            for (int i = 1; i < entities.Length; i++)
            {
                SimpleEnemy simpleEnemy = entities[i].GetComponent<SimpleEnemy>();

                if (simpleEnemy != null)
                {
                    return simpleEnemy;
                }
            }
        }

        return null;
    }

    public void RecieveOnPickUpSpawned(Vector3 pHealth, Vector3 pAmmo)
    {
        m_Active = false;
    }

    public void RecieveOnPickUpCollected()
    {
        m_Active = true;
    }
}