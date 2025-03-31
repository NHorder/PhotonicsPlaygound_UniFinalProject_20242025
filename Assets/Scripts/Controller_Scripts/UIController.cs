using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum FixedUIPanel
{
    Teaching,
    ConfirmAction,
    Settings,
    LevelComplete
}

public class UIController : MonoBehaviour
{
    public float panelMovementSpeed = 10f;

    public AdvancedSettings advancedSettings;

    public UIExpectations_ uiExpectations;

    public MovingPanelSettings movingPanelSettings;
    public FixedPanelSettings fixedPanelSettings;


    private GameController _gameController;
    private Language _language = Language.English;

    private MovingPanels _movingPanels = new MovingPanels();
    private FixedPanels _fixedPanels = new FixedPanels();


    //private bool 

    // Start is called before the first frame update
    void Start() 
    {
        _language = PersistenceController.GetLanguage();

        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        _gameController.SetUIController(this);


        // Moving Panels have an open and close location, Fixed panels have one location.

        // Collect all panel objects for moving panels.
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


        // Collect all panel objects for fixed panels.
        if (uiExpectations.expectTeachingPanel)
        {
            _fixedPanels.teachingPanel = GameObject.FindGameObjectsWithTag("TeachingPanel")[0].GetComponent<TeachingPanel>();
            PresentFixedPanel(FixedUIPanel.Teaching,true);
        }
    }


    // Update is called once per frame
    void Update()
    {

        if (uiExpectations.expectLevelInformationPanel && _movingPanels.levelInformationPanel != null && _movingPanels.levelInformationMoving)
        {
            var rectTransform = _movingPanels.levelInformationPanel.GetComponent<RectTransform>();
            var currentPosition = rectTransform.anchoredPosition;

            MovePanel(rectTransform,currentPosition,_movingPanels.levelInformationMoveTo);

            if (rectTransform.anchoredPosition == _movingPanels.communicationsMoveTo) _movingPanels.communicationPanelMoving = false;
        }

        if (uiExpectations.expectCommunicationPanel && _movingPanels.communicationsPanel != null && _movingPanels.communicationPanelMoving)
        {
            var rectTransform = _movingPanels.communicationsPanel.GetComponent<RectTransform>();
            var currentPosition = rectTransform.anchoredPosition;

            MovePanel(rectTransform,currentPosition,_movingPanels.communicationsMoveTo);

            if (rectTransform.anchoredPosition == _movingPanels.communicationsMoveTo) _movingPanels.communicationPanelMoving = false;
        }

        if (uiExpectations.expectSatelliteControlPanel && _movingPanels.satelliteControlsPanel != null && _movingPanels.satelliteControlsMoving)
        {
            var rectTransform = _movingPanels.satelliteControlsPanel.GetComponent<RectTransform>();
            var currentPosition = rectTransform.anchoredPosition;

            MovePanel(rectTransform,currentPosition,_movingPanels.satelliteControlsMoveTo);

            if (rectTransform.anchoredPosition == _movingPanels.satelliteControlsMoveTo) _movingPanels.satelliteControlsMoving = false;

        }


        if (uiExpectations.expectSatelliteInformationPanel && _movingPanels.levelInformationPanel != null && _movingPanels.satelliteInformationMoving)
        {
            
            var rectTransform = _movingPanels.satelliteInformationPanel.GetComponent<RectTransform>();
            var currentPosition = rectTransform.anchoredPosition;

            MovePanel(rectTransform,currentPosition,_movingPanels.satelliteInformationMoveTo);

            if (rectTransform.anchoredPosition == _movingPanels.satelliteInformationMoveTo) _movingPanels.satelliteInformationMoving = false;
        }


        if (uiExpectations.expectShopPanel && _movingPanels.shopPanel != null && _movingPanels.shopPanelMoving)
        {
            var rectTransform = _movingPanels.shopPanel.GetComponent<RectTransform>();
            var currentPosition = rectTransform.anchoredPosition;

            MovePanel(rectTransform,currentPosition,_movingPanels.shopMoveTo);

            if (rectTransform.anchoredPosition == _movingPanels.shopMoveTo)
            {
                _movingPanels.shopPanelMoving = false;
            }
        }
        
    }

    public GameController GetGameController()
    {
        return _gameController;
    }



    public void PresentFixedPanel(FixedUIPanel panel, bool bVisible)
    {
        // Fixed Panels have a single location and instead work by handling their 'active' state
        // As such they only appear in one location or not appear in said location. They don't move.

        if (panel == FixedUIPanel.Settings)
        {
            var rectTransform = _fixedPanels.settingsPanel.GetComponent<RectTransform>();
            
            if (rectTransform.anchoredPosition != fixedPanelSettings.settingsPanelLocation) rectTransform.anchoredPosition = fixedPanelSettings.settingsPanelLocation;
            
            _fixedPanels.settingsPanel.gameObject.active = bVisible;
            _fixedPanels.settingsPanel.Reset();
        }

        else if (panel == FixedUIPanel.ConfirmAction)
        {
            var rectTransform = _fixedPanels.confirmationPanel.GetComponent<RectTransform>();
            
            if (rectTransform.anchoredPosition != fixedPanelSettings.confirmationPanelLocation) rectTransform.anchoredPosition = fixedPanelSettings.confirmationPanelLocation;
            
            _fixedPanels.confirmationPanel.gameObject.active = bVisible;
        }

        else if (panel == FixedUIPanel.Teaching)
        {
            var rectTransform = _fixedPanels.teachingPanel.GetComponent<RectTransform>();
            
            if (rectTransform.anchoredPosition != fixedPanelSettings.teachingPanelLocation) rectTransform.anchoredPosition = fixedPanelSettings.teachingPanelLocation;
            
            _fixedPanels.teachingPanel.gameObject.active = bVisible;
        }


        else if (panel == FixedUIPanel.LevelComplete)
        {
            var rectTransform = _fixedPanels.levelCompletePanel.GetComponent<RectTransform>();
            
            if (rectTransform.anchoredPosition != fixedPanelSettings.levelCompeletePanelLocation) rectTransform.anchoredPosition = fixedPanelSettings.levelCompeletePanelLocation;
            
            _fixedPanels.levelCompletePanel.gameObject.active = bVisible;

            // As the level has been completed. Close all open moving UI elements
            if (uiExpectations.expectLevelInformationPanel && _movingPanels.levelInformationVisible) ToggleVisibleLevelInfomation();

            if (uiExpectations.expectCommunicationPanel && _movingPanels.communicationPanelVisible) ToggleVisibleCommunications();
            
            if (uiExpectations.expectSatelliteControlPanel && _movingPanels.satelliteControlsVisible) ToggleVisibleSatelliteControls();

            if (uiExpectations.expectSatelliteInformationPanel && _movingPanels.satelliteInformationVisible) ToggleVisibleSatelliteInformation();

            if (uiExpectations.expectShopPanel && _movingPanels.shopPanelVisible) ToggleVisibleShop();


            _fixedPanels.levelCompletePanel.GameComplete();

        }

    }



    private void MovePanel(RectTransform rectTransform,Vector3 currentPosition,Vector2 newPosition)
    {
        var moveSpeed = panelMovementSpeed;

        if (panelMovementSpeed <= 0) panelMovementSpeed = 1;

        // Check Y positions, move if needed
        if (currentPosition.y < newPosition.y)
        {
            // Move towards destination based on moveSpeed
            rectTransform.anchoredPosition = new Vector2(currentPosition.x, currentPosition.y + moveSpeed);

            // If overshot location in this move, then set to the position
            if (currentPosition.y + moveSpeed > newPosition.y) rectTransform.anchoredPosition = new Vector2(currentPosition.x,newPosition.y);
            
        }

        else if (currentPosition.y > newPosition.y)
        {
            // Move towards destination based on moveSpeed
            rectTransform.anchoredPosition = new Vector2(currentPosition.x, currentPosition.y - moveSpeed);

            // If overshot location in this move, then set to the position
            if (currentPosition.y - moveSpeed < newPosition.y) rectTransform.anchoredPosition = new Vector2(currentPosition.x,newPosition.y);
            
        }
        
        
        // Check X positions, move if needed
        if (currentPosition.x < newPosition.x)
        {
            // Move towards destination based on moveSpeed
            rectTransform.anchoredPosition = new Vector2(currentPosition.x + moveSpeed, currentPosition.y);

            // If it overextends then move it to the exact location needed
            if (currentPosition.x - moveSpeed > newPosition.x) rectTransform.anchoredPosition = new Vector2(newPosition.x,currentPosition.y);
            
        }

        else if (currentPosition.x > newPosition.x)
        {
            // Move towards destination based on moveSpeed
            rectTransform.anchoredPosition = new Vector2(currentPosition.x - moveSpeed, currentPosition.y);

            // If it overextends then move it to the exact location needed
            if (currentPosition.x - moveSpeed < newPosition.x) rectTransform.anchoredPosition = new Vector2(newPosition.x,currentPosition.y);
            
        }

    }




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

    public void ToggleVisibleCommunicationsIfClosed()
    {

        if (!_movingPanels.communicationPanelVisible && !advancedSettings.overwriteCommunicationMovement)
        {
            ToggleVisibleCommunications();
        }
    }

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

    public void ToggleVisibleShopIfClosed()
    {
        if (!_movingPanels.shopPanelVisible)
        {
            ToggleVisibleShop();
        }
    }

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

    public bool CloseSatelliteControlsIfOpen()
    {
        if (_movingPanels.satelliteControlsVisible)
        {
            ToggleVisibleSatelliteControls();
            return true;
        }
        return false;
    }

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



    public void UpdateSettings(bool languageChanged)
    {
        _language = PersistenceController.GetLanguage();

        advancedSettings.overwriteCommunicationMovement = PersistenceController.GetForceNoChangeWhenReceivingCommunications();

        if (languageChanged)
        {
            if (uiExpectations.expectCommunicationPanel) _movingPanels.communicationsPanel.UpdateLanguage(_language);
            if (uiExpectations.expectLevelInformationPanel) _movingPanels.levelInformationPanel.UpdateLanguage(_language);

            if (uiExpectations.expectShopPanel) _movingPanels.shopPanel.UpdateLanguage(_language);
            if (uiExpectations.expectSatelliteInformationPanel) _movingPanels.satelliteInformationPanel.UpdateLanguage(_language);

            if (uiExpectations.expectConfirmationPanel) _fixedPanels.confirmationPanel.UpdateLanguage(_language);
            if (uiExpectations.expectLevelCompletePanel) _fixedPanels.levelCompletePanel.UpdateLanguage(_language);

            //if (uiExpectations.expectTeachingPanel) _fixedPanels.teachingPanel.UpdateLanguage(_language);

            if (_gameController.thisLevel == Level.LevelSelection)
            {
                var levelSelectManager = GameObject.FindGameObjectsWithTag("LevelSelectManager")[0];

                var levelButtons = levelSelectManager.GetComponentsInChildren<LevelSelectButton>();

                foreach (LevelSelectButton levelSelectButton in levelButtons) levelSelectButton.UpdateLanguage();

                var childTextList = levelSelectManager.gameObject.GetComponentsInChildren<TMP_Text>();
                foreach (TMP_Text childText in childTextList)
                {

                    if (childText.gameObject.name == "LevelSelectText" && _language == Language.English)
                    {
                        childText.text = "Level Select";
                        break;
                    }

                    else if (childText.gameObject.name == "LevelSelectText" && _language == Language.Welsh)
                    {
                        childText.text = "Dewis Lefel";
                        break;
                    }
                }
            }
        
            if (_gameController.thisLevel == Level.Titlescreen)
            {
                var titlescreenController = GameObject.FindGameObjectsWithTag("TitlescreenManager")[0].GetComponent<Titlescreen>();
                titlescreenController.UpdateLanguage(_language);
            }
        }
    }

    public void LevelHasEnded()
    {
        PresentFixedPanel(FixedUIPanel.LevelComplete,true);
    }
}


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

}

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
}

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

[System.Serializable]
public class FixedPanelSettings
{
    public Vector2 settingsPanelLocation = new Vector2(0,0);
    public Vector2 teachingPanelLocation = new Vector2(0,0);
    public Vector2 confirmationPanelLocation = new Vector2(0,0);
    public Vector2 levelCompeletePanelLocation = new Vector2(0,0);

}

[System.Serializable]
public class AdvancedSettings
{
    public bool skipConfirmation = false;
    public bool overwriteCommunicationMovement = false;
    public int communicationRecordRetentionNumber = 10;
}