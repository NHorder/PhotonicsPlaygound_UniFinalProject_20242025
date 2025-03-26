using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingularityLaser : MonoBehaviour
{
    private Vector2 _singularityPosition;

    private LineRenderer _lineRenderer;
    private List<Vector2> _listPositions = new List<Vector2>();

    private bool _bendRight;

    public Vector3 _firstPosition;
    public Vector3 _firstPosAfterChanges;

    public float _specifiedLimit = 10f;
    private bool _determineBendDuringLoop = false;

    private bool _hitEdge = false;
    private bool _hitEventHorizon = false;


    private GravitationalAnomalyPhysics anomalyPhysics;
    private GravitationalAnomalySettings anomalySettings;


    private Singularity _singularity;

    private Transform _helper;


    private OutgoingLaserInfo _laserInfo;


    private float yOffset = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (_hitEdge && !_hitEventHorizon)
        {
            _laserInfo.angle = this.transform.eulerAngles.z;
            _laserInfo.external = true;

            Vector3 point = this.transform.position;

            // Shift the laser to be OUTSIDE the ring (Hence why cannot move other offset code into a function and call it that way)
            if (this.transform.position.y < _singularityPosition.y) point.y -= yOffset;
            else if (this.transform.position.y > _singularityPosition.y) point.y += yOffset;

            if (this.transform.position.x < _singularityPosition.x) point.x -= yOffset;
            else if (this.transform.position.x > _singularityPosition.x) point.x += yOffset; 


            
            _laserInfo.origin = point;
            _laserInfo.raycastPosition = point;
            _singularity.NewExternalLaser(_laserInfo);


            _hitEdge = false;

        }
    }

    private void FireLaser()
    {
        Vector2 currentPoint = _listPositions[0];

        while (!_hitEdge)
        {
            this.transform.position = currentPoint;
            float angularDeflection = CalculateAngularDeflection(currentPoint);

            if (_determineBendDuringLoop)
            {
                float helperAngle = _singularity.PrepareHelper(angularDeflection,currentPoint);
                if (helperAngle < angularDeflection) _bendRight = true;
                else _bendRight = false;

            }

            if (_bendRight && !float.IsNaN(-angularDeflection)) this.transform.Rotate(0f,0f,-angularDeflection);
            else if (!float.IsNaN(angularDeflection)) this.transform.Rotate(0f,0f,angularDeflection);

            Ray ray = new Ray(currentPoint,this.transform.up);

            RaycastHit2D raycast = Physics2D.Raycast(currentPoint,this.transform.up,0.1f);

            if (raycast.collider != null && raycast.collider.gameObject.tag == "Singularity")
            {

                if (raycast.collider is CircleCollider2D)
                {
                    _hitEdge = true;
                    _hitEventHorizon = true;
                    Debug.Log("Hit Event Horizon");
                }
                else if (raycast.collider is PolygonCollider2D)
                {
                    Debug.Log("Hit Edge!");
                    _hitEdge = true;
                }
                else
                {
                    Debug.Log("???");
                }

                }
            else
            {
                currentPoint = ray.GetPoint(0.1f);
                _listPositions.Add(currentPoint);
            }
        }

        var points = new Vector3[_listPositions.Count];
        _lineRenderer.positionCount = _listPositions.Count;
        // Reset Line Renderer
        _lineRenderer.SetPositions(points);

        for (int i = 0; i < _listPositions.Count; i++)
        {
            points[i] = _listPositions[i];
        }
        _lineRenderer.SetPositions(points);
    }

    public void Activate(Singularity singularity,OutgoingLaserInfo laserInfo, float helperAngle, GravitationalAnomalyPhysics physics, GravitationalAnomalySettings settings)
    {
        _laserInfo = laserInfo;


        _lineRenderer = gameObject.GetComponent<LineRenderer>();

        Color newColour = Color.white;
        if (laserInfo.laserColour == LaserColour.White) newColour = Color.white;
        else if (laserInfo.laserColour == LaserColour.Red) newColour = Color.red;
        else if (laserInfo.laserColour == LaserColour.Blue) newColour = Color.blue;
        else if (laserInfo.laserColour == LaserColour.Green) newColour = Color.green;
        else if (laserInfo.laserColour == LaserColour.Yellow) newColour = Color.yellow;
        else if (laserInfo.laserColour == LaserColour.Cyan) newColour = Color.cyan;
        else if (laserInfo.laserColour == LaserColour.Magenta) newColour = Color.magenta;

        _lineRenderer.startColor = newColour;
        _lineRenderer.endColor = newColour;

        _firstPosition = laserInfo.raycastPosition;

        _lineRenderer = gameObject.GetComponent<LineRenderer>();
        anomalyPhysics = physics;
        anomalySettings = settings;


        _specifiedLimit = settings.distanceFromEventHorizonToChangeAngleSign;

        _singularity = singularity;
        _singularityPosition = singularity.transform.position;

        Vector3 point = laserInfo.raycastPosition;

        if (laserInfo.raycastPosition.y < _singularityPosition.y) point.y += yOffset;
        else if (laserInfo.raycastPosition.y > _singularityPosition.y) point.y -= yOffset;

        // If the point.x is different from what is known, apply an offset. 
        // This allows the point to consistently be within the refraction collider.
        if (laserInfo.raycastPosition.x < _singularityPosition.x) point.x += yOffset;
        else if (laserInfo.raycastPosition.x > _singularityPosition.x) point.x -= yOffset; 

        if (helperAngle <= _specifiedLimit && helperAngle >= -_specifiedLimit) _determineBendDuringLoop = true; 
        else if (helperAngle < laserInfo.angle) _bendRight = true;
        else _bendRight = false;


        _listPositions.Add(point);
        _hitEdge = false;
        _hitEventHorizon = false;
        this.transform.eulerAngles = new Vector3(0f,0f,laserInfo.angle);


        _firstPosAfterChanges = point;

        _hitEdge = false;
        _hitEventHorizon = false;

        FireLaser();

    }

    private float CalculateAngularDeflection(Vector2 point)
    {

        float distanceOfClosestApproach = Mathf.Sqrt(Mathf.Pow((_singularityPosition.x - point.x),2) + Mathf.Pow((_singularityPosition.y - point.y),2));

        distanceOfClosestApproach *=  ((anomalyPhysics.distanceMultipler * Mathf.Pow(10,anomalyPhysics.dsitanceMultiplerToPowerOf10)));

        float gravitationalConstant = (6.67430f * Mathf.Pow(10,-11));

        float massOfAnomaly = (anomalyPhysics.anomalyMass * Mathf.Pow(10.0f,anomalyPhysics.toPowerOf10)) * (2 * Mathf.Pow(10,30));

        float schwarzschildRadius = ( 2.0f * gravitationalConstant * massOfAnomaly) / Mathf.Pow(299792458,2);
       

        float impactParameter = distanceOfClosestApproach * Mathf.Sqrt(((distanceOfClosestApproach) / (distanceOfClosestApproach - schwarzschildRadius)));
        

        float angularDeflection =  (2.0f * schwarzschildRadius) / impactParameter;
        
        angularDeflection *= anomalyPhysics.angleMulitplier;
        return angularDeflection;

    }


}



