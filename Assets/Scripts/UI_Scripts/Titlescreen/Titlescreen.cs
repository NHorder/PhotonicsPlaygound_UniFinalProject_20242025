using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Titlescreen : MonoBehaviour
{

    private TMP_Text _titleText;
    private TMP_Text _startGameText;
    private TMP_Text _settingsText;

    // Start is called before the first frame update
    void Start()
    {
        var childrenText = gameObject.GetComponentsInChildren<TMP_Text>();

        foreach (TMP_Text childText in childrenText)
        {
            if (childText.name == "GameTitle") _titleText = childText;
            else if (childText.name == "StartGameText") _startGameText = childText;
            else if (childText.name == "SettingsText") _settingsText = childText;
        }
    }

    public void UpdateLanguage(Language language)
    {

        if (language == Language.English)
        {
            _titleText.text = "Photonics Playground";
            _startGameText.text = "Start Game";
            _settingsText.text = "Settings";
        }
        else if (language == Language.Welsh)
        {
            _titleText.text = "Maes Chwarae Ffotoneg";
            _startGameText.text = "Dechrau Gêm";
            _settingsText.text = "Gosodiadau";

        }

    }
}
