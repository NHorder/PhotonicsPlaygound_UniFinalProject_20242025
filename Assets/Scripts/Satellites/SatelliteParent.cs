using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteParent : MonoBehaviour
{
    protected int _numLaser = 0;

    protected OriginSatellite _trueOrigin;
    protected bool _active;

    protected GameController _gameController;


    protected int _currentUpdateCount = 0;
    protected int _updateDelay = 1;

    protected List<IncomingLaser> _incomingLasers;



    protected List<GameObject> _outgoingLaserObjects;
    protected List<Vector3> _outgoingLaserOrigins;

    protected List<OutgoingLaserInfo> _outgoingLaserInfo;

    protected Satellite_Info _thisSatelliteInfo;



    // Start is called before the first frame update
    protected void Start()
    {
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        _updateDelay = _gameController.framerateRelatedSettings.laserCycleDelay;

        _thisSatelliteInfo = gameObject.GetComponent<Satellite_Info>();

        _incomingLasers = new List<IncomingLaser>();
        _outgoingLaserInfo = new List<OutgoingLaserInfo>();
        _outgoingLaserOrigins = new List<Vector3>();
        _outgoingLaserObjects = new List<GameObject>();
            
    }

    // Update is called once per frame
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



            if (_active)
            {
                _active = false;

                foreach (OutgoingLaserInfo laserInfo in _outgoingLaserInfo)
                {
                    this.FireLaser(laserInfo);
                }

                _outgoingLaserInfo.Clear();
            }

            // Clear incoming laser list
            _incomingLasers.Clear();

        }
        else _currentUpdateCount ++;
        
    }

    private void FireLaser(OutgoingLaserInfo laserInfo)
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


    private void AddLaser(GameObject newLaser,Vector2 newLaserOrigin)
    {
        
        if (!_outgoingLaserOrigins.Contains(newLaserOrigin))
        {
            // Add laser object and origin to lists respectively
            _outgoingLaserObjects.Add(newLaser);
            _outgoingLaserOrigins.Add(newLaserOrigin);
        }
        else
        {
            // Destroy laser if unneeded
            Destroy(newLaser);
        }

    }


    virtual public void SetActive(Laser laser, RaycastHit2D raycast)
    {

        if (_trueOrigin == null) _trueOrigin = laser.origin;

        _active = true;

        IncomingLaser newIncomingLaser = new IncomingLaser();
        newIncomingLaser.laser = laser;
        newIncomingLaser.raycast = raycast;

        _incomingLasers.Add(newIncomingLaser);

        Interaction(newIncomingLaser);
        
    }

    virtual public void Interaction(IncomingLaser incomingLaser)
    {
        // Do laser interaction, there should be enough information to do so
        
    }


    public void DeleteLasers()
    {
        foreach (GameObject laser in _outgoingLaserObjects) Destroy(laser);
        
        _outgoingLaserObjects.Clear();
        _outgoingLaserOrigins.Clear();
    }

}


public class IncomingLaser
{
    public Laser laser;
    public RaycastHit2D raycast;
}

public class OutgoingLaserInfo
{
    public float angle = 90;
    public Vector2 origin;
    public Vector2 raycastPosition;
    public Satellite_Info satelliteInfo = null;
    public LaserColour laserColour;
    public float laserTransparency = 1;
}