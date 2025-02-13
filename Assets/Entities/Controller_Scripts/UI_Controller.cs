using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UIPanel
{
    Satellite_Controls,
    Shop,
    Settings,
}

public class UI_Controller : MonoBehaviour
{
    
    GameObject satelliteControlPanel;

    public float controlPanelMovementSpeed = 1;
    private bool movingSatelliteControlPanel = false;
    private float satelliteControlPanelNewYLoc;

    // Start is called before the first frame update
    void Start()
    {
        // Find and save connection to Satellite Control Panel UI Parent
        satelliteControlPanel = GameObject.FindGameObjectsWithTag("Satellite_Controls")[0];

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
        else if (panel == UIPanel.Shop)
        {

        }
        else if (panel == UIPanel.Settings)
        {

        }

    }


}
