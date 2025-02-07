using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Interaction_Functions
{
    public static float Reflection_Interaction(Transform laserTransform, RaycastHit2D rayCast)
    {
        float surfaceAngle = rayCast.collider.transform.eulerAngles.z;
        float laserAngle = laserTransform.eulerAngles.z;


        var reflect = Vector2.Reflect(rayCast.normal,laserTransform.up);
        var angle = Vector2.SignedAngle(laserTransform.up,reflect);


        return surfaceAngle - angle;
    }

    public static float Refraction_Interaction(float incident_index,float refracted_index,Vector2 normal, Laser laser)
    {

        float incident_angle = Vector2.SignedAngle(laser.transform.up,normal);

        float incident = Mathf.Sin(incident_angle * (Mathf.PI / 180)) * incident_index;

        float refracted_angle = Mathf.Asin( (incident / refracted_index)) * (180 / Mathf.PI);

        //Debug.Log("Incident Index:"+incident_index + " | Incident Angle: "+incident_angle+ " | Refracted Index: "+refracted_index+ " | Refracted Angle: "+refracted_angle);



        return laser.transform.rotation.eulerAngles.z + refracted_angle;
    }

}