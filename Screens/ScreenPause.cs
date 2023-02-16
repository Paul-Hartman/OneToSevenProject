using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenPause : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.Find("PausedText").GetComponent<TextMeshProUGUI>().text = LanguageController.Instance.GetText("screen.pause");
        
        this.transform.Find("ResumeButton").GetComponent<Button>().onClick.AddListener(PressedResumeGame);

        this.transform.Find("ReloadButton").GetComponent<Button>().onClick.AddListener(PressedGoToMainMenu);
    }

    private void PressedGoToMainMenu()
    {
        GameController.Instance.UserHasPressedReloadGame();
    }

    private void PressedResumeGame()
    {
        GameController.Instance.UserHasPressedReturnGame();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
