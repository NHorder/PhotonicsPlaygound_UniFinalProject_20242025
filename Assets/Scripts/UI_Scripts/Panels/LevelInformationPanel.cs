using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;

public class LevelInformationPanel : MonoBehaviour
{
    private Language _language = Language.English;

    private GameController _gameController;
    private UIController _uiController;
    private ConfirmationPanel _confirmationPanel;

    private TMP_Text _titleText;
    private TMP_Text _descriptionText;
    private TMP_Text _resetLevelText;

    // Start is called before the first frame update
    void Start()
    {
        _language = PersistenceController.GetLanguage();

        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        _uiController = GameObject.FindGameObjectsWithTag("UI_Controller")[0].GetComponent<UIController>();
        _confirmationPanel = GameObject.FindGameObjectsWithTag("ConfirmationPanel")[0].GetComponent<ConfirmationPanel>();

        var childTextList = gameObject.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text childText in childTextList)
        {
            var childObject = childText.gameObject;

            // filter for log text and progress text
            if (childObject.name == "LevelTitle") _titleText = childText;
            else if (childObject.name == "LevelDescription") _descriptionText = childText;
            else if (childObject.name == "ResetLevelText") _resetLevelText = childText;

            if (_titleText != null && _descriptionText != null && _resetLevelText != null) break;
        }


        UpdateText();
    }

    private void UpdateText()
    {
        if (_language == Language.English)
        {
            _titleText.text = _gameController.levelNameEnglish;
            _descriptionText.text = _gameController.levelDescriptionEnglish;
            _resetLevelText.text = "Reset Level";
        }
        else if (_language == Language.Welsh)
        {
            _titleText.text = _gameController.levelNameWelsh;
            _descriptionText.text = _gameController.levelDescriptionWelsh;
            _resetLevelText.text = "Ailosod Lefel";
        }
    }



    public void SettingsInteract()
    {
        _uiController.PresentFixedPanel(FixedUIPanel.Settings, true);
    }

    public void ResetLevelInteract()
    {

        if (PersistenceController.GetDisableConfirmations()) _gameController.ResetLevel();
        else
        {
            _confirmationPanel.UpdateUIComponents(ConfirmAction.ResetLevel);
            _uiController.PresentFixedPanel(FixedUIPanel.ConfirmAction,true);
        }
        
    }


    public void UpdateLanguage(Language newLanguage)
    {
        _language = newLanguage;
        UpdateText();
    }
}
