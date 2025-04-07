using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SatelliteInformationPanel : MonoBehaviour
{
    private Language _language = Language.English;


    private SatelliteController _satelliteController;


    private SatelliteInfo _selectedSatelliteInfo;

    private TMP_Text _titleText;
    private TMP_Text _descriptionText;
    private TMP_Text _sellText;

    private TMP_Text _statusText;
    private TMP_Text _lightColour;
    private bool _forceTrigger = true;



    // Start is called before the first frame update
    void Start()
    {
        _language = PersistenceController.GetLanguage();
        
        _satelliteController = GameObject.FindGameObjectsWithTag("MouseController")[0].GetComponent<SatelliteController>();


        var childTextList = gameObject.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text childText in childTextList)
        {
            var childObject = childText.gameObject;

            // filter for log text and progress text
            if (childObject.name == "SatelliteName") _titleText = childText;
            else if (childObject.name == "SatelliteDescription") _descriptionText = childText;
            else if (childObject.name == "SellText") _sellText = childText;
            else if (childObject.name == "SatelliteStatus") _statusText = childText;
            else if (childObject.name == "LightColour") _lightColour = childText;
        }

    }

    void Update()
    {
        // Update information if applicable
        // Doing this on update to avoid the link between controller and this

        if (_selectedSatelliteInfo != _satelliteController.selectedSatelliteInfo || _forceTrigger)
        {
            _forceTrigger = false;

            _selectedSatelliteInfo = _satelliteController.selectedSatelliteInfo;

            if (_selectedSatelliteInfo != null)
            {
                _titleText.text = _selectedSatelliteInfo.satelliteName;
                _descriptionText.text = _selectedSatelliteInfo.satelliteDescription;

                if (_selectedSatelliteInfo.canBeSold)
                {
                    _sellText.text = "£"+_selectedSatelliteInfo.satelliteSellPrice;
                }
                else
                {
                    if (_language == Language.English) _sellText.text = "Not For Sale";
                    else if (_language == Language.Welsh) _sellText.text = "Ddim Ar Werth";
                }


                if (_selectedSatelliteInfo.satelliteType == SatelliteType.Destination)
                {
                    var destinationSat = _selectedSatelliteInfo.gameObject.GetComponent<DestinationSatellite>();

                    if (_language == Language.English && destinationSat.allLocksOpen) _statusText.text = "Status: Active";
                    else if (_language == Language.English && !destinationSat.allLocksOpen) _statusText.text = "Status: Inactive";
                    else if (_language == Language.Welsh && destinationSat.allLocksOpen) _statusText.text = "Statws: Gweithredol";
                    else if (_language == Language.Welsh && !destinationSat.allLocksOpen) _statusText.text = "Statws: Anweithredol";
                }
                else
                {
                    _statusText.text = "";
                }

            }
            else
            {
                if (_language == Language.English)
                {
                    _titleText.text = "Nothing";
                    _descriptionText.text = "Nothing has been selected. Please select a satellite to view it's information or close this tab.";
                    _sellText.text = "";
                    _statusText.gameObject.active = false;
                    
                }
                else if (_language == Language.Welsh)
                {
                    _titleText.text = "Dim";
                    _descriptionText.text = "Does dim byd wedi'i ddewis. Dewiswch loeren i weld ei gwybodaeth neu cau'r tab hwn.";
                    _sellText.text = " ";
                    _statusText.gameObject.active = false;
                }

            }
            
        }
    }

    public void UpdateLanguage(Language newLanguage)
    {
        _language = newLanguage;
        _forceTrigger = true;
    }
}
