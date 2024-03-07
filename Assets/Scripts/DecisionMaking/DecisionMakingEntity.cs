using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

public class DecisionMakingEntity : MovingEntity
{
    public static Action OnPlayerDead;

    public float m_Acceleration;
    public bool m_CanMoveWhileAttacking;

    List<WeaponImpl> m_Weapons = new List<WeaponImpl>();
    public WeaponImpl m_current;

    SteeringBehaviour_Manager m_SteeringBehavioursManager;
    FuzzyStates_Manger m_FuzzyStatesManager;

    public void Awake()
    {
        base.Awake();

        m_Weapons = GetComponentsInChildren<WeaponImpl>(true).ToList();
        m_current = m_Weapons.FirstOrDefault();

        m_SteeringBehavioursManager = GetComponent<SteeringBehaviour_Manager>();
        if (!m_SteeringBehavioursManager)
        {
            Debug.LogError("Object doesn't have a Steering Behavior Manager attached", this);
        }

        m_FuzzyStatesManager = GetComponent<FuzzyStates_Manger>();
        if (!m_FuzzyStatesManager)
        {
            Debug.LogError("Object doesn't have a FuzzyState Manager attached", this);
        }
    }

    void Update()
    {
        m_FuzzyStatesManager.CalculateActiveStates();
        m_FuzzyStatesManager.RunActiveStates();
    }

    protected override Vector2 GenerateVelocity()
    {
        return m_SteeringBehavioursManager.GenerateSteeringForce();
    }

	public override void DestroyEntity()
	{
        OnPlayerDead?.Invoke();
		base.DestroyEntity();
	}
}
