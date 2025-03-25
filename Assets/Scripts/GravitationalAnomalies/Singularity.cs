using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singularity : MonoBehaviour
{

    private LineRenderer _lineRenderer;

    private Transform _blackholeHelper;

    public GameObject _singularityLaser;

    private List<GameObject> _listLasers = new List<GameObject>();

    private List<Vector2> _listOrigins = new List<Vector2>();

    private int _currentUpdateCount = 0;
    private int _updateDelay = 1;

    private GameController _gameController;


    private bool _reset = false;


    public GravitationalAnomalyPhysics gravitationalAnomalyPhysics;
    public GravitationalAnomalySettings gravitationalAnomalySettings;


    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = gameObject.GetComponent<LineRenderer>();
        _blackholeHelper = GameObject.FindGameObjectsWithTag("BlackholeHelper")[0].GetComponent<Transform>();
    }

    // Update is called once per frame
    private void Reset()
    {
        _reset = false;
        // Reset update count
        _currentUpdateCount = 0;

        // Delete all lasers
        foreach (GameObject laser in _listLasers) Destroy(laser);

        // Clear list
        _listLasers.Clear();
        _listOrigins.Clear();
    }

    public void SetActive(Laser laser, RaycastHit2D raycast)
    {

        if (!_listOrigins.Contains(raycast.point))
        {
            Reset();
            _listOrigins.Add(raycast.point);

            bool bendRight = PrepareHelper(laser,raycast);

            var newLaser = Instantiate(_singularityLaser);
            newLaser.GetComponent<SingularityLaser>().Activate(this,laser.transform,raycast.point, bendRight,gravitationalAnomalyPhysics,gravitationalAnomalySettings);
            _listLasers.Add(newLaser);
        }
    }

    private bool PrepareHelper(Laser laser, RaycastHit2D raycast)
    {
        _blackholeHelper.position = raycast.point;
        _blackholeHelper.eulerAngles = laser.transform.eulerAngles;

        _blackholeHelper.up = transform.position - _blackholeHelper.position;

        float _blackholeHelperAngle = _blackholeHelper.eulerAngles.z;
        if (_blackholeHelperAngle > 180) _blackholeHelperAngle = (_blackholeHelperAngle - 360);

        bool bendRight = false;

        if (_blackholeHelperAngle < laser.transform.eulerAngles.z) bendRight = true;

        return bendRight;
    }
}
