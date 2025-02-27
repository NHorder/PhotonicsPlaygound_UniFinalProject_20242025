using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDestination : MonoBehaviour
{

    private GameController gameController;

    private int numLocksForLevelCompletion;
    private int counterToHoldForLockCompletion;
    private int laserInteractionsPerSecond;


    private bool lockAdvanceRequest = false;


    private int updateCounter;
    private int lockProgressionCounter;
    private int numUnlocks = 0;

    private int laserDelay;


    private bool allLocksOpen = false;

    private Animator animator;
    private LevelProgressPanel levelProgressPanel;


    private string satelliteName;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

        try{
            levelProgressPanel = GameObject.FindGameObjectsWithTag("LevelProgressPanel")[0].GetComponent<LevelProgressPanel>();
        }
        catch{}

        numLocksForLevelCompletion = gameController.framerateRelatedSettings.numLocksForLevelCompletion;
        counterToHoldForLockCompletion = gameController.framerateRelatedSettings.counterToHoldForLevelCompletion;
        laserDelay = gameController.framerateRelatedSettings.laserCycleDelay;

        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Force retrieval of information from gameController if the counterToHoldForLockCompletion is less than or equal to 0
        // Can occur when the gameController hasn't done the framerate maths before this is run.
        if (counterToHoldForLockCompletion <= 0)
        {
            numLocksForLevelCompletion = gameController.framerateRelatedSettings.numLocksForLevelCompletion;
            counterToHoldForLockCompletion = gameController.framerateRelatedSettings.counterToHoldForLevelCompletion;
            laserDelay = gameController.framerateRelatedSettings.laserCycleDelay;
        }   

        // Forced update for satellite name, as the Satellite_Info start function may not be complete by this time
        if (satelliteName == null) satelliteName = gameObject.GetComponent<Satellite_Info>().satelliteName;

        // Sync delay of that to laser creation and destruction - meaning it will be seen as consistent
        if (updateCounter > laserDelay && levelProgressPanel != null)
        {

            // If lock advance request received, then update the lockProgressionCounter, and reset the lock advance request
            if (lockAdvanceRequest)
            {
                lockAdvanceRequest = false;
                lockProgressionCounter += 1;
            }
            else 
            {

                // Update UI components to announce that a locks have been reset
                if (numUnlocks > 0) levelProgressPanel.LogCommunications(satelliteName,-1);
                // Note: -1 indicates that the locks have been reset, as the connection is lost
                

                // If no request, then reset the locks - as the laser isn't consistently connected.
                lockProgressionCounter = 0;
                numUnlocks = 0;


                // If all locks were open but have been reset, notify gameController. 
                if (allLocksOpen) 
                {
                    // Update gameObject animator if locks reset, but they were open
                    animator.SetBool("Active",false);

                    gameController.DestinationTrigger(false);

                    levelProgressPanel.UpdateSuccessText();

                    allLocksOpen = false;
                }
            }

            // if the laser has been held long enough for a lock to be opened, open lock
            if (lockProgressionCounter >= counterToHoldForLockCompletion)
            {
                // Open lock
                numUnlocks += 1;

                // Reset progression counter
                lockProgressionCounter = 0;

                // Update UI components to annouce that a lock has been opened
                if (numUnlocks <= numLocksForLevelCompletion) levelProgressPanel.LogCommunications(satelliteName,numUnlocks);

                // if the number of unlocks is more or equal to the number of locks being used
                if (numUnlocks >= numLocksForLevelCompletion)
                {
                    // Update gameObject animator if all locks open
                    animator.SetBool("Active",true);

                    // By placing this here, it means it will only run once - preventing one destination from being marked as more than one
                    if (!allLocksOpen)
                    {
                        gameController.DestinationTrigger(true);
                        levelProgressPanel.UpdateSuccessText();
                    }

                    // Set all locks open to false and notify gamecontroller of this update
                    allLocksOpen = true;


                    // Enforces a limit of number of unlocks - prevents ever increasing lock unlocks
                    numUnlocks = numLocksForLevelCompletion + 1;
                }
            }

            // Reset update counter
            updateCounter = 0;
        }
        else updateCounter += 1;
    }


    public void AdvanceLock()
    {
        lockAdvanceRequest = true;
    }


}
