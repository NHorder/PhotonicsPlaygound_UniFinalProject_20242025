using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OriginSatellite : MonoBehaviour
{
    /// <summary>
    /// This class handles origin interaction with the laser
    /// </summary>

    // Determines which layers the laser can hit
    public LayerMask layersToHit;

    // Determines the maximum distance the laser can extend to
    public float maxDistance = 200f;

    // Determines the laser colour
    public LaserColour laserColour;

    // First laser is placed manually, then attached
    public GameObject prefabLaser;

    // The update delay between laser creation
    // This is needed to allow the laser time to render before it is deleted
    // Otherwise it flickers noticibly
    private int _updateDelay = 1;
    private int _currentUpdateCount = 0;

    // Laser list is created from connections to 
    private List<GameObject> _listOfLaserObjects;

    private List<Vector2> _listOfLaserOrigins;

    private GameController _gameController;

    private Animator _animator;

    // Start is called before the first frame update
    /// <summary>
    /// Inialisation method
    /// </summary>
    void Start()
    {
        // Determine the colour ID an update animator
        var colourID = 0;
        _animator = gameObject.GetComponent<Animator>();
        if (laserColour == LaserColour.Red) colourID = 1;
        else if (laserColour == LaserColour.Blue) colourID = 2;
        else if (laserColour == LaserColour.Green) colourID = 3;
        else if (laserColour == LaserColour.Yellow) colourID = 4;
        else if (laserColour == LaserColour.Cyan) colourID = 5;
        else if (laserColour == LaserColour.Magenta) colourID = 6; 
        _animator.SetInteger("ColourID",colourID);

        // Collect game controller and update delay settings
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
    
    /// <summary>
    /// Method called once per frame
    /// </summary>
    void Update()
    {

        // Delay updates by a specified amount
        // This is done to allow the laser time to render before deletion
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

    /// <summary>
    /// Method to create the initial laser
    /// </summary>
    private void Fire_Initial_Laser()
    {

        // Check to make sure the prefab laser has been provided
        if (prefabLaser != null)
        {
            // Instantiate the first laser
            var newLaserObj = Instantiate(prefabLaser);
            var newLaser = newLaserObj.GetComponent<Laser>();
            
            newLaser.origin = this;

            // Set position and rotatation of inital laser
            newLaserObj.transform.position = this.transform.position;
            newLaserObj.transform.rotation = this.transform.rotation;

            // Reset scale as the prefab laser may not be unscaled.
            newLaserObj.transform.localScale = new Vector3(1f,1f,1f);

            // Set energy and layers to hit.
            newLaser.maxDistance = maxDistance;
            newLaser.layersToHit = layersToHit;

            newLaser.SetLaserColour(laserColour);

            // Add laser to lists.
            AddLaser(newLaserObj,this.transform.position);
        }
        else Debug.LogError("ERROR: Laser Origin has no connection to Prefab Laser");

    }

    /// <summary>
    /// Method to add a laser to the origin
    /// Results in the ability to track laser progress within a scene from a single origin
    /// </summary>
    /// <param name="newLaser"></param>
    /// <param name="newLaserOrigin"></param>
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
            Debug.Log("Destroying Laser!");
            // Destroy laser if unneeded
            Destroy(newLaser);
        }

    }

    /// <summary>
    /// Method to get game controller
    /// </summary>
    /// <returns></returns>
    public GameController GetGameController()
    {
        return _gameController;
    }

}
