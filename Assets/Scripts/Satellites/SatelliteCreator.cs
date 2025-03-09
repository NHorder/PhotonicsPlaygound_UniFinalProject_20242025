using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
            levelProgressPanel.LogCommunications("Elysia",-1,"Printer space occupied, please move satellite in printer space before making another request\n");

            return false;
        }

        if (satelliteType == SatelliteType.SingleSideReflector)
        {
            _chosenPrefab = satellitePrefabs.singlePanelReflectionSatellite;

            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("SingleReflector");
        }
        
        else if (satelliteType == SatelliteType.DoubleSideReflector)
        {

            _chosenPrefab = satellitePrefabs.doublePanelReflectionSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("DoubleReflector");
        }
        
        else if (satelliteType == SatelliteType.GlassRefractor)
        {

            _chosenPrefab = satellitePrefabs.glassRefractionSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Refractor");
        }

        else if (satelliteType == SatelliteType.ColourFilter)
        {

            _chosenPrefab = satellitePrefabs.filterSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Filter");
        }

        else if (satelliteType == SatelliteType.Combiner)
        {

            _chosenPrefab = satellitePrefabs.combinerSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Combiner");
        }
        
        else if (satelliteType == SatelliteType.Splitter) 
        {

            _chosenPrefab = satellitePrefabs.splitterSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Splitter");
        }
    
        else{
            levelProgressPanel.LogCommunications("Elysia",-1,"Satellite not recognised\n");
            Debug.LogWarning("WARNING: Satellite type not recognised");
            return false;
        }

        return true;
    }

    public void InstantiateSatellite()
    {
        if (_chosenPrefab != null)
        {
            levelProgressPanel.LogCommunications("Elysia",-1,"Satellite created, you can find it in the printing bay\n");
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
    public GameObject doublePanelReflectionSatellite;
    public GameObject glassRefractionSatellite;
    public GameObject splitterSatellite;
    public GameObject combinerSatellite;
    public GameObject filterSatellite;
}
