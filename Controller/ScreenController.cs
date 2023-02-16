using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    private static ScreenController _instance;

    public static ScreenController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType<ScreenController>();
            }
            return _instance;
        }
    }

    public GameObject[] Screens;

    //public GameObject ScreenMenuMain;
    //public GameObject ScreenLoadGame;
    //public GameObject ScreenWinGame;
    //public GameObject ScreenLoseGame;

    //public GameObject ScreenPause;
    private List<GameObject> m_screensCreated = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CreateScreen(string _nameScreen, bool _destroyPreviosScreen)
    {

        if (_destroyPreviosScreen)
        {
            for(int i = 0; i < m_screensCreated.Count; i++)
            {
                GameObject.Destroy(m_screensCreated[i]);

            }
            m_screensCreated.Clear();
        }
        for(int i = 0; i < Screens.Length; i++)
        {
            if(Screens[i].name == _nameScreen)
            {
                GameObject newScreen = Instantiate(Screens[i]);
                m_screensCreated.Add(newScreen);
            }
        }
    }
    public void DestroyScreens()
    {
        for (int i = 0; i < m_screensCreated.Count; i++)
        {
            GameObject.Destroy(m_screensCreated[i]);

        }
        m_screensCreated.Clear();
    }
}
