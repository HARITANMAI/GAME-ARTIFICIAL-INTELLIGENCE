using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackState : FuzzyStates
{
    List<WeaponImpl> m_Weapons = new List<WeaponImpl>();
    WeaponImpl m_currentWeapon;

    public bool m_EnemySpotted = false;
    public DecisionMakingEntity m_entity;
    public SimpleEnemy closestEnemy;

    private void Start()
    {
        m_entity = GetComponent<DecisionMakingEntity>();
        m_Weapons = GetComponentsInChildren<WeaponImpl>(true).ToList();
        m_currentWeapon = m_entity.m_current;
    }

    private void FixedUpdate()
    {
        closestEnemy = CheckForEnemies();
    }

    public override float CalculateActivation()
    {
        if(closestEnemy != null)
        {
            float dist = (transform.position - closestEnemy.transform.position).magnitude;
            m_DegreeOfActivation = 1 - Mathf.Clamp01(dist/8f);

            return m_DegreeOfActivation;
        }

        m_DegreeOfActivation = 0f;
        return m_DegreeOfActivation;
    }

    public override void Enter()
    {
        Debug.Log("AttackState is Running");
        m_currentWeapon = m_entity.m_current;
    }

    public override void Exit()
    {
        EquipWeaponOfType(WeaponType.Handgun);
    }

    public override void Run()
    {
        if(closestEnemy != null)
        {
            Vector2 enemyDirection = closestEnemy.transform.position - transform.position;
            float angle = (Mathf.Atan2(enemyDirection.y, enemyDirection.x) * Mathf.Rad2Deg) + 90f;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = q; //Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 24f);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, enemyDirection, 6f);

            if(hit.collider != null)
            {
                Fire();
            }
        }
    }

    private void Fire()
    {
        m_currentWeapon.PullTrigger();
    }

    SimpleEnemy CheckForEnemies()
    {
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, 5.0f);

        if (entities.Length > 1)
        {
            m_Active = true;
            m_EnemySpotted = true;

            for (int i = 0; i < entities.Length; i++)
            {
                SimpleEnemy simpleEnemy = entities[i].GetComponent<SimpleEnemy>();

                if (simpleEnemy != null)
                {
                    return simpleEnemy;
                }
            }
        }
        else
        {
            m_EnemySpotted =false;
        }

        return null;
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
}