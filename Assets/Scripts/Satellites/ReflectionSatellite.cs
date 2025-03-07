using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionSatellite : SatelliteParent
{
    override public void Interaction(IncomingLaser incomingLaser)
    {

        float reflected_angle = InteractionFunctions.ReflectionInteraction(incomingLaser.laser.transform,incomingLaser.raycast);

        OutgoingLaserInfo newOutGoingLaserInfo = new OutgoingLaserInfo();
        newOutGoingLaserInfo.angle = reflected_angle;
        newOutGoingLaserInfo.origin = incomingLaser.raycast.point;
        newOutGoingLaserInfo.raycastPosition = incomingLaser.raycast.point;

        newOutGoingLaserInfo.laserTransparency = incomingLaser.laser.GetTransparency() - _thisSatelliteInfo.advanced_Satellite_Info.absorbance;
        newOutGoingLaserInfo.laserColour = incomingLaser.laser.GetLaserColour();

        _outgoingLaserInfo.Add(newOutGoingLaserInfo);
   
    }
}
