using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    public Satellite_Info refractionSatelliteInfo;


    public bool hitSomething;


    public LaserOrigin origin;

    private Vector3 laserCoordinates;

    public float energy;

    private LayerMask layersToHit;
    private RaycastHit2D rayCast;

    // Start is called before the first frame update
    void Start()
    {
        if (origin != null)
        {
            this.layersToHit = origin.layersToHit;
            this.energy = origin.startingEnergy;
        }
        else
        {
            Debug.Log("ERROR: Laser has no defined origin");
        }

        transform.localScale = new Vector3(1,1,1);

        FireLaser();
    }

    public GameObject InstantiateNewLaser(Vector2 position, float angle)
    {
        

        // Instatiate a new laser, using the prefab first laser
        GameObject newLaser = Instantiate(origin.firstLaser);

        // Set the new laser position at the provided position
        newLaser.transform.position = new Vector3(position.x, position.y,-2);

        // Rotate new laser by the provided angle
        newLaser.transform.Rotate(0f,0f,angle);

        // Reset the scale - scale is modified during execution to ensure correct size of the laser
        newLaser.transform.localScale = new Vector3(1f,1f,1f);

        // Return new laser object
        return newLaser;

        // NOTE: Adding the new laser to the origin is not included in this instantiation as some additional modifications may be needed
        // by specific interactions, I.e refraction changing the satellite info, as such this allows a brief delay to update that before adding it
        // it prevents bugs / unexpected events from occuring.

    }

    // Update is called once per frame
    public void FireLaser()
    {
        Debug.Log("Firing Laser!");
        this.RayCast();

        if (origin != null && rayCast != null)
        {
            if (rayCast.collider == null)
            {
                hitSomething = false;
                Debug.Log("No Object Hit!");
                this.transform.localScale = new Vector3(1f,energy,1f);
            }
            else
            {
                //Debug.Log(rayCast.collider is PolygonCollider2D);

                hitSomething = true;
                this.transform.localScale = new Vector3(1f,rayCast.distance,1f);

                HitObject();
            }


        }
        else{
            Debug.Log("No Origin!");
        }
        
    }

    private void RayCast()
    {
        // Cast a ray and determine what objects (if any) are hit
        var listOfRayCasts= Physics2D.RaycastAll(transform.position,transform.up,energy,layersToHit);

        foreach (RaycastHit2D rayCastInLoop in listOfRayCasts)
        {

            if (rayCastInLoop.distance > 0.1 && (rayCastInLoop.collider is PolygonCollider2D))
            {
                rayCast = rayCastInLoop;
                break;
            }
        }

        Debug.DrawRay(transform.position, transform.up, Color.black, 0.01f, true);
    }


    private void HitObject()
    {
        GameObject hitObject = rayCast.collider.gameObject;
        Satellite_Info satelliteInfo = null;

        try{
            satelliteInfo = hitObject.GetComponent<Satellite_Info>();
        }
        catch
        {
            // Do Nothing
        }

        if (satelliteInfo != null)
        {
            Interaction interaction = satelliteInfo.interaction;

            if (interaction == Interaction.Absorb)
            {
                // Do Nothing
            }
            else if (interaction == Interaction.Reflection)
            {

                // Calculate the rotation angle
                var rotateAngle = Interaction_Functions. Reflection_Interaction(this.transform,rayCast);

                // Instantiate new laser
                GameObject newLaser = InstantiateNewLaser(new Vector3(rayCast.point.x, rayCast.point.y,-2), rotateAngle);

                // Connect new laser to the origin
                origin.AddLaser(newLaser,rayCast.point);
            }

            else if (interaction == Interaction.Refraction)
            {
                float incident_index;
                float refractive_index;

                // Defines a small minimal offset, this is to make sure the raycast does not re-collide the same collider
                // Assumes 
                float yOffset = 0.01f;
                float xOffset = 0.01f;

                // Definition of some intial information that's needed regardless of rerfraction type        
                var point = rayCast.point;
                Vector3 normal;

                // This is default to null as assumption made is that the laser incident or refraction is in a vacuum. Hence it links to a satellite's info, the new
                // laser should not have associated satellite info. Null checks are done, so it doesn't cause problems.
                Satellite_Info newSatelliteInfo = null;

                // Diffraction when entering a refracton satellite with a higher refractive index than a vacuum
                if (refractionSatelliteInfo == null)
                {
                    // Assumption that we are in a vacuum
                    incident_index = 1;

                    // Collect the refractive index from the linked satellite info we hit using the rayCast earlier
                    refractive_index = satelliteInfo.refractiveIndex;

                    // Shift if used to make sure the new raycast / laser won't rehit the exact same collider position as this raycast
                    // Assumes that the object has a HOLLOW collider - made using polygonal colliders.

                    // Apply a shift on the y axis as needed
                    if (point.y < hitObject.transform.position.y) point.y += yOffset;
                    else if (point.y > hitObject.transform.position.y) point.y -= yOffset;
                    
                    // Apply a shift on the x axis as needed
                    if (point.x < hitObject.transform.position.x) point.x += xOffset;
                    else if (point.x > hitObject.transform.position.x) point.x -= xOffset;               

                    newSatelliteInfo = satelliteInfo;
                }

                // Diffraction when exiting a satellite with a higher refractive index than a vacuum
                else
                {
                    // Collect the refractive index of the satellite the laser is currently within
                    incident_index = refractionSatelliteInfo.refractiveIndex;

                    // Assumption we are exiting into a vacuum
                    refractive_index = 1;

                    // Shift if used to make sure the new raycast / laser won't rehit the exact same collider position as this raycast
                    // Assumes that the object has a HOLLOW collider - made using polygonal colliders.

                    // Apply a shift on the y axis if needed
                    if (point.y < hitObject.transform.position.y) point.y -= yOffset;
                    else if (point.y > hitObject.transform.position.y) point.y +=yOffset;
                    
                    // Apply a shift on the x axis if needed
                    if (point.x < hitObject.transform.position.x) point.x -= xOffset;
                    else if (point.x > hitObject.transform.position.x) point.x += xOffset;     
                }

                var refracted_angle = Interaction_Functions.Refraction_Interaction(incident_index,refractive_index,rayCast.normal,this);

                if (!float.IsNaN(refracted_angle))
                {

                    // THIS ROTATION ANGLE IS WRONG - the refracted angle is correct. But I don't know what I need to do to calculate how to get to the right rotation angle.
                    //var rotateAngle = this.transform.eulerAngles.z + refracted_angle;


                    // Instantiate a new laser
                    GameObject newLaser = InstantiateNewLaser(point,refracted_angle);

                    // Update the laser satellite info - based on whether leaving or entering
                    // This may be redundant when leaving, as the prefab laser doesn't have any links to satellites. I think of it as confirmation that it's doing what it should be.
                    newLaser.GetComponent<Laser>().refractionSatelliteInfo = newSatelliteInfo;

                    // Add laser to origin
                    origin.AddLaser(newLaser,rayCast.point);                    
                }            
            }
        }
    }
}
