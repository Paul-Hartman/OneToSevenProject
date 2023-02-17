using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Avatar : StateMachine
{
    public int InitialLife = 100;
    public float Speed = 2f;
    public Animator Model3d;
    public float HeightDetection = 4;
    public TextMeshProUGUI InGameLife;

    protected int m_life = 100;
    protected PatrolWaypoints m_patrolComponent;
    protected RotateToTarget m_rotateComponent;
    protected AreaVisionDetection m_areaVisionDetection;

    protected int m_currentAnimation = -1;
    protected bool m_useRigidBody = true;
    public int Life
    {
        get { return m_life; }
    }

    public bool UseRigidBody
    {
        get { return m_useRigidBody; }
        set { m_useRigidBody = value; }
    }

    
    // Start is called before the first frame update
    protected virtual void Start()
    {   
        
        m_life = InitialLife;
        if (InGameLife != null)
        {
            InGameLife.text = m_life.ToString();
        }
        m_patrolComponent = this.GetComponent<PatrolWaypoints>();
        if(m_patrolComponent != null)
        {
            m_patrolComponent.MoveEvent += OnMoveEvent;
            m_patrolComponent.StandEvent += OnStandEvent;
        }
        m_rotateComponent = this.GetComponent<RotateToTarget>();

        m_areaVisionDetection = this.GetComponent<AreaVisionDetection>();
        if (m_areaVisionDetection != null)
        {
            m_areaVisionDetection.DetectionEvent += PlayerDetectionEvent;
            m_areaVisionDetection.LostEvent += PlayerLostEvent;
        }
    }

    protected void SetTargetLife(Transform _target)
    {
        if(InGameLife != null)
        {
            if(InGameLife.GetComponent<LookAtTarget>() != null)
            {
                InGameLife.GetComponent<LookAtTarget>().SetTarget(_target);
            }
        }
    }

    protected virtual void OnStandEvent()
    {
        
    }

    protected virtual void OnMoveEvent()
    {
        
    }

    protected void ChangeAnimation(int _animationID)
    {   
        if(Model3d != null)
        {   
            if(m_currentAnimation != _animationID)
            {
                m_currentAnimation = _animationID;
                Model3d.SetInteger("animationID", _animationID);
            }
            
        }
        
    }

    protected virtual void PlayerLostEvent(GameObject _objectDetected)
    {

        
    }

    protected virtual void PlayerDetectionEvent(GameObject _objectDetected)
    {
        
    }

    public abstract void InitLogic();

    public abstract void StopLogic();


    public virtual void IncreaseLife(int _unitsToIncrease)
    {
        m_life += _unitsToIncrease;

        if (m_life > 100)
        {
            m_life = 100;
        }

        if (InGameLife != null)
        {
            InGameLife.text = m_life.ToString();
        }
    }


    public virtual void DecreaseLife(int _unitsToDecrease)
    {
        m_life -= _unitsToDecrease;

        if (m_life < 0)
        {
            m_life = 0;
        }

        if(InGameLife != null)
        {
            InGameLife.text = m_life.ToString();
        }

        
    }

    protected virtual void OnTriggerEnter(Collider _collision)
    {
        if (_collision.gameObject.GetComponent<Spikes>() != null)
        {

            DecreaseLife(_collision.gameObject.GetComponent<Spikes>().DamageLife);
            
        }
        if (_collision.gameObject.GetComponent<Portal>() != null)
        {

            IncreaseLife(_collision.gameObject.GetComponent<Portal>().MoreLife);
            
        }

       
    }

    protected Vector3 GetDirection(Vector3 target, Vector3 origin)
    {
        return (target - origin).normalized;
    }

    

    protected void MoveToPosition(Vector3 _increment)
    {
        if (m_useRigidBody)
        {
            transform.GetComponent<Rigidbody>().MovePosition(transform.position + _increment);
        }
        else
        {
            transform.position += _increment;
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
