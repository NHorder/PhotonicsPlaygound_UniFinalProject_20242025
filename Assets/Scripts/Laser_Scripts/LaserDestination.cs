using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDestination : MonoBehaviour
{

    private GameController _gameController;
    private string _satelliteName;

    private int _numLocksForLevelCompletion;
    private int _counterToHoldForLockCompletion;

    private bool _lockAdvanceRequest = false;
    private int _updateCounter;
    private int _lockProgressionCounter;


    private int _numUnlocks = 0;
    private bool _allLocksOpen = false;


    private int _laserDelay;
    private Animator _animator;
    private LevelProgressPanel _levelProgressPanel;
    

    // Start is called before the first frame update
    void Start()
    {
        // Find the game controller, establish link
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();


        // Try to collect the level progress panel - as it may not exist / not be used
        // Cannot use UIController checks, as UIController may not have attached itself to the gamecontroller yet
        // Cannot be moved to Update, as it would repeatedly check and collect the UI controller, regardless
        // of whether the level progress panel is intended Not to exist
        try
        {
            _levelProgressPanel = GameObject.FindGameObjectsWithTag("LevelProgressPanel")[0].GetComponent<LevelProgressPanel>();
        } 
        catch
        {
            // No error or log, this is expected for Title Screen and Level Selection scenes
        }

        // Retrieve this game objects animator - used to visually change the destination sprite when activated.
        _animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Retrieve game controller framerate settings
        // Present here instead of Start, as maths is not always complete during the initialisation of this component
        if (_numLocksForLevelCompletion <= 0)
        {
            _numLocksForLevelCompletion = _gameController.framerateRelatedSettings.numLocksForLevelCompletion;
            _counterToHoldForLockCompletion = _gameController.framerateRelatedSettings.counterToHoldForLevelCompletion;
            _laserDelay = _gameController.framerateRelatedSettings.laserCycleDelay;
        }   

        // Forced update for satellite name, as the Satellite_Info start function may not be complete by this time
        if (_satelliteName == null) _satelliteName = gameObject.GetComponent<Satellite_Info>().satelliteName;

        // Sync delay of that to laser creation and destruction - meaning it will be seen as consistent
        if (_updateCounter > _laserDelay && _levelProgressPanel != null)
        {
            // If lock advance request received, then update the lockProgressionCounter, and reset the lock advance request
            if (_lockAdvanceRequest)
            {
                // Reset the advance request - in order to get the next request
                _lockAdvanceRequest = false;
                
                // Increase the progression counter
                _lockProgressionCounter += 1;
            }
            else 
            {
                // Update UI components to announce that a locks have been reset
                if (_numUnlocks > 0) _levelProgressPanel.LogCommunications(_satelliteName,-1);
                // Note: -1 indicates that the locks have been reset, as the connection is lost
                
                // If no request, then reset the locks - as the laser isn't consistently connected.
                _lockProgressionCounter = 0;
                _numUnlocks = 0;

                // If all locks were open but have been reset, notify gameController. 
                if (_allLocksOpen) 
                {
                    // Update gameObject animator if locks reset, but they were open
                    _animator.SetBool("Active",false);

                    // Notify game controller
                    _gameController.DestinationTrigger(false);

                    // Update the level progress panel text - this states "<SATNAME>: Connection Lost"
                    _levelProgressPanel.UpdateSuccessText();

                    // Set all locks open to false
                    _allLocksOpen = false;
                }
            }

            // if the laser has been held long enough for a lock to be opened, open lock
            if (_lockProgressionCounter >= _counterToHoldForLockCompletion)
            {
                // Open lock
                _numUnlocks += 1;

                // Reset progression counter
                _lockProgressionCounter = 0;

                // Update UI components to annouce that a lock has been opened
                if (_numUnlocks <= _numLocksForLevelCompletion) _levelProgressPanel.LogCommunications(_satelliteName,_numUnlocks);

                // if the number of unlocks is more or equal to the number of locks being used
                if (_numUnlocks >= _numLocksForLevelCompletion)
                {
                    // Update gameObject animator if all locks open
                    _animator.SetBool("Active",true);

                    // By placing this here, it means it will only run once - preventing one destination from being marked as more than one
                    if (!_allLocksOpen)
                    {
                        // Notify Game Controller that a destination has become active
                        _gameController.DestinationTrigger(true);

                        // Update text to show success
                        _levelProgressPanel.UpdateSuccessText();
                    }

                    // Set all locks open to false and notify gamecontroller of this update
                    _allLocksOpen = true;

                    // Enforces a limit of number of unlocks - prevents ever increasing lock unlocks
                    _numUnlocks = _numLocksForLevelCompletion + 1;
                }
            }

            // Reset update counter
            _updateCounter = 0;
        }
        else _updateCounter += 1;
    }


    public void AdvanceLock()
    {
        // Call from Laser interaction to advance the lock
        _lockAdvanceRequest = true;
    }


}
