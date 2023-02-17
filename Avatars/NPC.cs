using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Avatar
{
    
    public enum NPC_STATES { PATROL_AND_CHECK = 0, GO_TO_PLAYER, TALK_TO_PLAYER, DIE, INITIAL, NULL };

    

    private enum ANIMATION_STATES { ANIMATION_IDLE = 0, ANIMATION_RUN, ANIMATION_DEATH, ANIMATION_TALK }

    public float DistanceDetection = 10f;
    public float DistanceTalking = 10f;
    
    private Vector3 m_initialPosition;

    private int m_totalEnemiesDead = 0;
    private float m_timerToTalk = 0;

    private bool m_detectedPlayer = false;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        m_initialPosition = this.transform.position;
        SystemEventController.Instance.Event += ProcessSystemEvent;
        ChangeState((int)NPC_STATES.INITIAL);
        ChangeAnimation((int)ANIMATION_STATES.ANIMATION_IDLE);

    }

    protected override void OnStandEvent()
    {
        ChangeAnimation((int)ANIMATION_STATES.ANIMATION_IDLE);
    }

    protected override void OnMoveEvent()
    {
        ChangeAnimation((int)ANIMATION_STATES.ANIMATION_RUN);
    }
    void OnDestroy()
    {
        SystemEventController.Instance.Event -= ProcessSystemEvent;
    }

    private void ProcessSystemEvent(string _nameEvent, object[] _parameters)
    {
        if (_nameEvent == SystemEventController.EVENT_ENEMY_DEAD)
        {   
            m_totalEnemiesDead++;
            Debug.Log($"<color= blue>NPC has recieved the event of Enemy Dead!! {m_totalEnemiesDead} Enemies dead</color>");
        }
    }

    protected override void PlayerLostEvent(GameObject _objectDetected)
    {
        m_detectedPlayer = false;
    }

    protected override void PlayerDetectionEvent(GameObject _objectDetected)
    {
        m_detectedPlayer = true;
    }

    public override void InitLogic()
    {
        ChangeState((int)NPC_STATES.PATROL_AND_CHECK);
    }

    public override void StopLogic()
    {
        ChangeState((int)NPC_STATES.INITIAL);
    }

    public override void DecreaseLife(int _unitsToDecrease)
    {
        base.DecreaseLife(_unitsToDecrease);

        if (m_life <= 0)
        {
            Player gamePlayer = GameObject.FindObjectOfType<Player>();
            gamePlayer.Score -= 10;
            ChangeState((int)NPC_STATES.DIE);
            Debug.Log("Current Score: " + gamePlayer.Score);

        }
        
    }

    // Update is called once per frame
    private void GoToInitialPosition()
    {


        if (Vector3.Distance(m_initialPosition, this.transform.position) > 1)
        {
            Vector3 directionVector = GetDirection(m_initialPosition, this.transform.position);
            MoveToPosition(directionVector * Speed * Time.deltaTime);
            ChangeAnimation((int)ANIMATION_STATES.ANIMATION_RUN);
        }
        else
        {
            ChangeAnimation((int)ANIMATION_STATES.ANIMATION_IDLE);
        }

        
    }

    private void UpdateInitialPositionRandom()
    {
        m_initialPosition = new Vector3(UnityEngine.Random.Range(20, -20), m_initialPosition.y, UnityEngine.Random.Range(20, -20));
    }

    private void WalkToPlayer()
    {

        Vector3 directionToPlayer = GetDirection(GameController.Instance.MyPlayer.transform.position, this.transform.position);
        MoveToPosition(directionToPlayer.normalized * Speed * Time.deltaTime);


    }
    private void TalkToPlayer()
    {   
        m_timerToTalk += Time.deltaTime;
        if(m_timerToTalk > 2)
        {
            m_timerToTalk = 0;
            switch (m_totalEnemiesDead)
            {
                case 0:
                    Debug.Log("Message 1");
                    SoundsController.Instance.PlaySoundSpeech(SoundsController.SPEECH_NPC, false, 1);
                    break;
                case 1:
                    Debug.Log("Message 2");
                    break;
                case 2:
                    Debug.Log("Message 3");
                    break;
                default:
                    Debug.Log("Message 4");
                    break;
            }
        }
        //Debug.Log("I SHOULD BE TALKING");
       

    }


    private bool IsInsideDetectionRange()
    {
        if ((m_areaVisionDetection != null) && (m_areaVisionDetection.UseBehavior == true))
        {
            return m_detectedPlayer;
        }
        else
        {
            if (Vector3.Distance(GameController.Instance.MyPlayer.transform.position, this.transform.position) < DistanceDetection)
            {

                return true;
            }
            else
            {
                return false;
            }
        }

        


    }

    private bool IsInTalkingRange()
    {
        if ((m_areaVisionDetection != null) && (m_areaVisionDetection.UseBehavior == true))
        {
            if (m_detectedPlayer)
            {
                return (Vector3.Distance(GameController.Instance.MyPlayer.transform.position, this.transform.position) < DistanceTalking);
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (Vector3.Distance(GameController.Instance.MyPlayer.transform.position, this.transform.position) < DistanceTalking)
            {


                return true;
            }
            else
            {

                return false;
            }
        }
        

        
    }

    protected override void ChangeState(int newState)
    {
        base.ChangeState(newState);
        switch ((NPC_STATES)m_state)
        {
            case NPC_STATES.INITIAL:
                break;
            case NPC_STATES.PATROL_AND_CHECK:
                if ((m_patrolComponent == null) || (m_patrolComponent.AreThereAnyWaypoints() == false))
                {
                    if (m_previousState != (int)NPC_STATES.INITIAL)
                    {
                        UpdateInitialPositionRandom();
                    }
                }



                else
                {
                    m_patrolComponent.Activatepatrol(Speed);
                }
                
                
                break;
            case NPC_STATES.GO_TO_PLAYER:
                ChangeAnimation((int)ANIMATION_STATES.ANIMATION_RUN);
                if ((m_patrolComponent != null) && (m_patrolComponent.AreThereAnyWaypoints() == true))
                {
                    m_patrolComponent.DeactivatePatrol();
                }
                
                break;
            case NPC_STATES.TALK_TO_PLAYER:
                ChangeAnimation((int)ANIMATION_STATES.ANIMATION_TALK);

                break;
            case NPC_STATES.DIE:
                if (m_patrolComponent != null)
                {
                    m_patrolComponent.DeactivatePatrol();
                }
                if (m_areaVisionDetection != null)
                {
                    m_areaVisionDetection.DestroyVisualArea();
                }
                ChangeAnimation((int)ANIMATION_STATES.ANIMATION_DEATH);
                SystemEventController.Instance.DispatchSystemEvent(SystemEventController.EVENT_NPC_DEAD);
                SoundsController.Instance.PlaySoundFX(SoundsController.FX_DEAD_NPC, false, 1);
                
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {




        switch ((NPC_STATES)m_state)
        {
            case NPC_STATES.INITIAL:

                break;
            case NPC_STATES.PATROL_AND_CHECK:
                if ((m_patrolComponent == null) || (m_patrolComponent.AreThereAnyWaypoints() == false))
                {
                    GoToInitialPosition();
                    if(m_rotateComponent != null)
                    {
                        
                       m_rotateComponent.ActivateRotation(m_initialPosition);
                        
                    }
                }

                if (IsInsideDetectionRange())
                {

                    ChangeState((int)NPC_STATES.GO_TO_PLAYER);
                }
                if (IsInTalkingRange())
                {

                    ChangeState((int)NPC_STATES.TALK_TO_PLAYER);
                }

                if (m_life == 0)
                {

                    ChangeState((int)NPC_STATES.DIE);
                }
                break;




            case NPC_STATES.GO_TO_PLAYER:
                if (m_rotateComponent != null)
                {
                    m_rotateComponent.ActivateRotation(GameController.Instance.MyPlayer.transform.position);
                }
                WalkToPlayer();
                if (IsInTalkingRange())
                {

                    ChangeState((int)NPC_STATES.TALK_TO_PLAYER);
                }
                if (m_life == 0)
                {

                    ChangeState((int)NPC_STATES.DIE);
                }
                if (!IsInsideDetectionRange())
                {

                    ChangeState((int)NPC_STATES.PATROL_AND_CHECK);
                }

                break;


            case NPC_STATES.TALK_TO_PLAYER:
                if (m_rotateComponent != null)
                {
                    m_rotateComponent.ActivateRotation(GameController.Instance.MyPlayer.transform.position);
                }
                TalkToPlayer();
                
                if (m_life == 0)
                {

                    ChangeState((int)NPC_STATES.DIE);
                }
                if (!IsInsideDetectionRange())
                {

                    ChangeState((int)NPC_STATES.PATROL_AND_CHECK);
                }
                if (!IsInTalkingRange())
                {

                    ChangeState((int)NPC_STATES.GO_TO_PLAYER);
                }
                break;


            case NPC_STATES.DIE:
                m_timeCounter += Time.deltaTime;

                if (m_timeCounter > 4)
                {
                    ChangeState((int)NPC_STATES.NULL);
                    GameObject.Destroy(this.gameObject);
                }

                break;

        }
    }

    
}
