using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Avatar
{
    
    
    public enum ENEMY_STATES { PATROL_AND_CHECK = 0, GO_TO_PLAYER, SHOOT_TO_PLAYER, DIE, INITIAL, NULL };

    
    
    private enum ANIMATION_STATES { ANIMATION_IDLE = 0 , ANIMATION_RUN, ANIMATION_DEATH, ANIMATION_ATTACK }


    public float DistanceDetection = 10f;
    public float DistanceShooting = 5f;
    

    private Vector3 m_initialPosition;

    public GameObject BulletEnemy;

    private float m_timerToShoot = 0;

    

    private bool m_detectedPlayer = false;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        m_initialPosition = this.transform.position;
        SystemEventController.Instance.Event += ProcessSystemEvent;
        
        ChangeState((int)ENEMY_STATES.INITIAL);
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
        if (_nameEvent == SystemEventController.EVENT_NPC_DEAD)
        {

            
            DistanceDetection *= 1.5f;
            if(m_areaVisionDetection != null)
            {
                m_areaVisionDetection.ChangeDistanceArea(m_areaVisionDetection.DetectionDistance * 1.15f);
            }
        }
    }

    

    protected override void PlayerLostEvent(GameObject _objectDetected)
    {
        
        Debug.Log("<color=yellow>player left feild of view!!!!!!! </color>");
        m_detectedPlayer = false;
    }

    protected override void PlayerDetectionEvent(GameObject _objectDetected)
    {
        Debug.Log("<color=green>Enemy Listened to detection!! </color>");
        m_detectedPlayer = true;
    }

    public override void InitLogic()
    {
        ChangeState((int)ENEMY_STATES.PATROL_AND_CHECK); 
    }

    public override void StopLogic()
    {
        ChangeState((int)ENEMY_STATES.INITIAL);
    }

    public override void DecreaseLife(int _unitsToDecrease)
    {   
        base.DecreaseLife(_unitsToDecrease);
        if (m_life <= 0) 
        {   

            
            GameController.Instance.MyPlayer.Score += 10;
            ChangeState((int)ENEMY_STATES.DIE);
            Debug.Log("Current Score: " + GameController.Instance.MyPlayer.Score);
            
        }
        
        
        //increase Player score
    }

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
        m_initialPosition =new Vector3(UnityEngine.Random.Range(20,-20), m_initialPosition.y , UnityEngine.Random.Range(20,-20));
    }

    private void WalkToPlayer()
    {   
        
            Vector3 directionToPlayer = GetDirection(GameController.Instance.MyPlayer.transform.position, this.transform.position);
        
            MoveToPosition(directionToPlayer.normalized * Speed * Time.deltaTime);
        
        
    }
    private void ShootToPlayer()
    {
        m_timerToShoot += Time.deltaTime;
        if(m_timerToShoot > 2)
        {
            m_timerToShoot = 0;
            GameObject bullet = Instantiate(BulletEnemy);
            Physics.IgnoreCollision(bullet.GetComponent<Collider>(), this.gameObject.GetComponent<Collider>());
            int type = Bullets.TYPE_BULLET_ENEMY;
            Vector3 position = this.transform.position;
            Vector3 directionToPlayer = GetDirection(GameController.Instance.MyPlayer.transform.position, this.transform.position);
            bullet.GetComponent<Bullets>().Shoot(type, position, directionToPlayer);
            SoundsController.Instance.PlaySoundFX(SoundsController.FX_SHOOT, false, 1);



        }
    }

    
    private bool IsInsideDetectionRange()
    {
        if((m_areaVisionDetection != null) && (m_areaVisionDetection.UseBehavior == true))
        {
            return m_detectedPlayer;
        }
        else
        {
            if (Vector3.Distance(GameController.Instance.MyPlayer.transform.position, this.transform.position) < DistanceDetection)
            {   
                if(Mathf.Abs(GameController.Instance.MyPlayer.transform.position.y - this.transform.position.y) < HeightDetection)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
                
            }
            else
            {
                return false;
            }
        }
        

        
    }

    private bool IsInShootingRange()
    {
        if ((m_areaVisionDetection != null) && (m_areaVisionDetection.UseBehavior == true))
        {   
            if(m_detectedPlayer)
            {
                if  (Vector3.Distance(GameController.Instance.MyPlayer.transform.position, this.transform.position) < DistanceShooting)
                {
                    if (Mathf.Abs(GameController.Instance.MyPlayer.transform.position.y - this.transform.position.y) < HeightDetection)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            
        }
        else
        {
            if (Vector3.Distance(GameController.Instance.MyPlayer.transform.position, this.transform.position) < DistanceShooting)
            {
                if (Mathf.Abs(GameController.Instance.MyPlayer.transform.position.y - this.transform.position.y) < HeightDetection)
                {
                    return true;
                }
                else
                {
                    return false;
                }
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
        switch ((ENEMY_STATES)m_state)
        {

            case ENEMY_STATES.INITIAL:
                break;

            case ENEMY_STATES.PATROL_AND_CHECK:

                if ((m_patrolComponent == null) || (m_patrolComponent.AreThereAnyWaypoints()== false))
                {
                    if (m_previousState != (int)ENEMY_STATES.INITIAL)
                    {   
                        
                        UpdateInitialPositionRandom();
                    }
                }

                

                else
                {
                    m_patrolComponent.Activatepatrol(Speed);
                    
                }
                


                
                break;
            case ENEMY_STATES.GO_TO_PLAYER:
                ChangeAnimation((int)ANIMATION_STATES.ANIMATION_RUN);
                if((m_patrolComponent != null) && (m_patrolComponent.AreThereAnyWaypoints() == true))
                {
                    m_patrolComponent.DeactivatePatrol();
                }
                
                break;
            case ENEMY_STATES.SHOOT_TO_PLAYER:
                ChangeAnimation((int)ANIMATION_STATES.ANIMATION_ATTACK);
                break;
            case ENEMY_STATES.DIE:
                if (m_patrolComponent != null)
                {
                    m_patrolComponent.DeactivatePatrol();
                }
                if(m_areaVisionDetection != null)
                {
                    m_areaVisionDetection.DestroyVisualArea();
                }
                ChangeAnimation((int)ANIMATION_STATES.ANIMATION_DEATH);
                SystemEventController.Instance.DispatchSystemEvent(SystemEventController.EVENT_ENEMY_DEAD);
                SoundsController.Instance.PlaySoundFX(SoundsController.FX_DEAD_ENEMY, false, 1);
                
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

        


        switch ((ENEMY_STATES)m_state)
        {
            case ENEMY_STATES.INITIAL:

                break;
            case ENEMY_STATES.PATROL_AND_CHECK:
                if ((m_patrolComponent == null) || (m_patrolComponent.AreThereAnyWaypoints() == false))
                { 
                    GoToInitialPosition();
                    if (m_rotateComponent != null)
                    {

                        m_rotateComponent.ActivateRotation(m_initialPosition);

                    }
                }
                
                
                if (IsInsideDetectionRange())
                    { 
                    
                    ChangeState((int)ENEMY_STATES.GO_TO_PLAYER);
                }
                if (IsInShootingRange())
                { 
                    
                    ChangeState((int)ENEMY_STATES.SHOOT_TO_PLAYER);
                }

                if (m_life == 0) 
                { 
                    
                    ChangeState((int)ENEMY_STATES.DIE);
                }
                break;

           


            case ENEMY_STATES.GO_TO_PLAYER:
                if(m_rotateComponent != null)
                {
                    m_rotateComponent.ActivateRotation(GameController.Instance.MyPlayer.transform.position);
                }
                WalkToPlayer();
                if (IsInShootingRange()) 
                {
                    
                    ChangeState((int)ENEMY_STATES.SHOOT_TO_PLAYER);
                }
                if (m_life == 0)
                {
                    
                    ChangeState((int)ENEMY_STATES.DIE);
                }
                if (!IsInsideDetectionRange())
                {
                    
                    ChangeState((int)ENEMY_STATES.PATROL_AND_CHECK);
                }

                break;


            case ENEMY_STATES.SHOOT_TO_PLAYER:
                if (m_rotateComponent != null)
                {
                    m_rotateComponent.ActivateRotation(GameController.Instance.MyPlayer.transform.position);
                }
                ShootToPlayer();
                if (m_life == 0)
                { 
                    
                    ChangeState((int)ENEMY_STATES.DIE);
                }
                if (!IsInsideDetectionRange()) 
                {
                    
                    ChangeState((int)ENEMY_STATES.PATROL_AND_CHECK);
                }
                if (!IsInShootingRange()) 
                {
                    
                    ChangeState((int)ENEMY_STATES.GO_TO_PLAYER);
                }
                break;


            case ENEMY_STATES.DIE:
                m_timeCounter+= Time.deltaTime;
                
                if (m_timeCounter > 4)
                {
                    ChangeState((int)ENEMY_STATES.NULL);
                    GameObject.Destroy(this.gameObject);
                }
                
                
                break;

        }
    }

   
}
