using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickupState : FuzzyStates
{
    public bool m_PickupExists = false;
    Vector3 m_HealthPickupLocation = Vector2.zero;
    Vector3 m_AmmoPickupLocation = Vector2.zero;

    Health m_Health;
    SteeringBehaviour_Manager m_SteeringBehavioursManager;
    SteeringBehaviour_Seek m_Seek;

    public AnimationCurve m_HealthCurve;
    public AnimationCurve m_AmmoCurve;
    private float m_HealthMembership;
    private float m_AmmoMembership;

    WeaponImpl m_Ammo;
    DecisionMakingEntity m_Entity;

    void Awake()
    {
        PickupManager.OnPickUpSpawned += RecieveOnPickUpSpawned;
        Pickup.PickUpCollected += RecieveOnPickUpCollected;

        m_SteeringBehavioursManager = GetComponent<SteeringBehaviour_Manager>();
        if (!m_SteeringBehavioursManager)
        {
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
        }

        m_Seek = gameObject.AddComponent<SteeringBehaviour_Seek>();
        if (!m_Seek)
        {
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
        }

        m_Health = GetComponent<Health>();
        if (!m_Health)
        {
            Debug.LogError("Object doesn't have a Health attached", this);
        }

        m_Entity = GetComponent<DecisionMakingEntity>();
        if (!m_Entity)
        {
            Debug.LogError("Object doesn't have this script attached", this);
        }
    }

    private void FixedUpdate()
    {
        if(m_PickupExists)
        {
            m_Active = true;
        }
    }

    public override float CalculateActivation()
    {
        if (m_PickupExists)
        {
            m_DegreeOfActivation = 1 - m_Health.HealthRatio;
            return m_DegreeOfActivation;
        }

        m_DegreeOfActivation = 0f;
        return m_DegreeOfActivation;
    }

    public override void Enter()
    {
        m_Ammo = m_Entity.m_current;

        m_SteeringBehavioursManager.AddSteeringBehaviour(m_Seek);

        float healthValue = 1.0f - m_HealthCurve.Evaluate(m_Health.HealthRatio);
        float healthExists = m_PickupExists ? 1.0f : 0.0f;
        m_HealthMembership = Mathf.Min(healthValue, healthExists);

        float ammoValue = 1.0f - m_AmmoCurve.Evaluate(m_Ammo.AmmoRatio);
        float ammoExists = m_PickupExists ? 1.0f : 0.0f;
        m_AmmoMembership = Mathf.Min(ammoValue, ammoExists);

        if (m_HealthMembership > m_AmmoMembership)
        {
            m_Seek.m_TargetPosition = m_HealthPickupLocation;
        }
        else
        {
            m_Seek.m_TargetPosition = m_AmmoPickupLocation;
        }
    }

    public override void Exit()
    {
        if(m_SteeringBehavioursManager.m_SteeringBehaviours.Contains(m_Seek))
        {
            m_SteeringBehavioursManager.RemoveSteeringBehaviour(m_Seek);
        }
    }

    public override void Run()
    {
        m_Seek.m_Weight = Mathf.Lerp(5, 30, m_DegreeOfActivation);
    }

    public void RecieveOnPickUpSpawned(Vector3 pHealth, Vector3 pAmmo)
    {
        m_Active = true;
        m_PickupExists = true;
        m_HealthPickupLocation = pHealth;
        m_AmmoPickupLocation = pAmmo;

        Debug.Log($"Health Ratio = {m_Health.HealthRatio}");
        Debug.Log($"Ammo Ration = {m_Ammo.AmmoRatio}");
    }

    public void RecieveOnPickUpCollected()
    {
        m_Seek.m_TargetPosition = Vector2.zero;
        m_Active = false;
        m_PickupExists = false;
    }
}