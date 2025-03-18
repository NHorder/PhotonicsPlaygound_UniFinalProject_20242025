using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ConfirmAction
{
    ResetLevel,
    LeaveLevel
}

public class ConfirmationPanel : MonoBehaviour
{

    private Language _language = Language.English;

    private UIController _uiController;
    private GameController _gameController;


    private Button _confirmButton;
    private TMP_Text _titleText;
    private TMP_Text _warningText;


    // Start is called before the first frame update
    void Start()
    {
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        _language = _gameController.activeLanguage;
        _uiController = GameObject.FindGameObjectsWithTag("UI_Controller")[0].GetComponent<UIController>();

        // Collect child gameObjects
        var childTransformsList = gameObject.GetComponentsInChildren<RectTransform>();

        // Loop through and sort to find needed objects to update 
        foreach (RectTransform childTransform in childTransformsList)
        {
            var childObject = childTransform.gameObject;

            if (childObject.name == "ConfirmButton") _confirmButton = childObject.GetComponent<Button>();
            else if (childObject.name == "ConfirmActionTitle") _titleText = childObject.GetComponent<TMP_Text>(); 
            else if (childObject.name == "ConfirmActionWarning") _warningText = childObject.GetComponent<TMP_Text>();
        }
    }

    public void UpdateUIComponents(ConfirmAction confirmAction)
    {

        // Define the action as text
        string action = null;

        _confirmButton.onClick.RemoveAllListeners();


        // Go through statements to confirm what action is to be conducted and their associated action to confirmButton
        if (confirmAction == ConfirmAction.ResetLevel)
        {
            if (_language == Language.English) action = "reset the level";
            else if (_language == Language.Welsh) action = "reset level (NOT TRANSLATED)";
            _confirmButton.onClick.AddListener(ResetLevel);
        }

        else if (confirmAction == ConfirmAction.LeaveLevel)
        {
            if (_language == Language.English) action = "leave the level";
            else if (_language == Language.Welsh) action = "leave the level (NOT TRANSLATED)";
            _confirmButton.onClick.AddListener(LevelSelection);
        }

        // if action is null, throw an error
        if (action == null)
        {
            Debug.LogError("ERROR: An non-fatal error has occurred when trying to display confirmation warnings");
        }
        // Else if not missing the text components, then update their text
        else if (_titleText != null && _warningText != null)
        {
            if (_language == Language.English)
            {
                _titleText.text = $"Are you sure you want to {action}?";
                _warningText.text = $"Are you sure you want to {action}? All progress on this level will be lost to the darkest depths of space and cannot be retreived at a later date. Do you wish to continue?";
            }
            else if (_language == Language.Welsh)
            {
                _titleText.text = $"Are you sure you want to {action}?";
                _warningText.text = $"Are you sure you want to {action}? All progress on this level will be lost to the darkest depths of space and cannot be retreived at a later date. Do you wish to continue?";
            }
        }
        // Else throw a warning that this is missing the objects
        else
        {
            Debug.LogWarning("WARNING: Needed text objects for confirmation panel not found");
        }
    }


    public void CloseConfirmation()
    {
        _uiController.PresentFixedPanel(FixedUIPanel.ConfirmAction,false);
    }

    public void ResetLevel()
    {
        _gameController.ResetLevel();
    }

    public void LevelSelection()
    {
        SceneController.ToLevelSelection();
    }

}
