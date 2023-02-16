using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Player : Avatar
{
    
    public GameObject BulletPlayer;
    
    public TextMeshProUGUI DisplayScore;
    public TextMeshProUGUI DisplayLife;
    public TextMeshProUGUI DisplayShots;
    public TextMeshProUGUI DisplayDangerinfo;
    public TextMeshProUGUI DisplayNPCNearby;


    private int m_bulletsShot;

    public float Sensitivity = 7f;
    private float m_rotationY = 0f;

   
    public enum PLAYER_STATES { IDLE = 0, WALK, DIE, INITIAL }

    

    public enum ANIMATION_STATES { ANIMATION_IDLE = 0, ANIMATION_WALK, ANIMATION_DIE , ANIMATION_ATTACK }


    private Vector3 m_initialPosition;

    private int m_score;
    public int Score
    {
        get { return m_score; }
        set
        {
            m_score = value;
            DisplayScore.text = "Score: " + m_score.ToString();
        }
    }
    
    private int m_coins = 0;
    private Vector3 m_fowardCamera;
    private Vector3 m_positionCamera;

    public Vector3 FowardCamera
    {
        get { return m_fowardCamera; }
    }

    public Vector3 PositionCamera
    {
        get { return m_positionCamera; }
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        DisplayScore.text = "Score: " + Score;
        DisplayLife.text = "Life: " + m_life;
        DisplayShots.text = "Shots: " + m_bulletsShot;
        DisplayDangerinfo.text = "";
        DisplayNPCNearby.text = "";
        CameraController.Instance.GameCamera.transform.parent = null;
        m_initialPosition = this.transform.position;
        ChangeState((int)PLAYER_STATES.INITIAL);
        ChangeAnimation((int)ANIMATION_STATES.ANIMATION_IDLE);

        SystemEventController.Instance.Event += OnSystemEvent;
        Model3d.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        SystemEventController.Instance.Event -= OnSystemEvent;
    }

    private void OnSystemEvent(string _nameEvent, object[] _parameters)
    {
        if(_nameEvent == SystemEventController.EVENT_CAMERA_SWITCHED_TO_1ST_PERSON)
        {
            Model3d.gameObject.SetActive(false);
        }
        if (_nameEvent == SystemEventController.EVENT_CAMERA_SWITCHED_TO_3RD_PERSON)
        {
            Model3d.gameObject.SetActive(true);
        }
    }

    public void ResetPlayerPosition()
    {
        this.transform.position = m_initialPosition;
        m_positionCamera = this.transform.position;
    }

    public void ResetPlayerLife()
    {
        m_life = InitialLife;
    }

    public override void InitLogic()
    {
        ChangeState((int)PLAYER_STATES.IDLE);
    }

    public override void StopLogic()
    {
        if (m_life > 0) 
        { 
            ChangeState((int)PLAYER_STATES.INITIAL); 
        }
        
    }

    private void Move()
    {
        float axisVertical = Input.GetAxis("Vertical");
        float axisHorizontal = Input.GetAxis("Horizontal");
        Vector3 foward = axisVertical * CameraController.Instance.GameCamera.transform.forward * Speed * Time.deltaTime;
        Vector3 lateral = axisHorizontal * CameraController.Instance.GameCamera.transform.right * Speed * Time.deltaTime;
        MoveToPosition(foward+lateral);
        m_positionCamera = this.transform.position;
        
    }

    public override void IncreaseLife(int _unitsToIncrease)
    {
        base.IncreaseLife(_unitsToIncrease);
        DisplayLife.text = "Life: " + m_life;
    }


    public override void DecreaseLife(int _unitsToDecrease)
    {
       base.DecreaseLife(_unitsToDecrease);
        DisplayLife.text = "Life: " + m_life;
    }



    private void RotateCamera()
    {
        float rotationX = CameraController.Instance.GameCamera.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * Sensitivity;
        m_rotationY += Input.GetAxis("Mouse Y") * Sensitivity;
        m_rotationY = Mathf.Clamp(m_rotationY, -60, 60);
        Quaternion rotation = Quaternion.Euler(-m_rotationY, rotationX, 0);
        m_fowardCamera = rotation * Vector3.forward;
        this.transform.forward = new Vector3(m_fowardCamera.x, 0, m_fowardCamera.z);
        




    }


    private void ShootBullet()
    {

        if (Input.GetMouseButtonDown(0)) 
        {
            GameObject newBulletGo = Instantiate(BulletPlayer);
            Bullets bullet = newBulletGo.GetComponent<Bullets>();
            Vector3 positionBullet = Vector3.zero;
            Vector3 fowardBullet = Vector3.zero;
            if (CameraController.Instance.IsFirstPersonCamera())
            {
                positionBullet = CameraController.Instance.GameCamera.transform.position;
                fowardBullet = CameraController.Instance.GameCamera.transform.forward;
                
            }
            else
            {
                positionBullet = this.transform.position;
                fowardBullet = new Vector3(CameraController.Instance.GameCamera.transform.forward.x, 0, CameraController.Instance.GameCamera.transform.forward.z);
            }
            bullet.Shoot(Bullets.TYPE_BULLET_PLAYER, positionBullet, fowardBullet);
            Physics.IgnoreCollision(this.GetComponent<Collider>(), newBulletGo.GetComponent<Collider>());
            m_bulletsShot++;
            DisplayShots.text = "Shots: " + m_bulletsShot;
            SoundsController.Instance.PlaySoundFX(SoundsController.FX_SHOOT, false, 1);
            ChangeAnimation((int)ANIMATION_STATES.ANIMATION_ATTACK);
            
            
        }
            
            
    }
    

    private bool ArrowKeyPressed()
    {
        if(Input.GetKey(KeyCode.RightArrow)|| Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)||
            Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.S)|| Input.GetKey(KeyCode.D)|| Input.GetKey(KeyCode.W)){
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public void AddCoin()
    {
        m_coins++;
        Debug.Log("the Player has " + m_coins + " Coins");
    }

    protected override void OnTriggerEnter(Collider _collision)
    {
        base.OnTriggerEnter(_collision);

        if (_collision.gameObject.GetComponent<Coin>() != null)
        {
            AddCoin();
            GameObject.Destroy(_collision.gameObject);
        }
    }


    private void CheckToDisplayDanger()
    {
        if (LevelController.Instance.AlarmEnemyNearby(6))
        {
            DisplayDangerinfo.text = "Danger Enemy nearby!!!";

        }
        else
        {
            DisplayDangerinfo.text = "";
        }
    }

    private void CheckToDisplayNPCNearby()
    {
        if (LevelController.Instance.AlarmNPCNearby(6))
        {
            
            DisplayNPCNearby.text = "Hey lets Talk!!!";

        }
        else
        {
            
            DisplayNPCNearby.text = "";
        }
    }
    protected override void ChangeState(int newState)
    {
        base.ChangeState(newState);
        switch ((PLAYER_STATES)m_state)
        {
            case PLAYER_STATES.INITIAL:
                ChangeAnimation((int)ANIMATION_STATES.ANIMATION_DIE);
                break;
            case PLAYER_STATES.IDLE:
                ChangeAnimation((int)ANIMATION_STATES.ANIMATION_IDLE);
                break;
            case PLAYER_STATES.WALK:
                ChangeAnimation((int)ANIMATION_STATES.ANIMATION_WALK);
                break;
            case PLAYER_STATES.DIE:
                Debug.Log("The Player has Died");
                ChangeAnimation((int)ANIMATION_STATES.ANIMATION_DIE);

                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        

        switch ((PLAYER_STATES)m_state)
        {
            case PLAYER_STATES.IDLE:
                
                RotateCamera();


                ShootBullet();
                CheckToDisplayDanger();
                CheckToDisplayNPCNearby();
                //Debug.Log("in IDLE state");
                if (ArrowKeyPressed() == true)
                {
                    ChangeState((int)PLAYER_STATES.WALK);
                }
                if (m_life <= 0)
                {
                    ChangeState((int)PLAYER_STATES.DIE);
                }
                break;
            case PLAYER_STATES.WALK:
                Move();
                RotateCamera();
                CheckToDisplayDanger();
                CheckToDisplayNPCNearby();
                ShootBullet();
                //Debug.Log("in WALK state");
                if (ArrowKeyPressed() == false)
                {
                    ChangeState((int)PLAYER_STATES.IDLE);
                }
                if (m_life <= 0)
                {
                    ChangeState((int)PLAYER_STATES.DIE);
                }
                break;
            case PLAYER_STATES.DIE:
                
                //Debug.Log("The Player has Died");
                break;
        }
       


    }

    
}
