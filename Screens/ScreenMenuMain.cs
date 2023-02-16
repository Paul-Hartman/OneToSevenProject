using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenMenuMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    { this.transform.Find("Language/Text").GetComponent<TextMeshProUGUI>().text = LanguageController.Instance.CodeLanguage;
        this.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = LanguageController.Instance.GetText("screen.main.menu");
        this.transform.Find("PlayGame").GetComponent<Button>().onClick.AddListener(PressedPlayButton);

        this.transform.Find("Language").GetComponent<Button>().onClick.AddListener(SwitchLanguage);
        
    }

    private void SwitchLanguage()
    {

        if (LanguageController.Instance.CodeLanguage == "es")
        {
            LanguageController.Instance.CodeLanguage = "en";
        }
        else if (LanguageController.Instance.CodeLanguage == "en")
        {
            LanguageController.Instance.CodeLanguage = "de";
        }
        else if (LanguageController.Instance.CodeLanguage == "de")
        {
            LanguageController.Instance.CodeLanguage = "es";
        }
        this.transform.Find("Language/Text").GetComponent<TextMeshProUGUI>().text = LanguageController.Instance.CodeLanguage;
        this.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = LanguageController.Instance.GetText("screen.main.menu");
    }

    private void PressedPlayButton()
    {
        GameController.Instance.PlayClicked();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
