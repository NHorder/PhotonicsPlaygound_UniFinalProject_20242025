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
    Settings,
    LevelComplete
}

public class UI_Controller : MonoBehaviour
{
    
    private GameObject satelliteControlPanel;

    public SatelliteControlPanelSettings satelliteControlPanelSettings;
    private bool movingSatelliteControlPanel = false;
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

        gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        gameController.SetUIController(this);


        GameObject[] teachingControllerObjects = GameObject.FindGameObjectsWithTag("TeachingController");

        if (teachingControllerObjects.Length != 0)
        {
            teachingControllerObject = GameObject.FindGameObjectsWithTag("TeachingController")[0];
            teachingControllerObject.GetComponent<TeachingController>().SetUIController(this);
        }


        
        // Find and save connection to Satellite Control Panel UI Parent
        satelliteControlPanel = GameObject.FindGameObjectsWithTag("Satellite_Controls")[0];

        satelliteInfoUIPanel = GameObject.FindGameObjectsWithTag("UI_Satellite_Info")[0];
        satelliteInfoUIObjects = GameObject.FindGameObjectsWithTag("UI_Satellite_Info_Obj");

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

        PresentPanel(UIPanel.Teaching,true);
    }

    // Update is called once per frame
    void Update()
    {

        if (shopBudgetText != null && knownBudget != gameController.currentBudget)
        {
            shopBudgetText.text = "Current Budget: £"+gameController.currentBudget;
            knownBudget = gameController.currentBudget;
        }

        // If moving satellite control panel, and the new location is not null
        if (movingSatelliteControlPanel && satelliteControlPanelNewYLoc != null)
        {
            var panelRectTransform = satelliteControlPanel.GetComponent<RectTransform>();
            var position = panelRectTransform.anchoredPosition;

            if (position.y < satelliteControlPanelNewYLoc)
            {
                if (satelliteControlPanel.active == false) satelliteControlPanel.active = true;

                panelRectTransform.anchoredPosition = new Vector2(position.x, position.y + satelliteControlPanelSettings.controlPanelMovementSpeed);

                // If it over extends, have it move back to the exact needed location.
                if (position.y + satelliteControlPanelSettings.controlPanelMovementSpeed > satelliteControlPanelNewYLoc)
                {
                    panelRectTransform.anchoredPosition = new Vector2(position.x,satelliteControlPanelNewYLoc);
                }
            }

            else if (position.y > satelliteControlPanelNewYLoc)
            {
                panelRectTransform.anchoredPosition = new Vector2(position.x, position.y - satelliteControlPanelSettings.controlPanelMovementSpeed);

                if (position.y - satelliteControlPanelSettings.controlPanelMovementSpeed < satelliteControlPanelNewYLoc)
                {
                    panelRectTransform.anchoredPosition = new Vector2(position.x,satelliteControlPanelNewYLoc);
                }

            }
            else
            {
                movingSatelliteControlPanel = false;
                if (satelliteControlPanelNewYLoc < satelliteControlPanelSettings.controlPanelVisibleLoc) satelliteControlPanel.active = false;
            }
            
        }

        // If the shop panel is moving and has a defined location
        if (shopPanelMoving && shopPanelNewXLoc != null)
        {
            // Retrieve neccessary transform components and current position
            var panelRectTransform = shopPanel.GetComponent<RectTransform>();
            var position = panelRectTransform.anchoredPosition;

            // If the X location (as the shop panel slides left and right) is less, then increase it
            if (position.x < shopPanelNewXLoc)
            {
                panelRectTransform.anchoredPosition = new Vector2(position.x + shopPanelSettings.shopPanelMovementSpeed, position.y);

                // If it overextends then move it to the exact location needed
                if (position.x - shopPanelSettings.shopPanelMovementSpeed > shopPanelNewXLoc)
                {
                    panelRectTransform.anchoredPosition = new Vector2(shopPanelNewXLoc,position.y);
                }
                
            }


            // Else decrease it if the position is more than the new location
            else if (position.x > shopPanelNewXLoc)
            {
                panelRectTransform.anchoredPosition = new Vector2(position.x - shopPanelSettings.shopPanelMovementSpeed, position.y);

                // If it overextends then move it to the exact location needed
                if (position.x - shopPanelSettings.shopPanelMovementSpeed < shopPanelNewXLoc)
                {
                    panelRectTransform.anchoredPosition = new Vector2(shopPanelNewXLoc,position.y);
                }
            }

            
            // Once complete (else works, as it means it's at the new location - event if it overextends a bit)
            else
            {
                // Set moving to false
                shopPanelMoving = false;

                // Retrieve animator
                var shopAnimator = shopPanel.GetComponent<Animator>();
                // Update animator based on whether in the closed or open position
                shopAnimator.SetBool("Open",(shopPanelNewXLoc < shopPanelSettings.shopPanelCloseXLoc));
            }
        }
        
        
    }


    public void PresentPanel(UIPanel panel, bool bVisible)
    {
        // If the presentation of a panel is the satellite control AND it's not already being moved
        // This is to prevent potential errors when a user spam clicks a specific satellite.
        if (panel == UIPanel.Satellite_Controls)
        {
            // Update moving to true, meaning update will now move the satellite.
            movingSatelliteControlPanel = true;

            // If the satellite is not on screen, move it onto the screen, else move it off screen
            // It is hard coded, in the update, when moved offscreen it will be disabled.

            var position = satelliteControlPanel.GetComponent<RectTransform>().anchoredPosition;

            if (bVisible && position.y < satelliteControlPanelSettings.controlPanelVisibleLoc)  satelliteControlPanelNewYLoc = satelliteControlPanelSettings.controlPanelVisibleLoc;
            else if (!bVisible) satelliteControlPanelNewYLoc = satelliteControlPanelSettings.controlPanelNotVisibleLoc;
            else movingSatelliteControlPanel = false;
        }

        else if (panel == UIPanel.Satellite_Info_UI)
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
        
        else if (panel == UIPanel.Settings)
        {

        }

        else if ((panel == UIPanel.Teaching) &&  (teachingControllerObject != null))
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


        else if (panel == UIPanel.LevelComplete)
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

        if (interactionEnabled)
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
        if (active)
        {
            if (shopPanelIsOpen) OpenCloseShop();
            interactionEnabled = false;

            PresentPanel(UIPanel.LevelComplete,true);
        }
        else 
        {
            interactionEnabled = true;
            PresentPanel(UIPanel.LevelComplete,false);
        }

    }


    public bool GetInteractionEnabled(){return interactionEnabled;}
    
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