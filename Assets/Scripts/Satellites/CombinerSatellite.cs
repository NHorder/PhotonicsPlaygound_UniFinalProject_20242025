using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinerSatellite : SatelliteParent
{
    private bool _whiteLaser = false;

    private bool _redLaser = false;
    private bool _greenLaser = false;
    private bool _blueLaser = false;
    private bool _cyanLaser = false;
    private bool _yellowLaser = false;
    private bool _magentaLaser = false;

    private LaserColour _outputLaserColour;


    // Update is called once per frame
    void Update()
    {
        Debug.Log(_outputLaserColour);
        

        if (_incomingLasers.Count > 0)
        {
            float energyPerLaser = _incomingLasers[0].laser.GetTransparency() - _thisSatelliteInfo.advanced_Satellite_Info.absorbance;

            if (_incomingLasers.Count > 1)
            {
                energyPerLaser = Mathf.Clamp01(_incomingLasers[0].laser.GetTransparency() + _incomingLasers[1].laser.GetTransparency());

                CalculateOutputLaserColour(_incomingLasers[0].laser,_incomingLasers[1].laser);
            }
            else CalculateOutputLaserColour(_incomingLasers[0].laser,null);

            OutgoingLaserInfo newOutgoingLaser = new OutgoingLaserInfo();
            newOutgoingLaser.origin = this.transform.position;

            newOutgoingLaser.angle = this.transform.eulerAngles.z;

            newOutgoingLaser.raycastPosition = this.transform.position;


            newOutgoingLaser.satelliteInfo = null;
            newOutgoingLaser.laserTransparency = energyPerLaser;
            newOutgoingLaser.laserColour = _outputLaserColour;


            _outgoingLaserInfo.Add(newOutgoingLaser);
        }

        base.Update();
    }


    private void CalculateOutputLaserColour(Laser firstLaser, Laser secondLaser = null)
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

        else if (secondLaser != null)
        {
            // If they're equal to each other, then take the first laser colour
            if (firstLaser.GetLaserColour() == secondLaser.GetLaserColour()) _outputLaserColour = firstLaser.GetLaserColour();

            // If one of the laser colours is white, take the other laser colour
            else if (firstLaser.GetLaserColour() != LaserColour.White && secondLaser.GetLaserColour() == LaserColour.White) _outputLaserColour = firstLaser.GetLaserColour();
            else if (firstLaser.GetLaserColour() == LaserColour.White && secondLaser.GetLaserColour() != LaserColour.White)  _outputLaserColour = secondLaser.GetLaserColour();
            
        }
        
        // If there is only one laser
        else if (secondLaser == null) _outputLaserColour = firstLaser.GetLaserColour();
        
        // Else soemthing went wrong
        else  Debug.Log("Something went wrong");


        // Reset values.
        _whiteLaser = false;
        _redLaser = false;
        _blueLaser = false;
        _greenLaser = false;
        _yellowLaser = false;
        _cyanLaser = false;
        _magentaLaser = false;
        
    }

    override public void SetActive(Laser laser, RaycastHit2D raycast)
    {
        _active = true;

        var assigned = false;

        if (_incomingLasers.Count == 0)
        {
            _trueOrigin = laser.origin;
            IncomingLaser newIncomingLaser = new IncomingLaser();
            newIncomingLaser.laser = laser;
            newIncomingLaser.raycast = raycast;
            _incomingLasers.Add(newIncomingLaser);

            assigned = true;
        }
        else if (_incomingLasers.Count == 1)
        {
            IncomingLaser newIncomingLaser = new IncomingLaser();
            newIncomingLaser.laser = laser;
            newIncomingLaser.raycast = raycast;
            _incomingLasers.Add(newIncomingLaser);
            
            assigned = true;
        }


        if (assigned)
        {
            if (laser.GetLaserColour() == LaserColour.White) _whiteLaser = true;
            else if (laser.GetLaserColour()  == LaserColour.Red) _redLaser = true;
            else if (laser.GetLaserColour()  == LaserColour.Green) _greenLaser = true;
            else if (laser.GetLaserColour()  == LaserColour.Blue) _blueLaser = true;
            else if (laser.GetLaserColour() == LaserColour.Cyan) _cyanLaser = true;
            else if (laser.GetLaserColour() == LaserColour.Yellow) _yellowLaser = true;
            else if (laser.GetLaserColour() == LaserColour.Magenta) _magentaLaser = true;
        }
    }
}
