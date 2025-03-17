using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// public enum UIPanel
// {
//     Satellite_Controls,
//     Satellite_Info_UI,
//     Shop,
//     Teaching,
//     LogCommunications,
//     ConfirmAction,
//     Settings,
//     LevelComplete
// }

public class UIController_ : MonoBehaviour
{
    public float panelMovementSpeed;

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
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        //_gameController.SetUIController(this);


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
            _movingPanels.satelliteControlsPanel = GameObject.FindGameObjectsWithTag("SatelliteControlsPanel")[0].GetComponent<SatelliteControlsPanel>();
        }

        if (uiExpectations.expectSatelliteInformationPanel)
        {
            _movingPanels.satelliteInformationPanel = GameObject.FindGameObjectsWithTag("SatelliteInformationPanel")[0].GetComponent<SatelliteInformationPanel>();
        }

        if (uiExpectations.expectShopPanel)
        {
            _movingPanels.shopPanel = GameObject.FindGameObjectsWithTag("ShopPanel")[0].GetComponent<ShopPanel>();
        } 


        // Collect all panel objects for fixed panels.
        if (uiExpectations.expectTeachingPanel)
        {
            _fixedPanels.teachingPanel = GameObject.FindGameObjectsWithTag("TeachingPanel")[0].GetComponent<TeachingPanel>();
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
            _fixedPanels.levelCompletePanel = GameObject.FindGameObjectsWithTag("LevelCompletePanel")[0].GetComponent<LevelCompletePanel>();
        } 
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void PresentFixedPanel(UIPanel panel, bool bVisible)
    {
        // Fixed Panels have a single location and instead work by handling their 'active' state
        // As such they only appear in one location or not appear in said location. They don't move.

        if (panel == UIPanel.Settings)
        {

        }

        else if (panel == UIPanel.LevelComplete)
        {

        }

        else if (panel == UIPanel.ConfirmAction)
        {

        }

        else if (panel == UIPanel.Teaching)
        {

        }

    }



    private void MovePanel(RectTransform rectTransform,Vector3 currentPosition,Vector2 newPosition)
    {

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

    }

    public void ToggleVisibleShop()
    {

    }

    public void ToggleVisibleSatelliteControls()
    {

    }

    public void ToggleVisibleSatelliteInformation()
    {

    }

    public void UpdateLanguage(Language newLanguage)
    {
        _language = newLanguage;
        
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
    public bool levelInformationVisible = true;
    public bool levelInformationMoving = false;

    public ShopPanel shopPanel;
    public Vector2 shopMoveTo;
    public bool shopPanelVisible = false;
    public bool shopPanelMoving = false;


    public CommunicationsPanel communicationsPanel;
    public Vector2 communicationsMoveTo;
    public bool communicationPanelVisible = false;
    public bool communicationPanelMoving = false;

    public SatelliteControlsPanel satelliteControlsPanel;
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
    public Vector2 levelInformationOpenLocation;
    public Vector2 levelInformationCloseLocation;

    public Vector2 shopOpenLocation;
    public Vector2 shopCloseLocation;

    public Vector2 satelliteControlsOpenLocation;
    public Vector2 satelliteControlsCloseLocation;

    public Vector2 satelliteInfomationOpenLocation;
    public Vector2 satelliteInformationCloseLocation;

    public Vector2 communicationsOpenLocation;
    public Vector2 communicationsCloseLocation;


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