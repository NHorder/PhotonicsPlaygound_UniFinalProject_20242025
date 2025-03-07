using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefractionSatellite : SatelliteParent
{

    override public void Interaction(IncomingLaser incomingLaser)
    {
        var yOffset = 0.02f;

        float incidentIndex;
        float refractiveIndex; 
        var point = incomingLaser.raycast.point;
        Vector3 normal;

        Satellite_Info newSatelliteInfo = null;
        Satellite_Info refractionSatelliteInfo = incomingLaser.laser.refractionSatelliteInfo;

        if (refractionSatelliteInfo == null)
        {
            incidentIndex = 1;
            refractiveIndex = _thisSatelliteInfo.advanced_Satellite_Info.refractiveIndex;

            // If the point.y is different from what is known, apply an offset. 
            // This allows the point to consistently be within the refraction collider.
            if (point.y < this.transform.position.y) point.y += yOffset;
            else if (point.y > this.transform.position.y) point.y -= yOffset;

            // If the point.x is different from what is known, apply an offset. 
            // This allows the point to consistently be within the refraction collider.
            if (point.x < this.transform.position.x) point.y += yOffset;
            else if (point.x > this.transform.position.x) point.y -= yOffset;               

            newSatelliteInfo = _thisSatelliteInfo;
        }
        else
        {
            // Collect the refractive index of the satellite the laser is currently within
            incidentIndex = refractionSatelliteInfo.advanced_Satellite_Info.refractiveIndex;

            // Assumption we are exiting into a vacuum
            refractiveIndex = 1;

            // Shift if used to make sure the new raycast / laser won't rehit the exact same collider position as this raycast
            // Assumes that the object has a HOLLOW collider - made using polygonal colliders.

            // If the point.y is different from what is known, apply an offset. 
            // This allows the point to consistently be within the refraction collider.
            if (point.y < this.transform.position.y) point.y -= yOffset;
            else if (point.y > this.transform.position.y) point.y +=yOffset;
            
            // If the point.x is different from what is known, apply an offset. 
            // This allows the point to consistently be within the refraction collider.
            if (point.x < this.transform.position.x) point.y -= yOffset;
            else if (point.x > this.transform.position.x) point.y += yOffset;     
        }


        var refractedAngle = InteractionFunctions.RefractionInteraction(incidentIndex,refractiveIndex,incomingLaser.raycast.normal,incomingLaser.laser);

        if (!float.IsNaN(refractedAngle))
        {

            OutgoingLaserInfo newOutgoingLaser = new OutgoingLaserInfo();
            newOutgoingLaser.origin = point;
            newOutgoingLaser.angle = refractedAngle;
            newOutgoingLaser.raycastPosition = incomingLaser.raycast.point;
            newOutgoingLaser.satelliteInfo = newSatelliteInfo;
            newOutgoingLaser.laserColour = incomingLaser.laser.GetLaserColour();

            newOutgoingLaser.laserTransparency = incomingLaser.laser.GetTransparency() - _thisSatelliteInfo.advanced_Satellite_Info.absorbance; 

            _outgoingLaserInfo.Add(newOutgoingLaser);
        }


    }


}

                