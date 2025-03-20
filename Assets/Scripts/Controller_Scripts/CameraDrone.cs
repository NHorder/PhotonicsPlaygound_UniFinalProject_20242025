using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrone : MonoBehaviour
{

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
        satelliteInfo.satellite_Movement_Info.intialMovementMultiplier = _gameController.eyeOfZetaCameraDroneSettings.cameraDroneMovementSpeed;
        satelliteInfo.satellite_Movement_Info.maxMovementMultiplier = _gameController.eyeOfZetaCameraDroneSettings.cameraDroneMovementSpeed + 5;

        _minX = _gameController.eyeOfZetaCameraDroneSettings.minimumXPosition;
        _maxX = _gameController.eyeOfZetaCameraDroneSettings.maximumXPosition;

        _minY = _gameController.eyeOfZetaCameraDroneSettings.minimumYPosition;
        _maxY = _gameController.eyeOfZetaCameraDroneSettings.maximumYPosition;


    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dronePosition = this.transform.position;
        
        if (_attached && _linkedSatelliteTransform != null && !_parented)
        {
            Vector3 linkedSatelliteLocation = _linkedSatelliteTransform.position;

            float xForce = 0f;
            float yForce = 0f;

            // If the drone position.x is less than the location of the linked satellite - allowed range. Or more than the location + the allowed range
            // then move the drone towards the satellite.
            if (dronePosition.x < linkedSatelliteLocation.x - eyeOfZetaAutomaticDroneSettings.xRange)
            {
                xForce = _gameController.eyeOfZetaCameraDroneSettings.cameraDroneMovementSpeed;

                if (dronePosition.x > linkedSatelliteLocation.x + eyeOfZetaAutomaticDroneSettings.xRange) _currentMovementSpeed /= 4;

                if (_currentMovementSpeed < eyeOfZetaAutomaticDroneSettings.minimumMovementSpeed) _currentMovementSpeed = eyeOfZetaAutomaticDroneSettings.minimumMovementSpeed;
            }
            else if (dronePosition.x > linkedSatelliteLocation.x + eyeOfZetaAutomaticDroneSettings.xRange)
            {
                xForce = -_gameController.eyeOfZetaCameraDroneSettings.cameraDroneMovementSpeed;

                if (dronePosition.x < linkedSatelliteLocation.x - eyeOfZetaAutomaticDroneSettings.xRange) _currentMovementSpeed /= 4;

                if (_currentMovementSpeed < eyeOfZetaAutomaticDroneSettings.minimumMovementSpeed) _currentMovementSpeed = eyeOfZetaAutomaticDroneSettings.minimumMovementSpeed;
            }

            if (dronePosition.y < linkedSatelliteLocation.y - eyeOfZetaAutomaticDroneSettings.yRange)
            {
                yForce = _gameController.eyeOfZetaCameraDroneSettings.cameraDroneMovementSpeed;

                // If overshot, reduce drone speed by four
                if (dronePosition.y > linkedSatelliteLocation.y + eyeOfZetaAutomaticDroneSettings.yRange) _currentMovementSpeed /= 4;

                // If movement speed is less than 2, then set movement speed to 2
                if (_currentMovementSpeed < eyeOfZetaAutomaticDroneSettings.minimumMovementSpeed) _currentMovementSpeed = eyeOfZetaAutomaticDroneSettings.minimumMovementSpeed;

            }
            else if (dronePosition.y > linkedSatelliteLocation.y + eyeOfZetaAutomaticDroneSettings.yRange)
            {
                yForce = -_gameController.eyeOfZetaCameraDroneSettings.cameraDroneMovementSpeed;

                // If overshot, reduce drone speed by four
                if (dronePosition.y < linkedSatelliteLocation.y - eyeOfZetaAutomaticDroneSettings.yRange) _currentMovementSpeed /= 4;

                // If movement speed is less than 2, then set movement speed to 2
                if (_currentMovementSpeed < eyeOfZetaAutomaticDroneSettings.minimumMovementSpeed) _currentMovementSpeed = eyeOfZetaAutomaticDroneSettings.minimumMovementSpeed;


            }


            if ( (dronePosition.x >= linkedSatelliteLocation.x - eyeOfZetaAutomaticDroneSettings.xRange) &&
                (dronePosition.x <= linkedSatelliteLocation.x + eyeOfZetaAutomaticDroneSettings.xRange) && 
                (dronePosition.y >= linkedSatelliteLocation.y - eyeOfZetaAutomaticDroneSettings.yRange) &&
                (dronePosition.y <= linkedSatelliteLocation.y + eyeOfZetaAutomaticDroneSettings.yRange))
            {
                _parented = true;
                this.transform.SetParent(_linkedSatelliteTransform);

                _droneRigidbody2D.drag = 1f;

            }
            else
            {
                _droneRigidbody2D.AddForce(new Vector3(xForce,yForce,0));
            }

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
public class EyeOfZetaAutomaticDroneSettings
{
    public float movementSpeed = 10f;
    public float minimumMovementSpeed = 0.5f;
    public float intialDrag = 4f;
    public float xRange = 2f;
    public float yRange = 2f;


}