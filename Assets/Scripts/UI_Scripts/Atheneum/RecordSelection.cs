using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecordSelection : MonoBehaviour
{
    /// <summary>
    /// Class to handle record selection within the Atheneaum
    /// </summary>

    private string _englishTitle;
    private string _welshTitle;

    private TMP_Text[] _contentElements;

    private Record[] _records;
    private Record _visibleRecord;


    private bool _firstLoad = true;

    private UIController _uiController;

    // Start is called before the first frame update
    /// <summary>
    /// Initalisation Method
    /// </summary>
    void Start()
    {
        _uiController = GameObject.FindGameObjectsWithTag("UI_Controller")[0].GetComponent<UIController>();

        // Find the Athenaeum and collect the records and content
        var _athenaeum = GameObject.FindGameObjectsWithTag("Athenaeum")[0];
        _records = _athenaeum.GetComponentsInChildren<Record>();
        _contentElements = gameObject.GetComponentsInChildren<TMP_Text>();

        // Update language - to make sure language is correct
        UpdateLanguage(PersistenceController.GetLanguage());

        // Display general information
        ShowGeneralInfo();

        // Set first load to false (When set to true, bypasses reset of previous selection)
        _firstLoad = false;
    }

    /// <summary>
    /// Method to determine text based on record types
    /// </summary>
    /// <param name="recordType"></param>
    private void GetText(RecordType recordType)
    {
        if (recordType == RecordType.GeneralInformation)
        {
            _englishTitle = "General Information";
            _welshTitle = "";
        }
        else if (recordType == RecordType.Light)
        {
            _englishTitle = "Light";
            _welshTitle = "";
        }
        else if (recordType == RecordType.Reflection)
        {
            _englishTitle = "Reflection";
            _welshTitle = "";
        }
        else if (recordType == RecordType.Refraction)
        {
            _englishTitle = "Refraction";
            _welshTitle = "";
        }
        else if (recordType == RecordType.FresnelEquations)
        {
            _englishTitle = "Fresnel Equations";
            _welshTitle = "";
        }
        else if (recordType == RecordType.Colour)
        {
            _englishTitle = "Colour";
            _welshTitle = "";
        }
        else if (recordType == RecordType.Blackholes)
        {
            _englishTitle = "Gravitational Anomalies";
            _welshTitle = "";
        }
        else
        {
            _englishTitle = "Silver Athenaeum";
            _welshTitle = "";
        }
    }

    /// <summary>
    /// Method to update langauge
    /// </summary>
    /// <param name="language"></param>
    public void UpdateLanguage(Language language)
    {

        // Loop through all content and update based on text retrieved
        foreach (TMP_Text contentText in _contentElements)
        {
            if (contentText.gameObject.name == "GeneralInfoText") GetText(RecordType.GeneralInformation);
            else if (contentText.gameObject.name == "LightText") GetText(RecordType.Light);
            else if (contentText.gameObject.name == "ReflectionText") GetText(RecordType.Reflection);
            else if (contentText.gameObject.name == "RefractionText") GetText(RecordType.Refraction);
            else if (contentText.gameObject.name == "FresnelEquationsText") GetText(RecordType.FresnelEquations);
            else if (contentText.gameObject.name == "ColourText") GetText(RecordType.Colour);
            else if (contentText.gameObject.name == "BlackholeText") GetText(RecordType.Blackholes);


            if (contentText.gameObject.name == "Title")
            {
                _englishTitle = "Silver Athenaeum";
                _welshTitle = "";
            }

            if (language == Language.English)contentText.text = _englishTitle;
            else if (language == Language.Welsh)contentText.text = _welshTitle;
        }
    
        // Loop through each record and notify them to update
        foreach (Record record in _records)
        {
            record.UpdateLanguage(language);
        }
    }

    /// <summary>
    /// Method to display a specific record
    /// </summary>
    /// <param name="recordType"></param>
    private void ShowRecord(RecordType recordType)
    {
        // Set currently visible record active to false (hides it)
        if (_visibleRecord != null) _visibleRecord.gameObject.active = false;

        // Loop through and find the wanted record, set it active to true (makes it visible) and update the visible record
        foreach (Record nonVisibleRecord in _records)
        {
            if (nonVisibleRecord.recordType == recordType)
            {
                nonVisibleRecord.gameObject.active = true;
                _visibleRecord = nonVisibleRecord;
                if (!_firstLoad) break;
            }
            
            else if (_firstLoad)
            {
                nonVisibleRecord.gameObject.active = false;
            }
        }
    }

    /// <summary>
    /// UI Method allows showing of General Infomation Record, calls ShowRecord
    /// </summary>
    public void ShowGeneralInfo()
    {
        ShowRecord(RecordType.GeneralInformation);
    }

    /// <summary>
    /// UI Method allows showing of Light Record, calls ShowRecord
    /// </summary>
    public void ShowLightRecord()
    {
        ShowRecord(RecordType.Light);
    }

    /// <summary>
    /// UI Method allows showing of Reflection Record, calls ShowRecord
    /// </summary>
    public void ShowReflectionRecord()
    {
        ShowRecord(RecordType.Reflection);
    }

    /// <summary>
    /// UI Method allows showing of Refraction Record, calls ShowRecord
    /// </summary>
    public void ShowRefractionRecord()
    {
        ShowRecord(RecordType.Refraction);
    }

    /// <summary>
    /// UI Method allows showing of Fresnel Equations Record, calls ShowRecord
    /// </summary>
    public void ShowFresnelEquationsRecord()
    {
        ShowRecord(RecordType.FresnelEquations);
    }

    /// <summary>
    /// UI Method allows showing of Colour Record, calls ShowRecord
    /// </summary>
    public void ShowColourRecord()
    {
        ShowRecord(RecordType.Colour);
    }

    /// <summary>
    /// UI Method allows showing of Blackhole Record, calls ShowRecord
    /// </summary>
    public void ShowBlackholeRecord()
    {
        ShowRecord(RecordType.Blackholes);
    }

    /// <summary>
    /// UI Method to hide the Athenaeum
    /// </summary>
    public void CloseAthenaeum ()
    {
        _uiController.ToggleAthenaeum();
    }

}
