using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singularity : SatelliteParent
{

    private Transform _blackholeHelper;

    public GameObject _singularityLaser;

    public GravitationalAnomalyPhysics gravitationalAnomalyPhysics;
    public GravitationalAnomalySettings gravitationalAnomalySettings;



    // Start is called before the first frame update
    void Start()
    {
        _blackholeHelper = GameObject.FindGameObjectsWithTag("BlackholeHelper")[0].GetComponent<Transform>();

        base.Start();
    }

    override public void FireLaser(OutgoingLaserInfo laserInfo)
    {
        bool bendRight = PrepareHelper(laserInfo.angle,laserInfo.raycastPosition);

        var newLaser = Instantiate(_singularityLaser);
        newLaser.GetComponent<SingularityLaser>().Activate(this,laserInfo.angle,laserInfo.raycastPosition, bendRight, gravitationalAnomalyPhysics, gravitationalAnomalySettings);

        if (!_outgoingLaserOrigins.Contains(laserInfo.raycastPosition))
        {
            // Add laser object and origin to lists respectively
            _outgoingLaserObjects.Add(newLaser);
            _outgoingLaserOrigins.Add(laserInfo.raycastPosition);
        }
        else
        {
            // Destroy laser if unneeded
            Destroy(newLaser);
        }
    }


    override public void Interaction(IncomingLaser incomingLaser)
    {
        OutgoingLaserInfo newOutGoingLaserInfo = new OutgoingLaserInfo();

        newOutGoingLaserInfo.angle = incomingLaser.laser.transform.eulerAngles.z;

        newOutGoingLaserInfo.origin = incomingLaser.raycast.point;
        newOutGoingLaserInfo.raycastPosition = incomingLaser.raycast.point;
        newOutGoingLaserInfo.laserTransparency = incomingLaser.laser.GetTransparency() - _thisSatelliteInfo.advanced_Satellite_Info.absorbance;
        newOutGoingLaserInfo.laserColour = incomingLaser.laser.GetLaserColour();
        _outgoingLaserInfo.Add(newOutGoingLaserInfo);
    }




    private bool PrepareHelper(float laserAngle, Vector3 raycastPosition)
    {
        _blackholeHelper.position = raycastPosition;
        _blackholeHelper.eulerAngles = new Vector3(0f,0f,laserAngle);

        _blackholeHelper.up = transform.position - _blackholeHelper.position;

        float _blackholeHelperAngle = _blackholeHelper.eulerAngles.z;
        if (_blackholeHelperAngle > 180) _blackholeHelperAngle = (_blackholeHelperAngle - 360);

        bool bendRight = false;

        if (_blackholeHelperAngle < laserAngle) bendRight = true;

        return bendRight;
    }
}