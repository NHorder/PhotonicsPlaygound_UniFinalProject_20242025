using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrone : MonoBehaviour
{

    private GameObject _camera;
    private GameController _gameController;


    private float _minX = -20f;
    private float _maxX = 20f;
    private float _minY = -20f;
    private float _maxY = 20f;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

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
        Vector3 cameraPosition = _camera.transform.position;
        Vector3 dronePosition = this.transform.position;
        float newCameraX = cameraPosition.x;
        float newCameraY = cameraPosition.y;
        
        if (dronePosition.x > _minX && dronePosition.x < _maxX)newCameraX = dronePosition.x;

        if (dronePosition.y > _minY && dronePosition.y < _maxY) newCameraY = dronePosition.y;

        _camera.transform.position = new Vector3(newCameraX, newCameraY, cameraPosition.z);
    }



}
