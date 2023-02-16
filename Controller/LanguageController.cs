using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class LanguageController : MonoBehaviour
{
    private static LanguageController _instance;
    public static LanguageController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType<LanguageController>();
            }
            return _instance;
        }
    }

    public TextAsset GameText;
    public string CodeLanguage = "en";
    private Hashtable m_texts = new Hashtable();
    

    private void LoadGameTexts()
    {
        if (m_texts.Count != 0) return;
        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(GameText.text);
        XmlNodeList textList = xmlDoc.GetElementsByTagName("text");
        foreach (XmlNode textEntry in textList)
        {
            XmlNodeList textNodes = textEntry.ChildNodes;
            string idText = textEntry.Attributes["id"].Value;
            m_texts.Add(idText, new TextEntry(idText, textNodes));
        }
    }



    public string GetText(string _id)
    {
        LoadGameTexts();
        if(m_texts[_id] != null)
        {
            return((TextEntry)m_texts[_id]).GetText(CodeLanguage);
        }
        else
        {
            return _id;
        }
    }
}
