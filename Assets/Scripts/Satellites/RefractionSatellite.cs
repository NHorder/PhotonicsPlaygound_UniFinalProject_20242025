using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefractionSatellite : SatelliteParent
{
    /// <summary>
    /// Class to handle refraction interactions
    /// </summary>
    
    /// <summary>
    /// Overwritten inherited method to handle interaction
    /// </summary>
    /// <param name="incomingLaser"></param>
    override public void Interaction(IncomingLaser incomingLaser)
    {
        var offset = 0.02f;

        float incidentIndex;
        float refractiveIndex; 
        var point = incomingLaser.raycast.point;
        Vector3 normal;

        SatelliteInfo newSatelliteInfo = null;
        SatelliteInfo refractionSatelliteInfo = incomingLaser.laser.refractionSatelliteInfo;

        if (refractionSatelliteInfo == null)
        {
            incidentIndex = 1;
            refractiveIndex = _thisSatelliteInfo.advanced_Satellite_Info.refractiveIndex;

            // If the point.y is different from what is known, apply an offset. 
            // This allows the point to consistently be within the refraction collider.
            if (point.y < this.transform.position.y) point.y += offset;
            else if (point.y > this.transform.position.y) point.y -= offset;

            // If the point.x is different from what is known, apply an offset. 
            // This allows the point to consistently be within the refraction collider.
            if (point.x < this.transform.position.x) point.x += offset;
            else if (point.x > this.transform.position.x) point.x -= offset;               

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
            if (point.y < this.transform.position.y) point.y -= offset;
            else if (point.y > this.transform.position.y) point.y +=offset;
            
            // If the point.x is different from what is known, apply an offset. 
            // This allows the point to consistently be within the refraction collider.
            if (point.x < this.transform.position.x) point.x -= offset;
            else if (point.x > this.transform.position.x) point.x += offset;     
        }

        // Determine the refracted angle from the interaction functions
        var refractedAngle = InteractionFunctions.RefractionInteraction(incidentIndex,refractiveIndex,incomingLaser.raycast.normal,incomingLaser.laser);

        // Set expected refraction energy (if no Fresnel Equations)
        var refractedEnergy = incomingLaser.laser.GetTransparency() - _thisSatelliteInfo.advanced_Satellite_Info.absorbance;

        // If using Fresnel Equations
        if (_gameController.specializedInteractionSettings.allowReflectionDuringRefraction)
        {
            ReflectionSatellite reflectionSatellite = null;
            gameObject.TryGetComponent<ReflectionSatellite>(out reflectionSatellite);

            if (reflectionSatellite != null)
            {
                // Fresnels Equations |  Special Cases | Normal Incidence
                // Using the Normal Incidence special case from Fresnel law, as we are not concerned with the
                // polarisation of the light, hence we can use the special case to caluclate the power of reflectance
                float powerReflectance = Mathf.Clamp01(Mathf.Pow((incidentIndex - refractiveIndex) / (incidentIndex + refractiveIndex),2f));

                // Using the refrated energy (which has already absorbed the energy from the laser), calculate the energy for the reflected
                // and then negate that from the refracted.
                var reflectedEnergy = refractedEnergy * powerReflectance;
                refractedEnergy -= reflectedEnergy;

                // In cases of glass, the powerReflectance would be roughly 0.04 (4%), hence it's more of a aestetic choice over function.
                // Though it could become functional given enough combiners...

                // Call the attached reflection satellite information, overwrite it's advanced interaction energy (from 0) and set it to active
                // this will make display the reflection the same way refraction happens.
                reflectionSatellite.SetAdvancedInteractionEnergyOverwrite(reflectedEnergy);
                reflectionSatellite.SetActive(incomingLaser.laser,incomingLaser.raycast);
            }
        }

        // If the angle is not NaN (which it will be for cases of complete internal reflection)
        if (!float.IsNaN(refractedAngle))
        {
            // Create a new outgoing laser, update it's variables then add it to the protected outgoing laser list
            OutgoingLaserInfo newOutgoingLaser = new OutgoingLaserInfo();
            newOutgoingLaser.origin = point;
            newOutgoingLaser.angle = refractedAngle;
            newOutgoingLaser.raycastPosition = incomingLaser.raycast.point;
            newOutgoingLaser.satelliteInfo = newSatelliteInfo;
            newOutgoingLaser.laserColour = incomingLaser.laser.GetLaserColour();
            newOutgoingLaser.laserTransparency = refractedEnergy;
            _outgoingLaserInfo.Add(newOutgoingLaser);

        }

    }
}

                