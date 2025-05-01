using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomColourFilter :  ColourFilterSatellite
{
    /// <summary>
    /// Class to handle custom colour filter interactions
    /// Inherits ColourFilterSatellite (which in turn inherits SatelliteParent)
    /// Unlike other satellites, this satellite is placed under the Canvas in the UI Satellites folder
    /// </summary>

    // UI element offsets
    public float nextButtonOffsetY = 5f;
    public float previousButtonOffsetY = 5f;

    // Current colour index
    private int colourIndex = 0;

    // List of available colours
    private List<LaserColour> _listColours = new List<LaserColour>();

    // Links to next and previous colours, as well as camera
    private Button _nextColourButton;
    private RectTransform _nextButtonTransform;
    private Button _previousColourButton;
    private RectTransform _previousButtonTransform;
    private Camera _camera;

    /// <summary>
    /// Initialisation Method
    /// </summary>
    void Start()
    {
        /// Set local transform to 1,1,1. This is present due to an issue with prefab that scales the satellite to a low number
        /// attempts to fix has failed
        this.transform.localScale = new Vector3(1f,1f,1f);

        // Retrieve animator
        _animator = gameObject.GetComponent<Animator>();

        // Add colours to the list of colours
        _listColours.Add(LaserColour.White);
        _listColours.Add(LaserColour.Red);
        _listColours.Add(LaserColour.Blue);
        _listColours.Add(LaserColour.Green);
        _listColours.Add(LaserColour.Yellow);
        _listColours.Add(LaserColour.Cyan);
        _listColours.Add(LaserColour.Magenta);

        // Locate camera
        _camera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();

        // Loop through and find UI component buttons
        var childButtons = gameObject.GetComponentsInChildren<Button>();

        foreach (Button childButton in childButtons)
        {
            if (childButton.name == "NextColour")
            {
                _nextColourButton = childButton;
                _nextButtonTransform = childButton.GetComponent<RectTransform>();
            }
            else if (childButton.name == "PreviousColour")
            {
                _previousColourButton = childButton;
                _previousButtonTransform = childButton.GetComponent<RectTransform>();
            }
        }

        // Attach script to each button
        _nextColourButton.onClick.AddListener(NextColour);
        _previousColourButton.onClick.AddListener(PreviousColour);

        // Call inherited intialisation method
        base.Start();
    }

    /// <summary>
    /// Method called once per frame
    /// </summary>
    void Update()
    {
        // Have the UI elements follow this transform, will need to convert from screenspace to worldspace.
        var screenPoint = _camera.WorldToScreenPoint(this.transform.position);
        _nextButtonTransform.position = new Vector2(screenPoint.x,screenPoint.y + nextButtonOffsetY);
        _previousButtonTransform.position = new Vector2(screenPoint.x,screenPoint.y+previousButtonOffsetY);

        // Call inherited update method
        base.Update();
    }

    /// <summary>
    /// Method to update the colour fitlers colour
    /// </summary>
    private void UpdateColour()
    {
        // Check edge cases for index (negatives and over list of colours count)
        if (colourIndex < 0) colourIndex = _listColours.Count -1;
        else if (colourIndex >= _listColours.Count) colourIndex = 0;

        // Find the wanted colour
        var newFiterColour = _listColours[colourIndex];

        // Update animator for each colour - NOTE: Animator does not use enum, hence Int is used instead
        if (newFiterColour == LaserColour.White) _animator.SetInteger("ColourID",0);
        else if (newFiterColour == LaserColour.Red) _animator.SetInteger("ColourID",1);
        else if (newFiterColour == LaserColour.Blue) _animator.SetInteger("ColourID",2);
        else if (newFiterColour == LaserColour.Green) _animator.SetInteger("ColourID",3);
        else if (newFiterColour == LaserColour.Yellow) _animator.SetInteger("ColourID",4);
        else if (newFiterColour == LaserColour.Cyan) _animator.SetInteger("ColourID",5);
        else if (newFiterColour == LaserColour.Magenta) _animator.SetInteger("ColourID",6);

        // Call inherited function to set the fitler colour
        base.SetFilterColour(newFiterColour);
    }

    /// <summary>
    /// Method to swap to next colour - called by UI Button
    /// </summary>
    public void NextColour()
    {
        colourIndex +=1;
        UpdateColour();
    }

    /// <summary>
    /// Method to swap to previous colour - called by UI Button
    /// </summary>
    public void PreviousColour()
    {
        colourIndex -=1;
        UpdateColour();
    }
}
