using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : StateMachine
{   

    private static GameController _instance;
    public static GameController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType<GameController>();
            }
           return _instance;
        }
    }

   
    public enum CAMERA_STATES { MAIN_MENU = 0, LOAD_GAME, GAME_RUNNING, WIN, LOSE, PAUSE }

    public LevelController[] Levels;
    public int CurrentLevel = 0;


    public Player MyPlayer;
    

    
    public GameObject GameHUD;

    private bool m_hasPlayerPressedButton = false;
    private float m_timeLoading = 0f;
    private bool m_hasCompletedLoadingResources = false;
    
    private bool m_pressedReturnToGame = false;
    private bool m_pressedReloadGame = false;
    private bool m_hasPressedGoToNextLevel = false;

    private int m_counterDeadEnemies = 0;
    private int m_counterDeadNPCs = 0;

    // Start is called before the first frame update
    void Start()
    {   
        MyPlayer.GetComponent<Rigidbody>().useGravity = false;
        ChangeState((int)CAMERA_STATES.MAIN_MENU);
        SystemEventController.Instance.Event += ProcessSystemEvent;
    }
    void OnDestroy()
    {
        SystemEventController.Instance.Event -= ProcessSystemEvent;
    }

    private void ProcessSystemEvent(string _nameEvent, object[] _parameters)
    {
        if(_nameEvent == SystemEventController.EVENT_ENEMY_DEAD)
        {   
            
            m_counterDeadEnemies++;
            Debug.Log($"<color= orange>GameController has recieved the event of Enemy Dead!! {m_counterDeadEnemies} Enemies are Dead</color>");
        }
        if (_nameEvent == SystemEventController.EVENT_NPC_DEAD)
        {
            m_counterDeadNPCs++;
        }
    }

    public void PlayClicked()
    {
        m_hasPlayerPressedButton = true;
    }

    

    private void ResetGameControllerState()
    {
        m_hasPlayerPressedButton = false;
        m_timeLoading = 0f;
        m_hasCompletedLoadingResources = false;
        m_hasPressedGoToNextLevel = false;
        m_pressedReloadGame = false;
        m_pressedReturnToGame = false;

    }
    private void RenderMenu()
    {

    }

    private bool UserPressedPlayButton()
    {
        
        return m_hasPlayerPressedButton;
    }

    private void LoadGame()
    {
        m_timeLoading += Time.deltaTime;
        if(m_timeLoading > 2f)
        {
            m_hasCompletedLoadingResources = true;
        }
    }

    private bool HasLoadedGame()
    {
        return m_hasCompletedLoadingResources;
    }

    private void RunGame()
    {

    }

    

    private bool PlayerisDead()
    {
        if(MyPlayer.Life <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    private void ShowWinScreen()
    {

    }

    public void PressedGoToNextLevel()
    {
        m_hasPressedGoToNextLevel = true;
    }

    private bool PressedNextButton()
    {
        return m_hasPressedGoToNextLevel;
    }

    private void ShowLoseScreen()
    {

    }
    public void UserHasPressedReturnGame()
    {
       m_pressedReturnToGame = true;
    }

    public void UserHasPressedReloadGame()
    {
        m_pressedReloadGame = true;
    }

    private bool ButtonPressedReturnGame()
    {
        return m_pressedReturnToGame;
    }

    private bool ButtonPressedReloadGame()
    {
        return m_pressedReloadGame;
    }
    private void ActivationScreenMenuMain()
    {   
        ScreenController.Instance.CreateScreen("MainMenu", true);
       
    }

    private void ActivationScreenLoadGame()
    {
        ScreenController.Instance.CreateScreen("LoadScreen", true);
        
    }

    private void ActivationScreenWinGame()
    {
        ScreenController.Instance.CreateScreen("WinScreen", true);
        
    }

    private void ActivationScreenLoseGame()
    {
        ScreenController.Instance.CreateScreen("LoseScreen", true);
        
    }

    private void ActivationGameHUD(bool activate)
    {
        GameHUD.SetActive(activate);
    }

    private void ActivationScreenPauseGame()
    {
        ScreenController.Instance.CreateScreen("PauseScreen", true);
    }



    private void DisableAllScreens()
    {
        //ScreenMenuMain.SetActive(false);
        //ScreenLoadGame.SetActive(false);
        //ScreenWinGame.SetActive(false);
        //ScreenLoseGame.SetActive(false);
        GameHUD.SetActive(false);
        //ScreenPause.SetActive(false);
}

    private void ResetPauseVariables()
    {
        m_pressedReloadGame = false;
        m_pressedReturnToGame = false;
    }


    private void StopLogicGameElements()
    {
        MyPlayer.StopLogic();
        LevelController.Instance.StopLogicGameElements();
    }


    private void InitializeLogicGameElements()
    {
        MyPlayer.InitLogic();
        LevelController.Instance.InitializeLogicGameElements();
    }





    protected override void ChangeState(int newState)
    {
        base.ChangeState(newState);
        switch ((CAMERA_STATES)m_state)
        {
            case CAMERA_STATES.MAIN_MENU:
                Cursor.lockState = CursorLockMode.None;
                ResetGameControllerState();
                DisableAllScreens();
                ActivationScreenMenuMain();
                SoundsController.Instance.PlaySoundBackground(SoundsController.MELODY_MAIN_MENU, true, 1);
                
                break;
            case CAMERA_STATES.LOAD_GAME:
                MyPlayer.ResetPlayerPosition();
                m_hasPressedGoToNextLevel = false;
                DisableAllScreens();
                if (LevelController.Instance != null)LevelController.Instance.Destroy();
                Instantiate(Levels[CurrentLevel]);
                SoundsController.Instance.StopSoundsBackground();
                
                ActivationScreenLoadGame();
                
                break;
            case CAMERA_STATES.GAME_RUNNING:
                Cursor.lockState = CursorLockMode.Locked;
                ScreenController.Instance.DestroyScreens();
                MyPlayer.GetComponent<Rigidbody>().useGravity = true;
                SoundsController.Instance.PlaySoundBackground(SoundsController.MELODY_INGAME, true, 1);
                switch ((CAMERA_STATES)m_previousState)
                {
                    case CAMERA_STATES.PAUSE:
                        CameraController.Instance.FreezeCamera(false);
                        ResetPauseVariables();
                        
                        InitializeLogicGameElements();
                        break;
                    case CAMERA_STATES.LOAD_GAME:
                        InitializeLogicGameElements();

                        break;

                }
                
                
                ActivationGameHUD(true);
                
                break;
            case CAMERA_STATES.WIN:
                Cursor.lockState = CursorLockMode.None;
                CurrentLevel++;
                if(CurrentLevel >2) CurrentLevel=0;
                MyPlayer.ResetPlayerLife();
                StopLogicGameElements();
                ActivationGameHUD(false);
                ActivationScreenWinGame();
                
                SoundsController.Instance.StopSoundsFX();
                SoundsController.Instance.StopSoundsBackground();
                SoundsController.Instance.PlaySoundBackground(SoundsController.MELODY_WIN, false, 1);
                
                break;
            case CAMERA_STATES.LOSE:
                Cursor.lockState = CursorLockMode.None;
                MyPlayer.ResetPlayerLife();
                StopLogicGameElements();
                ActivationGameHUD(false);
                ActivationScreenLoseGame();
                
                SoundsController.Instance.StopSoundsFX();
                SoundsController.Instance.StopSoundsBackground();
                SoundsController.Instance.PlaySoundBackground(SoundsController.MELODY_LOSE, false, 1);
               
                break;

            case CAMERA_STATES.PAUSE:
                CameraController.Instance.FreezeCamera(true);
                Cursor.lockState = CursorLockMode.None;
                ActivationScreenPauseGame();
                ActivationGameHUD(false );
                StopLogicGameElements();
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        switch ((CAMERA_STATES)m_state) 
        {
            case CAMERA_STATES.MAIN_MENU:
                RenderMenu();
                if (UserPressedPlayButton()) 
                {
                    ChangeState((int)CAMERA_STATES.LOAD_GAME);
                }
                break;

            case CAMERA_STATES.LOAD_GAME:
                LoadGame();
                if (HasLoadedGame())
                {
                    ChangeState((int)CAMERA_STATES.GAME_RUNNING);
                }
                break ;

                case CAMERA_STATES.GAME_RUNNING:
                RunGame();

                if (LevelController.Instance.PlayerHasKilledAllEnemies())
                {
                    ChangeState((int)CAMERA_STATES.WIN);
                }

                if (PlayerisDead())
                {
                    ChangeState((int)CAMERA_STATES.LOSE);
                }

                if (Input.GetKeyDown(KeyCode.P))
                {
                    ChangeState((int)CAMERA_STATES.PAUSE);
                }

                if (Input.GetKeyDown(KeyCode.N))
                {
                    ChangeState((int)CAMERA_STATES.WIN);
                }
                break;

            case CAMERA_STATES.WIN:
                ShowWinScreen();
                if (PressedNextButton())
                {
                    ChangeState((int)CAMERA_STATES.LOAD_GAME);
                    
                }
                if (ButtonPressedReloadGame())
                {
                    SceneManager.LoadScene("Game");
                }

                break;

            case CAMERA_STATES.LOSE:
                ShowLoseScreen();
                if (PressedNextButton())
                {
                    ChangeState((int)CAMERA_STATES.LOAD_GAME);
                    
                }

                if (ButtonPressedReloadGame())
                {
                    SceneManager.LoadScene("Game");
                }

                break;

            case CAMERA_STATES.PAUSE:
                if (ButtonPressedReturnGame())
                {
                    ChangeState((int)CAMERA_STATES.GAME_RUNNING);
                }

                if (ButtonPressedReloadGame())
                {
                    SceneManager.LoadScene("Game");
                }
                break;
        }
    }
}
