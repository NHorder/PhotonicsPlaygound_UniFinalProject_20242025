using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionSatellite : SatelliteParent
{
    private float _advancedInteractionEnergy = 0f;


    public void SetAdvancedInteractionEnergyOverwrite(float energy)
    {
        _advancedInteractionEnergy = energy;
    }

    override public void Interaction(IncomingLaser incomingLaser)
    {

        float reflected_angle = InteractionFunctions.ReflectionInteraction(incomingLaser.laser.transform,incomingLaser.raycast);

        OutgoingLaserInfo newOutGoingLaserInfo = new OutgoingLaserInfo();
        newOutGoingLaserInfo.angle = reflected_angle;
        newOutGoingLaserInfo.origin = incomingLaser.raycast.point;
        newOutGoingLaserInfo.raycastPosition = incomingLaser.raycast.point;

        if (_advancedInteractionEnergy == 0f) newOutGoingLaserInfo.laserTransparency = incomingLaser.laser.GetTransparency() - _thisSatelliteInfo.advanced_Satellite_Info.absorbance;
        else newOutGoingLaserInfo.laserTransparency =  _advancedInteractionEnergy;
        newOutGoingLaserInfo.laserColour = incomingLaser.laser.GetLaserColour();

        _outgoingLaserInfo.Add(newOutGoingLaserInfo);
   
    }
}
