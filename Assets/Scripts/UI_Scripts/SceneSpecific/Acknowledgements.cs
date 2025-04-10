using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Acknowledgements : MonoBehaviour
{
    /// <summary>
    /// Method to display Acknoledgements Screen
    /// Note: Does not have UpdateLanguage as the settings menu cannot be displayed in this scene
    /// </summary>


    private bool _foundTitle = false;
    private bool _foundAcknowledgements = false;

    // Start is called before the first frame update
    /// <summary>
    /// Initialisation Method
    /// </summary>
    void Start()
    {
        var _childTexts = gameObject.GetComponentsInChildren<TMP_Text>();

        var _language = PersistenceController.GetLanguage();

        if (_language == Language.English)
        {
            // Loop through all elements and update language, once all found break loop
            foreach (TMP_Text childText in _childTexts)
            {
                if (childText.name == "Title")
                {
                    childText.text = "Acknowledgements";
                    _foundTitle = true;
                }
                else if (childText.name == "AcknowledgementsText")
                {
                    childText.text = "Developer: Nathan Horder\n\nArtist: Nathan Horder\n\nTranslator: Helen Miles\n\nPlay Testers:\n- Leo Lange\n- John Callaghan\n- Mark Williamson\n- Michael McRae\n- Jamie Stammers\n";
                    
                    _foundAcknowledgements = true;
                }

                if (_foundAcknowledgements && _foundTitle)
                {
                    break;
                }
            }

        }
        else if (_language == Language.Welsh)
        {
            Debug.Log("????");

            // Loop through all elements and update language, once all found break loop
            foreach (TMP_Text childText in _childTexts)
            {
                if (childText.name == "Title")
                {
                    childText.text = "Cydnabyddiaethau";
                    _foundTitle = true;
                }
                else if (childText.name == "AcknowledgementsText")
                {
                    childText.text = "Datblygwr: Nathan Horder\n\nArlunydd: Nathan Horder\n\nCyfieithydd: Helen Miles\n\nProfwyr Chwarae:\n- Leo Lange\n- John Callaghan\n- Mark Williamson\n-Michael McRae\n- Jamie Stammers\n";
                    _foundAcknowledgements = true;
                }

                if (_foundAcknowledgements && _foundTitle)
                {
                    break;
                }
            }
        }

    }


}
