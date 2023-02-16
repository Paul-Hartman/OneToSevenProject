using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenWin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.Find("Win Text").GetComponent<TextMeshProUGUI>().text = LanguageController.Instance.GetText("screen.win");

        this.transform.Find("NextLevelButton").GetComponent<Button>().onClick.AddListener(PressedGoToNextLevel);

        this.transform.Find("MainMenuButton").GetComponent<Button>().onClick.AddListener(PressedGoToMainMenu);
    }

    private void PressedGoToMainMenu()
    {
        GameController.Instance.UserHasPressedReloadGame();
    }

    private void PressedGoToNextLevel()
    {
        GameController.Instance.PressedGoToNextLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
