using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Acknowledgements : MonoBehaviour
{

    private bool _foundTitle = false;
    private bool _foundAcknowledgements = false;

    // Start is called before the first frame update
    void Start()
    {
        var _childTexts = gameObject.GetComponentsInChildren<TMP_Text>();

        var _language = PersistenceController.GetLanguage();

        if (_language == Language.English)
        {

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
            foreach (TMP_Text childText in _childTexts)
            {
                if (childText.name == "Title")
                {
                    childText.text = "";
                    _foundTitle = true;
                }
                else if (childText.name == "Acknowledgements")
                {
                    childText.text = ": Nathan Horder\n\n: Nathan Horder\n\n: Helen Miles\n\n:\n- Leo Lange\n- John Callaghan\n- Mark Williamson\n";
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
