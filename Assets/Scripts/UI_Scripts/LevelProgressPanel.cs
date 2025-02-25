using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using TMPro;

public class LevelProgressPanel : MonoBehaviour
{
    private bool forceNoChangeOnNewCommunication = false;

    private TMP_Text logText;
    private TMP_Text progressText;

    private List<string> logTextList;

    private int recordRetentionNumber = 10;

    private UI_Controller ui_Controller;
    private GameController gameController;


    private int numLocks;

    
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        numLocks = gameController.framerateRelatedSettings.numLocksForLevelCompletion;
        
        ui_Controller = GameObject.FindGameObjectsWithTag("UI_Controller")[0].GetComponent<UI_Controller>();
        forceNoChangeOnNewCommunication = ui_Controller.levelProgressPanelSettings.forceNoChangeOnNewCommunication;
        recordRetentionNumber = ui_Controller.levelProgressPanelSettings.recordRetentionNumber;

        logTextList = new List<string>();

        RectTransform[] childTransforms = gameObject.GetComponentsInChildren<RectTransform>();
        foreach (RectTransform childTransform in childTransforms)
        {
            GameObject childObject = childTransform.gameObject;

            if (childObject.name == "LevelProgressLogText") logText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "LevelProgressProgressText") progressText = childObject.GetComponent<TMP_Text>();
        }

        // Inital trigger to remove default text
        UpdateUIComponents();

    }


    public void LogCommunications(string satellite_name, float numUnlocks)
    {
        string logText = satellite_name;

        if (numUnlocks <= 0) logText += ": Connection Lost";
        else
        {
            // Using a percentage allows for more locks to be in place
            float unlockPercentage = (numUnlocks / numLocks);

            if (unlockPercentage > 0.95f) logText += ": Connection Established";
            else if (unlockPercentage > 0.75f) logText += ": Confirming Permissions....";
            else if (unlockPercentage > 0.5f) logText += ": Securing Connection....";
            else if (unlockPercentage > 0.25f) logText += ": Stabilizing Connection....";
            else logText += ": Connection Found";
        }

        logTextList.Add(logText);

        // Trims list to wanted size
        if (logTextList.Count > recordRetentionNumber) logTextList.RemoveAt(0);

        UpdateUIComponents();
    }


    private void UpdateUIComponents()
    {

        // If the user has not specified in settings that this should not appear each time communications are made
        // and if it isn't expanded, then open it.
        if (!forceNoChangeOnNewCommunication) ui_Controller.PresentPanel(UIPanel.LogCommunications,true);

        string loopText = "";

        // Loop through logs and create a string to update the log with
        foreach (string record in logTextList)
        {
            loopText += record + "\n";
        }

        // Update the log text
        if (!gameController.gameEnd) logText.text = loopText;

        // Update the progress text
        progressText.text = "Connections Established: "+gameController.activeDestinations +" / "+gameController.worldInfo.numDestinations;
    }




}
