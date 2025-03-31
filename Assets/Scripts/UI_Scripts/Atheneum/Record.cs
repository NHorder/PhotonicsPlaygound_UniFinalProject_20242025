using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum RecordType
{
    GeneralInformation,
    Light,
    Reflection,
    Refraction,
    FresnelEquations,
    Colour,
    Blackholes
}

public class Record : MonoBehaviour
{

    public RecordType recordType;
    private string _englishTitle;
    private string _englishText;
    private string _welshTitle;
    private string _welshText;

    private TMP_Text[] _contentElements;
    // Start is called before the first frame update
    void Start()
    {
        _contentElements = gameObject.GetComponentsInChildren<TMP_Text>();
    }

    private void GetText()
    {
        if (recordType == RecordType.GeneralInformation)
        {
            _englishTitle = "General Information";
            _englishText = "Goal of the Game\n\nThe goal of this game is to redirect the light from Prometheus to the corresponding Fyrefly satellite. Take note of the colour of both satellites, as Fyrefly will only respond to specific colours of light - White Fyrefly satellites respond to all light colours. \n\n\nGame Controls:\n\nMake use of the cursor to select satellites and the various objects within the scene, then use WASD to move selected satellites, QE to rotate. Or make use of the satellite control panel located at the bottom of the screen.\n\n\nEyes of Zeta\n\nThe Eye of Zeta is your view of the map, moving this satellite around will allow you to see different parts of the map, additionally it will move alongside any selected satellite.\n\n\nLevel Information\n\nThis describes the main purpose of the level, and has a settings and reset button should you need it.\n\n\nThe Shop:\n\nThe Shop is the basket icon on the lower right side of your screen, selecting this icon will open the shop, select a category and scroll to find the satellite you wish to purchase. Your budget is located at the top of the shop panel. Purchasing a satellite will send a request to Elysia to create it. Once created the satellite can be moved from Elysia’s printing bay anywhere you want it to.\n\n\nCommunications\n\nLocated in the top left of your screen, the communications panel displays all communications with Elysia and Fyrefly satellites as well as showing level progress. This panel will automatically open whenever you receive communications from these satellites, however this can be disabled in the settings menu. \n\n";
            _welshTitle = "";
            _welshText = "";
        }
        else if (recordType == RecordType.Light)
        {
            _englishTitle = "";
            _englishText = "";
            _welshTitle = "";
            _welshText = "";
        }
        else if (recordType == RecordType.Reflection)
        {
            _englishTitle = "";
            _englishText = "";
            _welshTitle = "";
            _welshText = "";
        }
        else if (recordType == RecordType.Refraction)
        {
            _englishTitle = "";
            _englishText = "";
            _welshTitle = "";
            _welshText = "";
        }
        else if (recordType == RecordType.FresnelEquations)
        {
            _englishTitle = "";
            _englishText = "";
            _welshTitle = "";
            _welshText = "";
        }
        else if (recordType == RecordType.Colour)
        {
            _englishTitle = "";
            _englishText = "";
            _welshTitle = "";
            _welshText = "";
        }
        else if (recordType == RecordType.Blackholes)
        {
            _englishTitle = "";
            _englishText = "";
            _welshTitle = "";
            _welshText = "";
        }
    }

    public void UpdateLanguage(Language language)
    {

        GetText();

        foreach (TMP_Text contentText in _contentElements)
        {
            if (language == Language.English)
            {
                if (contentText.gameObject.name == "Title") contentText.text = _englishTitle;
                if (contentText.gameObject.name == "CoreText") contentText.text = _englishText;
            }

            else if (language == Language.Welsh)
            {
                if (contentText.gameObject.name == "Title") contentText.text = _welshTitle;
                if (contentText.gameObject.name == "CoreText") contentText.text = _welshText;
            }
        }
        
    }

}
