using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourFilterSatellite : SatelliteParent
{
    protected LaserColour filterColour = LaserColour.White;


    protected Animator _animator;

    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        base.Start();
    }
    
    public void SetFilterColour(LaserColour newFiterColour)
    {
        filterColour = newFiterColour;
    }

    override public void Interaction(IncomingLaser incomingLaser)
    {

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



        if (makeNewLaser)
        {
            OutgoingLaserInfo newOutGoingLaserInfo = new OutgoingLaserInfo();
            newOutGoingLaserInfo.angle = incomingLaser.laser.transform.eulerAngles.z;
            newOutGoingLaserInfo.origin = incomingLaser.raycast.point;
            newOutGoingLaserInfo.raycastPosition = incomingLaser.raycast.point;

            newOutGoingLaserInfo.laserTransparency = incomingLaser.laser.GetTransparency() - _thisSatelliteInfo.advanced_Satellite_Info.absorbance;
            
            if (filterColour == LaserColour.White)  newOutGoingLaserInfo.laserColour = incomingLaserColour;
            else if (changeColour != LaserColour.Null) newOutGoingLaserInfo.laserColour = changeColour;
            else newOutGoingLaserInfo.laserColour = filterColour;

            _outgoingLaserInfo.Add(newOutGoingLaserInfo);
        }
        
    }
}
