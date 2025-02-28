using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using TMPro;

public class LevelProgressPanel : MonoBehaviour
{
    private bool _forceNoChangeOnNewCommunication = false;

    private TMP_Text _logText;
    private TMP_Text _progressText;

    private List<string> _logTextList;

    private int _recordRetentionNumber = 10;

    private UIController _uiController;
    private GameController _gameController;


    private int _numLocks;

    private bool _panelOpen = false;

    
    // Start is called before the first frame update
    void Start()
    {
        
        _uiController = GameObject.FindGameObjectsWithTag("UI_Controller")[0].GetComponent<UIController>();
        _gameController = _uiController.GetGameController();

        _numLocks = _gameController.framerateRelatedSettings.numLocksForLevelCompletion;

        _forceNoChangeOnNewCommunication = _uiController.levelProgressPanelSettings.forceNoChangeOnNewCommunication;
        _recordRetentionNumber = _uiController.levelProgressPanelSettings.recordRetentionNumber;

        _logTextList = new List<string>();

        var childTransformList = gameObject.GetComponentsInChildren<RectTransform>();
        foreach (RectTransform childTransform in childTransformList)
        {
            var childObject = childTransform.gameObject;

            if (childObject.name == "LevelProgressLogText") _logText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "LevelProgressProgressText") _progressText = childObject.GetComponent<TMP_Text>();
        }

        // Inital trigger to remove default text
        UpdateUIComponents();

        _uiController.PresentPanel(UIPanel.LogCommunications,false);

    }


    public void LogCommunications(string satelliteName, float numUnlocks)
    {
        // If the user has not specified in settings that this should not appear each time communications are made
        // and if it isn't expanded, then open it.
        if (!_forceNoChangeOnNewCommunication) _uiController.PresentPanel(UIPanel.LogCommunications,true);

        var logText = satelliteName;

        if (numUnlocks <= 0) logText += ": Connection Lost";
        else
        {
            // Using a percentage allows for more locks to be in place
            var unlockPercentage = (numUnlocks / _numLocks);

            if (unlockPercentage > 0.95f) logText += ": Connection Established";
            else if (unlockPercentage > 0.75f) logText += ": Confirming Permissions....";
            else if (unlockPercentage > 0.5f) logText += ": Securing Connection....";
            else if (unlockPercentage > 0.25f) logText += ": Stabilizing Connection....";
            else logText += ": Connection Found";
        }

        _logTextList.Add(logText);

        // Trims list to wanted size
        if (_logTextList.Count > _recordRetentionNumber) _logTextList.RemoveAt(0);

        UpdateUIComponents();
    }


    private void UpdateUIComponents()
    {
        var loopText = "";

        // Loop through logs and create a string to update the log with
        foreach (string record in _logTextList)
        {
            loopText += $"{record}\n";
        }

        // Update the log text
        _logText.text = loopText;
        UpdateSuccessText();
    }

    public void UpdateSuccessText()
    {
        // Update the progress text
        _progressText.text = $"Connections Established: {_gameController.activeDestinations} / {_gameController.worldInfo.numDestinations}";
    }

    public void HideExpandCommunicationsPanel()
    {
        _panelOpen = !_panelOpen;
        _uiController.PresentPanel(UIPanel.LogCommunications,_panelOpen);
    }


}
