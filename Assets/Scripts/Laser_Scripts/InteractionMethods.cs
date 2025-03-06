using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Interaction
{
    SelfDetermine,
    Absorb,
    Reflection,
    Refraction,
    Splitter,
    Origin,
    Destination,
}

class InteractionFunctions
{
    public static float ReflectionInteraction(Transform laserTransform, RaycastHit2D raycast)
    {

        // Handles rotation about the normal, instead of surface area as previous
        float angleOfNormal = Vector2.SignedAngle(Vector2.up,raycast.normal);
        
        float laserAngle = laserTransform.eulerAngles.z;

        // Calculate the reflected vector
        var reflect = Vector2.Reflect(raycast.normal,laserTransform.up);

        // Convert the reflected normal into an angle from laser transform.up
        var angle = Vector2.SignedAngle(laserTransform.up,reflect);


        // Negates angle from the angle of the normal, For example if angle is negative the laser is coming from the right, hence the reflected is positive.
        return angleOfNormal - angle;
    }

    public static float RefractionInteraction(float incidentIndex,float refractedIndex,Vector2 normal, Laser laser)
    {

        // Using the normal vector that the raycast provides, we can get an angle through the use of Vector2.up 
        // which is assumed to be 0 degree rotation. Hence, as this is refraction, we can invert it to get the 
        // internal or external normal from the collision location.
        float inverseAngleOfNormal = Vector2.SignedAngle(Vector2.up,normal) + 180f;

        // Calculate the incident angle, based on the surface hit and the laser up vectors
        float incident_angle = Vector2.SignedAngle(laser.transform.up,normal);

        // Calculate the incident (after converting angle to radians)
        float incident = Mathf.Sin(incident_angle * (Mathf.PI / 180)) * incidentIndex;

        // Calculate the refracted angle (and convert from radians to angle)
        float refracted_angle = Mathf.Asin( (incident / refractedIndex)) * (180 / Mathf.PI);

        // Through using the inverse normal and the calculated refracted angle
        return inverseAngleOfNormal + refracted_angle;
    }

    public static void DestinationInteraction(Laser laser, RaycastHit2D raycast)
    {
        // Retrieve destination script
        var destination = raycast.collider.gameObject.GetComponent<LaserDestination>();

        // Call to advance lock, as interaction has occurred.
        if (destination != null) destination.AdvanceLock();
    }

    public static void SplitterInteraction(Laser laser, RaycastHit2D raycast)
    {

        SplitterSatellite splitterSatellite = raycast.collider.gameObject.GetComponent<SplitterSatellite>();
        splitterSatellite.SetLaser(laser,raycast);

    }

}