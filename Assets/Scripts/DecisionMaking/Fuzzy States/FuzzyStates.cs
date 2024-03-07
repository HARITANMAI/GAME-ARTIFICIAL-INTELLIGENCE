using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FuzzyStates : MonoBehaviour
{
    public bool m_Active;
    public float m_DegreeOfActivation;

    public abstract void Enter();

    public abstract void Exit();

    public abstract void Run();

    public abstract float CalculateActivation();
}
