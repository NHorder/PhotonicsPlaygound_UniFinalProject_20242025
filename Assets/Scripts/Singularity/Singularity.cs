using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singularity : SatelliteParent
{

    private Transform _singularityHelper;

    public GameObject _singularityLaser;

    public GravitationalAnomalySettings gravitationalAnomalySettings;



    // Start is called before the first frame update
    void Start()
    {

        // Singularities require helpers - this is in order to determine the "bend" direction of the light
        // As such the Singularity prefab comes with a dedicated helper, find the helper and set it's parent to null
        // This is done so the transform is world position instead of local.
        _singularityHelper = gameObject.GetComponentsInChildren<Transform>()[1];
        _singularityHelper.SetParent(null);

        // Call Inherited Start
        base.Start();
    }

    override public void FireLaser(OutgoingLaserInfo laserInfo)
    {
        // Override FireLaser, Singularities handle Internal and External lasers.

        // If the laser info is external, call the inherited method (which is designed for 'external' lasers)
        if (laserInfo.external) base.FireLaser(laserInfo);

        // Otherwise, use custom method to handle 'internal' lasers.
        else {

            // Determine the helper angle, making use of this Singularities Helper
            float helperAngle = PrepareHelper(laserInfo.angle,laserInfo.raycastPosition);

            // Instantiate a Singularity Laser - this differs from regular lasers in both script and function
            // It instead uses a LineRenderer over a sprite, in order to have the light bend correctly.
            var newLaser = Instantiate(_singularityLaser);

            // Get the singularity laser component and activate it
            newLaser.GetComponent<SingularityLaser>().Activate(this, laserInfo, helperAngle, gravitationalAnomalySettings);

            // Then apply checks to see if it should continue existing.
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
        // Create an outgoing laser
        OutgoingLaserInfo newOutGoingLaserInfo = new OutgoingLaserInfo();

        // Don't change the angle here, as we need this for the internal laser method
        newOutGoingLaserInfo.angle = incomingLaser.laser.transform.eulerAngles.z;
        newOutGoingLaserInfo.origin = incomingLaser.raycast.point;
        newOutGoingLaserInfo.raycastPosition = incomingLaser.raycast.point;
        newOutGoingLaserInfo.incomingLaserDirection = incomingLaser.laser.transform.up;

        // Absorbance is 0, so can be ignored
        newOutGoingLaserInfo.laserTransparency = incomingLaser.laser.GetTransparency();

        // Need laser colour for both internal and external lasers
        newOutGoingLaserInfo.laserColour = incomingLaser.laser.GetLaserColour();

        // Set it to be "Internal" - resulting in a different rendering method
        newOutGoingLaserInfo.external = false;


        _outgoingLaserInfo.Add(newOutGoingLaserInfo);
    }


    public void NewExternalLaser(OutgoingLaserInfo outgoingLaser)
    {
        // Method called by Internal Laser to create an external laser with given parameters
        _outgoingLaserInfo.Add(outgoingLaser);
    }

    public float PrepareHelper(float laserAngle, Vector3 raycastPosition)
    {
        // A method that uses the helper to determine the angle between the laser direction and the singularity centre.

        _singularityHelper.position = raycastPosition;
        _singularityHelper.eulerAngles = new Vector3(0f,0f,laserAngle);

        // This line of code below was taken and adapted from
        // https://discussions.unity.com/t/lookat-2d-equivalent/88118
        // In order to get the angle.
        // By setting the singularity helper sprite renderer to active, you can see how the laser location affects it.
        _singularityHelper.up = transform.position - _singularityHelper.position;

        float _blackholeHelperAngle = _singularityHelper.eulerAngles.z;

        // Determine whether positive or negative angle.
        if (_blackholeHelperAngle > 180) _blackholeHelperAngle = (_blackholeHelperAngle - 360);

        return _blackholeHelperAngle;
    }
}

[System.Serializable]
public class GravitationalAnomalySettings
{
    public float distanceFromEventHorizonToChangeAngleSign = 20f;
    public float anomalyMass = 10f;
    public float massToPowerOf10 = -1f;

    public float distanceMultipler = 12f;
    public float distanceMultiplierToPowerOf10 = 3f;

    public float angleMulitplier = 10f;

    public float laserOffset = 0.05f;
}

