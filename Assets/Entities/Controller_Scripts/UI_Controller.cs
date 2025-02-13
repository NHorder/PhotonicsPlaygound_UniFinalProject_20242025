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
    private bool movingSatelliteControlPanel = false;
    private float satelliteControlPanelNewYLoc;

    private GameObject satelliteInfoUIPanel;
    private GameObject[] satelliteInfoUIObjects;


    public Satellite_Info selectedSatelliteInfo;

    // Start is called before the first frame update
    void Start()
    {
        // Find and save connection to Satellite Control Panel UI Parent
        satelliteControlPanel = GameObject.FindGameObjectsWithTag("Satellite_Controls")[0];

        satelliteInfoUIPanel = GameObject.FindGameObjectsWithTag("UI_Satellite_Info")[0];
        satelliteInfoUIObjects = GameObject.FindGameObjectsWithTag("UI_Satellite_Info_Obj");

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
                if (satelliteControlPanelNewYLoc < -465f) satelliteControlPanel.active = false;
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

            if (bVisible && position.y < -465)  satelliteControlPanelNewYLoc = -465f;
            else if (!bVisible) satelliteControlPanelNewYLoc = -650f;
            else movingSatelliteControlPanel = false;
        }

        else if (panel == UIPanel.Satellite_Info_UI)
        {
            var rectTransform = satelliteInfoUIPanel.GetComponent<RectTransform>();
            var position = rectTransform.anchoredPosition;

            if (bVisible)
            {
                rectTransform.anchoredPosition = new Vector2(position.x, -368);

                satelliteInfoUIPanel.active = true;
                foreach (GameObject obj in satelliteInfoUIObjects)
                {
                    // Type found by Stephan_B, taken from https://discussions.unity.com/t/access-textmeshpro-text-through-script/699157 
                    // then used to get the TextMeshPro Text component of the game object.
                    TMP_Text textComponent = obj.GetComponent<TMP_Text >();
                    
                    if (textComponent != null && selectedSatelliteInfo != null){
                        textComponent.text = "Hi?";

                        if (obj.name == "Satellite_Name") textComponent.text = selectedSatelliteInfo.satelliteName;
                        else if (obj.name == "Satellite_Description") textComponent.text = selectedSatelliteInfo.satelliteDescription;
                        else if (obj.name == "Sell_Text") textComponent.text = "Sell £"+selectedSatelliteInfo.satelliteSellPrice;

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
        
        else if (panel == UIPanel.Shop)
        {

        }
        else if (panel == UIPanel.Settings)
        {

        }

    }


}
