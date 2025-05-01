using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Enum for fixed panels, and fixed and moving panels are interacted differently
/// </summary>
public enum FixedUIPanel
{
    Teaching,
    ConfirmAction,
    Settings,
    LevelComplete,
    Athenaeum 
}

public class UIController : MonoBehaviour
{
    /// <summary>
    /// UI Controller class used to move and update UI elements
    /// </summary>
    

    // Base movement speed of panels
    public float panelMovementSpeed = 10f;

    // Advanced settings (See dedicated class for more information)
    public AdvancedSettings advancedSettings;

    // UI Expectatations (See dedicated class for more information)
    public UIExpectations_ uiExpectations;

    // Moving Panel settings (See dedicated class for more information)
    public MovingPanelSettings movingPanelSettings;

    // Fixed Panel settings (See dedicated class for more information)
    public FixedPanelSettings fixedPanelSettings;


    private GameController _gameController;
    private Language _language = Language.English;

    // MovingPanels and FixedPanels store visibility and links to relevant panels.
    private MovingPanels _movingPanels = new MovingPanels();
    private FixedPanels _fixedPanels = new FixedPanels();


    
    /// <summary>
    /// Initialisation Method
    /// </summary>
    void Start() 
    {
        // Retrieve language from persistence controller
        _language = PersistenceController.GetLanguage();

        // Find, link to and notify game controller
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        _gameController.SetUIController(this);


        // Moving Panels have an open and close location, Fixed panels have one location.

        // Retrieve all UI Panels that are expected to exist within the scene
        if (uiExpectations.expectLevelInformationPanel)
        {
            _movingPanels.levelInformationPanel = GameObject.FindGameObjectsWithTag("LevelInformation")[0].GetComponent<LevelInformationPanel>();
        }

        if (uiExpectations.expectCommunicationPanel)
        {
            _movingPanels.communicationsPanel = GameObject.FindGameObjectsWithTag("CommunicationsPanel")[0].GetComponent<CommunicationsPanel>();
        }
        
        if (uiExpectations.expectSatelliteControlPanel) 
        {
            _movingPanels.satelliteControlsPanel = GameObject.FindGameObjectsWithTag("SatelliteControlsPanel")[0];
        }

        if (uiExpectations.expectSatelliteInformationPanel)
        {
            _movingPanels.satelliteInformationPanel = GameObject.FindGameObjectsWithTag("SatelliteInformationPanel")[0].GetComponent<SatelliteInformationPanel>();
        }

        if (uiExpectations.expectShopPanel)
        {
            _movingPanels.shopPanel = GameObject.FindGameObjectsWithTag("ShopPanel")[0].GetComponent<ShopPanel>();
        } 

        if (uiExpectations.expectSettingsPanel) 
        {
            _fixedPanels.settingsPanel = GameObject.FindGameObjectsWithTag("Settings")[0].GetComponent<SettingsPanel>();
        }

        if (uiExpectations.expectConfirmationPanel)
        {
            _fixedPanels.confirmationPanel = GameObject.FindGameObjectsWithTag("ConfirmationPanel")[0].GetComponent<ConfirmationPanel>(); 
        }

        if (uiExpectations.expectLevelCompletePanel) 
        {
            _fixedPanels.levelCompletePanel = GameObject.FindGameObjectsWithTag("LevelCompleteController")[0].GetComponent<LevelCompletePanel>();
        } 


        if (uiExpectations.expectAthenaeum) _fixedPanels.athenaeum = GameObject.FindGameObjectsWithTag("Athenaeum")[0];

        // Collect all panel objects for fixed panels.
        if (uiExpectations.expectTeachingPanel)
        {
            _fixedPanels.teachingPanel = GameObject.FindGameObjectsWithTag("TeachingPanel")[0].GetComponent<TeachingPanel>();
            PresentFixedPanel(FixedUIPanel.Teaching,true);
        }
    }


    /// <summary>
    /// Method called once per system tick. 1 per second for 60fps
    /// </summary>
    void Update()
    {
        // If expecting the level information panel, and it is marked as moving, then move the panel
        if (uiExpectations.expectLevelInformationPanel && _movingPanels.levelInformationPanel != null && _movingPanels.levelInformationMoving)
        {
            // Retrieve transformation and position
            var rectTransform = _movingPanels.levelInformationPanel.GetComponent<RectTransform>();
            var currentPosition = rectTransform.anchoredPosition;

            // Call MovePanel
            MovePanel(rectTransform,currentPosition,_movingPanels.levelInformationMoveTo);

            // If at the wanted location, set moving to false
            if (rectTransform.anchoredPosition == _movingPanels.communicationsMoveTo) _movingPanels.communicationPanelMoving = false;
        }

        // If Expecting communciations panel and it is marked as moving, move the panel
        if (uiExpectations.expectCommunicationPanel && _movingPanels.communicationsPanel != null && _movingPanels.communicationPanelMoving)
        {
            // Retrieve the communciations panel transform and location
            var rectTransform = _movingPanels.communicationsPanel.GetComponent<RectTransform>();
            var currentPosition = rectTransform.anchoredPosition;

            // Call MovePanel to move it
            MovePanel(rectTransform,currentPosition,_movingPanels.communicationsMoveTo);

            // If at end destination, set moving to false
            if (rectTransform.anchoredPosition == _movingPanels.communicationsMoveTo) _movingPanels.communicationPanelMoving = false;
        }

        // If expecting the satellite control panel and it is marked as moving, move the panel
        if (uiExpectations.expectSatelliteControlPanel && _movingPanels.satelliteControlsPanel != null && _movingPanels.satelliteControlsMoving)
        {
            // Retrieve the transform and position of the satellite controls panel
            var rectTransform = _movingPanels.satelliteControlsPanel.GetComponent<RectTransform>();
            var currentPosition = rectTransform.anchoredPosition;

            // Move the panel
            MovePanel(rectTransform,currentPosition,_movingPanels.satelliteControlsMoveTo);

            // If it has reached it's destination, set moving to false
            if (rectTransform.anchoredPosition == _movingPanels.satelliteControlsMoveTo) _movingPanels.satelliteControlsMoving = false;

        }

        // If expecting the satellite information panel, and it is marked as moving, move the panel
        if (uiExpectations.expectSatelliteInformationPanel && _movingPanels.levelInformationPanel != null && _movingPanels.satelliteInformationMoving)
        {
            // Retrieve transform and position
            var rectTransform = _movingPanels.satelliteInformationPanel.GetComponent<RectTransform>();
            var currentPosition = rectTransform.anchoredPosition;

            // Move the panel
            MovePanel(rectTransform,currentPosition,_movingPanels.satelliteInformationMoveTo);

            // If at destination, set moving to false
            if (rectTransform.anchoredPosition == _movingPanels.satelliteInformationMoveTo) _movingPanels.satelliteInformationMoving = false;
        }

        // If expecting the shop panel, and it is moving, move the panel
        if (uiExpectations.expectShopPanel && _movingPanels.shopPanel != null && _movingPanels.shopPanelMoving)
        {
            // Retrieve the transform and position of the panel
            var rectTransform = _movingPanels.shopPanel.GetComponent<RectTransform>();
            var currentPosition = rectTransform.anchoredPosition;

            // Move the panel
            MovePanel(rectTransform,currentPosition,_movingPanels.shopMoveTo);

            // If at the wanted destination, set moving to false
            if (rectTransform.anchoredPosition == _movingPanels.shopMoveTo) _movingPanels.shopPanelMoving = false;
            
        }
        
    }

    /// <summary>
    /// Method for UI element scripts to retrieve the game controller
    /// Removes 'unneccesarry' tag searches to find the game controller
    /// </summary>
    /// <returns></returns>
    public GameController GetGameController()
    {
        return _gameController;
    }


    /// <summary>
    /// Method to move a fixed panel
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="bVisible"></param>
    public void PresentFixedPanel(FixedUIPanel panel, bool bVisible)
    {
        // Fixed Panels have a single location and instead work by handling their 'active' state
        // As such they only appear in one location or not appear in said location. They don't move.


        
        if (panel == FixedUIPanel.Settings)
        {
            // Retrieve the settings transform
            var rectTransform = _fixedPanels.settingsPanel.GetComponent<RectTransform>();

            // As the settings panel covers the full screen, set position to 0,0. May not already be at 0,0 to assist developers
            if (rectTransform.anchoredPosition != fixedPanelSettings.settingsPanelLocation) rectTransform.anchoredPosition = fixedPanelSettings.settingsPanelLocation;

            // Set active to it's visibility. Due to link, this becomes toggleable
            // If the panel is intially set to false (from developer) this panel cannot be retrieved.
            _fixedPanels.settingsPanel.gameObject.active = bVisible;

            // Reset the settings panel
            _fixedPanels.settingsPanel.Reset();
        }

        else if (panel == FixedUIPanel.ConfirmAction)
        {
            // Retrieve transform
            var rectTransform = _fixedPanels.confirmationPanel.GetComponent<RectTransform>();

            // As the confirmation screen takes the entire screen space, set location to 0,0. May not be 0,0 for developer assistance
            if (rectTransform.anchoredPosition != fixedPanelSettings.confirmationPanelLocation) rectTransform.anchoredPosition = fixedPanelSettings.confirmationPanelLocation;
            
            // Set visiblity to choice
            // If the panel is intially set to false (from developer) this panel cannot be retrieved.
            _fixedPanels.confirmationPanel.gameObject.active = bVisible;
        }

        else if (panel == FixedUIPanel.Teaching)
        {
            // retrieve transform
            var rectTransform = _fixedPanels.teachingPanel.GetComponent<RectTransform>();
            
            // Set the position to be 0,0 as the teaching panel covers the entire screen. May not be 0,0 intially, for developer assistance
            if (rectTransform.anchoredPosition != fixedPanelSettings.teachingPanelLocation) rectTransform.anchoredPosition = fixedPanelSettings.teachingPanelLocation;
            
            // If the panel is intially set to false (from developer) this panel cannot be retrieved.
            _fixedPanels.teachingPanel.gameObject.active = bVisible;
        }


        else if (panel == FixedUIPanel.LevelComplete)
        {
            // Retrieve transform
            var rectTransform = _fixedPanels.levelCompletePanel.GetComponent<RectTransform>();

            // Set position to be 0,0 as the level end panel takes up the entire screen
            if (rectTransform.anchoredPosition != fixedPanelSettings.levelCompeletePanelLocation) rectTransform.anchoredPosition = fixedPanelSettings.levelCompeletePanelLocation;

            // Set game object to active or inactive based on request
            // If the panel is intially set to false (from developer) this panel cannot be retrieved.
            _fixedPanels.levelCompletePanel.gameObject.active = bVisible;

            // As the level has been completed. Close all open moving UI elements if they are open
            if (uiExpectations.expectLevelInformationPanel && _movingPanels.levelInformationVisible) ToggleVisibleLevelInfomation();
            if (uiExpectations.expectCommunicationPanel && _movingPanels.communicationPanelVisible) ToggleVisibleCommunications();
            if (uiExpectations.expectSatelliteControlPanel && _movingPanels.satelliteControlsVisible) ToggleVisibleSatelliteControls();
            if (uiExpectations.expectSatelliteInformationPanel && _movingPanels.satelliteInformationVisible) ToggleVisibleSatelliteInformation();
            if (uiExpectations.expectShopPanel && _movingPanels.shopPanelVisible) ToggleVisibleShop();

            // Notify panel that the game is complete
            _fixedPanels.levelCompletePanel.GameComplete();

        }

        else if (panel == FixedUIPanel.Athenaeum )
        {
            // Retrieve transform
            var rectTransform = _fixedPanels.athenaeum.GetComponent<RectTransform>();


            // Set position to 0,0 as this panels fills the screen. May not be at 0,0 for developer assistance
            if (rectTransform.anchoredPosition != fixedPanelSettings.teachingPanelLocation) rectTransform.anchoredPosition = fixedPanelSettings.athenaeumLocation;
            
            // If the panel is intially set to false (from developer) this panel cannot be retrieved.
            _fixedPanels.athenaeum.active = bVisible;
            _fixedPanels.athenaeumVisible = bVisible;

            // Close panels if they are open
            if (_movingPanels.shopPanelVisible) ToggleVisibleShop();
            if (_movingPanels.levelInformationVisible) ToggleVisibleLevelInfomation();
            if (_movingPanels.communicationPanelVisible) ToggleVisibleCommunications();
        }

    }


    /// <summary>
    /// Method to move a moving panel
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="currentPosition"></param>
    /// <param name="newPosition"></param>
    private void MovePanel(RectTransform rectTransform,Vector3 currentPosition,Vector2 newPosition)
    {

        if (this.panelMovementSpeed <= 0) this.panelMovementSpeed = 1;

        // Check Y positions, move if needed
        if (currentPosition.y < newPosition.y)
        {
            // Move towards destination based on moveSpeed
            rectTransform.anchoredPosition = new Vector2(currentPosition.x, currentPosition.y + panelMovementSpeed);

            // If overshot location in this move, then set to the position
            if (currentPosition.y + panelMovementSpeed > newPosition.y) rectTransform.anchoredPosition = new Vector2(currentPosition.x,newPosition.y);
            
        }

        else if (currentPosition.y > newPosition.y)
        {
            // Move towards destination based on moveSpeed
            rectTransform.anchoredPosition = new Vector2(currentPosition.x, currentPosition.y - panelMovementSpeed);

            // If overshot location in this move, then set to the position
            if (currentPosition.y - panelMovementSpeed < newPosition.y) rectTransform.anchoredPosition = new Vector2(currentPosition.x,newPosition.y);
            
        }
        
        
        // Check X positions, move if needed
        if (currentPosition.x < newPosition.x)
        {
            // Move towards destination based on moveSpeed
            rectTransform.anchoredPosition = new Vector2(currentPosition.x + panelMovementSpeed, currentPosition.y);

            // If it overextends then move it to the exact location needed
            if (currentPosition.x - panelMovementSpeed > newPosition.x) rectTransform.anchoredPosition = new Vector2(newPosition.x,currentPosition.y);
            
        }

        else if (currentPosition.x > newPosition.x)
        {
            // Move towards destination based on moveSpeed
            rectTransform.anchoredPosition = new Vector2(currentPosition.x - panelMovementSpeed, currentPosition.y);

            // If it overextends then move it to the exact location needed
            if (currentPosition.x - panelMovementSpeed < newPosition.x) rectTransform.anchoredPosition = new Vector2(newPosition.x,currentPosition.y);
            
        }

    }



    /// <summary>
    /// Toggle Visibility for Level Information
    /// </summary>
    public void ToggleVisibleLevelInfomation()
    {

        // Set the opposite of current
        _movingPanels.levelInformationVisible = !_movingPanels.levelInformationVisible;

        // Set moving to true
        _movingPanels.levelInformationMoving = true;

        // Set move location based on whether now wanted open or closed
        if (_movingPanels.levelInformationVisible) _movingPanels.levelInformationMoveTo = movingPanelSettings.levelInformationOpenLocation;
        else _movingPanels.levelInformationMoveTo = movingPanelSettings.levelInformationCloseLocation;
    }

    /// <summary>
    /// Toggle visibility for communciations panel
    /// </summary>
    public void ToggleVisibleCommunications()
    {
        // Set the opposite of current
        _movingPanels.communicationPanelVisible = !_movingPanels.communicationPanelVisible;

        // Set moving to true
        _movingPanels.communicationPanelMoving = true;

        // Set move location based on whether now wanted open or closed
        if (_movingPanels.communicationPanelVisible) _movingPanels.communicationsMoveTo = movingPanelSettings.communicationsOpenLocation;
        else _movingPanels.communicationsMoveTo = movingPanelSettings.communicationsCloseLocation;
    }

    /// <summary>
    /// Toggle visible if closed for communications panel
    /// </summary>
    public void ToggleVisibleCommunicationsIfClosed()
    {
        // If closed, open it
        if (!_movingPanels.communicationPanelVisible && !advancedSettings.overwriteCommunicationMovement)
        {
            ToggleVisibleCommunications();
        }
    }

    /// <summary>
    /// Toggle visible for shop panel
    /// </summary>
    public void ToggleVisibleShop()
    {
        // Set the opposite of current
        _movingPanels.shopPanelVisible = !_movingPanels.shopPanelVisible;

        // Set moving to true
        _movingPanels.shopPanelMoving = true;

        // Set move location based on whether now wanted open or closed
        if (_movingPanels.shopPanelVisible) _movingPanels.shopMoveTo = movingPanelSettings.shopOpenLocation;
        else _movingPanels.shopMoveTo = movingPanelSettings.shopCloseLocation;
    }

    /// <summary>
    /// Toggle visible if closed for shop
    /// </summary>
    public void ToggleVisibleShopIfClosed()
    {
        // If the panel is not visible, make it so
        if (!_movingPanels.shopPanelVisible)
        {
            ToggleVisibleShop();
        }
    }

    /// <summary>
    /// Close shop if it is open
    /// </summary>
    /// <returns></returns>
    public bool CloseShopIfOpen()
    {
        if (_movingPanels.shopPanelVisible)
        {
            ToggleVisibleShop();
            return true;
        }
        return false;
    }


    /// <summary>
    /// Toggle visibility for satellite controls
    /// </summary>
    public void ToggleVisibleSatelliteControls()
    {
        // Set the opposite of current
        _movingPanels.satelliteControlsVisible = !_movingPanels.satelliteControlsVisible;

        // Set moving to true
        _movingPanels.satelliteControlsMoving = true;

        // Set move location based on whether now wanted open or closed
        if (_movingPanels.satelliteControlsVisible) _movingPanels.satelliteControlsMoveTo = movingPanelSettings.satelliteControlsOpenLocation;
        else _movingPanels.satelliteControlsMoveTo = movingPanelSettings.satelliteControlsCloseLocation;
    }

    /// <summary>
    /// Close satellite controls if they are open
    /// </summary>
    /// <returns></returns>
    public bool CloseSatelliteControlsIfOpen()
    {
        if (_movingPanels.satelliteControlsVisible)
        {
            ToggleVisibleSatelliteControls();
            return true;
        }
        return false;
    }

    
    /// <summary>
    /// Toggle visibility for satellite information
    /// </summary>
    public void ToggleVisibleSatelliteInformation()
    {
        // Set the opposite of current
        _movingPanels.satelliteInformationVisible = !_movingPanels.satelliteInformationVisible;

        // Set moving to true
        _movingPanels.satelliteInformationMoving = true;

        // Set move location based on whether now wanted open or closed
        if (_movingPanels.satelliteInformationVisible) _movingPanels.satelliteInformationMoveTo = movingPanelSettings.satelliteInfomationOpenLocation;
        else _movingPanels.satelliteInformationMoveTo = movingPanelSettings.satelliteInformationCloseLocation;
    }

    /// <summary>
    /// Toggle visiblity for anthenaeum panel
    /// </summary>
    public void ToggleAthenaeum()
    {
        PresentFixedPanel(FixedUIPanel.Athenaeum ,!_fixedPanels.athenaeumVisible);
    }

    /// <summary>
    /// Toggle visibility for settings panel
    /// </summary>
    public void ToggleSettingsVisible()
    {
        PresentFixedPanel(FixedUIPanel.Settings,true);
        _fixedPanels.settingsVisible = true;
        _fixedPanels.settingsPanel.gameObject.active = true;
    }


    /// <summary>
    /// Method to update the settings for the UI elements
    /// </summary>
    /// <param name="languageChanged"></param>
    public void UpdateSettings(bool languageChanged)
    {

        // Retrieve language
        _language = PersistenceController.GetLanguage();

        // update communication movement
        advancedSettings.overwriteCommunicationMovement = PersistenceController.GetForceNoChangeWhenReceivingCommunications();

        // If the language was changed
        if (languageChanged)
        {
            // If expecting the panel, notify it to update it's language
            if (uiExpectations.expectCommunicationPanel) _movingPanels.communicationsPanel.UpdateLanguage(_language);
            if (uiExpectations.expectLevelInformationPanel) _movingPanels.levelInformationPanel.UpdateLanguage(_language);
            if (uiExpectations.expectShopPanel) _movingPanels.shopPanel.UpdateLanguage(_language);
            if (uiExpectations.expectSatelliteInformationPanel) _movingPanels.satelliteInformationPanel.UpdateLanguage(_language);
            if (uiExpectations.expectConfirmationPanel) _fixedPanels.confirmationPanel.UpdateLanguage(_language);
            if (uiExpectations.expectLevelCompletePanel) _fixedPanels.levelCompletePanel.UpdateLanguage(_language);
            if (uiExpectations.expectTeachingPanel) _fixedPanels.teachingPanel.UpdateLanguage(_language);
            if (uiExpectations.expectAthenaeum) _fixedPanels.athenaeum.GetComponentInChildren<RecordSelection>().UpdateLanguage(_language);

            // If this level is LevelSelection
            if (_gameController.thisLevel == Level.LevelSelection)
            {
                // Find the level manager
                var levelSelectManager = GameObject.FindGameObjectsWithTag("LevelSelectManager")[0];

                // find the level buttons
                var levelButtons = levelSelectManager.GetComponentsInChildren<LevelSelectButton>();

                // Loop through and notify each button to update it's language
                foreach (LevelSelectButton levelSelectButton in levelButtons) levelSelectButton.UpdateLanguage();

            
                var foundLevelSelectText = false;
                var foundSettings = false;

                // Loop through all child text and update the text based on selected language
                var childTextList = levelSelectManager.gameObject.GetComponentsInChildren<TMP_Text>();
                foreach (TMP_Text childText in childTextList)
                {

                    if (childText.gameObject.name == "LevelSelectText" && _language == Language.English)
                    {
                        childText.text = "Level Select";
                        foundLevelSelectText = true;
                    }
                    else if (childText.gameObject.name == "LevelSelectText" && _language == Language.Welsh)
                    {
                        childText.text = "Dewis Lefel";
                        foundLevelSelectText = true;
                    }
                    
                    else if (childText.gameObject.name == "SettingsText" && _language == Language.English)
                    {
                        childText.text = "Settings";
                        foundSettings = true;
                    }
                    else if (childText.gameObject.name == "SettingsText" && _language == Language.Welsh)
                    {
                        childText.text = "Gosodiadau";
                        foundSettings = true;
                    }

                    // If the two text items are found, then break the loop. 
                    // Prevents extra loops for no reason
                    if (foundLevelSelectText && foundSettings)
                    {
                        break;
                    }
                }
            }

            // If this level is the Titlescreen
            if (_gameController.thisLevel == Level.Titlescreen)
            {
                // Find the titlescreen controller and notify to update language
                var titlescreenController = GameObject.FindGameObjectsWithTag("TitlescreenManager")[0].GetComponent<Titlescreen>();
                titlescreenController.UpdateLanguage(_language);
            }
        }
    }

    /// <summary>
    /// Method to announce that the level has ended
    /// </summary>
    public void LevelHasEnded()
    {
        // Present the fixed panel for level complete
        PresentFixedPanel(FixedUIPanel.LevelComplete,true);
    }
}


/// <summary>
/// Fixed Panel information, contains information related to the panel and it's current visibility
/// </summary>
public class FixedPanels
{
    public SettingsPanel settingsPanel;
    public bool settingsVisible = false;

    public ConfirmationPanel confirmationPanel;
    public bool confirmationVisible = false;

    public LevelCompletePanel levelCompletePanel;
    public bool levelCompelteVisible = false;

    public TeachingPanel teachingPanel;
    public bool teachingPanelVisible = true;

    public GameObject athenaeum ;
    public bool athenaeumVisible = false; 

}

/// <summary>
/// Moving panel information, contains information related to the panel, it's current visibility, whether it's currently moving and 
/// it's destination
/// </summary>
public class MovingPanels
{
    public LevelInformationPanel levelInformationPanel;
    public Vector2 levelInformationMoveTo;
    public bool levelInformationVisible = false;
    public bool levelInformationMoving = false;

    public ShopPanel shopPanel;
    public Vector2 shopMoveTo;
    public bool shopPanelVisible = false;
    public bool shopPanelMoving = false;


    public CommunicationsPanel communicationsPanel;
    public Vector2 communicationsMoveTo;
    public bool communicationPanelVisible = false;
    public bool communicationPanelMoving = false;

    public GameObject satelliteControlsPanel;
    public Vector2 satelliteControlsMoveTo;
    public bool satelliteControlsVisible = false;
    public bool satelliteControlsMoving = false;


    public SatelliteInformationPanel satelliteInformationPanel;
    public Vector2 satelliteInformationMoveTo;
    public bool satelliteInformationVisible = false;
    public bool satelliteInformationMoving = false;
}



/// <summary>
/// UI Expectations, used to determine which panels are expected to appear within the scene
/// </summary>
[System.Serializable]
public class UIExpectations_
{
    public bool expectLevelInformationPanel = true;
    public bool expectSettingsPanel = true;
    public bool expectShopPanel = false;
    public bool expectTeachingPanel = false;
    public bool expectSatelliteControlPanel = false;
    public bool expectSatelliteInformationPanel = false;
    public bool expectCommunicationPanel = false;
    public bool expectConfirmationPanel = false;
    public bool expectLevelCompletePanel = false;
    public bool expectAthenaeum   = false;
}


/// <summary>
/// MovingPanel Settings, contains settings related to moving panels, focuses on the open and close locations for each moving panel
/// </summary>
[System.Serializable]
public class MovingPanelSettings
{    
    public Vector2 levelInformationOpenLocation = new Vector2(659f,339f);
    public Vector2 levelInformationCloseLocation = new Vector2(1178.8f, 339f);

    public Vector2 shopOpenLocation = new Vector2(737f,-204f);
    public Vector2 shopCloseLocation = new Vector2(1096f, -204f);

    public Vector2 satelliteControlsOpenLocation = new Vector2(0f, -472.9f);
    public Vector2 satelliteControlsCloseLocation = new Vector2(0f,-611f);

    public Vector2 satelliteInfomationOpenLocation = new Vector2(-709.5f,-389.3f) ;
    public Vector2 satelliteInformationCloseLocation = new Vector2(-1139.2f,-389.3f);

    public Vector2 communicationsOpenLocation = new Vector2(-694.8f, 377.6f);
    public Vector2 communicationsCloseLocation =  new Vector2(-694.8f,643f);


}

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class FixedPanelSettings
{
    public Vector2 settingsPanelLocation = new Vector2(0,0);
    public Vector2 teachingPanelLocation = new Vector2(0,0);
    public Vector2 confirmationPanelLocation = new Vector2(0,0);
    public Vector2 levelCompeletePanelLocation = new Vector2(0,0);
    public Vector2  athenaeumLocation = new Vector2(0,0);

}

/// <summary>
/// Advanced settings information. Contains some information settings, and how many records to retain for communciations menu
/// </summary>
[System.Serializable]
public class AdvancedSettings
{
    public bool skipConfirmation = false;
    public bool overwriteCommunicationMovement = false;
    public int communicationRecordRetentionNumber = 10;
}