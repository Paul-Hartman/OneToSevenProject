using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : StateMachine
{
    private static CameraController _instance;

    public static CameraController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType<CameraController>();
            }
            return _instance;
        }
    }

   
    public enum CAMERA_STATES { CAMERA_1ST_PERSON = 0, CAMERA_3RD_PERSON, CAMERA_FROZEN }

    public const float SPEED_ROTATION = 10f;

    public Camera GameCamera;
    public Player GamePlayer;
    public Vector3 Offset = new Vector3(0, 3, 5);
    
    
    // Start is called before the first frame update
    void Start()
    {
        ChangeState((int)CAMERA_STATES.CAMERA_1ST_PERSON);
    }

    private void CameraFollowAvatar()
    {
        Offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * SPEED_ROTATION, Vector3.up) * Offset;
        
        GameCamera.transform.position = Offset + GamePlayer.transform.position;
        GameCamera.transform.forward = (GamePlayer.transform.position - GameCamera.transform.position).normalized;
        
    }

    public bool IsFirstPersonCamera()
    {
        return m_state == (int)CAMERA_STATES.CAMERA_1ST_PERSON;
    }

    

    public void FreezeCamera(bool _activateFreeze)
    {
        if (_activateFreeze)
        {
            ChangeState((int)CAMERA_STATES.CAMERA_FROZEN);
        }
        else
        {
            RestorePreviousState();
        }
    }

    

    private void SwitchCameraState()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            switch ((CAMERA_STATES)m_state)
            {
                case CAMERA_STATES.CAMERA_1ST_PERSON:
                    ChangeState((int)CAMERA_STATES.CAMERA_3RD_PERSON);
                    break;
                case CAMERA_STATES.CAMERA_3RD_PERSON:
                    ChangeState((int)CAMERA_STATES.CAMERA_1ST_PERSON);
                    break;
            }
        }
    }

    protected override void ChangeState(int newState)
    {   
        base.ChangeState(newState);
        switch ((CAMERA_STATES)m_state)
        {
            case CAMERA_STATES.CAMERA_1ST_PERSON:
                SystemEventController.Instance.DispatchSystemEvent(SystemEventController.EVENT_CAMERA_SWITCHED_TO_1ST_PERSON);
                
    
                break;
            case CAMERA_STATES.CAMERA_3RD_PERSON:
                SystemEventController.Instance.DispatchSystemEvent(SystemEventController.EVENT_CAMERA_SWITCHED_TO_3RD_PERSON);
                break;
            case CAMERA_STATES.CAMERA_FROZEN:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {   
        SwitchCameraState();
        switch ((CAMERA_STATES)m_state)
        {
            case CAMERA_STATES.CAMERA_1ST_PERSON:
                GameCamera.transform.position = GamePlayer.PositionCamera;
                GameCamera.transform.forward = GamePlayer.FowardCamera;
                break;
            case CAMERA_STATES.CAMERA_3RD_PERSON:
                CameraFollowAvatar();
                break;
            case CAMERA_STATES.CAMERA_FROZEN:
                break;
        }
    }
}
