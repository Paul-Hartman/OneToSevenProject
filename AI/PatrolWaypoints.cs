using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolWaypoints : StateMachine
{
    public delegate void MovingEvent();
    public delegate void StandingEvent();

    public event MovingEvent MoveEvent;
    public event StandingEvent StandEvent;

    public void DispatchMovingEvent()
    {
        if(MoveEvent != null)
        {
            MoveEvent();
        }
    }
    public void DispatchStandingEvent()
    {
        if (StandEvent != null)
        {
            StandEvent();
        }
    }

    public Waypoint[] Waypoints;
    public int CurrentWaypoint = 0;
    private bool m_activated = false;
    private float m_timeDone = 0;
    
    public float Speed =5;

    public bool AutoStart = false;
    

    public enum WAYPOINT_ACTIONS { SYNCHRONIZATION = 0, UPDATE_WAYPOINT, GO_TO_WAYPOINT, STAY_IN_WAYPOINT, LOOK_TO_WAYPOINT }


    private bool m_hasRigidBody;
    private RotateToTarget m_rotateComponent;
    // Start is called before the first frame update
    void Start()
    {   
        m_rotateComponent = this.GetComponent<RotateToTarget>();

        m_hasRigidBody = this.GetComponent<Rigidbody>() != null;
        
            for (int i = 0; i < Waypoints.Length; i++)
            {

                Waypoints[i].Position = Waypoints[i].Target.transform.position;

                GameObject.Destroy(Waypoints[i].Target);


            }

        if (AutoStart)
        {
            Activatepatrol(10);
        }
    }

    

    private bool ReachedCurrentWaypoint()
    {   
        if(Vector3.Distance(this.transform.position, Waypoints[CurrentWaypoint].Position) < 1)
        {
            return true;
        }
        else 
        { 
            return false;
        }
        
    }

    public void Activatepatrol(float _speed)
    {   
        if(Waypoints.Length > 0)
        {
            Speed = _speed;
            m_activated = true;
            ChangeState((int)WAYPOINT_ACTIONS.SYNCHRONIZATION);
        }
        
    }

    public bool AreThereAnyWaypoints()
    {
        return Waypoints.Length > 0;
    }

    public void DeactivatePatrol()
    {
        m_activated = false;
    }

    private void WalkToCurrentWaypoint()
    {
        
            
        
        m_timeDone += Time.deltaTime;
        float duration = Waypoints[CurrentWaypoint].Duration;
        Vector3 origin = GetPreviousPositionWaypoint(CurrentWaypoint);
        Vector3 fowardTarget = (Waypoints[CurrentWaypoint].Position - origin);
        float increaseFactor = m_timeDone / duration;
        Vector3 nextPosition = origin + (increaseFactor * fowardTarget);
        if (m_hasRigidBody) 
        {
            transform.GetComponent<Rigidbody>().MovePosition(new Vector3(nextPosition.x, transform.position.y, nextPosition.z));
        }
        else 
        {
            transform.position = nextPosition;
        }
    }

    private void WalkWithSpeedToWaypoint()
    {   
  
        
        Vector3 directionToTarget = Waypoints[CurrentWaypoint].Position - this.transform.position  ;
        
        directionToTarget.Normalize();
        Vector3 nextPosition = this.transform.position + (directionToTarget * Speed *Time.deltaTime);
        
        if (m_hasRigidBody)
        {
            
            transform.GetComponent<Rigidbody>().MovePosition(new Vector3(nextPosition.x, transform.position.y, nextPosition.z));
        }
        else
        {
            
            transform.position = nextPosition;
            
        }
    }

    private Vector3 GetPreviousPositionWaypoint(int _waypointIndex)
    {
        int finalIndexCheck = _waypointIndex;
        do
        {
            finalIndexCheck--;
            if (finalIndexCheck < 0)
            {
                finalIndexCheck = Waypoints.Length - 1;

            }
        }
        while (Waypoints[finalIndexCheck].Action != Waypoint.ActionsPatrol.GO);
        

        return Waypoints[finalIndexCheck].Position;
    }

    private bool IsThereRotationComponent()
    {
        return m_rotateComponent != null;
    }

    protected override void ChangeState(int newState)
    {
        base.ChangeState(newState);

        switch ((WAYPOINT_ACTIONS)m_state)
        {
            case WAYPOINT_ACTIONS.SYNCHRONIZATION:
                if (IsThereRotationComponent())
                {
                    m_rotateComponent.ActivateRotation(Waypoints[CurrentWaypoint].Position);
                }
                DispatchMovingEvent();
                break;


            case WAYPOINT_ACTIONS.UPDATE_WAYPOINT:

                break;


            case WAYPOINT_ACTIONS.GO_TO_WAYPOINT:
                DispatchMovingEvent();
                break;

            case WAYPOINT_ACTIONS.STAY_IN_WAYPOINT:
                DispatchStandingEvent();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_activated) return;
         switch((WAYPOINT_ACTIONS)m_state)
        {
            case WAYPOINT_ACTIONS.SYNCHRONIZATION:
                WalkWithSpeedToWaypoint();
                if (ReachedCurrentWaypoint())
                {
                ChangeState((int)WAYPOINT_ACTIONS.UPDATE_WAYPOINT);
                }
                break;


            case WAYPOINT_ACTIONS.UPDATE_WAYPOINT:
                CurrentWaypoint++;
                if(CurrentWaypoint > Waypoints.Length -1)
               {
                CurrentWaypoint = 0;
               }
                if (IsThereRotationComponent())
                {
                    m_rotateComponent.ActivateRotation(Waypoints[CurrentWaypoint].Position);
                }
                
                m_timeDone = 0;
                switch (Waypoints[CurrentWaypoint].Action) 
                {
                    case Waypoint.ActionsPatrol.GO:
                        ChangeState((int)WAYPOINT_ACTIONS.GO_TO_WAYPOINT);
                        break;
                    case Waypoint.ActionsPatrol.STAY:
                        ChangeState((int)WAYPOINT_ACTIONS.STAY_IN_WAYPOINT);
                        break;

                    case Waypoint.ActionsPatrol.LOOK:
                        ChangeState((int)WAYPOINT_ACTIONS.LOOK_TO_WAYPOINT);
                        break;

                }
               
                
                break;


            case WAYPOINT_ACTIONS.GO_TO_WAYPOINT:
                WalkToCurrentWaypoint();
                if(m_timeDone > Waypoints[CurrentWaypoint].Duration)
                {
                    
                 ChangeState((int)WAYPOINT_ACTIONS.UPDATE_WAYPOINT);
                }
            break;

            case WAYPOINT_ACTIONS.STAY_IN_WAYPOINT:
                
                m_timeDone += Time.deltaTime;
                if(m_timeDone > Waypoints[CurrentWaypoint].Duration)
                {
                    
                    ChangeState((int)WAYPOINT_ACTIONS.UPDATE_WAYPOINT);
                }
                break;

            case WAYPOINT_ACTIONS.LOOK_TO_WAYPOINT:

                m_timeDone += Time.deltaTime;
                if (IsThereRotationComponent())
                {
                    m_rotateComponent.ActivateRotation(Waypoints[CurrentWaypoint].Position);
                }
                if (m_timeDone > Waypoints[CurrentWaypoint].Duration)
                {

                    ChangeState((int)WAYPOINT_ACTIONS.UPDATE_WAYPOINT);
                }
                break;
        }
    }

   
}
