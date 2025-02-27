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
    Settings,
    LevelComplete
}

public class UI_Controller : MonoBehaviour
{

    public UI_Expectations uiExpectations;
    
    private GameObject satelliteControlPanel;

    public SatelliteControlPanelSettings satelliteControlPanelSettings;
    private bool satelliteControlPanelMoving = false;
    private float satelliteControlPanelNewYLoc;

    private GameObject satelliteInfoUIPanel;
    private GameObject[] satelliteInfoUIObjects;


    public ShopPanelSettings shopPanelSettings;
    private bool shopPanelIsOpen = false;
    private bool shopPanelMoving = false;
    private float shopPanelNewXLoc;
    private GameObject shopPanel;
    private TMP_Text shopBudgetText;
    private int knownBudget;


    public LevelProgressPanelSettings levelProgressPanelSettings;
    private bool levelProgressIsOpen = false;
    private bool levelProgressMoving = false;
    private float levelProgressNewYLoc;
    private GameObject levelProgressPanel;



    private bool userWantsToLeaveLevel = false;
    private bool interactionEnabled = true;



    [HideInInspector]
    public Satellite_Info selectedSatelliteInfo;

    private GameController gameController;

    private GameObject teachingControllerObject;

    private GameObject levelCompletePanel;

    // Start is called before the first frame update
    void Start()
    {

        // Find and connect to gameController
        gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        gameController.SetUIController(this);

        // If expecting satellite communication panel (displays level progress)
        if (uiExpectations.expectSatelliteComms_LevelProgress_Panel )levelProgressPanel = GameObject.FindGameObjectsWithTag("LevelProgressPanel")[0];
        
        // if expecting satellite control and info panel
        if (uiExpectations.expectSatelliteControlsAndInfoPanels)
        {
            // Find and save connection to Satellite Control Panel UI Parent
            satelliteControlPanel = GameObject.FindGameObjectsWithTag("Satellite_Controls")[0];

            satelliteInfoUIPanel = GameObject.FindGameObjectsWithTag("UI_Satellite_Info")[0];
            satelliteInfoUIObjects = GameObject.FindGameObjectsWithTag("UI_Satellite_Info_Obj");
        }

        // If expecting shop UI, collect relevant inforamation
        if (uiExpectations.expectShopUIPanel)
        {
            shopPanel = GameObject.FindGameObjectsWithTag("Shop")[0];

            GameObject[] shopInformationText = GameObject.FindGameObjectsWithTag("Shop_Information_Text");

            foreach (GameObject shopInfoTextObject in shopInformationText)
            {
                TMP_Text textComponent = shopInfoTextObject.GetComponent<TMP_Text>();

                if (shopInfoTextObject.name == "LevelName") textComponent.text = gameController.levelName;
                else if (shopInfoTextObject.name == "LevelDescription") textComponent.text = gameController.levelDescription;
                else if (shopInfoTextObject.name == "CurrentBudget")
                {
                    textComponent.text = "Current Budget: £"+gameController.startingBudget;
                    knownBudget = gameController.startingBudget;
                    shopBudgetText = textComponent;
                }

            }

        }

        // If expecting level complete panel, retrieve information
        if (uiExpectations.expectLevelCompletePanel) levelCompletePanel = GameObject.FindGameObjectsWithTag("LevelCompleteController")[0];

        // If expecting teaching UI Panel, then retrieve needed information
        if (uiExpectations.expectTeachingUIPanel)
        {
            GameObject[] teachingControllerObjects = GameObject.FindGameObjectsWithTag("TeachingController");

            if (teachingControllerObjects.Length != 0)
            {
                teachingControllerObject = GameObject.FindGameObjectsWithTag("TeachingController")[0];
                teachingControllerObject.GetComponent<TeachingController>().SetUIController(this);
            }

            PresentPanel(UIPanel.Teaching,true);
        }

    }

    // Update is called once per frame
    void Update()
    {

        // If the object is not null and the known budget is not what should be
        if (uiExpectations.expectShopUIPanel && shopBudgetText != null && knownBudget != gameController.currentBudget)
        {
            // Retrieve the budget, update the shop budget text.
            shopBudgetText.text = "Current Budget: £"+gameController.currentBudget;
            knownBudget = gameController.currentBudget;
        }


        // If moving satellite control panel, and the new location is not null
        if (uiExpectations.expectSatelliteControlsAndInfoPanels && satelliteControlPanelMoving && satelliteControlPanelNewYLoc != null)
        {
            // Retrieve transform and current position of the panel
            var rectTransform = satelliteControlPanel.GetComponent<RectTransform>();
            var currentPosition = rectTransform.anchoredPosition;

            // Call move panel, moving to the wanted location
            MovePanel(rectTransform,currentPosition,currentPosition.x, satelliteControlPanelNewYLoc,satelliteControlPanelSettings.controlPanelMovementSpeed,satelliteControlPanelMoving);

            // If it has stopped moving and in the close location - set active to false, this the rendering of unseen panels
            if (!satelliteControlPanelMoving && (satelliteControlPanelNewYLoc < satelliteControlPanelSettings.controlPanelVisibleLoc)) satelliteControlPanel.active = false;
        }

        // If the shop panel is moving and has a defined location
        if (uiExpectations.expectShopUIPanel && shopPanelMoving && shopPanelNewXLoc != null)
        {
            // Retrieve the transform and current position of the panel
            var rectTransform = shopPanel.GetComponent<RectTransform>();
            var currentPosition = rectTransform.anchoredPosition;

            // Move to wantted location
            MovePanel(rectTransform,currentPosition,shopPanelNewXLoc,currentPosition.y,shopPanelSettings.shopPanelMovementSpeed,shopPanelMoving);

            // Retrieve animator
            var shopAnimator = shopPanel.GetComponent<Animator>();

            //Update animator based on whether in the closed or open position
            shopAnimator.SetBool("Open",(shopPanelNewXLoc < shopPanelSettings.shopPanelCloseXLoc));
        }
        
        if (uiExpectations.expectSatelliteComms_LevelProgress_Panel && levelProgressMoving && levelProgressNewYLoc != null)
        {
            // Retrieve transform and current position
            var rectTransform = levelProgressPanel.GetComponent<RectTransform>();
            var currentPosition = rectTransform.anchoredPosition;

            // Move to wanted location
            MovePanel(rectTransform,currentPosition, currentPosition.x, levelProgressNewYLoc,levelProgressPanelSettings.levelProgressPanelMovementSpeed,levelProgressMoving);
        }
        
    }




    private void MovePanel(RectTransform rectTransform, Vector3 currentPosition,float newXLoc,float newYLoc,float moveSpeed, bool isMovingRef)
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

        // If at correct location, set the moving reference to false - announces that movement is no longer occuring
        else isMovingRef = false;
        
    }



    public void PresentPanel(UIPanel panel, bool bVisible)
    {
        // If the presentation of a panel is the satellite control AND it's not already being moved
        // This is to prevent potential errors when a user spam clicks a specific satellite.
        if (uiExpectations.expectSatelliteControlsAndInfoPanels && panel == UIPanel.Satellite_Controls)
        {
            // Update moving to true, meaning update will now move the satellite.
            satelliteControlPanelMoving = true;

            // If the satellite is not on screen, move it onto the screen, else move it off screen
            // It is hard coded, in the update, when moved offscreen it will be disabled.

            var position = satelliteControlPanel.GetComponent<RectTransform>().anchoredPosition;

            if (bVisible && position.y < satelliteControlPanelSettings.controlPanelVisibleLoc)  satelliteControlPanelNewYLoc = satelliteControlPanelSettings.controlPanelVisibleLoc;
            else if (!bVisible) satelliteControlPanelNewYLoc = satelliteControlPanelSettings.controlPanelNotVisibleLoc;
            else satelliteControlPanelMoving = false;
        }

        else if (uiExpectations.expectSatelliteControlsAndInfoPanels && panel == UIPanel.Satellite_Info_UI)
        {
            var rectTransform = satelliteInfoUIPanel.GetComponent<RectTransform>();
            var position = rectTransform.anchoredPosition;

            if (bVisible)
            {
                rectTransform.anchoredPosition = new Vector2(position.x, -408);

                satelliteInfoUIPanel.active = true;
                foreach (GameObject obj in satelliteInfoUIObjects)
                {
                    // Type found by Stephan_B, taken from https://discussions.unity.com/t/access-textmeshpro-text-through-script/699157 
                    // then used to get the TextMeshPro Text component of the game object.
                    TMP_Text textComponent = obj.GetComponent<TMP_Text >();

                    if (textComponent != null && selectedSatelliteInfo != null){
                        textComponent.text = "";

                        if (obj.name == "Satellite_Name") textComponent.text = selectedSatelliteInfo.satelliteName;
                        else if (obj.name == "Satellite_Description") textComponent.text = selectedSatelliteInfo.satelliteDescription;
                        else if (obj.name == "Sell_Text")
                        {
                            if (selectedSatelliteInfo.satelliteType == SatelliteType.Origin || selectedSatelliteInfo.satelliteType == SatelliteType.Destination)
                            {
                                textComponent.text = "Not for sale";
                            }
                            else textComponent.text = "Sell £"+selectedSatelliteInfo.satelliteSellPrice;
                        }

                        else if (obj.name == "LightColour") 
                        {
                            if (selectedSatelliteInfo.satelliteType == SatelliteType.Origin) textComponent.text = "Creates "+selectedSatelliteInfo.advanced_Satellite_Info.lightColor.ToString() + " Laser"; 
                            
                            else if (selectedSatelliteInfo.satelliteType == SatelliteType.Destination) textComponent.text = "Needs "+selectedSatelliteInfo.advanced_Satellite_Info.lightColor.ToString() + " Laser"; 
                            else textComponent.text = "";
                        }

                        else if (obj.name == "Satellite_Status")
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

                satelliteInfoUIPanel.active = false;

                foreach (GameObject obj in satelliteInfoUIObjects)
                {
                    obj.active = false;
                }
            }

        }
        
        else if (uiExpectations.expectSettingsPanel && panel == UIPanel.Settings)
        {

        }

        else if (uiExpectations.expectTeachingUIPanel && (panel == UIPanel.Teaching) &&  (teachingControllerObject != null))
        {
            if (bVisible)
            {
                // This is simply for Developer view - meaning the developer can move the UI panel itself elsewhere to test components when editing
                RectTransform teachingTransform = teachingControllerObject.GetComponent<RectTransform>();
                // Moves level complete to 0,0 (of the UI) if not already there - in cases where the dev has moved it for testing purposes.
                if (teachingTransform.anchoredPosition != new Vector2(0,0)) teachingTransform.anchoredPosition = new Vector2(0,0);
            }

            // Sets whether active or not based on visbility wanted.
            teachingControllerObject.active = bVisible;
        }

        else if (uiExpectations.expectSatelliteComms_LevelProgress_Panel && panel == UIPanel.LogCommunications && levelProgressPanel != null)
        {
    
            levelProgressMoving = true;

            var position = levelProgressPanel.GetComponent<RectTransform>().anchoredPosition;

            if (bVisible && position.y > levelProgressPanelSettings.levelProgressPanelOpenYLoc)  levelProgressNewYLoc = levelProgressPanelSettings.levelProgressPanelOpenYLoc;
            else if (!bVisible) levelProgressNewYLoc = levelProgressPanelSettings.levelProgressPanelCloseYLoc;
            else levelProgressMoving = false;
            
        }

        else if (uiExpectations.expectLevelCompletePanel && panel == UIPanel.LevelComplete)
        {

            // Retrieve levelCompletePanel
            levelCompletePanel = GameObject.FindGameObjectsWithTag("LevelCompleteController")[0];

            // This is simply for Developer view - meaning the developer can move the UI panel itself elsewhere to test components when editing
            RectTransform levelCompletetransform = levelCompletePanel.GetComponent<RectTransform>();
            // Moves level complete to 0,0 (of the UI) if not already there - in cases where the dev has moved it for testing purposes.
            if (levelCompletetransform.anchoredPosition != new Vector2(0,0)) levelCompletetransform.anchoredPosition = new Vector2(0,0);

            // Sets whether active or not based on visbility wanted.
            levelCompletePanel.active = bVisible;
            

        }
    }

    

    public void OpenCloseShop()
    {
        // If the shop is open close it, if it's closed, open it.
        // This makes use of a manual linear interpolation to show and close it - as RectTransform doesn't support linear interpolated movement
        // This is done mainly for animation purposes.

        if (uiExpectations.expectShopUIPanel && interactionEnabled)
        {
            if (shopPanelIsOpen)
            {
                shopPanelIsOpen = false;
                shopPanelMoving = true;
                shopPanelNewXLoc = shopPanelSettings.shopPanelCloseXLoc;
            }
            else
            {
                shopPanelIsOpen = true;
                shopPanelMoving = true;
                shopPanelNewXLoc = shopPanelSettings.shopPanelOpenXLoc;
            }
        }
    }

    public void SetCompletedModeActive(bool active)
    {
        if (uiExpectations.expectLevelCompletePanel)
        {
            if (active)
            {
                if (shopPanelIsOpen) OpenCloseShop();
                interactionEnabled = false;

                levelCompletePanel.GetComponent<LevelCompleteController>().GameComplete();
                PresentPanel(UIPanel.LevelComplete,true);
            }
            else 
            {
                interactionEnabled = true;
                PresentPanel(UIPanel.LevelComplete,false);
            }
        }
    }


    public bool GetInteractionEnabled(){return interactionEnabled;}
    
}




[System.Serializable]
public class UI_Expectations
{
    public bool expectShopUIPanel = true;
    public bool expectTeachingUIPanel  = true;
    public bool expectSatelliteControlsAndInfoPanels = true;
    public bool expectLevelCompletePanel  = true;
    public bool expectSettingsPanel  = true;
    public bool expectSatelliteComms_LevelProgress_Panel  = true;
}

[System.Serializable]
public class SatelliteControlPanelSettings
{
    public float controlPanelMovementSpeed = 1;
    public float controlPanelVisibleLoc = -465f;
    public float controlPanelNotVisibleLoc = -650f;
}

[System.Serializable]
public class ShopPanelSettings
{
    public float shopPanelMovementSpeed = 1;
    public float shopPanelOpenXLoc = 596f;
    public float shopPanelCloseXLoc = 1147f;
}

[System.Serializable]
public class LevelProgressPanelSettings
{
    public int recordRetentionNumber = 10;
    public bool forceNoChangeOnNewCommunication = false;

    public float levelProgressPanelMovementSpeed = 1;

    public float levelProgressPanelOpenYLoc = 376.7f;
    public float levelProgressPanelCloseYLoc = 646.0f;

}