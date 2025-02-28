using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LaserOrigin : MonoBehaviour
{

    public LayerMask layersToHit;
    public float maxDistance = 50f;

    // First laser is placed manually, then attached
    public GameObject prefabLaser;


    private int _updateDelay = 1;

    private int _currentUpdateCount = 0;

    // Laser list is created from connections to 
    private List<GameObject> _listOfLaserObjects;

    private List<Vector2> _listOfLaserOrigins;

    private GameController _gameController;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        _updateDelay = _gameController.framerateRelatedSettings.laserCycleDelay;

        // Initialise laser related lists
        _listOfLaserObjects = new List<GameObject>();
        _listOfLaserOrigins = new List<Vector2>();

        // Check that update delay is not less than 0.
        if (_updateDelay < 0){
            _updateDelay = 32;
            Debug.LogWarning("WARNING: Laser Origin updateDelay cannot be less than 0. Setting to default");
        }   
    }
 
    // Update is called once per frame
    void Update()
    {

        // Delay updates by a specified amount - this is done to allow lasers time to render before being destroyed.
        if (_currentUpdateCount > _updateDelay)
        {
            // Reset update count
            _currentUpdateCount = 0;

            // Delete all lasers
            foreach (GameObject laser in _listOfLaserObjects) Destroy(laser);

            // Clear lists
            _listOfLaserObjects.Clear();
            _listOfLaserOrigins.Clear();

            // Fire the initial laser to begin the recursive laser chain
            Fire_Initial_Laser();
        }
        else
        {
            // Increment count
            _currentUpdateCount++;
        }
    }

    private void Fire_Initial_Laser()
    {

        // Check to make sure the firstLaser has been provided
        if (prefabLaser != null)
        {
            // Instantiate new laser
            var newLaser = Instantiate(prefabLaser);
            newLaser.GetComponent<Laser>().origin = this;

            // Set position and rotatation of inital laser
            newLaser.transform.position = this.transform.position;
            newLaser.transform.rotation = this.transform.rotation;

            // Reset scale as the prefab laser may not be unscaled.
            newLaser.transform.localScale = new Vector3(1f,1f,1f);

            // Set energy and layers to hit.
            newLaser.GetComponent<Laser>().maxDistance = maxDistance;
            newLaser.GetComponent<Laser>().layersToHit = layersToHit;

            // Add laser to lists.
            AddLaser(newLaser,this.transform.position);
        }
        else Debug.LogError("ERROR: Laser Origin has no connection to Prefab Laser");

    }

    public void AddLaser(GameObject newLaser,Vector2 newLaserOrigin)
    {
        // Method to add laser to lists

        // Check if laser origin already exists - prevents creation of unneccary lasers
        if (!_listOfLaserOrigins.Contains(newLaserOrigin))
        {
            // Add laser object and origin to lists respectively
            _listOfLaserObjects.Add(newLaser);
            _listOfLaserOrigins.Add(newLaserOrigin);

            // Change the new laser name to "Laser_X" dependning on when it was made - unneeded by helpful for debugging.
            newLaser.name = "Laser_"+_listOfLaserObjects.Count;
        }
        else
        {
            // Destroy laser if unneeded
            Destroy(newLaser);
        }

    }

    public GameController GetGameController()
    {
        return _gameController;
    }

}
