using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum UIPanel
{
    Satellite_Controls,
    Satellite_Info_UI,
    Shop,
    Teaching,
    LogCommunications,
    ConfirmAction,
    Settings,
    LevelComplete
}

public class UIController : MonoBehaviour
{

    public UIExpectations uiExpectations;

    public SatelliteControlPanelSettings satelliteControlPanelSettings;

    private GameObject _satelliteControlPanel;
    private bool _satelliteControlPanelMoving = false;
    private float _satelliteControlPanelNewYLoc;

    private GameObject _satelliteInfoUIPanel;
    private GameObject[] _satelliteInfoUIObjects;


    public ShopPanelSettings shopPanelSettings;
    private bool _shopPanelIsOpen = false;
    private bool _shopPanelMoving = false;
    private float _shopPanelNewXLoc;
    private GameObject _shopPanel;
    private TMP_Text _shopBudgetText;
    private int _knownBudget;


    public LevelProgressPanelSettings levelProgressPanelSettings;
    private bool _levelProgressMoving = false;
    private float _levelProgressNewYLoc;
    private GameObject _levelProgressPanel;


    public ConfirmationPanelSettings confirmationPanelSettings;
    private ConfirmationPanel _confirmationPanel;



    private GameObject _settingsPanel;
    private bool _settingsPanelVisible = false;


    private bool _userWantsToLeaveLevel = false;
    private bool _interactionEnabled = true;

    private Language _language;


    [HideInInspector]
    public Satellite_Info selectedSatelliteInfo;

    private GameController _gameController;

    private GameObject _teachingControllerObject;

    private GameObject _levelCompletePanel;

    // Start is called before the first frame update
    void Start()
    {

        // Find and connect to gameController
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        _gameController.SetUIController(this);

        // If expecting satellite communication panel (displays level progress)
        if (uiExpectations.expectSatelliteCommsLevelProgressPanel )_levelProgressPanel = GameObject.FindGameObjectsWithTag("LevelProgressPanel")[0];
        
        // if expecting satellite control and info panel
        if (uiExpectations.expectSatelliteControlsAndInfoPanels)
        {
            // Find and save connection to Satellite Control Panel UI Parent
            _satelliteControlPanel = GameObject.FindGameObjectsWithTag("Satellite_Controls")[0];

            _satelliteInfoUIPanel = GameObject.FindGameObjectsWithTag("UI_Satellite_Info")[0];
            _satelliteInfoUIObjects = GameObject.FindGameObjectsWithTag("UI_Satellite_Info_Obj");
        }

        // If expecting shop UI, collect relevant inforamation
        if (uiExpectations.expectShopUIPanel)
        {
            _shopPanel = GameObject.FindGameObjectsWithTag("Shop")[0];

            var shopInformationText = GameObject.FindGameObjectsWithTag("Shop_Information_Text");

            foreach (GameObject shopInfoTextObject in shopInformationText)
            {
                var textComponent = shopInfoTextObject.GetComponent<TMP_Text>();

                if (shopInfoTextObject.name == "LevelName") textComponent.text = _gameController.levelName;
                else if (shopInfoTextObject.name == "LevelDescription") textComponent.text = _gameController.levelDescription;
                else if (shopInfoTextObject.name == "CurrentBudget")
                {
                    textComponent.text = "Current Budget: £"+_gameController.startingBudget;
                    _knownBudget = _gameController.startingBudget;
                    _shopBudgetText = textComponent;
                }

            }

        }

        // If expecting level complete panel, retrieve information
        if (uiExpectations.expectLevelCompletePanel) _levelCompletePanel = GameObject.FindGameObjectsWithTag("LevelCompleteController")[0];

        // If expecting teaching UI Panel, then retrieve needed information
        if (uiExpectations.expectTeachingUIPanel)
        {
            var teachingControllerObjects = GameObject.FindGameObjectsWithTag("TeachingController");

            if (teachingControllerObjects.Length != 0)
            {
                _teachingControllerObject = GameObject.FindGameObjectsWithTag("TeachingController")[0];
                _teachingControllerObject.GetComponent<TeachingController>().SetUIController(this);
                PresentPanel(UIPanel.Teaching,true);
            }
        }

        if (uiExpectations.expectSettingsPanel)
        {
            _settingsPanel = GameObject.FindGameObjectsWithTag("SettingsPanel")[0];
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {

        // If the object is not null and the known budget is not what should be
        if (uiExpectations.expectShopUIPanel && _shopBudgetText != null && _knownBudget != _gameController.currentBudget)
        {
            // Retrieve the budget, update the shop budget text.
            _shopBudgetText.text = "Current Budget: £"+_gameController.currentBudget;
            _knownBudget = _gameController.currentBudget;
        }


        // If moving satellite control panel, and the new location is not null
        if (uiExpectations.expectSatelliteControlsAndInfoPanels && _satelliteControlPanelMoving && _satelliteControlPanelNewYLoc != null)
        {
            // Retrieve transform and current position of the panel
            var rectTransform = _satelliteControlPanel.GetComponent<RectTransform>();
            var currentPosition = rectTransform.anchoredPosition;

            // Call move panel, moving to the wanted location
            MovePanel(rectTransform,currentPosition,currentPosition.x, _satelliteControlPanelNewYLoc,satelliteControlPanelSettings.controlPanelMovementSpeed);

            if (currentPosition.y == _satelliteControlPanelNewYLoc) _satelliteControlPanelMoving = false;

            // If it has stopped moving and in the close location - set active to false, this the rendering of unseen panels
            if (!_satelliteControlPanelMoving && (_satelliteControlPanelNewYLoc < satelliteControlPanelSettings.controlPanelVisibleLoc)) _satelliteControlPanel.active = false;
        }

        // If the shop panel is moving and has a defined location
        if (uiExpectations.expectShopUIPanel && _shopPanelMoving && _shopPanelNewXLoc != null)
        {
            // Retrieve the transform and current position of the panel
            var rectTransform = _shopPanel.GetComponent<RectTransform>();
            var currentPosition = rectTransform.anchoredPosition;

            // Move to wantted location
            MovePanel(rectTransform,currentPosition,_shopPanelNewXLoc,currentPosition.y,shopPanelSettings.shopPanelMovementSpeed);

            // Retrieve animator
            var shopAnimator = _shopPanel.GetComponent<Animator>();

            //Update animator based on whether in the closed or open position
            shopAnimator.SetBool("Open",(_shopPanelNewXLoc < shopPanelSettings.shopPanelCloseXLoc));

            //if (currentPosition.x == _shopPanelNewXLoc) _shopPanelMoving = false;
        }
        
        if (uiExpectations.expectSatelliteCommsLevelProgressPanel && _levelProgressMoving && _levelProgressNewYLoc != null)
        {
            // Retrieve transform and current position
            var rectTransform = _levelProgressPanel.GetComponent<RectTransform>();
            var currentPosition = rectTransform.anchoredPosition;

            // Move to wanted location
            MovePanel(rectTransform,currentPosition, currentPosition.x, _levelProgressNewYLoc,levelProgressPanelSettings.levelProgressPanelMovementSpeed);

            if (currentPosition.y == _levelProgressNewYLoc) _levelProgressMoving = false;
        }
        
        
        if (Input.GetButtonDown("Cancel"))
        {   
            if (!_settingsPanelVisible) PresentPanel(UIPanel.Settings,true);

            _settingsPanelVisible = !_settingsPanelVisible;
            
        }

    }

    public GameController GetGameController()
    {
        return _gameController;
    }

    public void UpdateLanguage(Language newLanguage)
    {
        _language = newLanguage;
    }

    private void MovePanel(RectTransform rectTransform, Vector3 currentPosition,float newXLoc,float newYLoc,float moveSpeed)
    {

        // Check Y positions, move if needed
        if (currentPosition.y < newYLoc)
        {
            // Move towards destination based on moveSpeed
            rectTransform.anchoredPosition = new Vector2(currentPosition.x, currentPosition.y + moveSpeed);

            // If overshot location in this move, then set to the position
            if (currentPosition.y + moveSpeed > newYLoc) rectTransform.anchoredPosition = new Vector2(currentPosition.x,newYLoc);
            
        }

        else if (currentPosition.y > newYLoc)
        {
            // Move towards destination based on moveSpeed
            rectTransform.anchoredPosition = new Vector2(currentPosition.x, currentPosition.y - moveSpeed);

            // If overshot location in this move, then set to the position
            if (currentPosition.y - moveSpeed < newYLoc) rectTransform.anchoredPosition = new Vector2(currentPosition.x,newYLoc);
            
        }
        
        
        // Check X positions, move if needed
        if (currentPosition.x < newXLoc)
        {
            // Move towards destination based on moveSpeed
            rectTransform.anchoredPosition = new Vector2(currentPosition.x + moveSpeed, currentPosition.y);

            // If it overextends then move it to the exact location needed
            if (currentPosition.x - moveSpeed > newXLoc) rectTransform.anchoredPosition = new Vector2(newXLoc,currentPosition.y);
            
        }

        else if (currentPosition.x > newXLoc)
        {
            // Move towards destination based on moveSpeed
            rectTransform.anchoredPosition = new Vector2(currentPosition.x - moveSpeed, currentPosition.y);

            // If it overextends then move it to the exact location needed
            if (currentPosition.x - moveSpeed < newXLoc) rectTransform.anchoredPosition = new Vector2(newXLoc,currentPosition.y);
            
        }

        
    }



    public void PresentPanel(UIPanel panel, bool bVisible)
    {
        // If the presentation of a panel is the satellite control AND it's not already being moved
        // This is to prevent potential errors when a user spam clicks a specific satellite.
        if (uiExpectations.expectSatelliteControlsAndInfoPanels && panel == UIPanel.Satellite_Controls)
        {
            // Update moving to true, meaning update will now move the satellite.
            _satelliteControlPanelMoving = true;

            // If the satellite is not on screen, move it onto the screen, else move it off screen
            // It is hard coded, in the update, when moved offscreen it will be disabled.

            var position = _satelliteControlPanel.GetComponent<RectTransform>().anchoredPosition;

            if (bVisible && position.y < satelliteControlPanelSettings.controlPanelVisibleLoc) 
            {
                _satelliteControlPanelNewYLoc = satelliteControlPanelSettings.controlPanelVisibleLoc;
                _satelliteControlPanel.active = true;
            }

            else if (!bVisible) _satelliteControlPanelNewYLoc = satelliteControlPanelSettings.controlPanelNotVisibleLoc;
            else _satelliteControlPanelMoving = false;
        }

        else if (uiExpectations.expectSatelliteControlsAndInfoPanels && panel == UIPanel.Satellite_Info_UI)
        {

            var rectTransform = _satelliteInfoUIPanel.GetComponent<RectTransform>();
            var position = rectTransform.anchoredPosition;

            if (bVisible)
            {
                rectTransform.anchoredPosition = new Vector2(position.x, -408);

                _satelliteInfoUIPanel.active = true;
                foreach (GameObject obj in _satelliteInfoUIObjects)
                {
                    // Type found by Stephan_B, taken from https://discussions.unity.com/t/access-textmeshpro-text-through-script/699157 
                    // then used to get the TextMeshPro Text component of the game object.
                    var textComponent = obj.GetComponent<TMP_Text >();

                    if (textComponent != null && selectedSatelliteInfo != null){
                        textComponent.text = "";

                        if (obj.name == "SatelliteName") textComponent.text = selectedSatelliteInfo.satelliteName;
                        else if (obj.name == "SatelliteDescription") textComponent.text = selectedSatelliteInfo.satelliteDescription;
                        else if (obj.name == "SellText")
                        {
                            if (selectedSatelliteInfo.canBeSold) textComponent.text = $"Sell £{selectedSatelliteInfo.satelliteSellPrice}";
                            else textComponent.text = "Not for sale";
                        }

                        else if (obj.name == "LightColour") 
                        {
                            if (selectedSatelliteInfo.satelliteType == SatelliteType.Origin) textComponent.text = $"Creates {selectedSatelliteInfo.advanced_Satellite_Info.lightColor.ToString()} Laser"; 
                            
                            else if (selectedSatelliteInfo.satelliteType == SatelliteType.Destination) textComponent.text = $"Needs {selectedSatelliteInfo.advanced_Satellite_Info.lightColor.ToString()} Laser"; 
                            else textComponent.text = "";
                        }

                        else if (obj.name == "SatelliteStatus")
                        {
                            if (selectedSatelliteInfo.satelliteType == SatelliteType.Destination && selectedSatelliteInfo.advanced_Satellite_Info.IsSatelliteReceivedCorrectLaser) textComponent.text = "Status: Active";
                            else if (selectedSatelliteInfo.satelliteType == SatelliteType.Destination) textComponent.text = "Status: Inactive";
                            else textComponent.text = "";
                        }

                    }

                    obj.active = true;

                    

                }
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(position.x, -746);

                _satelliteInfoUIPanel.active = false;

                foreach (GameObject obj in _satelliteInfoUIObjects)
                {
                    obj.active = false;
                }
            }

        }
        
        else if (uiExpectations.expectSettingsPanel && panel == UIPanel.Settings)
        {
            
            var rectTransform = _settingsPanel.GetComponent<RectTransform>();

            if (rectTransform.anchoredPosition != new Vector2(0,0)) rectTransform.anchoredPosition = new Vector2(0,0);

            _settingsPanel.active = bVisible;

        }

        else if (uiExpectations.expectTeachingUIPanel && (panel == UIPanel.Teaching) &&  (_teachingControllerObject != null))
        {
            if (bVisible)
            {
                // This is simply for Developer view - meaning the developer can move the UI panel itself elsewhere to test components when editing
                var teachingTransform = _teachingControllerObject.GetComponent<RectTransform>();
                // Moves level complete to 0,0 (of the UI) if not already there - in cases where the dev has moved it for testing purposes.
                if (teachingTransform.anchoredPosition != new Vector2(0,0)) teachingTransform.anchoredPosition = new Vector2(0,0);
            }

            // Sets whether active or not based on visbility wanted.
            _teachingControllerObject.active = bVisible;
        }

        else if (uiExpectations.expectSatelliteCommsLevelProgressPanel && panel == UIPanel.LogCommunications && _levelProgressPanel != null)
        {
            _levelProgressMoving = true;

            var position = _levelProgressPanel.GetComponent<RectTransform>().anchoredPosition;

            if (bVisible && position.y > levelProgressPanelSettings.levelProgressPanelOpenYLoc) _levelProgressNewYLoc = levelProgressPanelSettings.levelProgressPanelOpenYLoc;
            
            else if (!bVisible ) _levelProgressNewYLoc = levelProgressPanelSettings.levelProgressPanelCloseYLoc;
            
            else _levelProgressMoving = false;
            
        }

        else if (uiExpectations.expectLevelCompletePanel && panel == UIPanel.LevelComplete)
        {

            // Retrieve levelCompletePanel
            _levelCompletePanel = GameObject.FindGameObjectsWithTag("LevelCompleteController")[0];

            // This is simply for Developer view - meaning the developer can move the UI panel itself elsewhere to test components when editing
            var levelCompletetransform = _levelCompletePanel.GetComponent<RectTransform>();
            // Moves level complete to 0,0 (of the UI) if not already there - in cases where the dev has moved it for testing purposes.
            if (levelCompletetransform.anchoredPosition != new Vector2(0,0)) levelCompletetransform.anchoredPosition = new Vector2(0,0);

            // Sets whether active or not based on visbility wanted.
            _levelCompletePanel.active = bVisible;
        
        }

        else if (uiExpectations.expectConfirmationPanel && panel == UIPanel.ConfirmAction)
        {
            // If expecting the confirmation panel, then retrieve the needed information
            if (_confirmationPanel == null)
            {
                _confirmationPanel = GameObject.FindGameObjectsWithTag("ConfirmationPanel")[0].GetComponent<ConfirmationPanel>();
            }

            var rectTransform = _confirmationPanel.gameObject.GetComponent<RectTransform>();

            if (rectTransform.anchoredPosition != new Vector2(0,0)) rectTransform.anchoredPosition = new Vector2(0,0);

            _confirmationPanel.gameObject.active = bVisible;
        }
    }
    

    public void OpenCloseShop()
    {
        // This is not present with in the PresentPanel function in order for it to be accessible to buttons
        // Buttons can call functions when clicked, with the condition the function does not have arguments

        // If the shop is open close it, if it's closed, open it.
        // This makes use of a manual linear interpolation to show and close it - as RectTransform doesn't support linear interpolated movement
        // This is done mainly for animation purposes.

        if (uiExpectations.expectShopUIPanel && _interactionEnabled)
        {
            if (_shopPanelIsOpen)
            {
                _shopPanelIsOpen = false;
                _shopPanelMoving = true;
                _shopPanelNewXLoc = shopPanelSettings.shopPanelCloseXLoc;
            }
            else
            {
                _shopPanelIsOpen = true;
                _shopPanelMoving = true;
                _shopPanelNewXLoc = shopPanelSettings.shopPanelOpenXLoc;
            }
        }
    }

    public void SetCompletedModeActive(bool active)
    {
        if (uiExpectations.expectLevelCompletePanel)
        {
            // If the level is won, then close all panels, and present the LevelComplete panel
            if (active)
            {

                // Close the shop if it's open
                if (_shopPanelIsOpen) OpenCloseShop();

                // Disable further UI interaction - prevents user continuing play whilst Level is Completed.
                _interactionEnabled = false;

                // Notify LevelCompletePanel
                _levelCompletePanel.GetComponent<LevelCompleteController>().GameComplete();

                // Present the levelComplete panel
                PresentPanel(UIPanel.LevelComplete,true);
            }
            else 
            {
                // Enable / re-enable interactions
                _interactionEnabled = true;

                // Hide the level complete panel
                PresentPanel(UIPanel.LevelComplete,false);
            }
        }
    }

    public void OpenSettings()
    {
        PresentPanel(UIPanel.Settings,true);
    }

    public bool GetInteractionEnabled()
    {
        return _interactionEnabled;
    }

    public void ResetLevel()
    {
        PresentPanel(UIPanel.ConfirmAction,true);
        _confirmationPanel.UpdateUIComponents(ConfirmAction.ResetLevel);
    }

    public void LeaveLevel()
    {
        PresentPanel(UIPanel.ConfirmAction,true);
        _confirmationPanel.UpdateUIComponents(ConfirmAction.LeaveLevel);
    }


}




[System.Serializable]
public class UIExpectations
{
    public bool expectShopUIPanel = true;
    public bool expectTeachingUIPanel  = true;
    public bool expectSatelliteControlsAndInfoPanels = true;
    public bool expectLevelCompletePanel  = true;
    public bool expectSettingsPanel  = true;
    public bool expectSatelliteCommsLevelProgressPanel  = true;
    public bool expectConfirmationPanel = true;
}

[System.Serializable]
public class SatelliteControlPanelSettings
{
    public float controlPanelMovementSpeed = 10f;
    public float controlPanelVisibleLoc = -465f;
    public float controlPanelNotVisibleLoc = -650f;
}

[System.Serializable]
public class ShopPanelSettings
{
    public float shopPanelMovementSpeed = 10f;
    public float shopPanelOpenXLoc = 596f;
    public float shopPanelCloseXLoc = 1147f;
}

[System.Serializable]
public class LevelProgressPanelSettings
{
    public int recordRetentionNumber = 10;
    public bool forceNoChangeOnNewCommunication = false;

    public float levelProgressPanelMovementSpeed = 10f;

    public float levelProgressPanelOpenYLoc = 376.7f;
    public float levelProgressPanelCloseYLoc = 646.0f;
}

[System.Serializable]
public class ConfirmationPanelSettings
{
    public bool forceSkipConfirmation = false;
    public float confirmationPanelXLoc = 0;
    public float confirmationPanelYLoc = 0;
}