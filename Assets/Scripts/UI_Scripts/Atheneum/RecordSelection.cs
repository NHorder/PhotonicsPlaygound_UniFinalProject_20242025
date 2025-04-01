using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecordSelection : MonoBehaviour
{

    private string _englishTitle;
    private string _welshTitle;

    private TMP_Text[] _contentElements;

    private Record[] _records;
    private Record _visibleRecord;


    private bool _firstLoad = true;

    private UIController _uiController;

    // Start is called before the first frame update
    void Start()
    {
        _uiController = GameObject.FindGameObjectsWithTag("UI_Controller")[0].GetComponent<UIController>();

        var _athenaeum = GameObject.FindGameObjectsWithTag("Athenaeum")[0];
        _records = _athenaeum.GetComponentsInChildren<Record>();


        _contentElements = gameObject.GetComponentsInChildren<TMP_Text>();
        UpdateLanguage(PersistenceController.GetLanguage());

        ShowGeneralInfo();
        _firstLoad = false;
    }

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

    public void UpdateLanguage(Language language)
    {

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
    
        foreach (Record record in _records)
        {
            record.UpdateLanguage(language);
        }
    }


    private void ShowRecord(RecordType recordType)
    {
        if (_visibleRecord != null) _visibleRecord.gameObject.active = false;

        foreach (Record nonVisibleRecord in _records)
        {
            if (nonVisibleRecord.recordType == recordType)
            {
                Debug.Log("?");
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

    public void ShowGeneralInfo()
    {
        ShowRecord(RecordType.GeneralInformation);
    }

    public void ShowLightRecord()
    {
        ShowRecord(RecordType.Light);
    }

    public void ShowReflectionRecord()
    {
        ShowRecord(RecordType.Reflection);
    }

    public void ShowRefractionRecord()
    {
        ShowRecord(RecordType.Refraction);
    }

    public void ShowFresnelEquationsRecord()
    {
        ShowRecord(RecordType.FresnelEquations);
    }

    public void ShowColourRecord()
    {
        ShowRecord(RecordType.Colour);
    }

    public void ShowBlackholeRecord()
    {
        ShowRecord(RecordType.Blackholes);
    }


    public void CloseAthenaeum ()
    {
        _uiController.ToggleAthenaeum();
    }

}
