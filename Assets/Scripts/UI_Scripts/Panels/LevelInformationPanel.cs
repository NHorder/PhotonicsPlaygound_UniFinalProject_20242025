using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;

public class LevelInformationPanel : MonoBehaviour
{
    private Language _langauge = Language.English;

    private GameController _gameController;
    private UIController _uiController;
    private ConfirmationPanel _confirmationPanel;

    private TMP_Text _titleText;
    private TMP_Text _descriptionText;

    // Start is called before the first frame update
    void Start()
    {
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

            if (_titleText != null && _descriptionText != null) break;
        }


        UpdateText();
    }

    private void UpdateText()
    {
        if (_langauge == Language.English)
        {
            _titleText.text = _gameController.levelNameEnglish;
            _descriptionText.text = _gameController.levelDescriptionEnglish;
        }
        else if (_langauge == Language.Welsh)
        {
            _titleText.text = _gameController.levelNameWelsh;
            _descriptionText.text = _gameController.levelDescriptionWelsh;
        }
    }



    public void SettingsInteract()
    {
        _uiController.PresentFixedPanel(FixedUIPanel.Settings, true);
    }

    public void ResetLevelInteract()
    {
        _confirmationPanel.UpdateUIComponents(ConfirmAction.ResetLevel);
        _uiController.PresentFixedPanel(FixedUIPanel.ConfirmAction,true);
    }
}
