using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourFilterSatellite : SatelliteParent
{
    /// <summary>
    /// Class used for the colour filter satellite interactions
    /// Inherits SatelliteParent class
    /// </summary>
    


    protected LaserColour filterColour = LaserColour.White;

    protected Animator _animator;


    /// <summary>
    /// Initialisation Script
    /// </summary>
    void Start()
    {
        // Retrieve animation component
        if (_animator == null) _animator = gameObject.GetComponent<Animator>();

        // Call inherited start
        base.Start();
    }
    
    /// <summary>
    /// Method to set the filters colour
    /// </summary>
    /// <param name="newFiterColour"></param>
    public void SetFilterColour(LaserColour newFiterColour)
    {
        filterColour = newFiterColour;
    }

    /// <summary>
    /// Method handling light interactions
    /// </summary>
    /// <param name="incomingLaser"></param>
    override public void Interaction(IncomingLaser incomingLaser)
    {
        // Retrieve the incoming laser colour
        var incomingLaserColour = incomingLaser.laser.GetLaserColour();
        var makeNewLaser = false;
        LaserColour changeColour = LaserColour.Null;

        // If the filter or the laser is white OR they equal each other than make a new laser
        if ( filterColour == LaserColour.White || incomingLaserColour == LaserColour.White || filterColour == incomingLaserColour) makeNewLaser = true;

        // If the filter is red, and the incoming laser involves red, then create a new laser
        else if (incomingLaserColour == LaserColour.Magenta && filterColour == LaserColour.Red) makeNewLaser = true;
        else if (incomingLaserColour == LaserColour.Yellow && filterColour == LaserColour.Red) makeNewLaser = true;

        // If the filter is blue, and the incoming laser involves blue, then create a new laser
        else if (incomingLaserColour == LaserColour.Cyan && filterColour == LaserColour.Blue) makeNewLaser = true;
        else if (incomingLaserColour == LaserColour.Magenta && filterColour == LaserColour.Blue) makeNewLaser = true;

        // If the filter is green, and the incoming laser involves green, then create a new laser
        else if (incomingLaserColour == LaserColour.Yellow && filterColour == LaserColour.Green) makeNewLaser = true;
        else if (incomingLaserColour == LaserColour.Cyan && filterColour == LaserColour.Green)  makeNewLaser = true;

        // If the filter is cyan, and the incoming laser involves blue or green, then create a new laser
        else if ((incomingLaserColour == LaserColour.Green || incomingLaserColour == LaserColour.Blue) && filterColour == LaserColour.Cyan)
        {
            makeNewLaser = true;
            changeColour = incomingLaserColour;
        }

        // If the filter is magenta, and the incoming laser involves blue or red, then create a new laser
        else if ((incomingLaserColour == LaserColour.Blue || incomingLaserColour == LaserColour.Red) && filterColour == LaserColour.Magenta)
        {
            makeNewLaser = true;
            changeColour = incomingLaserColour;
        }

        // If the filter is yellow, and the incoming laser involves green or red, then create a new laser
        else if ((incomingLaserColour == LaserColour.Red || incomingLaserColour == LaserColour.Green) && filterColour == LaserColour.Yellow)
        {
            makeNewLaser = true;
            changeColour = incomingLaserColour;
        }

        // If a new laser is needed, then create a new outgoing laser
        if (makeNewLaser)
        {
            // Create a new outgoing laser object and set parameters
            OutgoingLaserInfo newOutGoingLaserInfo = new OutgoingLaserInfo();
            newOutGoingLaserInfo.angle = incomingLaser.laser.transform.eulerAngles.z;
            newOutGoingLaserInfo.origin = incomingLaser.raycast.point;
            newOutGoingLaserInfo.raycastPosition = incomingLaser.raycast.point;
            newOutGoingLaserInfo.laserTransparency = incomingLaser.laser.GetTransparency() - _thisSatelliteInfo.advanced_Satellite_Info.absorbance;

            // If the filter colour is white, there is no change in colour
            if (filterColour == LaserColour.White)  newOutGoingLaserInfo.laserColour = incomingLaserColour;
            else if (changeColour != LaserColour.Null) newOutGoingLaserInfo.laserColour = changeColour;
            else newOutGoingLaserInfo.laserColour = filterColour;

            // Add outgoing laser to protected ougoing laser info list.
            _outgoingLaserInfo.Add(newOutGoingLaserInfo);
        }
        
    }
}
