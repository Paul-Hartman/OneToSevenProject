using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenLose : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.Find("Lose Text").GetComponent<TextMeshProUGUI>().text = LanguageController.Instance.GetText("screen.lose");

        this.transform.Find("Replay button").GetComponent<Button>().onClick.AddListener(PressedReloadCurrentLevel);

        this.transform.Find("Go To Main").GetComponent<Button>().onClick.AddListener(PressedGoToMainMenu);
    }

    private void PressedGoToMainMenu()
    {
        GameController.Instance.UserHasPressedReloadGame();
    }

    private void PressedReloadCurrentLevel()
    {
        GameController.Instance.PressedGoToNextLevel();
    }

}
