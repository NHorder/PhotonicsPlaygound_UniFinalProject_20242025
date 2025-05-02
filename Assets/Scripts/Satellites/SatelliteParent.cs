using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteParent : MonoBehaviour
{
    /// <summary>
    /// Parent class for satellites interactions
    /// Inherited by most satellite interactions
    /// </summary>
    
    protected int _numLaser = 0;

    // Stores true origin of a laser
    protected OriginSatellite _trueOrigin;
    protected bool _active;

    protected GameController _gameController;


    protected int _currentUpdateCount = 0;
    protected int _updateDelay = 1;

    protected List<IncomingLaser> _incomingLasers;



    protected List<GameObject> _outgoingLaserObjects;
    protected List<Vector3> _outgoingLaserOrigins;

    protected List<OutgoingLaserInfo> _outgoingLaserInfo;

    protected SatelliteInfo _thisSatelliteInfo;

    // Start is called before the first frame update
    /// <summary>
    /// Initialisation Method
    /// </summary>
    protected void Start()
    {
        // Creat links to needed objects
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        _updateDelay = _gameController.framerateRelatedSettings.laserCycleDelay;
        _thisSatelliteInfo = gameObject.GetComponent<SatelliteInfo>();

        // Prepare lists
        _incomingLasers = new List<IncomingLaser>();
        _outgoingLaserInfo = new List<OutgoingLaserInfo>();
        _outgoingLaserOrigins = new List<Vector3>();
        _outgoingLaserObjects = new List<GameObject>();
            
    }

    /// <summary>
    /// Update Method is called once per frame
    /// </summary>
    protected void Update()
    {

        if ( _currentUpdateCount > _updateDelay)
        {
            // Reset update count
            _currentUpdateCount = 0;

            // Delete all lasers
            foreach (GameObject laser in _outgoingLaserObjects) Destroy(laser);

            // Clear list
            _outgoingLaserObjects.Clear();
            _outgoingLaserOrigins.Clear();

            // If active
            if (_active)
            {
                _active = false;

                // Loop through all outgoing lasers and fire them
                foreach (OutgoingLaserInfo laserInfo in _outgoingLaserInfo)
                {
                    this.FireLaser(laserInfo);
                }

                // Clear the list when done
                _outgoingLaserInfo.Clear();
            }

            // Clear incoming laser list
            _incomingLasers.Clear();

        }
        else _currentUpdateCount ++;
        
    }

    /// <summary>
    /// Method to fire a specific laser
    /// </summary>
    /// <param name="laserInfo"></param>
    virtual public void FireLaser(OutgoingLaserInfo laserInfo)
    {

        if (_trueOrigin.prefabLaser != null)
        {
            // Instantiate the first laser
            var newLaserObj = Instantiate(_trueOrigin.prefabLaser);

            var newLaser = newLaserObj.GetComponent<Laser>();

            newLaser.origin = _trueOrigin;

            newLaser.refractionSatelliteInfo = laserInfo.satelliteInfo;

            Vector3 modifiedAngles = this.transform.eulerAngles;
            modifiedAngles.z = laserInfo.angle;
            newLaserObj.transform.eulerAngles = modifiedAngles;

            newLaser.SetTransparency(laserInfo.laserTransparency);

            // Set position and rotatation of inital laser
            newLaserObj.transform.position = laserInfo.origin;

            // Reset scale as the prefab laser may not be unscaled.
            newLaserObj.transform.localScale = new Vector3(1f,1f,1f);

            // Set energy and layers to hit.
            newLaser.maxDistance = _trueOrigin.maxDistance;
            newLaser.layersToHit = _trueOrigin.layersToHit;

            newLaser.SetLaserColour(laserInfo.laserColour);

            // Add laser to lists.
            AddLaser(newLaserObj,laserInfo.raycastPosition);
        }
    }

    /// <summary>
    /// Method to add the laser to the prexisting lists or destroy them
    /// </summary>
    /// <param name="newLaser"></param>
    /// <param name="newLaserOrigin"></param>
    private void AddLaser(GameObject newLaser,Vector2 newLaserOrigin)
    {
        // Check if the new laser origin already exists in the known origins
        if (!_outgoingLaserOrigins.Contains(newLaserOrigin))
        {
            // Add laser object and origin to lists respectively
            _outgoingLaserObjects.Add(newLaser);
            _outgoingLaserOrigins.Add(newLaserOrigin);
        }
        // Otherwise destroy it
        else Destroy(newLaser);
    }

    /// <summary>
    /// Method for lasers to trigger interactions
    /// </summary>
    /// <param name="laser"></param>
    /// <param name="raycast"></param>
    virtual public void SetActive(Laser laser, RaycastHit2D raycast)
    {
        // Set origin to the laser origin
        if (_trueOrigin == null) _trueOrigin = laser.origin;

        _active = true;

        // Create a new incoming laser and add to known list
        IncomingLaser newIncomingLaser = new IncomingLaser();
        newIncomingLaser.laser = laser;
        newIncomingLaser.raycast = raycast;
        _incomingLasers.Add(newIncomingLaser);

        // Call interaction
        // NOTE: Not all children call this function, but it's present as it is still common 
        Interaction(newIncomingLaser);
        
    }

    /// <summary>
    /// Method for light interaction
    /// Note: Blank and supposed to be overwritten by children for interactions
    /// Note: Not all children use this method, but it's common
    /// </summary>
    /// <param name="incomingLaser"></param>
    virtual public void Interaction(IncomingLaser incomingLaser)
    {
        // Do laser interaction, there should be enough information to do so

        // Interactions should result in the creation of an outgoing laser, which can be added to the outgoing laser list.
        
    }

    /// <summary>
    /// Method to delete existing lasers
    /// </summary>
    public void DeleteLasers()
    {
        // Loop through each outogin laser and destroy it, then clear lists
        foreach (GameObject laser in _outgoingLaserObjects) Destroy(laser);
        _outgoingLaserObjects.Clear();
        _outgoingLaserOrigins.Clear();
    }

}

/// <summary>
/// Class to house incoming laser information
/// </summary>
public class IncomingLaser
{
    public Laser laser;
    public RaycastHit2D raycast;
}

/// <summary>
/// Class to house outgoing laser information
/// </summary>
public class OutgoingLaserInfo
{
    public float angle = 90;
    public Vector2 origin;
    public Vector2 raycastPosition;
    public SatelliteInfo satelliteInfo = null;
    public LaserColour laserColour;
    public float laserTransparency = 1;
    public Vector3 incomingLaserDirection = new Vector3(0f,0f,0f);
    public bool external = true;
}