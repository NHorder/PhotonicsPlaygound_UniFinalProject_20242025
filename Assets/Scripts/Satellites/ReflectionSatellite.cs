using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionSatellite : SatelliteParent
{
    override public void Interaction(IncomingLaser incomingLaser)
    {

        Debug.Log("Interaction occurring!");

        float reflected_angle = InteractionFunctions.ReflectionInteraction(incomingLaser.laser.transform,incomingLaser.raycast);

        OutgoingLaserInfo outgoingLaserInfo = new OutgoingLaserInfo();
        outgoingLaserInfo.angle = reflected_angle;
        outgoingLaserInfo.origin = incomingLaser.raycast.point;
        outgoingLaserInfo.raycastPosition = incomingLaser.raycast.point;

        outgoingLaserInfo.laserTransparency = incomingLaser.laser.GetTransparency() - _thisSatelliteInfo.advanced_Satellite_Info.absorbance;

        _outgoingLaserInfo.Add(outgoingLaserInfo);
   
    }
}
