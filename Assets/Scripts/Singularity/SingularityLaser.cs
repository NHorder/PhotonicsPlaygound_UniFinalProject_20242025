using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingularityLaser : MonoBehaviour
{
    private GravitationalAnomalySettings anomalySettings;
    private Singularity _singularity;
    private Vector2 _singularityPosition;
    private Transform _helper;
    private bool _bendRight;


    private LineRenderer _lineRenderer;
    private List<Vector2> _listPositions = new List<Vector2>();

    
    private bool _determineBendDuringLoop = false;
    private float _specifiedLimit = 10f;

    private bool _hitEdge = false;
    private bool _hitEventHorizon = false;



    private OutgoingLaserInfo _laserInfo;
    public Vector3 _firstPosition;
    public Vector3 _firstPosAfterChanges;


    private float laserOffset = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    void Update()
    {
        // If the laser has hit and edge and it's not the event horizon
        if (_hitEdge && !_hitEventHorizon)
        {
            // Get the current angle and location (Reminder: this object is following the line)
            _laserInfo.angle = this.transform.eulerAngles.z;

            // Set external to true, as we'll notify the singularity to create a laser using sprites instead of line renderer
            _laserInfo.external = true;

            Ray ray = new Ray(this.transform.position,this.transform.up);
            Vector3 point = ray.GetPoint(0.25f);
            _lineRenderer.SetPosition(_listPositions.Count-1,point);
            

            // Update the remaining information
            _laserInfo.origin = point;
            _laserInfo.raycastPosition = point;

            // Notify singularity to create an external laser
            _singularity.NewExternalLaser(_laserInfo);

            // Set hit edge to false
            _hitEdge = false;

        }
    }

    private void FireLaser()
    {

        // Set the current position to the first item in the list (the origin)
        Vector2 currentPoint = _listPositions[1];

        // While loop until an edge is hit.
        while (!_hitEdge)
        {
            // Set this transform to the current point
            this.transform.position = currentPoint;

            // Calculate the angular deflection
            float angularDeflection = CalculateAngularDeflection(currentPoint);

            // Check if determining bend during loop
            if (_determineBendDuringLoop)
            {
                // Re-prepare the helper (need angle from current position, instead of entry position)
                // Then determine angle and direction to bend
                float helperAngle = _singularity.PrepareHelper(angularDeflection,currentPoint);
                if (helperAngle < angularDeflection) _bendRight = true;
                else _bendRight = false;

            }
            // If you can set the angle to a negative do so  - it can easily become a NaN (I assume this is due to scale)
            if (_bendRight && !float.IsNaN(-angularDeflection)) this.transform.Rotate(0f,0f,-angularDeflection);

            // If the angle is not NaN
            else if (!float.IsNaN(angularDeflection)) this.transform.Rotate(0f,0f,angularDeflection);

            // Create a ray
            // Inspired by https://www.youtube.com/watch?v=pNE3rfMGEAw (2025/03/27), using ray to determine distance
            Ray ray = new Ray(currentPoint,this.transform.up);

            // Send a raycast a short distance into the scene to check for collisions
            RaycastHit2D raycast = Physics2D.Raycast(currentPoint,this.transform.up,0.1f);

            // If it has collided with something, and it's apart of this Singularity, then handle interaction
            // This is to prevent any possible satellites from entering and causing issues.
            if (raycast.collider != null && raycast.collider.gameObject == _singularity.gameObject)
            {

                // If a circle collider, it's the event horizon
                if (raycast.collider is CircleCollider2D)
                {
                    _hitEdge = true;
                    _hitEventHorizon = true;
                }

                // Otherwise it's an edge if its a polygon collider
                else if (raycast.collider is PolygonCollider2D) _hitEdge = true;

                // Else log as an error. It really should not be capable of hitting anything else.
                else Debug.LogError("ERROR: A Singularity has hit something unexpected");
                }
            else
            {
                // Get the point along the distance, and add it to the position list
                currentPoint = ray.GetPoint(0.1f);
                _listPositions.Add(currentPoint);
            }
        }

        // Create a new list of specific size
        // Line Renderer takes an array, not a list - hence we need a conversion
        // As we don't neccessarily know how many points we are going to make
        var points = new Vector3[_listPositions.Count];

        // Update the line renderer and reset it

       
        _lineRenderer.positionCount = _listPositions.Count;

        _lineRenderer.SetPositions(points);

        // Loop and add positions to the line renderer
        for (int i = 0; i < _listPositions.Count; i++)
        {
            points[i] = _listPositions[i];
        }
        _lineRenderer.SetPositions(points);
    }

    public void Activate(Singularity singularity,OutgoingLaserInfo laserInfo, float helperAngle, GravitationalAnomalySettings settings)
    {
        // Main Method

        // Save Singularity Information
        _singularity = singularity;
        _singularityPosition = singularity.transform.position;

        // Save laser info and set approprite information
        _laserInfo = laserInfo;
        _firstPosition = laserInfo.raycastPosition;

        // Locate the line renderer and determine the colour
        _lineRenderer = gameObject.GetComponent<LineRenderer>();
        Color newColour = Color.white;
        if (laserInfo.laserColour == LaserColour.White) newColour = Color.white;
        else if (laserInfo.laserColour == LaserColour.Red) newColour = Color.red;
        else if (laserInfo.laserColour == LaserColour.Blue) newColour = Color.blue;
        else if (laserInfo.laserColour == LaserColour.Green) newColour = Color.green;
        else if (laserInfo.laserColour == LaserColour.Yellow) newColour = Color.yellow;
        else if (laserInfo.laserColour == LaserColour.Cyan) newColour = Color.cyan;
        else if (laserInfo.laserColour == LaserColour.Magenta) newColour = Color.magenta;

        // Set colour - LineRender can do fades between colours (might be an interesting idea for later use)
        _lineRenderer.startColor = newColour;
        _lineRenderer.endColor = newColour;

        // Save settings and set appropriate information
        anomalySettings = settings;
        this.laserOffset = settings.laserOffset;
        _specifiedLimit = settings.distanceFromEventHorizonToChangeAngleSign;


        
        // Retreive the starting point, then apply an offset, calculated with a ray to maintain correct positioning - this is to prevent the laser from 
        // hitting the outer polygon collider (as then it won't trigger it again when it hits it again later on)
        Ray ray = new Ray(laserInfo.raycastPosition,laserInfo.incomingLaserDirection);
        Vector3 point = ray.GetPoint(0.25f);

        // Use the provided helper anlge to determine which way to bend the light (+ or - direction)
        // However, if the angle is within a specified limit, then have it determine it's bend DURING the laser creation loop (in FireLaser)
        if (helperAngle <= _specifiedLimit && helperAngle >= -_specifiedLimit) _determineBendDuringLoop = true; 
        else _bendRight = false;

        _listPositions.Add(laserInfo.raycastPosition);

        // Add Position tot he list
        _listPositions.Add(point);

        // Make sure the hit edge is false, same with hit event horizon
        _hitEdge = false;
        _hitEventHorizon = false;
        
        // Set this angle to the same as the laser
        // This object will move as it creates the line
        this.transform.eulerAngles = new Vector3(0f,0f,laserInfo.angle);

        // Set first pos (for later use)
        _firstPosAfterChanges = point;

        // Fire the laser!
        FireLaser();
    }

    private float CalculateAngularDeflection(Vector2 point)
    {
        // Angular Deflection - The Maths!
        // Please note that some multipliers have been added, purely for gameplay purposes as the 
        // angular deflection is often very small, and would require a signifcant number of points
        // that Unity cannott support during play. 
        // Additionally the mass is set to 10 Solar masses, it is advised not to change this as 
        // Unity has limitations on it's mass - during implementation -infinity was found to be the angle
        // at different points
        
        /// References / Helpful Links: (2025/03/27)
        /// - https://www.scribd.com/doc/25310028/Ij-i-3-o-j-c (Original Paper, Note is in German)
        /// - https://arxiv.org/abs/physics/9905030 (Translated Paper, Note is in English)
        /// - https://arxiv.org/abs/0709.2257 (Interpretation of the Paper)
        /// - https://en.wikipedia.org/wiki/Schwarzschild_geodesics#Bending_of_light_by_gravity (Main Article used for the equation)
        /// - https://en.wikipedia.org/wiki/Schwarzschild_radius 
        /// - https://en.wikipedia.org/wiki/Gravitational_lens#Explanation_in_terms_of_spacetime_curvature
        
        /// To summarise, this method makes use of the Angular Deflection equation as presented in the Schwarzschild Geodesics Wikipedia link
        
        /// The Equation (and related terms)
        /// δρ ≈ 2r_s / b = 4GM / c^2 b
        /// Angular Deflection ≈  2 (Schwarzschild Radius) / Impact Parameter

        /// Schwarzschild Radius (r_s) = 2GM / c^2
        /// Impact Parameter (b) = r_3 Sqrt (r_3 / (r_3 - r_s))
        /// Distance of closest approach (r_3) = Σ r  (Viewed as centre of point to centre of singularity)

        // Calcualte the distance of closest approach (Assumption it's singularity centre to light particle centre)
        // (It's easier to explain with light as particles)
        float distanceOfClosestApproach = Mathf.Sqrt(Mathf.Pow((_singularityPosition.x - point.x),2) + Mathf.Pow((_singularityPosition.y - point.y),2));

        // Apply a distance multiplier - NEEDED in order to actually show angle, otherwise it's either not noticible or too extreme
        distanceOfClosestApproach *=  ((anomalySettings.distanceMultipler * Mathf.Pow(10,anomalySettings.distanceMultiplierToPowerOf10)));

        // Calculate the gravitational constant
        float gravitationalConstant = (6.67430f * Mathf.Pow(10,-11));

        // Calculate the mass of the singularity in Solar Mass
        float massOfAnomaly = (anomalySettings.anomalyMass * Mathf.Pow(10.0f,anomalySettings.massToPowerOf10)) * (2 * Mathf.Pow(10,30));

        // Calcualte the Schwarzschold Radius
        float schwarzschildRadius = ( 2.0f * gravitationalConstant * massOfAnomaly) / Mathf.Pow(299792458,2);

        // Calculate the Impact Parameter
        float impactParameter = distanceOfClosestApproach * Mathf.Sqrt(((distanceOfClosestApproach) / (distanceOfClosestApproach - schwarzschildRadius)));

        // Calculat the Angular Deflection
        float angularDeflection =  (2.0f * schwarzschildRadius) / impactParameter;

        // Apply a muliplier - NEEDED in order to have the angle visible. Note: Recommended between 5 - 10 (inclusive) for best results
        angularDeflection *= anomalySettings.angleMulitplier;
        
        // Return angular deflection
        return angularDeflection;
    }
}



