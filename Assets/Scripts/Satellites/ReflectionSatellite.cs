using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionSatellite : SatelliteParent
{
    /// <summary>
    /// Class to handle reflection interactions with light
    /// Inherits Satellite Parent
    /// </summary>
    

    // Used for Fresnel Equations, where the power of reflected light is changed
    private float _advancedInteractionEnergy = 0f;


    /// <summary>
    /// Method to allow for advanced interaction updates
    /// </summary>
    /// <param name="energy"></param>
    public void SetAdvancedInteractionEnergyOverwrite(float energy)
    {
        _advancedInteractionEnergy = energy;
    }

    /// <summary>
    /// Overwritten inherited Interaction method, used to directly handle the interaction of light
    /// </summary>
    /// <param name="incomingLaser"></param>
    override public void Interaction(IncomingLaser incomingLaser)
    {
        // Call the linked interaction equation to calculate the angle of reflection
        float reflected_angle = InteractionFunctions.ReflectionInteraction(incomingLaser.laser.transform,incomingLaser.raycast);

        // Create a new outgoing laser and add to the protected outgoing laser list
        OutgoingLaserInfo newOutGoingLaserInfo = new OutgoingLaserInfo();
        newOutGoingLaserInfo.angle = reflected_angle;
        newOutGoingLaserInfo.origin = incomingLaser.raycast.point;
        newOutGoingLaserInfo.raycastPosition = incomingLaser.raycast.point;
        newOutGoingLaserInfo.laserColour = incomingLaser.laser.GetLaserColour();

        // Check to see if the energy / transparency needs overwriting
        if (_advancedInteractionEnergy == 0f) newOutGoingLaserInfo.laserTransparency = incomingLaser.laser.GetTransparency() - _thisSatelliteInfo.advanced_Satellite_Info.absorbance;
        else newOutGoingLaserInfo.laserTransparency =  _advancedInteractionEnergy;

        _outgoingLaserInfo.Add(newOutGoingLaserInfo);
   
    }
}
