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
        _blackholeHelper.SetParent(null);

        base.Start();
    }

    override public void FireLaser(OutgoingLaserInfo laserInfo)
    {
        if (laserInfo.external)
        {
            Debug.Log("Hello?");
            base.FireLaser(laserInfo);
        }
        
        else {
            float helperAngle = PrepareHelper(laserInfo.angle,laserInfo.raycastPosition);
            var newLaser = Instantiate(_singularityLaser);
            newLaser.GetComponent<SingularityLaser>().Activate(this, laserInfo, helperAngle, gravitationalAnomalyPhysics, gravitationalAnomalySettings);

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

        

        
    }


    override public void Interaction(IncomingLaser incomingLaser)
    {
        OutgoingLaserInfo newOutGoingLaserInfo = new OutgoingLaserInfo();

        newOutGoingLaserInfo.angle = incomingLaser.laser.transform.eulerAngles.z;

        newOutGoingLaserInfo.origin = incomingLaser.raycast.point;
        newOutGoingLaserInfo.raycastPosition = incomingLaser.raycast.point;
        newOutGoingLaserInfo.laserTransparency = incomingLaser.laser.GetTransparency() - _thisSatelliteInfo.advanced_Satellite_Info.absorbance;
        newOutGoingLaserInfo.laserColour = incomingLaser.laser.GetLaserColour();
        newOutGoingLaserInfo.external = false;
        _outgoingLaserInfo.Add(newOutGoingLaserInfo);
    }


    public void NewExternalLaser(OutgoingLaserInfo outgoingLaser)
    {
        _outgoingLaserInfo.Add(outgoingLaser);
    }

    public float PrepareHelper(float laserAngle, Vector3 raycastPosition)
    {
        _blackholeHelper.position = raycastPosition;
        _blackholeHelper.eulerAngles = new Vector3(0f,0f,laserAngle);

        _blackholeHelper.up = transform.position - _blackholeHelper.position;

        float _blackholeHelperAngle = _blackholeHelper.eulerAngles.z;
        if (_blackholeHelperAngle > 180) _blackholeHelperAngle = (_blackholeHelperAngle - 360);

        return _blackholeHelperAngle;
    }
}

[System.Serializable]
public class GravitationalAnomalySettings
{
    public float distanceFromEventHorizonToChangeAngleSign = 20f;
}

[System.Serializable]
public class GravitationalAnomalyPhysics
{
    public float anomalyMass = 10f;
    public float toPowerOf10 = -1f;

    public float distanceMultipler = 12f;
    public float dsitanceMultiplerToPowerOf10 = 3f;

    public float angleMulitplier = 10f;
}