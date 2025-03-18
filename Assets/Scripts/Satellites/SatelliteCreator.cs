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

    private bool _animating = false;
    private int _numberSatellitesInLoadingBay = 0;
    private bool _delayedCreation = false;

    private CommunicationsPanel _communicationsPanel;

    // Start is called before the first frame update
    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();

        _communicationsPanel = GameObject.FindGameObjectsWithTag("CommunicationsPanel")[0].GetComponent<CommunicationsPanel>();

    }


    public bool CreateSatellite(SatelliteType satelliteType)
    {
        // As each colour filter is a different type, and doesn't have different animations, then only run the one filter animaton
        // if the input type is a colour filter (prefabs are different, hence need for the else if statements)
        var bFilter = false;

        if (!_canCreateNewSatellite)
        {
            // Notify comns panel about text
            _communicationsPanel.LogCommunications("Elysia",-1,"Printer space occupied, please move satellite in printer space before making another request\n");

            return false;
        }

        if (satelliteType == SatelliteType.SingleSideReflector)
        {
            _animating = true;
            _chosenPrefab = satellitePrefabs.singlePanelReflectionSatellite;

            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("SingleReflector");
        }
        
        else if (satelliteType == SatelliteType.DoubleSideReflector)
        {
            _animating = true;
            _chosenPrefab = satellitePrefabs.doublePanelReflectionSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("DoubleReflector");
        }
        
        else if (satelliteType == SatelliteType.GlassRefractor)
        {
            _animating = true;
            _chosenPrefab = satellitePrefabs.glassRefractionSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Refractor");
        }
        else if (satelliteType == SatelliteType.SapphireRefractor)
        {
            _animating = true;
            _chosenPrefab = satellitePrefabs.sapphireRefractionSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Refractor");
        }
        else if (satelliteType == SatelliteType.SiliconRefractor)
        {
            _animating = true;
            _chosenPrefab = satellitePrefabs.siliconRefractionSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Refractor");
        }
        else if (satelliteType == SatelliteType.WaterRefractor)
        {
            _animating = true;
            _chosenPrefab = satellitePrefabs.waterRefractionSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Refractor");
        }


        else if (satelliteType == SatelliteType.WhiteBasicColourFilter)
        {
            
            bFilter = true;
            _chosenPrefab = satellitePrefabs.whiteFilterSatellite;
        }
        else if (satelliteType == SatelliteType.RedBasicColourFilter)
        {
            bFilter = true;
            _chosenPrefab = satellitePrefabs.redFilterSatellite;
        }
        else if (satelliteType == SatelliteType.BlueBasicColourFilter)
        {
            bFilter = true;
            _chosenPrefab = satellitePrefabs.blueFilterSatellite;
        }
        else if (satelliteType == SatelliteType.GreenBasicColourFilter)
        {
            bFilter = true;
            _chosenPrefab = satellitePrefabs.greenFilterSatellite;
        }
        else if (satelliteType == SatelliteType.YellowBasicColourFilter)
        {
            bFilter = true;
            _chosenPrefab = satellitePrefabs.yellowFilterSatellite;
        }
        else if (satelliteType == SatelliteType.CyanBasicColourFilter)
        {
            bFilter = true;
            _chosenPrefab = satellitePrefabs.cyanFilterSatellite;
        }
        else if (satelliteType == SatelliteType.MagentaBasicColourFilter)
        {
            bFilter = true;
            _chosenPrefab = satellitePrefabs.magentaFilterSatellite;
        }
        


        else if (satelliteType == SatelliteType.CustomColourFilter)
        {
            _animating = true;
            _chosenPrefab = satellitePrefabs.customFilterSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("CustomFilter");
        }

        else if (satelliteType == SatelliteType.Combiner)
        {
            _animating = true;
            _chosenPrefab = satellitePrefabs.combinerSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Combiner");
        }
        
        else if (satelliteType == SatelliteType.DuelSplitter) 
        {
            _animating = true;
            _chosenPrefab = satellitePrefabs.duelSplitterSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Splitter");
        }
        else if (satelliteType == SatelliteType.TrioSplitter)
        {
            _animating = true;
            _chosenPrefab = satellitePrefabs.trioSplitterSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Splitter_3Node");
        }
        else if (satelliteType == SatelliteType.HexSplitter)
        {
            _animating = true;
            _chosenPrefab = satellitePrefabs.hexSplitterSatellite;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Splitter_6Node");
        }
    
        else{
            _communicationsPanel.LogCommunications("Elysia",-1,"Satellite not recognised\n");
            Debug.LogWarning("WARNING: Satellite type not recognised");
            return false;
        }


        if (bFilter)
        {
            _animating = true;
            _canCreateNewSatellite = false;

            _animator.SetBool("Animating",true);
            _animator.Play("Filter");
        }


        return true;
    }

    public void InstantiateSatellite()
    {
        _animating = false;
        Debug.Log(_numberSatellitesInLoadingBay);

        if (_chosenPrefab != null && _numberSatellitesInLoadingBay == 0)
        {
            
            _communicationsPanel.LogCommunications("Elysia",-1,"Satellite created, you can find it in the printing bay\n");
            var newSatellite = Instantiate(_chosenPrefab);
            newSatellite.layer = LayerMask.NameToLayer("Object");
            newSatellite.transform.position = this.transform.position;

            _lastCreatedSatellite = newSatellite;
        }
        else if (_delayedCreation)
        {
            Debug.Log("Delaying Satellite Creation until item is out of the creator bay");
            _communicationsPanel.LogCommunications("Elysia",-1,"Printer space occupied, please move satellite in printer space.\n");
        }
        else
        {
            Debug.Log("No satellite made");
        }

        _animator.SetBool("Animating",false);
        if (!_delayedCreation) _chosenPrefab = null;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (!(collider.gameObject.name == "SatelliteController"))
        {
            _numberSatellitesInLoadingBay += 1;

            if (_animating)
            {
                _delayedCreation = true;
            }
        }

        
    }


    public void OnTriggerExit2D(Collider2D collider)
    {

        if (!(collider.gameObject.name == "SatelliteController"))
        {
            _numberSatellitesInLoadingBay -=1;

            if (_numberSatellitesInLoadingBay == 0 && _delayedCreation)
            {
                _delayedCreation = false;
                InstantiateSatellite();
            }

        }

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
    public GameObject whiteFilterSatellite;
    public GameObject redFilterSatellite;
    public GameObject blueFilterSatellite;
    public GameObject greenFilterSatellite;
    public GameObject yellowFilterSatellite;
    public GameObject cyanFilterSatellite;
    public GameObject magentaFilterSatellite;
    public GameObject customFilterSatellite;
}
