using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrone : MonoBehaviour
{
    public EyeOfZetaCameraDroneSettings eyeOfZetaCameraDroneSettings;
    public EyeOfZetaAutomaticDroneSettings eyeOfZetaAutomaticDroneSettings;

    private GameObject _camera;
    private GameController _gameController;


    private float _minX = -20f;
    private float _maxX = 20f;
    private float _minY = -20f;
    private float _maxY = 20f;




    private bool _attached = false;
    private bool _parented = false; 

    private Transform _linkedSatelliteTransform;
    private Rigidbody2D _droneRigidbody2D;
    private float _currentMovementSpeed;


    // Start is called before the first frame update
    void Start()
    {
        _camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

        _droneRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _currentMovementSpeed = eyeOfZetaAutomaticDroneSettings.movementSpeed;

        var satelliteInfo = this.gameObject.GetComponent<Satellite_Info>();
        satelliteInfo.satellite_Movement_Info.intialMovementMultiplier = eyeOfZetaCameraDroneSettings.cameraDroneMovementSpeed;
        satelliteInfo.satellite_Movement_Info.maxMovementMultiplier = eyeOfZetaCameraDroneSettings.cameraDroneMovementSpeed + 5;

        _minX = eyeOfZetaCameraDroneSettings.minimumXPosition;
        _maxX = eyeOfZetaCameraDroneSettings.maximumXPosition;

        _minY = eyeOfZetaCameraDroneSettings.minimumYPosition;
        _maxY = eyeOfZetaCameraDroneSettings.maximumYPosition;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dronePosition = this.transform.position;
        
        if (_attached && _linkedSatelliteTransform != null)
        {
            Vector3 linkedSatelliteLocation = _linkedSatelliteTransform.position;

            float xForce = 0f;
            float yForce = 0f;

            var movementSpeedX = eyeOfZetaAutomaticDroneSettings.movementSpeed;
            var movementSpeedY = eyeOfZetaAutomaticDroneSettings.movementSpeed;

            var canParentX = false;
            var canParentY = false;

            var allowedRange = eyeOfZetaAutomaticDroneSettings.allowedRange;

            if (_linkedSatelliteTransform.GetComponent<Satellite_Info>().satelliteType == SatelliteType.SatelliteCreator)
            {
                Debug.Log("Elysia!");
                allowedRange *= 8;
            }


            // Set a desired position which will be a slight offset to the bottom left of the select satellite
            var wantedPosition = new Vector2(_linkedSatelliteTransform.position.x - eyeOfZetaAutomaticDroneSettings.offsetFromObjectX, 
                                            _linkedSatelliteTransform.position.y - eyeOfZetaAutomaticDroneSettings.offsetFromObjectY);

            
            // If the drone x position is within an allowed range of the wanted, then set canParentX to true. Concluding X is near the right place.
            if (dronePosition.x > wantedPosition.x - allowedRange && dronePosition.x < wantedPosition.x + allowedRange) canParentX = true;
            else
            {
                // Otherwise add force on the x axis dependent on which side of the object is from the drone.
                if (dronePosition.x < wantedPosition.x) xForce = movementSpeedX;
                else xForce = -movementSpeedX;
            }

            // If the drone y position is within an allowed range of the wanted, then set canParentY to true. Conluding Y is near the right place.
            if (dronePosition.y > wantedPosition.y - allowedRange && dronePosition.y < wantedPosition.y + allowedRange) canParentY = true;
            else
            {
                // Otherwise add force on the y axis depending on the drones location relative to the selected objects.
                if (dronePosition.y < wantedPosition.y) yForce = movementSpeedY;
                else yForce = -movementSpeedY;
            }


            // If the X and Y of the drone are within the allowed range, then parent drone to object, such it will follow the object as it is moved.
            if (canParentX && canParentY)
            {
                _parented = true;
                this.transform.SetParent(_linkedSatelliteTransform);
            }

            // Otherwise apply preallocated force to the drone
            else _droneRigidbody2D.AddForce(new Vector3(xForce,yForce,0));
            
        }

        Vector3 cameraPosition = _camera.transform.position;
        float newCameraX = cameraPosition.x;
        float newCameraY = cameraPosition.y;
        
        if (dronePosition.x > _minX && dronePosition.x < _maxX) newCameraX = dronePosition.x;

        if (dronePosition.y > _minY && dronePosition.y < _maxY) newCameraY = dronePosition.y;

        _camera.transform.position = new Vector3(newCameraX, newCameraY, cameraPosition.z);
    }

    public void AttachDroneToSatellite(Transform transform)
    {   
        // For efficiency, detect the drone from whatever it was attached to
        DetachDroneFromSatellite();

        // Then attach it to the new satellite
        _attached = true;
        _linkedSatelliteTransform = transform;
        
    }

    public void DetachDroneFromSatellite()
    {
        transform.Rotate(new Vector3(0f,0f,0f));

        _attached = false;
        _parented = false;

        _droneRigidbody2D.drag = eyeOfZetaAutomaticDroneSettings.intialDrag;

        _linkedSatelliteTransform = null;
        this.transform.SetParent(null);
    }


}

[System.Serializable]
public class EyeOfZetaCameraDroneSettings
{
    public float minimumXPosition = -20f;
    public float maximumXPosition = 20f;
    public float minimumYPosition = -20f;
    public float maximumYPosition = 20f;

    public float cameraDroneMovementSpeed = 35;

}

[System.Serializable]
public class EyeOfZetaAutomaticDroneSettings
{
    public float movementSpeed = 10f;
    public float intialDrag = 4f;
    public float allowedRange = 0.5f;
    public float offsetFromObjectX = 0.5f;
    public float offsetFromObjectY = 0.5f;


}