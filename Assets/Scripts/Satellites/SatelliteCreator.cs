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
        else if (satelliteType == SatelliteType.SapphireRefractor)
        {
            _chosenPrefab = satellitePrefabs.sapphireRefractionSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Refractor");
        }
        else if (satelliteType == SatelliteType.SiliconRefractor)
        {
            _chosenPrefab = satellitePrefabs.siliconRefractionSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Refractor");
        }
        else if (satelliteType == SatelliteType.WaterRefractor)
        {
            _chosenPrefab = satellitePrefabs.waterRefractionSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Refractor");
        }

        else if (satelliteType == SatelliteType.BasicColourFilter)
        {
            _chosenPrefab = satellitePrefabs.filterSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Filter");
        }
        else if (satelliteType == SatelliteType.CustomColourFilter)
        {
            _chosenPrefab = satellitePrefabs.customFilterSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("CustomFilter");
        }

        else if (satelliteType == SatelliteType.Combiner)
        {

            _chosenPrefab = satellitePrefabs.combinerSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Combiner");
        }
        
        else if (satelliteType == SatelliteType.DuelSplitter) 
        {
            _chosenPrefab = satellitePrefabs.duelSplitterSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Splitter");
        }
        else if (satelliteType == SatelliteType.TrioSplitter)
        {
            _chosenPrefab = satellitePrefabs.trioSplitterSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Splitter_3Node");
        }
        else if (satelliteType == SatelliteType.HexSplitter)
        {
            _chosenPrefab = satellitePrefabs.hexSplitterSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Splitter_6Node");
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
        Debug.Log("Called!");

        if (_chosenPrefab != null)
        {
            levelProgressPanel.LogCommunications("Elysia",-1,"Satellite created, you can find it in the printing bay\n");
            var newSatellite = Instantiate(_chosenPrefab);
            newSatellite.layer = LayerMask.NameToLayer("Object");
            newSatellite.transform.position = this.transform.position;

            _lastCreatedSatellite = newSatellite;
        }
        else
        {
            Debug.Log("No satellite made");
        }

        _animator.SetBool("Animating",false);
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
    public GameObject sapphireRefractionSatellite;
    public GameObject siliconRefractionSatellite;
    public GameObject waterRefractionSatellite;
    public GameObject duelSplitterSatellite;
    public GameObject trioSplitterSatellite;
    public GameObject hexSplitterSatellite;
    public GameObject combinerSatellite;
    public GameObject filterSatellite;
    public GameObject customFilterSatellite;
}
