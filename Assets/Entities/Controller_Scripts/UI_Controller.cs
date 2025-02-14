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
    Settings,
}

public class UI_Controller : MonoBehaviour
{
    
    private GameObject satelliteControlPanel;

    public float controlPanelMovementSpeed = 1;
    public float controlPanelVisibleLoc = -465f;
    public float controlPanelNotVisibleLoc = -650f;
    private bool movingSatelliteControlPanel = false;
    private float satelliteControlPanelNewYLoc;

    private GameObject satelliteInfoUIPanel;
    private GameObject[] satelliteInfoUIObjects;


    public float shopPanelMovementSpeed = 1;
    public float shopPanelOpenXLoc = 596f;
    public float shopPanelCloseXLoc = 1147f;

    private bool shopPanelIsOpen = false;
    private bool shopPanelMoving = false;
    private float shopPanelNewXLoc;
    private GameObject shopPanel;
    private GameObject[] shopPanelText;
    private GameObject[] shopPanelItems;

    [HideInInspector]
    public Satellite_Info selectedSatelliteInfo;

    // Start is called before the first frame update
    void Start()
    {
        // Find and save connection to Satellite Control Panel UI Parent
        satelliteControlPanel = GameObject.FindGameObjectsWithTag("Satellite_Controls")[0];

        satelliteInfoUIPanel = GameObject.FindGameObjectsWithTag("UI_Satellite_Info")[0];
        satelliteInfoUIObjects = GameObject.FindGameObjectsWithTag("UI_Satellite_Info_Obj");

        shopPanel = GameObject.FindGameObjectsWithTag("Shop")[0];

    }

    // Update is called once per frame
    void Update()
    {
        // If moving satellite control panel, and the new location is not null
        if (movingSatelliteControlPanel && satelliteControlPanelNewYLoc != null)
        {
            var panelRectTransform = satelliteControlPanel.GetComponent<RectTransform>();
            var position = panelRectTransform.anchoredPosition;

            if (position.y < satelliteControlPanelNewYLoc)
            {
                if (satelliteControlPanel.active == false) satelliteControlPanel.active = true;

                panelRectTransform.anchoredPosition = new Vector2(position.x, position.y + controlPanelMovementSpeed);
            }
            else if (position.y > satelliteControlPanelNewYLoc)
            {
                panelRectTransform.anchoredPosition = new Vector2(position.x, position.y - controlPanelMovementSpeed);
            }
            else
            {
                movingSatelliteControlPanel = false;
                if (satelliteControlPanelNewYLoc < controlPanelVisibleLoc) satelliteControlPanel.active = false;
            }
            
        }

        // If the shop panel is moving and has a defined location
        if (shopPanelMoving && shopPanelNewXLoc != null)
        {
            // Retrieve neccessary transform components and current position
            var panelRectTransform = shopPanel.GetComponent<RectTransform>();
            var position = panelRectTransform.anchoredPosition;

            // If the X location (as the shop panel slides left and right) is less, then increase it
            if (position.x < shopPanelNewXLoc) panelRectTransform.anchoredPosition = new Vector2(position.x + shopPanelMovementSpeed, position.y);

            // Else decrease it if the position is more than the new location
            else if (position.x > shopPanelNewXLoc) panelRectTransform.anchoredPosition = new Vector2(position.x - shopPanelMovementSpeed, position.y);
            
            // Once complete (else works, as it means it's at the new location - event if it overextends a bit)
            else
            {
                // Set moving to false
                movingSatelliteControlPanel = false;

                // Retrieve animator
                var shopAnimator = shopPanel.GetComponent<Animator>();
                // Update animator based on whether in the closed or open position
                shopAnimator.SetBool("Open",(shopPanelNewXLoc < shopPanelCloseXLoc));
            }
        }
    }


    public void PresentPanel(UIPanel panel, bool bVisible)
    {
        // If the presentation of a panel is the satellite control AND it's not already being moved
        // This is to prevent potential errors when a user spam clicks a specific satellite.
        if (panel == UIPanel.Satellite_Controls && movingSatelliteControlPanel == false)
        {
            // Update moving to true, meaning update will now move the satellite.
            movingSatelliteControlPanel = true;

            // If the satellite is not on screen, move it onto the screen, else move it off screen
            // It is hard coded, in the update, when moved offscreen it will be disabled.

            var position = satelliteControlPanel.GetComponent<RectTransform>().anchoredPosition;

            if (bVisible && position.y < controlPanelVisibleLoc)  satelliteControlPanelNewYLoc = controlPanelVisibleLoc;
            else if (!bVisible) satelliteControlPanelNewYLoc = controlPanelNotVisibleLoc;
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
                            if (selectedSatelliteInfo.satelliteType == SatelliteType.Origin) textComponent.text = "Creates "+selectedSatelliteInfo.lightColor.ToString() + " Laser"; 
                            
                            else if (selectedSatelliteInfo.satelliteType == SatelliteType.Destination) textComponent.text = "Needs "+selectedSatelliteInfo.lightColor.ToString() + " Laser"; 
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

    }

    public void OpenCloseShop()
    {
        // If the shop is open close it, if it's closed, open it.
        // This makes use of a manual linear interpolation to show and close it - as RectTransform doesn't support linear interpolated movement
        // This is done mainly for animation purposes.
        if (shopPanelIsOpen)
        {
            shopPanelIsOpen = false;
            shopPanelMoving = true;
            shopPanelNewXLoc = shopPanelCloseXLoc;
        }
        else
        {
            shopPanelIsOpen = true;
            shopPanelMoving = true;
            shopPanelNewXLoc = shopPanelOpenXLoc;
        }
    }

    public void ShopPurchaseItem()
    {

    }

    public void ResetLevel()
    {

    }


}
