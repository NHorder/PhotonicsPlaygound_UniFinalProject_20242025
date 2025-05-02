using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum to restrict the number of colours the laser can be
/// </summary>
public enum LaserColour{
    Null,
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
    /// <summary>
    /// Class used to create the laser visual and object. It also handles interactions with Satellites
    /// </summary>
    /// 
    

    // Determine the true origin of the satellite, connected to manually by developer
    public OriginSatellite origin;

    // The laser colour
    public LaserColour _laserColour;

    // The maximum distance the laser sprite can be extended to
    public float maxDistance;

    // Which layers the laser can be hit
    // Hidden in the Inspector, as this is copied from the Origin satellite
    [HideInInspector]
    public LayerMask layersToHit;

    // Transparency of the laser
    public float _transparency = 1;

    // Raycast that is saved for convienience
    private RaycastHit2D _raycast;

    // Refraction satellite information, in cases where light is within an object
    public SatelliteInfo refractionSatelliteInfo;

    // Settings for Frensel Equations
    private bool _allowReflectionDuringRefraction;

    // Settings for minimum for reflection to occur during refraction
    private float _minimumTransparencyForReflectionDuringRefraction;

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// Method to fire the laser into the scene, includes code taken and adapted from a tutorial. Expand method to see more
    /// </summary>
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

    /// <summary>
    /// Method to send a raycast into the scene to find a polygon collider (after a minimum distance)
    /// </summary>
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

    /// <summary>
    /// Method to determine light interaction
    /// Has parameters (with impossible defaults) for repeated calls - I.e Frensel Equations has refraction then call reflection
    /// </summary>
    /// <param name="remainingLightStrength"></param>
    /// <param name="interaction"></param>
    private void HitObject(float remainingLightStrength = -1, Interaction interaction = Interaction.SelfDetermine)
    {
        // Has defaults in order to allow for more complex interactions to occur, I.e Fresnel Equations (refraction and reflection)
        // within the same interaction cycle. Plus it allows for changing the remaining 'light strength' (transparency) of the second
        // or more interaction laser.

        // Retrieve the object that was hit
        var hitObject = _raycast.collider.gameObject;

        // Prepare satellite information
        SatelliteInfo satelliteInfo = null;

        // Try and retrieve the satellite infomation - not all collided objects have them
        try{
            satelliteInfo = hitObject.GetComponent<SatelliteInfo>();
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


            // If an satellite specific interaction, locate the related script and calls it's SetActive script
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
                // Retrieve destination script
                var destination = _raycast.collider.gameObject.GetComponent<DestinationSatellite>();

                // Call to advance lock, as interaction has occurred.
                if (destination != null) destination.AdvanceLock(_laserColour,_transparency);
                
            }

            else if (interaction == Interaction.Splitter)
            {
                SplitterSatellite splitterSatellite = _raycast.collider.gameObject.GetComponent<SplitterSatellite>();
                if (splitterSatellite != null) splitterSatellite.SetActive(this,_raycast);
                else Debug.LogError("Interaction deteccted, could not find component");
            }

            else if (interaction == Interaction.Combiner)
            {
                CombinerSatellite combinerSatellite = _raycast.collider.gameObject.GetComponent<CombinerSatellite>();
                combinerSatellite.SetActive(this,_raycast);
            }

            else if (interaction == Interaction.ColourFilter)
            {
                ColourFilterSatellite colourFilterSatellite = _raycast.collider.gameObject.GetComponent<ColourFilterSatellite>();
                colourFilterSatellite.SetActive(this,_raycast);
            }
        
            else if (interaction == Interaction.GravitationalAnomaly)
            {
                Singularity gravitationalAnomaly = _raycast.collider.gameObject.GetComponent<Singularity>();
                gravitationalAnomaly.SetActive(this,_raycast);
            }
        
        }
    }

    /// <summary>
    /// Method to get transparency of the laser
    /// </summary>
    /// <returns></returns>
    public float GetTransparency()
    {
        return _transparency;
    }

    /// <summary>
    /// Method to set transparency of the lasser
    /// </summary>
    /// <param name="transparency"></param>
    public void SetTransparency(float transparency)
    {
        _transparency = transparency;

        // Retrieve sprite renders colour, and modify the alpha value to that of the transparency
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Color modifiedColour = spriteRenderer.color;
        modifiedColour.a = transparency;

        // Update the renderes colour
        spriteRenderer.color = modifiedColour;
    }

    /// <summary>
    /// Method to get laser colour
    /// </summary>
    /// <returns></returns>
    public LaserColour GetLaserColour()
    {
        return _laserColour;
    }

    /// <summary>
    /// Method to set laser colour
    /// </summary>
    /// <param name="laserColour"></param>
    public void SetLaserColour(LaserColour laserColour)
    {
        _laserColour = laserColour;

        var colourID = 0;

        if (laserColour == LaserColour.Null) SetTransparency(0f);
        else if (laserColour == LaserColour.White) colourID = 0;
        else if (laserColour == LaserColour.Red) colourID = 1;
        else if (laserColour == LaserColour.Blue) colourID = 2;
        else if (laserColour == LaserColour.Green) colourID = 3;
        else if (laserColour == LaserColour.Cyan) colourID = 4;
        else if (laserColour == LaserColour.Yellow) colourID = 5;
        else if (laserColour == LaserColour.Magenta) colourID = 6;
        else Debug.LogWarning("WARNING: Laser colour not known");

        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetInteger("colourID",colourID);
    }
}
