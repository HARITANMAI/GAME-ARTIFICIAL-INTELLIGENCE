using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyStates_Manger : MonoBehaviour
{
    [SerializeField]
    List<FuzzyStates> m_FuzzyStates;
    List<FuzzyStates> m_ActiveFuzzyStates;

    private void Awake()
    {
        m_ActiveFuzzyStates = new List<FuzzyStates>();
    }

    public void CalculateActiveStates()
    {
        //Save the last frame's active states
        List<FuzzyStates> lastActiveStates = new List<FuzzyStates>();
        lastActiveStates.AddRange(m_ActiveFuzzyStates);

        //Clear the active states list
        m_ActiveFuzzyStates.Clear();

        //Calculate to see if any of the states are active
        for(int i = 0; i < m_FuzzyStates.Count; ++i)
        {
            if (m_FuzzyStates[i].m_Active)
            {
                if (m_FuzzyStates[i].CalculateActivation() > 0.0f)
                {
                    m_ActiveFuzzyStates.Add(m_FuzzyStates[i]);

                    if (!lastActiveStates.Contains(m_FuzzyStates[i]))
                    {
                        m_FuzzyStates[i].Enter();
                    }
                }
            }
            else
            {
                m_FuzzyStates[i].Exit();
            }
        }
    }

    public void RunActiveStates()
    {
        List<FuzzyStates> lastActiveStates = new List<FuzzyStates>();
        lastActiveStates.AddRange(m_ActiveFuzzyStates);

        foreach (FuzzyStates state in m_ActiveFuzzyStates)
        {
            state.Run();
        }
    }
}