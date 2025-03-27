using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomColourFilter :  ColourFilterSatellite
{

    public float nextButtonOffsetX;
    public float previousButtonOffsetX;


    private int colourIndex = 0;
    private List<LaserColour> _listColours = new List<LaserColour>();

    private Button _nextColourButton;
    private Button _previousColourButton;

    // Start is called before the first frame update
    void Start()
    {
        _listColours.Add(LaserColour.White);
        _listColours.Add(LaserColour.Red);
        _listColours.Add(LaserColour.Blue);
        _listColours.Add(LaserColour.Green);
        _listColours.Add(LaserColour.Yellow);
        _listColours.Add(LaserColour.Cyan);
        _listColours.Add(LaserColour.Magenta);


        var childButtons = gameObject.GetComponentsInChildren<Button>();

        foreach (Button childButton in childButtons)
        {
            if (childButton.name == "NextColour") _nextColourButton = childButton;
            else if (childButton.name == "PreviousColour") _previousColourButton = childButton;
        }

        _nextColourButton.onClick.AddListener(NextColour);
        _previousColourButton.onClick.AddListener(PreviousColour);


        base.Start();
    }


    void Update()
    {
        // Have the UI elements follow this transform, will need to convert from screenspace to worldspace.
        

        base.Update();
    }


    private void UpdateColour()
    {

        if (colourIndex < 0) colourIndex = _listColours.Count -1;
        else if (colourIndex >= _listColours.Count) colourIndex = 0;

        var newFiterColour = _listColours[colourIndex];

        base.SetFilterColour(newFiterColour);
    }


    public void NextColour()
    {
        colourIndex +=1;
    }

    public void PreviousColour()
    {
        colourIndex -=1;
    }
}
