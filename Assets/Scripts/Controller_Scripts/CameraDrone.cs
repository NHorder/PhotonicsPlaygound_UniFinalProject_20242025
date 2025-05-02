using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrone : MonoBehaviour
{
    /// <summary>
    /// Level Camera Drone, allows a satellite to control and move the scenes MainCamera
    /// </summary>


    // Settings for the drone
    // Note: Drone speed in both settings overwrite the SatelliteInfo settings.
    // Intended to allow the drone to be faster than other satellites.
    public EyeOfZetaCameraDroneSettings eyeOfZetaCameraDroneSettings;
    public EyeOfZetaAutomaticDroneSettings eyeOfZetaAutomaticDroneSettings;

    private GameObject _camera;
    private GameController _gameController;


    // The edges of which the camera can move (Defaults)
    private float _minCameraX = -20f;
    private float _maxCameraX = 20f;
    private float _minCameraY = -20f;
    private float _maxCameraY = 20f;

    // Saved bools for attached or parented to another satellite
    private bool _attached = false;
    private bool _parented = false; 


    // Related links
    private Transform _linkedSatelliteTransform;
    private Rigidbody2D _droneRigidbody2D;
    private float _currentMovementSpeed;


    /// <summary>
    /// Initialisation Method
    /// </summary>
    void Start()
    {
        // Locate main camera, the main camera will follow the drone position
        _camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];

        // Locate game controller
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

        _droneRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _currentMovementSpeed = eyeOfZetaAutomaticDroneSettings.movementSpeed;

        // Retrieve satellite information and update movement modifiers - this allows for the SatelliteController code to be reused
        var satelliteInfo = this.gameObject.GetComponent<SatelliteInfo>();
        satelliteInfo.satellite_Movement_Info.intialMovementMultiplier = eyeOfZetaCameraDroneSettings.cameraDroneMovementSpeed;
        satelliteInfo.satellite_Movement_Info.maxMovementMultiplier = eyeOfZetaCameraDroneSettings.cameraDroneMovementSpeed + 5;

        _minCameraX = eyeOfZetaCameraDroneSettings.minimumXPosition;
        _maxCameraX = eyeOfZetaCameraDroneSettings.maximumXPosition;

        _minCameraY = eyeOfZetaCameraDroneSettings.minimumYPosition;
        _maxCameraY = eyeOfZetaCameraDroneSettings.maximumYPosition;

    }

    /// <summary>
    /// Update method. This is called once per system tick - which on 60 frames is 1 per second
    /// Handles automatic drone movement and movement of the main camera
    /// </summary>
    void Update()
    {
        /// Retrieve the drone position
        Vector3 dronePosition = this.transform.position;

        /// IF attached to a satellite and the linked satellite exists
        if (_attached && _linkedSatelliteTransform != null)
        {
            /// Retrieve linked Satellites location
            Vector3 linkedSatelliteLocation = _linkedSatelliteTransform.position;

            // Designate forces to apply to the x and y axis.
            float xForce = 0f;
            float yForce = 0f;

            /// Set movement speed
            var movementSpeedX = eyeOfZetaAutomaticDroneSettings.movementSpeed;
            var movementSpeedY = eyeOfZetaAutomaticDroneSettings.movementSpeed;

            /// Bools to allow parenting
            var canParentX = false;
            var canParentY = false;

            // Easier access to allowedRange.
            var allowedRange = eyeOfZetaAutomaticDroneSettings.allowedRange;        

            /// Calculate distance from object
            var distance = Vector2.Distance(dronePosition,linkedSatelliteLocation);

            
            // If the distance between the object is less than the minimum then apply a smaller force to move it slightly away from the satellite centre
            // Makes it easier to select satellites
            if (distance < eyeOfZetaAutomaticDroneSettings.minimumDistanceFromSatellite) 
            {
                /// Otherwise add a small force away from the satellite
                if (dronePosition.x < linkedSatelliteLocation.x) xForce = -movementSpeedX/4;
                else xForce = movementSpeedX/4;
            
                /// Otherwise add a small force away from the satellite
                if (dronePosition.y < linkedSatelliteLocation.y) yForce = -movementSpeedY/4;
                else yForce = movementSpeedY / 4;

                // Apply force to rigid body    
                _droneRigidbody2D.AddForce(new Vector3(xForce,yForce,0));
            }
                
            /// If the X and Y of the drone are within the allowed range, then parent drone to object, such it will follow the object as it is moved.
            else if (distance < allowedRange)
            {
                _parented = true;
                this.transform.SetParent(_linkedSatelliteTransform);
            }

            /// Otherwise calculate and apply force to drone
            else
            {
                /// Otherwise add force on the x axis dependent on which side of the object is from the drone.
                if (dronePosition.x < linkedSatelliteLocation.x) xForce = movementSpeedX;
                else xForce = -movementSpeedX;
            
                /// Otherwise add force on the y axis depending on the drones location relative to the selected objects.
                if (dronePosition.y < linkedSatelliteLocation.y) yForce = movementSpeedY;
                else yForce = -movementSpeedY;

                // Apply force to rigid body    
                _droneRigidbody2D.AddForce(new Vector3(xForce,yForce,0));
            }
        }
        
        // Retreive Camera position
        Vector3 cameraPosition = _camera.transform.position;
        float newCameraX = cameraPosition.x;
        float newCameraY = cameraPosition.y;

        // Update the camera position to match the drone position
        if (dronePosition.x > _minCameraX && dronePosition.x < _maxCameraX) newCameraX = dronePosition.x;
        if (dronePosition.y > _minCameraY && dronePosition.y < _maxCameraY) newCameraY = dronePosition.y;
        _camera.transform.position = new Vector3(newCameraX, newCameraY, cameraPosition.z);

    }

    /// <summary>
    /// Method to attach the drone to a selected satellite.
    /// </summary>
    /// <param name="transform"></param>
    public void AttachDroneToSatellite(Transform transform)
    {   
        ///For efficiency, detect the drone from whatever it was attached to initally
        DetachDroneFromSatellite();

        /// Update components to mark the drone as "attached"
        _attached = true;
        _linkedSatelliteTransform = transform;
        
    }

    /// <summary>
    /// Method to detach the drone from a satellite
    /// </summary>
    public void DetachDroneFromSatellite()
    {
        /// Reset rotation of the drone
        transform.Rotate(new Vector3(0f,0f,0f));

        /// Set attached and parented to false;
        _attached = false;
        _parented = false;

        // Set linked satellite and parent to null
        _linkedSatelliteTransform = null;
        this.transform.SetParent(null);

        // Note this is called by the parent satellite just before the satellite is destroyed.
    }

}

/// <summary>
/// Standard Eye of Zeta settings, states the camera range and manual drone movement speed
/// </summary>
[System.Serializable]
public class EyeOfZetaCameraDroneSettings
{
    public float minimumXPosition = -20f;
    public float maximumXPosition = 20f;
    public float minimumYPosition = -20f;
    public float maximumYPosition = 20f;

    public float cameraDroneMovementSpeed = 35;

}

/// <summary>
/// Eye of Zeta automatic settings, how it acts when attaching and parenting to a satellite
/// </summary>
[System.Serializable]
public class EyeOfZetaAutomaticDroneSettings
{
    public float movementSpeed = 10f;
    public float allowedRange = 3f;
    public float minimumDistanceFromSatellite = 1.5f;


}