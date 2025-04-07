using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationSatellite : MonoBehaviour
{
    /// <summary>
    /// Class for destination satellites
    /// </summary>
    

    // The laser colour this destination is looking for
    public LaserColour neededLaserColour = LaserColour.White;

    private GameController _gameController;
    private UIController _uiController;
    private string _satelliteName;

    // Information related to locks, collected from game controller
    private int _numLocksForLevelCompletion;
    private int _counterToHoldForLockCompletion;

    // If the light hits this satellite, this is set to true
    // Then the update counter starts, alongside the lock progression counter
    private bool _lockAdvanceRequest = false;
    private int _updateCounter;
    private int _lockProgressionCounter;
    private int _numUnlocks = 0;

    // If all locks are open, stop trying to open locks
    public bool allLocksOpen = false;

    
    private bool _colourAccepted = false;
    private bool _newColourSeen = false;
    private LaserColour _previousLaserColour;

    // Transparency related information
    private bool _newTransparencySeen = false;
    private bool _transparencyAccepted = false;
    private float _previousTransparency;
    private float _neededTransparency = 0.5f;

    // This class needs to know the laser delay, as to make sure the requests are consitent instead of per frame
    private int _laserDelay;
    private Animator _animator;

    // Link to communication panel to send messages (I.e wrong colour)
    private CommunicationsPanel _communicationsPanel;
    

    /// <summary>
    /// Initalisation Method
    /// </summary>
    void Start()
    {
        // Find the game controller, establish link
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        _uiController = GameObject.FindGameObjectsWithTag("UI_Controller")[0].GetComponent<UIController>();
        _neededTransparency = _gameController.specializedInteractionSettings.minimumTransparencyNeededForDestinationRecognition;

        // Try to collect the level progress panel - as it may not exist / not be used
        // Cannot use UIController checks, as UIController may not have attached itself to the gamecontroller yet
        // Cannot be moved to Update, as it would repeatedly check and collect the UI controller, regardless
        // of whether the level progress panel is intended Not to exist
        try
        {
            _communicationsPanel = GameObject.FindGameObjectsWithTag("CommunicationsPanel")[0].GetComponent<CommunicationsPanel>();
        } 
        catch
        {
            // No error or log, this is expected for Title Screen and Level Selection scenes
        }

        // Retrieve this game objects animator - used to visually change the destination sprite when activated.
        _animator = gameObject.GetComponent<Animator>();
        // Determine colour ID and notify animator
        var colourID = 0;
        if (neededLaserColour == LaserColour.Red) colourID = 1;
        else if (neededLaserColour == LaserColour.Blue) colourID = 2;
        else if (neededLaserColour == LaserColour.Green) colourID = 3;
        else if (neededLaserColour == LaserColour.Yellow) colourID = 4;
        else if (neededLaserColour == LaserColour.Cyan) colourID = 5;
        else if (neededLaserColour == LaserColour.Magenta) colourID = 6; 
        _animator.SetInteger("ColourID",colourID);
    }

    /// <summary>
    /// Method called once per frame, handles the locks
    /// </summary>
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
        if (_satelliteName == null) _satelliteName = gameObject.GetComponent<SatelliteInfo>().satelliteName;

        // Sync delay of that to laser creation and destruction - meaning it will be seen as consistent
        if (_updateCounter > _laserDelay && _communicationsPanel != null)
        {
            // If lock advance request received, then update the lockProgressionCounter, and reset the lock advance request
            if (_lockAdvanceRequest && _colourAccepted && _transparencyAccepted)
            {
                // Reset the advance request - in order to get the next request
                _lockAdvanceRequest = false;
                
                // Increase the progression counter
                _lockProgressionCounter += 1;
            }
            else 
            {
                
                // If the lock advance has been requested, but the colour isn't accepted and it's new
                if (_lockAdvanceRequest && !_colourAccepted && _newColourSeen)
                {
                    // If allowed (by settings) make the comns panel visible
                    if (!_uiController.advancedSettings.overwriteCommunicationMovement) _uiController.ToggleVisibleCommunicationsIfClosed();    

                    // Get language
                    var language = PersistenceController.GetLanguage();

                    // Send a message to communications panel
                    if (language == Language.English) _communicationsPanel.LogCommunications(_satelliteName,-1, "Incorrect colour\n");
                    else if (language == Language.Welsh) _communicationsPanel.LogCommunications(_satelliteName,-1, "Lliw Anghywir\n");

                    // Reset
                    _newColourSeen = false;
                }

                // If the lock advance has been requested and the beam isn't strong enough, and it's a new transparency
                else if (_lockAdvanceRequest && !_transparencyAccepted && _newTransparencySeen)
                {
                    // Move the comms panel if allowed by settings
                    if (!_uiController.advancedSettings.overwriteCommunicationMovement) _uiController.ToggleVisibleCommunicationsIfClosed();

                    // Collect language
                    var language = PersistenceController.GetLanguage();

                    // Send communications to the communications panel
                    if (language == Language.English) _communicationsPanel.LogCommunications(_satelliteName,-1, "Connection is too weak\n");
                    else if (language == Language.Welsh) _communicationsPanel.LogCommunications(_satelliteName,-1, "Cysylltiad yn rhy wan\n");

                    // Reset
                    _newTransparencySeen = false;
                }


                // Update UI components to announce that a locks have been reset
                else if (_numUnlocks > 0) _communicationsPanel.LogCommunications(_satelliteName,-1);
                // Note: -1 indicates that the locks have been reset, as the connection is lost
                
                // If no request, then reset the locks - as the laser isn't consistently connected.
                _lockProgressionCounter = 0;
                _numUnlocks = 0;

                // If all locks were open but have been reset, notify gameController. 
                if (allLocksOpen) 
                {
                    // Update gameObject animator if locks reset, but they were open
                    _animator.SetBool("Active",false);

                    // Notify game controller
                    _gameController.DestinationTrigger(false);

                    // Update the level progress panel text - this states "<SATNAME>: Connection Lost"
                    _communicationsPanel.UpdateSuccessText();

                    // Set all locks open to false
                    allLocksOpen = false;
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
                if (_numUnlocks <= _numLocksForLevelCompletion)
                {
                    if (!_uiController.advancedSettings.overwriteCommunicationMovement) _uiController.ToggleVisibleCommunicationsIfClosed();
                    _communicationsPanel.LogCommunications(_satelliteName,_numUnlocks);
                }

                // if the number of unlocks is more or equal to the number of locks being used
                if (_numUnlocks >= _numLocksForLevelCompletion)
                {
                    // Update gameObject animator if all locks open
                    _animator.SetBool("Active",true);

                    // By placing this here, it means it will only run once - preventing one destination from being marked as more than one
                    if (!allLocksOpen)
                    {
                        // Notify Game Controller that a destination has become active
                        _gameController.DestinationTrigger(true);

                        if (!_uiController.advancedSettings.overwriteCommunicationMovement) _uiController.ToggleVisibleCommunicationsIfClosed();
                        // Update text to show success
                        _communicationsPanel.UpdateSuccessText();
                    }

                    // Set all locks open to false and notify gamecontroller of this update
                    allLocksOpen = true;

                    // Enforces a limit of number of unlocks - prevents ever increasing lock unlocks
                    _numUnlocks = _numLocksForLevelCompletion + 1;
                }
            }

            // Reset update counter
            _updateCounter = 0;
        }
        else _updateCounter += 1;
    }

    /// <summary>
    /// Method called when a laser interacts with the destination
    /// </summary>
    /// <param name="laserColour"></param>
    /// <param name="transparency"></param>
    public void AdvanceLock(LaserColour laserColour, float transparency)
    {
        // Call from Laser interaction to advance the lock
        _lockAdvanceRequest = true;

        // If the tranparency is not the same as the previous (laser returns to the destination after being interrupted)
        if (transparency != _previousTransparency)
        {
            _newTransparencySeen = true;
            _previousTransparency = transparency;
        }

        // Check if the transparency is accepted
        if (transparency >= _neededTransparency) _transparencyAccepted = true;

        // Check if the colour has changed - will re-notify the communications if a new colour
        // prevents repeated messages when attempting to get the laser to the right place on the satellite
        if (laserColour != _previousLaserColour)
        {
            _newColourSeen = true;
            _previousLaserColour = laserColour;
        }
        // Check if the colour is accepted
        if (neededLaserColour == LaserColour.White) _colourAccepted = true;
        else if (neededLaserColour == laserColour) _colourAccepted = true;
    }


}
