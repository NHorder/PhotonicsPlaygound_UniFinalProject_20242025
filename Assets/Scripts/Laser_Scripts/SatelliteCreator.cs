using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SatelliteType{
    Unknown,
    SingleSideReflector,
    GlassRefractor,
    Origin,
    Destination,
    SatelliteCreator,

}

public class SatelliteCreator : MonoBehaviour
{

    public SatellitePrefabs satellitePrefabs;

    private bool _canCreateNewSatellite = true;
    private Animator _animator;

    private GameObject _chosenPrefab;

    private GameObject _lastCreatedSatellite;


    private LevelProgressPanel levelProgressPanel;

    // Start is called before the first frame update
    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();

        levelProgressPanel = GameObject.FindGameObjectsWithTag("LevelProgressPanel")[0].GetComponent<LevelProgressPanel>();

    }


    public bool CreateSatellite(SatelliteType satelliteType)
    {

        if (!_canCreateNewSatellite)
        {
            // Notify comns panel about text
            levelProgressPanel.LogCommunications("Elysia-003",-1,"Printer space occupied, please move satellite in printer space before making another request\n");

            return false;
        }


        if (satelliteType == SatelliteType.SingleSideReflector)
        {
            levelProgressPanel.LogCommunications("Elysia-003",-1,"Creating satellite\n");
            _chosenPrefab = satellitePrefabs.singlePanelReflectionSatellite;

            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("SingleReflector");

            return true;

        }
        else if (satelliteType == SatelliteType.GlassRefractor)
        {
            levelProgressPanel.LogCommunications("Elysia-003",-1,"Creating satellite\n");

            _chosenPrefab = satellitePrefabs.glassRefractionSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Refractor");

            return true;
            
        }
        
        else{
            levelProgressPanel.LogCommunications("Elysia-003",-1,"Satellite not recognised\n");
            Debug.LogWarning("WARNING: Satellite type not recognised");
            return false;
        }
    }

    public void InstantiateSatellite()
    {
        if (_chosenPrefab != null)
        {
            levelProgressPanel.LogCommunications("Elysia-003",-1,"Satellite created\n");
            var newSatellite = Instantiate(_chosenPrefab);
            newSatellite.layer = LayerMask.NameToLayer("Object");
            newSatellite.transform.position = this.transform.position;
            _animator.SetBool("Animating",false);

            _lastCreatedSatellite = newSatellite;
        }
        

        _chosenPrefab = null;
    }


    public void OnTriggerExit2D(Collider2D collider)
    {

        if (collider.gameObject == _lastCreatedSatellite)
        {
            _canCreateNewSatellite = true;
        }
    }
}

[System.Serializable]
public class SatellitePrefabs
{
    public GameObject singlePanelReflectionSatellite;
    public GameObject glassRefractionSatellite;
}