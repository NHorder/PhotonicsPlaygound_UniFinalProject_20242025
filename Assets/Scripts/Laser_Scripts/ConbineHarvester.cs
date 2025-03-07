using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConbineHarvester : MonoBehaviour
{


    private int _numLasers = 0;
    private bool _whiteLaser = false;

    private bool _redLaser = false;
    private bool _greenLaser = false;
    private bool _blueLaser = false;
    private bool _cyanLaser = false;
    private bool _yellowLaser = false;
    private bool _magentaLaser = false;



    private LaserColour _outputLaserColour;

    public Laser _firstLaser;
    public Laser _secondLaser;


    private RaycastHit2D _raycast;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (_numLasers > 0)
        {
            CalculateOutputLaserColour();

            // Create laser of specific colour
            float energyPerLaser = _firstLaser.GetTransparency();

            Debug.Log(energyPerLaser);

            if (_firstLaser != null && _secondLaser != null) energyPerLaser = Mathf.Clamp01(_firstLaser.GetTransparency() + _secondLaser.GetTransparency());

            // Instantiate new laser at position
            GameObject newLaser = _firstLaser.InstantiateNewLaser(this.transform.position,this.transform.eulerAngles.z, energyPerLaser);

            // Connect new laser to the origin
            _firstLaser.origin.AddLaser(newLaser,_raycast.point);

            Debug.Log(newLaser);
            Debug.Log(newLaser.GetComponent<Laser>().GetTransparency());
            Debug.Log(energyPerLaser);

            _numLasers = 0;
            _firstLaser = null;
            _secondLaser = null;
        }
    }

    private void CalculateOutputLaserColour()
    {
        if (_redLaser && _blueLaser) _outputLaserColour = LaserColour.Magenta;
        else if (_redLaser && _greenLaser) _outputLaserColour = LaserColour.Yellow;
        else if (_blueLaser && _greenLaser) _outputLaserColour = LaserColour.Cyan;

        else if (_redLaser && _cyanLaser) _outputLaserColour = LaserColour.White;
        else if (_redLaser && _magentaLaser) _outputLaserColour = LaserColour.Magenta;
        else if (_redLaser && _yellowLaser) _outputLaserColour = LaserColour.Yellow;
        
        else if (_blueLaser && _cyanLaser) _outputLaserColour = LaserColour.Cyan;
        else if (_blueLaser && _magentaLaser) _outputLaserColour = LaserColour.Magenta;
        else if (_blueLaser && _yellowLaser) _outputLaserColour = LaserColour.White;

        else if (_greenLaser && _cyanLaser) _outputLaserColour = LaserColour.Cyan;
        else if (_greenLaser && _magentaLaser) _outputLaserColour = LaserColour.White;
        else if (_greenLaser && _yellowLaser) _outputLaserColour = LaserColour.Yellow;

        else if (_firstLaser != null && _secondLaser != null)
        {
            // If they're equal to each other, then take the first laser colour
            if (_firstLaser.laserColour == _secondLaser.laserColour) _outputLaserColour = _firstLaser.laserColour;

            // If one of the laser colours is white, take the other laser colour
            else if (_firstLaser.laserColour != LaserColour.White && _secondLaser.laserColour == LaserColour.White) _outputLaserColour = _firstLaser.laserColour;
            else if (_firstLaser.laserColour == LaserColour.White && _secondLaser.laserColour != LaserColour.White)  _outputLaserColour = _secondLaser.laserColour;
            
        }
        
        // If there is only one laser
        else if (_firstLaser != null) _outputLaserColour = _firstLaser.laserColour;
        
        // Else soemthing went wrong
        else  Debug.Log("Something went wrong");
        
    }


    public void Add_Laser(Laser laser, RaycastHit2D raycast)
    {

        if (_firstLaser == null)
        {
            Debug.Log("One Laser");

            _firstLaser = laser;
            _raycast = raycast;
        }
        else if (_secondLaser == null)
        {
            Debug.Log("Two Laser");
            _secondLaser = laser;
        }

        _numLasers += 1;

        _whiteLaser = (laser.laserColour == LaserColour.White);
        _redLaser = (laser.laserColour == LaserColour.Red);
        _greenLaser = (laser.laserColour == LaserColour.Green);
        _blueLaser = (laser.laserColour == LaserColour.Blue);
        _cyanLaser = (laser.laserColour == LaserColour.Cyan);
        _yellowLaser = (laser.laserColour == LaserColour.Yellow);
        _magentaLaser = (laser.laserColour == LaserColour.Magenta);

    }
}
