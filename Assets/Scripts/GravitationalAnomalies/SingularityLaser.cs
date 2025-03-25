using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingularityLaser : MonoBehaviour
{
    private Vector2 _singularityPosition;

    private LineRenderer _lineRenderer;
    private List<Vector2> _listPositions = new List<Vector2>();

    private bool _bendRight;

    private Vector2 _firstPosition;
    private Transform _laserTransform;
    private Vector2 _priorFirstPosition;


    private bool _hitEdge = false;
    private bool _hitEventHorizon = false;


    private GravitationalAnomalyPhysics anomalyPhysics;
    private GravitationalAnomalySettings anomalySettings;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_firstPosition != _priorFirstPosition)
        {
            ResetLaser();

            Vector2 currentPoint = _listPositions[0];

            while (!_hitEdge)
            {
                this.transform.position = currentPoint;

                float angularDeflection = CalculateAngularDeflection(currentPoint);


                if (_bendRight && !float.IsNaN(-angularDeflection)) this.transform.Rotate(0f,0f,-angularDeflection);
                else if (!float.IsNaN(angularDeflection)) this.transform.Rotate(0f,0f,angularDeflection);

                Ray ray = new Ray(currentPoint,this.transform.up);

                RaycastHit2D raycast = Physics2D.Raycast(currentPoint,this.transform.up,0.1f);

                if (raycast.collider != null)
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
    }


    public void Activate(Singularity singularity,Transform laserTransform, Vector2 firstPosition, bool bendRight, GravitationalAnomalyPhysics physics, GravitationalAnomalySettings settings)
    {
        anomalyPhysics = physics;
        anomalySettings = settings;
        _singularityPosition = singularity.transform.position;
        _laserTransform = laserTransform;
        _bendRight = bendRight;

        var yOffset = 0.05f;

        this._firstPosition =  firstPosition;

        if (_firstPosition.y < this.transform.position.y) _firstPosition.y += yOffset;
        else if (_firstPosition.y > this.transform.position.y) _firstPosition.y -= yOffset;

        // If the point.x is different from what is known, apply an offset. 
        // This allows the point to consistently be within the refraction collider.
        if (_firstPosition.x < this.transform.position.x) _firstPosition.x += yOffset;
        else if (_firstPosition.x > this.transform.position.x) _firstPosition.x -= yOffset; 

    }


    private void ResetLaser()
    {
        _priorFirstPosition = _firstPosition;
        _listPositions.Clear();
        _listPositions.Add(_firstPosition);
        _hitEdge = false;
        _hitEventHorizon = false;

        this.transform.rotation = _laserTransform.rotation;
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
