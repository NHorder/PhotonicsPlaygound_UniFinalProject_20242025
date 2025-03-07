using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SplitterType
{
    TwoOutput,
    ThreeOutput,
    SixOutput
}


public class SplitterSatellite : SatelliteParent
{
    public SplitterType splitterType;
    private int _numExpectedOutput;

    private Laser _laser;
    private RaycastHit2D _raycast;

    private List<LaserColour> _laserColours;
    private List<float> _outputAngles;


    private LaserColour _twoOutputWhiteColourA;
    private LaserColour _twoOutputWhiteColourB;
    
    private float _energyPerLaser;


    public TwoOutputSplitterSettings twoOutputSplitterSettings;
    public ThreeOutputSplitterSettings threeOutputSplitterSettings;
    public SixOutputSplitterSettings sixOutputSplitterSettings;

    // Start is called before the first frame update
    void Start()
    {

        base.Start();

        _laserColours = new List<LaserColour>();
        _outputAngles = new List<float>();

        // Each offset and angle need to be added differently, as sprites can change the offset and angle 
        // of each laser output

        // The offsets and angle are 'hard coded' but can be changed at developer discretion through the editor.

        if (splitterType == SplitterType.TwoOutput)
        {
            _numExpectedOutput = 2;

            // Randomly select two colours to use if a white laser is to be split - with 2 nodes
            //twoOutputWhiteColourA
            //twoOutputWhiteColourB

            _outputAngles.Add(twoOutputSplitterSettings.angleAlpha);
            _outputAngles.Add(twoOutputSplitterSettings.angleBeta);


        }

        else if (splitterType == SplitterType.ThreeOutput)
        {
            _numExpectedOutput = 3;

            _outputAngles.Add(threeOutputSplitterSettings.angleAlpha);
            _outputAngles.Add(threeOutputSplitterSettings.angleBeta);
            _outputAngles.Add(threeOutputSplitterSettings.angleGamma);
        }

        else if (splitterType == SplitterType.SixOutput)
        {
            _numExpectedOutput = 6;
            _outputAngles.Add(sixOutputSplitterSettings.angleAlpha);
            _outputAngles.Add(sixOutputSplitterSettings.angleBeta);
            _outputAngles.Add(sixOutputSplitterSettings.angleGamma);
            _outputAngles.Add(sixOutputSplitterSettings.angleDelta);
            _outputAngles.Add(sixOutputSplitterSettings.angleEpsilon);
            _outputAngles.Add(sixOutputSplitterSettings.angleZeta);


        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_laserColours.Count != 0)
        {
            for (int i = 0; i < _laserColours.Count;i++)
            {
                Vector3 raycastPoint = _raycast.point;

                // If I is more than one, then add a minor offset to allow creation of X laser
                if (i > 0) raycastPoint.x += (0.00001f * i);

                OutgoingLaserInfo outgoingLaserInfo = new OutgoingLaserInfo();
                outgoingLaserInfo.angle = this.transform.eulerAngles.z + _outputAngles[i];
                outgoingLaserInfo.origin = this.transform.position;
                outgoingLaserInfo.raycastPosition = raycastPoint;
                outgoingLaserInfo.laserTransparency = _energyPerLaser;

                outgoingLaserInfo.laserColour = _laserColours[i]; 

                _outgoingLaserInfo.Add(outgoingLaserInfo);

            }
            _laserColours.Clear();
        }

        base.Update();
    }

    override public void SetActive(Laser laser, RaycastHit2D raycast)
    {

        if (_trueOrigin == null) _trueOrigin = laser.origin;

        _active = true;

        // Restricts input to only one laser
        if (_incomingLasers.Count == 0)
        {
            IncomingLaser newIncomingLaser = new IncomingLaser();
            newIncomingLaser.laser = laser;
            newIncomingLaser.raycast = raycast;

            _incomingLasers.Add(newIncomingLaser);


            LaserColour laserOriginColour = laser.GetLaserColour();

            if (_numExpectedOutput == 2) DetermineTwoOutputs(laserOriginColour);
            else if (_numExpectedOutput == 3) DetermineThreeOutputs(laserOriginColour);
            else if (_numExpectedOutput == 6) DetermineSixOutputs(laserOriginColour);
            else Debug.LogError("ERROR: An error has occurred when splitting the laser");
        }
    }

    
    private void DetermineTwoOutputs(LaserColour laserOriginColour, bool allocatedEnergyPerLaser = false)
    {
        // If the energy has not been already allocated, then calcualte it for two outputs
        if (!allocatedEnergyPerLaser) _energyPerLaser = Mathf.Clamp01((_incomingLasers[0].laser.GetTransparency() - _thisSatelliteInfo.advanced_Satellite_Info.absorbance) / 2);

        // If a white laser, then use the pre-defined random colours
        // This is pre-defined to prervent strobe effects as the laser is repeatedly created and destroyed.
        if (laserOriginColour == LaserColour.White)
        {
            _laserColours.Add(laserOriginColour);
            _laserColours.Add(laserOriginColour);

            //_laserColours.Add(_twoOutputWhiteColourA);
            //_laserColours.Add(_twoOutputWhiteColourB);
        }


        // Check and split based on secondary colours (Magenta, Yellow and Cyan)
        else if (laserOriginColour == LaserColour.Magenta)
        {
            // Magenta is made from Red and Blue
            _laserColours.Add(LaserColour.Red);
            _laserColours.Add(LaserColour.Blue);
        }
        else if (laserOriginColour == LaserColour.Yellow)
        {
            // Yellow is a mixture of Red and Green
            _laserColours.Add(LaserColour.Red);
            _laserColours.Add(LaserColour.Green);
        }
        else if (laserOriginColour == LaserColour.Cyan)
        {
            // Cyan is made of Blue and Green
            _laserColours.Add(LaserColour.Blue);
            _laserColours.Add(LaserColour.Green);
        }
        
        
        // Otherwise add two of the same colour
        // This will occur for the primary colours (Red, Green, Blue)
        // As primary colours can't be split / aren't a combination of other colours
        else
        {
            _laserColours.Add(laserOriginColour);
            _laserColours.Add(laserOriginColour);
        }
        // Then everything else is just two of the same, but with less energy

    }

    private void DetermineThreeOutputs(LaserColour laserOriginColour)
    {
        // Calcualte energy per laser for three outputs
        _energyPerLaser = Mathf.Clamp01((_incomingLasers[0].laser.GetTransparency() - _thisSatelliteInfo.advanced_Satellite_Info.absorbance) / 3);

        // Determine two colours based on the origin laser colour
        DetermineTwoOutputs(laserOriginColour,true);

        // Use the origin as the middle colour
        _laserColours.Insert(1,laserOriginColour);

        // Results in [Red, Magenta, Blue] from a Magenta origin. If the user only wants Red and Blue, they should have bought a two splitter
        // Instead of a three. 

    }

    private void DetermineSixOutputs(LaserColour laserOriginColour)
    {
        // Calculate energy needed for all six lasers
        _energyPerLaser = Mathf.Clamp01((_incomingLasers[0].laser.GetTransparency() - _thisSatelliteInfo.advanced_Satellite_Info.absorbance) / 6);

        // This type of 6 origin laser 
        if (laserOriginColour == LaserColour.White)
        {
            // Adds all six colours available
            _laserColours.Add(LaserColour.Red);
            _laserColours.Add(LaserColour.Magenta);
            _laserColours.Add(LaserColour.Blue);
            _laserColours.Add(LaserColour.Cyan);
            _laserColours.Add(LaserColour.Green);
            _laserColours.Add(LaserColour.Yellow);
        }
        else
        {
            // Determine the two colours that have been split into
            DetermineTwoOutputs(laserOriginColour,true);

            // Duplicate the second colour twice (total of three)
            _laserColours.Add(_laserColours[1]);
            _laserColours.Add(_laserColours[1]);

            // Insert the first colour twice at index 0
            // Example of output [Red,Red,Red, Blue,Blue,Blue] instead of [Red,Blue,Red,Blue,Red,Blue] - mainly done by developer preference
            _laserColours.Insert(1,_laserColours[0]);
            _laserColours.Insert(1,_laserColours[0]);

        }

    }

}




[System.Serializable]
public class TwoOutputSplitterSettings
{
    public float angleAlpha = 38f;
    public float angleBeta = -38f;

}

[System.Serializable]
public class ThreeOutputSplitterSettings
{
    public float angleAlpha;
    public float angleBeta;
    public float angleGamma;
}

[System.Serializable]
public class SixOutputSplitterSettings
{
    public float angleAlpha;
    public float angleBeta;
    public float angleGamma;
    public float angleDelta;
    public float angleEpsilon;
    public float angleZeta;

}

