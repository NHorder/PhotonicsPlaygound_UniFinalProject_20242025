using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum LaserColour{
    White,

}

public class Laser : MonoBehaviour
{
    public LaserOrigin origin;

    public float maxDistance;

    [HideInInspector]
    public LayerMask layersToHit;


    private float _transparency = 1;
    
    private RaycastHit2D _raycast;
    private Vector3 _laserCoordinates;
    private Satellite_Info _refractionSatelliteInfo;
    private bool _allowReflectionDuringRefraction;

    private float _minimumTransparencyNeededForDestinationRecognition;
    private float _minimumTransparencyForReflectionDuringRefraction;

    // Start is called before the first frame update
    void Start()
    {
        // Laser requires an origin in order to function, complete a check or throw error.
        if (origin != null)
        {
            // Copy the layers that can interact with the laser
            this.layersToHit = origin.layersToHit;

            // Copy the maximum distance allowed by the origin
            this.maxDistance = origin.maxDistance;

            // Retrieve the game controller from the origin
            var gameController = origin.GetGameController();

            // Retrieve Specialized interactions and settings from game controller.
            _allowReflectionDuringRefraction = gameController.specializedInteractionSettings.allowReflectionDuringRefraction;
            _minimumTransparencyNeededForDestinationRecognition = gameController.specializedInteractionSettings.minimumTransparencyNeededForDestinationRecognition;
            _minimumTransparencyForReflectionDuringRefraction = gameController.specializedInteractionSettings.minimumTransparencyForReflectionDuringRefraction;

            // Force reset the scale, as the prefab laser may have different scales
            transform.localScale = new Vector3(1,1,1);

            // Begin the process and fire the laser
            FireLaser();

        }
        else
        {
            Debug.LogError("ERROR: Laser has no defined origin");
        }

        
    }

    // Update is called once per frame
    public void FireLaser()
    {

        //// Please Note that the laser scale changes are an idea taken and adapted from: 
        //// https://www.youtube.com/watch?si=Tk7TxG4l7wRFz6ek&v=Z49UPByGEKE&feature=youtu.be (MaxMakesGames) (2025/02/28)
        //// that makes use of extending the sprite by distance units instead of using a line renderer as shown in other solutions

        // Call Raycast to see if anything will be hit
        this.Raycast();

        // check that raycast has been cast
        if (_raycast != null)
        {

            // If the collider is null, then nothing has been hit, set y scale to max distance
            if (_raycast.collider == null)
            {
                this.transform.localScale = new Vector3(1f,maxDistance,1f);
            }
            // Else, something has been hit, set scale to that object
            else
            {
                this.transform.localScale = new Vector3(1f,_raycast.distance,1f);

                // Something has been hit, call for interaction
                HitObject();
            }
        }
        
    }

    private void Raycast()
    {
        // Cast a ray and determine what objects (if any) are hit
        var listOfRayCasts= Physics2D.RaycastAll(transform.position,transform.up,maxDistance,layersToHit);

        // Loop through each
        foreach (RaycastHit2D rayCastInLoop in listOfRayCasts)
        {

            // Check that the distance from the ray origin and the object is more than minimum (defined in gameController)
            // AND that the collider is of type polygon2d 
            if (rayCastInLoop.distance > 0.01 && (rayCastInLoop.collider is PolygonCollider2D))
            {
                // Set the raycast and break the loop, as it's the first found
                _raycast = rayCastInLoop;
                break;
            }

            // PolygonCollider2D in this project / code is specifically used for light based interactions
            // BoxCollider2D is used for user interactions (I.e selecting a satellite)
        }

        // Draw the ray for debug developer purposes, only noticible within editor view
        Debug.DrawRay(transform.position, transform.up, Color.black, 0.01f, true);
    }


    private void HitObject(float remainingLightStrength = -1, Interaction interaction = Interaction.SelfDetermine)
    {
        // Has defaults in order to allow for more complex interactions to occur, I.e Fresnel Equations (refraction and reflection)
        // within the same interaction cycle. Plus it allows for changing the remaining 'light strength' (transparency) of the second
        // or more interaction laser.

        // Retrieve the object that was hit
        var hitObject = _raycast.collider.gameObject;

        // Prepare satellite information
        Satellite_Info satelliteInfo = null;

        // Try and retrieve the satellite infomation - not all collided objects have them
        try{
            satelliteInfo = hitObject.GetComponent<Satellite_Info>();
        }
        catch
        {
            // Don't catch the error as this interaction is expected
        }
        
        // if it does have satellite information, begin interaction specific checks
        if (satelliteInfo != null)
        {
            // If the interaction is a default (not overwritten by a prior interaction) (SelfDetermine) then retrieve the interaction from satellite info
            if (interaction == Interaction.SelfDetermine) interaction = satelliteInfo.interaction;

            // Check to make sure the absorbance is a percentage rather than an unknown number
            if (satelliteInfo.advanced_Satellite_Info.absorbance > 1) satelliteInfo.advanced_Satellite_Info.absorbance = 1;
            else if (satelliteInfo.advanced_Satellite_Info.absorbance < 0) satelliteInfo.advanced_Satellite_Info.absorbance = 0;

            // Calcualte the transparency for the new laser, clamp limits it to 2dp for effiecincy purposes (prevents large 
            // amounts of data allocated for a single number)
            if (remainingLightStrength == -1) remainingLightStrength = Mathf.Clamp01(_transparency * (1- satelliteInfo.advanced_Satellite_Info.absorbance));


            
            if (interaction == Interaction.SelfDetermine)
            {
                // Do Nothing, this is not a valid interaction, notify that this is an error
                Debug.LogError("ERROR: An error has occurred when assigning this satellites interaction");
            }
            
            else if (interaction == Interaction.Absorb || _transparency <= 0 || remainingLightStrength <= 0)
            {
                // Do Nothing, as the light is completly absorbed
            }

            else if (interaction == Interaction.Reflection)
            {
                // Calculate the rotation angle
                float rotateAngle = InteractionFunctions.ReflectionInteraction(this.transform,_raycast);

                // Instantiate new laser
                GameObject newLaser = InstantiateNewLaser(new Vector3(_raycast.point.x, _raycast.point.y,-2), rotateAngle,remainingLightStrength);

                // Connect new laser to the origin
                origin.AddLaser(newLaser,_raycast.point);
            }

            else if (interaction == Interaction.Refraction)
            {
                // Defines a small minimal offset, this is to make sure the raycast does not re-collide the same collider
                // Assumes collider is hollow and thickness is small.
                var yOffset = 0.01f;
                var xOffset = 0.01f;

                // Definition of some intial information that's needed regardless of rerfraction type       
                float incidentIndex;
                float refractiveIndex; 
                var point = _raycast.point;
                Vector3 normal;

                // This is default to null as assumption made is that the laser incident or refraction is in a vacuum. Hence it links to a satellite's info, the new
                // laser should not have associated satellite info. Null checks are done, so it doesn't cause problems.
                Satellite_Info newSatelliteInfo = null;

                // Diffraction when entering a refracton satellite with a higher refractive index than a vacuum
                if (_refractionSatelliteInfo == null)
                {
                    // Assumption that we are in a vacuum
                    incidentIndex = 1;

                    // Collect the refractive index from the linked satellite info we hit using the rayCast earlier
                    refractiveIndex = satelliteInfo.advanced_Satellite_Info.refractiveIndex;

                    // Shift if used to make sure the new raycast / laser won't rehit the exact same collider position as this raycast
                    // Assumes that the object has a HOLLOW collider - made using polygonal colliders.

                    // Apply a shift on the y axis as needed
                    if (point.y < hitObject.transform.position.y) point.y += yOffset;
                    else if (point.y > hitObject.transform.position.y) point.y -= yOffset;
                    
                    // Apply a shift on the x axis as needed
                    if (point.x < hitObject.transform.position.x) point.x += xOffset;
                    else if (point.x > hitObject.transform.position.x) point.x -= xOffset;               

                    // Set satellite info
                    newSatelliteInfo = satelliteInfo;
                }

                // Diffraction when exiting a satellite with a higher refractive index than a vacuum
                else
                {
                    // Collect the refractive index of the satellite the laser is currently within
                    incidentIndex = _refractionSatelliteInfo.advanced_Satellite_Info.refractiveIndex;

                    // Assumption we are exiting into a vacuum
                    refractiveIndex = 1;

                    // Shift if used to make sure the new raycast / laser won't rehit the exact same collider position as this raycast
                    // Assumes that the object has a HOLLOW collider - made using polygonal colliders.

                    // Apply a shift on the y axis if needed
                    if (point.y < hitObject.transform.position.y) point.y -= yOffset;
                    else if (point.y > hitObject.transform.position.y) point.y +=yOffset;
                    
                    // Apply a shift on the x axis if needed
                    if (point.x < hitObject.transform.position.x) point.x -= xOffset;
                    else if (point.x > hitObject.transform.position.x) point.x += xOffset;     
                }

                // Calculate the refracted angle through interaction functions
                var refractedAngle = InteractionFunctions.RefractionInteraction(incidentIndex,refractiveIndex,_raycast.normal,this);

                // If the refracted angle is not NaN - NaN occurs when total internal reflection occurs.
                if (!float.IsNaN(refractedAngle))
                {

                    // Default reflected light strength
                    float reflectedLightStrength = 0;

                    // If the setting is allowed, then calculate the reflected light strength (transparency)
                    // The light setting is used to show Fresnel Equations, which determines reflections during refraction
                    if (_allowReflectionDuringRefraction && _refractionSatelliteInfo == null)
                    {
                        // Calculate the reflected power using 'normal incidence' Fresnel Equation - as polarisation of the light is not considered.
                        float reflectedPower = Mathf.Pow(Mathf.Abs((incidentIndex - refractiveIndex) / (incidentIndex + refractiveIndex)),2);

                        // Determine the relected light strength.
                        reflectedLightStrength = Mathf.Clamp01(remainingLightStrength * reflectedPower);
                    }

                    // Calculate the refracted light strength (transparency)
                    float refractedLightStrength = Mathf.Clamp01(remainingLightStrength - reflectedLightStrength);

                    // Instantiate a new laser
                    GameObject newLaser = InstantiateNewLaser(point,refractedAngle,refractedLightStrength);

                    // Update the laser satellite info - based on whether leaving or entering
                    // This may be redundant when leaving, as the prefab laser doesn't have any links to satellites. I think of it as confirmation that it's doing what it should be.
                    newLaser.GetComponent<Laser>()._refractionSatelliteInfo = newSatelliteInfo;

                    // Add laser to origin
                    origin.AddLaser(newLaser,point);//rayCast.point);             

                    // If setting is allowed and the strength is not below the minimum needed for reflection during refraction
                    // Defaults to 0.02f  (2% transparency)
                    if (_allowReflectionDuringRefraction && reflectedLightStrength > _minimumTransparencyForReflectionDuringRefraction)
                    {
                        // Call HitObject again, this time overwriting the interaction and strength - forcing this interaction type to occur
                        HitObject(reflectedLightStrength,Interaction.Reflection);
                    }
                }            
            }
        
            else if (interaction == Interaction.Destination)
            {
                
                if (_transparency >= _minimumTransparencyNeededForDestinationRecognition) InteractionFunctions.DestinationInteraction(this,_raycast);
            }

        }
    }

    
    public GameObject InstantiateNewLaser(Vector2 position, float angle, float remainingEnergy)
    {

        // Instatiate a new laser, using the prefab laser
        var newLaser = Instantiate(origin.prefabLaser);

        // Update it's components based on this laser's private variables
        newLaser.GetComponent<Laser>().origin = this.origin;
        newLaser.GetComponent<Laser>()._transparency = remainingEnergy;

        // Collect the new lasers colour and overwrite it results in the new laser being 
        // more transparent
        // NOTE: The actual Hex colour values are NOT changed, only thte alpha value
        var overwrittenColor =newLaser.GetComponent<SpriteRenderer>().color;
        overwrittenColor.a = remainingEnergy;
        newLaser.GetComponent<SpriteRenderer>().color = overwrittenColor;


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
}
