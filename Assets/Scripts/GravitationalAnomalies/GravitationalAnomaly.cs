using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalAnomaly : MonoBehaviour
{

    public LineRenderer _lineRenderer;
    public List<Vector3> _listPositions = new List<Vector3>();

    private float _singularityX;
    private float _singularityY;

    private bool _activeInteraction = false;

    private bool _edgeHit = false;
    private bool _eventHorizonHit = false;

    private int _loopCount = 0;
    private int _maxLoopCount = 100;
    private float currentAngle = 0f;

    private Transform _blackholeHelper;


    public GravitationalAnomalyPhysics gravitationalAnomalyPhysics;
    public GravitationalAnomalySettings gravitationalAnomalySettings;

    private LayerMask layersToHit;

    private Vector2 newOrigin;
    private Vector2 priorOrigin;

    private bool bendRight;

    private float laserAngle = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = gameObject.GetComponent<LineRenderer>();
        _singularityX = transform.position.x;
        _singularityY = transform.position.y;

        _blackholeHelper = GameObject.FindGameObjectsWithTag("BlackholeHelper")[0].GetComponent<Transform>();

    }

    // Update is called once per frame
    void Update()
    {
        if (newOrigin != priorOrigin)
        {
            _singularityX = this.transform.position.x;
            _singularityY = this.transform.position.y;

            _listPositions.Clear();

            _listPositions.Add(newOrigin);
            priorOrigin = newOrigin;
            _edgeHit = false;

            // Reset 
            _blackholeHelper.Rotate(0f,0f,-(_blackholeHelper.eulerAngles.z));
            _blackholeHelper.Rotate(0f,0f, laserAngle);

            Vector2 currentOrigin = _listPositions[0];

            var bendAngle = Vector3.Angle(_blackholeHelper.up,this.transform.position);
            //bendAngle += laserAngle;

            Debug.Log("Bend Angle: "+bendAngle);

            if (bendAngle < laserAngle) bendRight = true;
            else bendRight = false;



            while (!_edgeHit)
            {
                
                _blackholeHelper.position = currentOrigin;
                float angle = CalculateAngularDeflection(currentOrigin);

                if (bendRight && !float.IsNaN(-angle)) _blackholeHelper.Rotate(0f,0f,-angle);
                else _blackholeHelper.Rotate(0f,0f,angle);


                Vector2 currentDirection = _blackholeHelper.up;

                // Ray is used to get the location IF the raycast doesn't hit anything
                Ray ray = new Ray(currentOrigin,currentDirection);

                // Raycast to check if it hits something
                RaycastHit2D raycast = Physics2D.Raycast(currentOrigin,currentDirection,0.1f);

                if (raycast.collider != null)
                {

                    if (raycast.collider is CircleCollider2D)
                    {
                        _edgeHit = true;
                        Debug.Log("Hit Event Horizon");
                    }
                    else if (raycast.collider is PolygonCollider2D)
                    {
                        Debug.Log("Hit Edge!");
                        _edgeHit = true;
                    }
                    else
                    {
                        Debug.Log("???");
                    }

                    }
                else
                {
                    currentOrigin = ray.GetPoint(0.1f);
                    _listPositions.Add(currentOrigin);
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



    public void SetActive(Laser laser, RaycastHit2D raycast)
    {
        _activeInteraction = true;
        layersToHit = laser.layersToHit;

        var yOffset = 0.05f;

        var point = raycast.point;

        if (point.y < this.transform.position.y) point.y += yOffset;
        else if (point.y > this.transform.position.y) point.y -= yOffset;

        // If the point.x is different from what is known, apply an offset. 
        // This allows the point to consistently be within the refraction collider.
        if (point.x < this.transform.position.x) point.x += yOffset;
        else if (point.x > this.transform.position.x) point.x -= yOffset; 

        newOrigin = point;

        laserAngle = laser.transform.eulerAngles.z;
    }


    public float CalculateAngularDeflection(Vector2 point)
    {

        float distanceOfClosestApproach = Mathf.Sqrt(Mathf.Pow((_singularityX - point.x),2) + Mathf.Pow((_singularityY - point.y),2));

        distanceOfClosestApproach *=  ((gravitationalAnomalyPhysics.distanceMultipler * Mathf.Pow(10,gravitationalAnomalyPhysics.dsitanceMultiplerToPowerOf10)));

        float gravitationalConstant = (6.67430f * Mathf.Pow(10,-11));

        float massOfAnomaly = (gravitationalAnomalyPhysics.anomalyMass * Mathf.Pow(10.0f,gravitationalAnomalyPhysics.toPowerOf10)) * (2 * Mathf.Pow(10,30));

        float schwarzschildRadius = ( 2.0f * gravitationalConstant * massOfAnomaly) / Mathf.Pow(299792458,2);
       

        float impactParameter = distanceOfClosestApproach * Mathf.Sqrt(((distanceOfClosestApproach) / (distanceOfClosestApproach - schwarzschildRadius)));
        

        float angularDeflection =  (2.0f * schwarzschildRadius) / impactParameter;
        
        angularDeflection *= gravitationalAnomalyPhysics.angleMulitplier;
        return angularDeflection;

    }

}


[System.Serializable]
public class GravitationalAnomalySettings
{
    public int maximumPoints = 100;
    public float laserDistance = 0.5f;

    public float distanceFromEventHorizonToChangeAngleSign = 0.05f;
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