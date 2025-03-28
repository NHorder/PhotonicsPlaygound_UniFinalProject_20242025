using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomColourFilter :  ColourFilterSatellite
{

    public float nextButtonOffsetY = 5f;
    public float previousButtonOffsetY = 5f;


    private int colourIndex = 0;
    private List<LaserColour> _listColours = new List<LaserColour>();

    private Button _nextColourButton;
    private RectTransform _nextButtonTransform;
    private Button _previousColourButton;
    private RectTransform _previousButtonTransform;
    private Camera _camera;

    //private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = new Vector3(1f,1f,1f);

        _animator = gameObject.GetComponent<Animator>();

        _listColours.Add(LaserColour.White);
        _listColours.Add(LaserColour.Red);
        _listColours.Add(LaserColour.Blue);
        _listColours.Add(LaserColour.Green);
        _listColours.Add(LaserColour.Yellow);
        _listColours.Add(LaserColour.Cyan);
        _listColours.Add(LaserColour.Magenta);


        _camera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();

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

        _nextColourButton.onClick.AddListener(NextColour);
        _previousColourButton.onClick.AddListener(PreviousColour);


        base.Start();
    }


    void Update()
    {
        // Have the UI elements follow this transform, will need to convert from screenspace to worldspace.

        
        
        var screenPoint = _camera.WorldToScreenPoint(this.transform.position);
        _nextButtonTransform.position = new Vector2(screenPoint.x,screenPoint.y + nextButtonOffsetY);
        _previousButtonTransform.position = new Vector2(screenPoint.x,screenPoint.y+previousButtonOffsetY);

        //_nextButtonTransform.pivot = screenPoint;
        //_previousButtonTransform.pivot = screenPoint;
        
        

        base.Update();
    }


    private void UpdateColour()
    {

        Debug.Log("Changed Colour!");

        if (colourIndex < 0) colourIndex = _listColours.Count -1;
        else if (colourIndex >= _listColours.Count) colourIndex = 0;

        var newFiterColour = _listColours[colourIndex];



        if (newFiterColour == LaserColour.White) _animator.SetInteger("ColourID",0);
        else if (newFiterColour == LaserColour.Red) _animator.SetInteger("ColourID",1);
        else if (newFiterColour == LaserColour.Blue) _animator.SetInteger("ColourID",2);
        else if (newFiterColour == LaserColour.Green) _animator.SetInteger("ColourID",3);
        else if (newFiterColour == LaserColour.Yellow) _animator.SetInteger("ColourID",4);
        else if (newFiterColour == LaserColour.Cyan) _animator.SetInteger("ColourID",5);
        else if (newFiterColour == LaserColour.Magenta) _animator.SetInteger("ColourID",6);

        base.SetFilterColour(newFiterColour);
    }


    public void NextColour()
    {
        colourIndex +=1;
        UpdateColour();
    }

    public void PreviousColour()
    {
        colourIndex -=1;
        UpdateColour();
    }
}
