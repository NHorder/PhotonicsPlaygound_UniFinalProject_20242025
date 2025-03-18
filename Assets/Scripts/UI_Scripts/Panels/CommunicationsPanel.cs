using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;

public class CommunicationsPanel : MonoBehaviour
{
    private Language _language = Language.English;

    private bool _forceNoChangeOnNewCommunication = false;
   
    private TMP_Text _logText;
    private TMP_Text _progressText;
    private TMP_Text _progressTextTwo;
    private List<string> _logTextList;

    private int _recordRetentionNumber = 10;

    private UIController _uiController;
    private GameController _gameController;


    private int _numLocks;
    private bool _panelOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        _uiController = GameObject.FindGameObjectsWithTag("UI_Controller")[0].GetComponent<UIController>();


        _numLocks = _gameController.framerateRelatedSettings.numLocksForLevelCompletion;
        _forceNoChangeOnNewCommunication = _uiController.advancedSettings.overwriteCommunicationMovement;
        _recordRetentionNumber = _uiController.advancedSettings.communicationRecordRetentionNumber;

        // Initialise list
        _logTextList = new List<string>();

        // Retrieve children objects
        var childTransformList = gameObject.GetComponentsInChildren<RectTransform>();
        foreach (RectTransform childTransform in childTransformList)
        {
            var childObject = childTransform.gameObject;

            // filter for log text and progress text
            if (childObject.name == "LevelProgressLogText") _logText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "LevelProgressProgressText") _progressText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "LevelProgressProgressTextTwo") _progressTextTwo = childObject.GetComponent<TMP_Text>();
        }
        // Inital trigger to remove default text
        UpdateUIComponents();
        UpdateSuccessText();

    }

    public void LogCommunications(string satelliteName, float numUnlocks, string otherText = "")
    {
        // If the user has not specified in settings that this should not appear each time communications are made
        // and if it isn't expanded, then open it.
        //if (!_forceNoChangeOnNewCommunication) _uiController.PresentPanel(UIPanel.LogCommunications,true);
        
        var logText = "";


        if (numUnlocks <= 0 && otherText != "")
        {
            logText =  $"{satelliteName}: {otherText}";

        }
        // If the number of unlocks is less than or equal to zero, assume the connection was lost.
        else if (numUnlocks <= 0)
        {
            if (_language == Language.English) logText = $"{satelliteName}: Connection Lost\n";
            else if (_language == Language.Welsh) logText = $"{satelliteName}: NOT TRANSLATED\n";
        }
        
        else
        {
            // Using a percentage allows for more locks to be in place
            var unlockPercentage = (numUnlocks / _numLocks);

            if (_language == Language.English)
            {
                // Determine Log text based on unlock percentage
                if (unlockPercentage > 0.95f) logText = $"{satelliteName}: Connection Established\n";
                else if (unlockPercentage > 0.75f) logText = $"{satelliteName}: Confirming Permissions....\n";
                else if (unlockPercentage > 0.5f) logText = $"{satelliteName}: Securing Connection....\n";
                else if (unlockPercentage > 0.25f) logText = $"{satelliteName}: Stabilizing Connection....\n";
                else logText = $"{satelliteName}: Connection Found\n";
            }
            else if (_language == Language.Welsh)
            {
                // Determine Log text based on unlock percentage
                if (unlockPercentage > 0.95f) logText = $"{satelliteName}: NEED TRANSLATING\n";
                else if (unlockPercentage > 0.75f) logText = $"{satelliteName}: NEED TRANSLATING\n";
                else if (unlockPercentage > 0.5f) logText = $"{satelliteName}: NEED TRANSLATING\n";
                else if (unlockPercentage > 0.25f) logText = $"{satelliteName}: NEED TRANSLATING\n";
                else logText = $"{satelliteName}: NEED TRANSLATING\n";
            }
            
        }

        // Append the log text to the list
        _logTextList.Add(logText);

        // Trims list to wanted size
        if (_logTextList.Count > _recordRetentionNumber) _logTextList.RemoveAt(0);

        // Call UpdateUIComponents to trigger visual updates
        UpdateUIComponents();
    }


    private void UpdateUIComponents()
    {
        // Following C# conventions (https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
        // Use a string builder to repeatedly add text to a single string
        var loopText = new StringBuilder();

        // Loop through logs and create a string to update the log with
        foreach (string record in _logTextList)
        {
            loopText.Append(record);
        }

        // Update the log text
        _logText.text = loopText.ToString();

        // Call to update the success
        UpdateSuccessText();
    }

    public void UpdateSuccessText()
    {
        // This is within a separate function as it can be called from LaserDestination

        // Update the progress text
        if (_language == Language.English)
        {
            _progressText.text = $"Connections Established: {_gameController.activeDestinations} / {_gameController.worldInfo.numDestinations}";
            _progressTextTwo.text = $"Connections Established: {_gameController.activeDestinations} / {_gameController.worldInfo.numDestinations}";
        }

        else if (_language == Language.Welsh)
        {
            _progressText.text = $"NOT TRANSLATED: {_gameController.activeDestinations} / {_gameController.worldInfo.numDestinations}";
            _progressTextTwo.text = $"NOT TRANSLATED: {_gameController.activeDestinations} / {_gameController.worldInfo.numDestinations}";
        }

    }
}
