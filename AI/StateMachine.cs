using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    protected int m_state = 0;
    protected int m_previousState;

    protected float m_timeCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void RestorePreviousState()
    {
        m_state = m_previousState;
        m_timeCounter = 0;
    }

    protected virtual void ChangeState(int newState)
    {
        m_previousState = m_state;
        m_state = newState;
        m_timeCounter = 0;
    }
}
