using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum LaserColour{
    White,
    Red,
    Green,
    Blue,
    Cyan,
    Yellow,
    Magenta,

}

public class Laser : MonoBehaviour
{
    public LaserOrigin origin;
    public LaserColour laserColour;

    public float maxDistance;

    [HideInInspector]
    public LayerMask layersToHit;


    private float _transparency = 1;
    
    private RaycastHit2D _raycast;
    private Vector3 _laserCoordinates;
    public Satellite_Info refractionSatelliteInfo;
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
            if (rayCastInLoop.distance > 0.05 && (rayCastInLoop.collider is PolygonCollider2D))
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
                ReflectionSatellite reflectionSatellite = _raycast.collider.gameObject.GetComponent<ReflectionSatellite>();
                reflectionSatellite.SetActive(this,_raycast);
            }

            else if (interaction == Interaction.Refraction)
            {
                RefractionSatellite refractionSatellite = _raycast.collider.gameObject.GetComponent<RefractionSatellite>();
                refractionSatellite.SetActive(this,_raycast);
            }
        
            else if (interaction == Interaction.Destination)
            {
                
                if (_transparency >= _minimumTransparencyNeededForDestinationRecognition) InteractionFunctions.DestinationInteraction(this,_raycast);
            }





            else if (interaction == Interaction.Splitter)
            {
                SplitterSatellite splitterSatellite = _raycast.collider.gameObject.GetComponent<SplitterSatellite>();
                splitterSatellite.SetActive(this,_raycast);
            }

            else if (interaction == Interaction.Combiner)
            {
                CombinerSatellite combinerSatellite = _raycast.collider.gameObject.GetComponent<CombinerSatellite>();
                combinerSatellite.SetActive(this,_raycast);
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

    public float GetTransparency()
    {
        return _transparency;
    }

    public void SetTransparency(float transparency)
    {
        _transparency = transparency;
    }

}
