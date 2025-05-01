using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SatelliteCreator : MonoBehaviour
{
    /// <summary>
    /// Class for satellite creator
    /// </summary>

    private Language _language = Language.English;

    // Stores all satellite prefabs
    public SatellitePrefabs satellitePrefabs;


    // Lock to prevent further creation during the creation of a satellite
    private bool _canCreateNewSatellite = true;
    private GameObject _chosenPrefab;
    private GameObject _lastCreatedSatellite;



    private Animator _animator;


    private bool _animating = false;
    private int _numberSatellitesInLoadingBay = 0;
    
    // The satellite being made can be delayed if another satellite is moved into the creation bay during animation
    private bool _delayedCreation = false;


    private CommunicationsPanel _communicationsPanel;
    private UIController _uiController;

    private GameObject _satWithUIElements;

    private string _occupiedText = "";
    private string _satNotRecognised = "";
    private string _satCreated = "";
    private string _satDelayed = "";

    // Start is called before the first frame update
    /// <summary>
    /// Inialisation Method
    /// </summary>
    void Start()
    {
        // Retrieve needed components
        _language = PersistenceController.GetLanguage();
        _uiController = GameObject.FindGameObjectsWithTag("UI_Controller")[0].GetComponent<UIController>();
        _animator = gameObject.GetComponent<Animator>();
        _satWithUIElements = GameObject.FindGameObjectsWithTag("Satellites_WithUIElements")[0];
        _communicationsPanel = GameObject.FindGameObjectsWithTag("CommunicationsPanel")[0].GetComponent<CommunicationsPanel>();

        // Call update language to set default names
        UpdateLanguage(_language);
    }

    /// <summary>
    /// Method to update language on notification
    /// </summary>
    /// <param name="newLanguage"></param>
    public void UpdateLanguage(Language newLanguage)
    {
        _language = newLanguage;
        
        if (_language == Language.English)
        {
            _occupiedText = "Printer space occupied, please move satellite in printer space before making another request\n";
            _satNotRecognised = "Satellite not recognised\n";
            _satCreated = "Satellite created, you can find it in the printing bay\n";
            _satDelayed = "Printer space occupied, please move satellite in printer space.\n";

        }
        else if (_language == Language.Welsh)
        {
            _occupiedText = "Gofod yr argraffydd wedi'i feddiannu, symudwch y lloeren yn y gofod argraffydd cyn gwneud cais arall os gwelwch yn dda\n";
            _satNotRecognised = "Lloeren heb ei hadnabod\n";
            _satCreated = "Lloeren wedi'i chreu, gallwch ddod o hyd iddo yn y bae argraffu\n";
            _satDelayed = "Gofod yr argraffydd wedi'i feddiannu, symudwch y lloeren yn y gofod argraffydd os gwelwch yn dda.\n";
        }
    }

    /// <summary>
    /// Method called to create a new satellite of a specific type
    /// </summary>
    /// <param name="satelliteType"></param>
    /// <returns></returns>
    public bool CreateSatellite(SatelliteType satelliteType)
    { 
        // As each colour filter is a different type, and doesn't have different animations, then only run the one filter animaton
        // if the input type is a colour filter (prefabs are different, hence need for the else if statements)
        var bFilter = false;

        // If a new satellite cannot be made, then send communciations and return false to caller
        if (!_canCreateNewSatellite)
        {
            if (!_uiController.advancedSettings.overwriteCommunicationMovement) _uiController.ToggleVisibleCommunicationsIfClosed();
            // Notify comns panel about text
            _communicationsPanel.LogCommunications("Elysia",-1,_occupiedText);

            return false;
        }
        else
        {
            // Find the satellite and begin the related animation
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
            
            else{
                _uiController.ToggleVisibleCommunicationsIfClosed();
                if (!_uiController.advancedSettings.overwriteCommunicationMovement) _communicationsPanel.LogCommunications("Elysia",-1,_satNotRecognised);
                Debug.LogWarning("WARNING: Satellite type not recognised");
                return false;
            }

            // Due to the number of colour filters (and prefab variants)
            // trigger this later on after determining the prefab
            if (bFilter)
            {
                _animating = true;
                _canCreateNewSatellite = false;

                _animator.SetBool("Animating",true);
                _animator.Play("Filter");
            }

        }
        return true;
    }

    /// <summary>
    /// Method to instantiate a satellite
    /// Called by a trigger in all satellite creation animations (besides base and selected)
    /// </summary>
    public void InstantiateSatellite()
    {
        // Notify finish of animation
        _animating = false;

        // if the chosen prefab is null and the loading bay is unoccupied
        if (_chosenPrefab != null && _numberSatellitesInLoadingBay == 0)
        {
            // Log communications (show panel if settings allow)
            if (!_uiController.advancedSettings.overwriteCommunicationMovement) _uiController.ToggleVisibleCommunicationsIfClosed();
             _communicationsPanel.LogCommunications("Elysia",-1,_satCreated);

            // Instantiate the satellite
            var newSatellite = Instantiate(_chosenPrefab);
            newSatellite.layer = LayerMask.NameToLayer("Object");
            newSatellite.transform.position = this.transform.position;

            // If a custom colour filter, then specifically parent it under the UI Canvas (otherwise UI elements won't be displayed)
            if (newSatellite.GetComponent<SatelliteInfo>().satelliteType == SatelliteType.CustomColourFilter)
            {
                newSatellite.transform.SetParent(_satWithUIElements.transform);
            }

            // Update angle to match the satellites creator one
            newSatellite.transform.eulerAngles = this.transform.eulerAngles;

            _lastCreatedSatellite = newSatellite;
        }
        else if (_delayedCreation)
        {
            // If the update has been delayed, notify comms
            if (!_uiController.advancedSettings.overwriteCommunicationMovement) _uiController.ToggleVisibleCommunicationsIfClosed();
            _communicationsPanel.LogCommunications("Elysia",-1,_satDelayed);
        }
        else
        {
            // State that no satellite has been made and reset the system
            Debug.Log("No satellite made");
            _canCreateNewSatellite = true;
            _delayedCreation = false;
        }

        // Update bool so that animation actually stops, and reset the chosen prefab unless delayed creation exists (as we still need it)
        _animator.SetBool("Animating",false);
        if (!_delayedCreation) _chosenPrefab = null;
    }



    /// <summary>
    /// Method to handle on trigger collisions
    /// </summary>
    /// <param name="collider"></param>
    public void OnTriggerEnter2D(Collider2D collider)
    {
        // If a satellite enters (and not a satellite controller AND not a camera drone)
        if (collider is PolygonCollider2D && collider.gameObject.GetComponent<SatelliteInfo>().satelliteType != SatelliteType.CameraDrone)
        {
            Debug.Log(collider.gameObject.name);

            // Increment number (prevents satellite creation)
            _numberSatellitesInLoadingBay += 1;

            // IF animating, then create a delay before creation
            if (_animating)
            {
                _delayedCreation = true;
            }
        }

        
    }

    /// <summary>
    /// Method to handle on triggger exit collision
    /// </summary>
    /// <param name="collider"></param>
    public void OnTriggerExit2D(Collider2D collider)
    {
        // If a polygon collider has left the loading bay
        if (collider is PolygonCollider2D)
        {
            _numberSatellitesInLoadingBay -=1;

            // Check if the number of satellites in the loading bay is 0, and creation is delayed
            // Immediately instantiate the satellite
            if (_numberSatellitesInLoadingBay == 0 && _delayedCreation)
            {
                _delayedCreation = false;
                InstantiateSatellite();
            }
        }
        // If the object leaving the loading bay was the most recent created satellite, then set can create to true
        if (collider.gameObject == _lastCreatedSatellite)
        {
            _canCreateNewSatellite = true;
        }
    }
}


/// <summary>
/// External class used to store all satellite prefabs
/// </summary>
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
