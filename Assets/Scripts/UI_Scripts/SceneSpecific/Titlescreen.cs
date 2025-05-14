using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Titlescreen : MonoBehaviour
{
    /// <summary>
    /// Class to handle titlescreen interactions
    /// </summary>

    private TMP_Text _titleText;
    private TMP_Text _startGameText;
    private TMP_Text _settingsText;
    private TMP_Text _acknowledgementsText;
    private TMP_Text _englishButtonText;
    private Animator _englishButtonAnimator;
    private TMP_Text _welshButtonText;
    private Animator _welshButtonAnimator;

    // Start is called before the first frame update
    /// <summary>
    /// Initialisation Method
    /// </summary>
    void Start()
    {
        var childrenText = gameObject.GetComponentsInChildren<TMP_Text>();
        /// Loop through children and find needed text
        foreach (TMP_Text childText in childrenText)
        {
            if (childText.name == "GameTitle") _titleText = childText;
            else if (childText.name == "StartGameText") _startGameText = childText;
            else if (childText.name == "SettingsText") _settingsText = childText;
            else if (childText.name == "AcknowledgementsText") _acknowledgementsText = childText;
            else if (childText.name == "EnglishTitleText") _englishButtonText = childText;
            else if (childText.name == "WelshTitleText")  _welshButtonText = childText;
        }

        var childrenAnimator = gameObject.GetComponentsInChildren<Animator>();
        
        foreach (Animator childAnimator in childrenAnimator)
        {
            if (childAnimator.name == "EnglishTitleButton") _englishButtonAnimator = childAnimator;
            else if (childAnimator.name == "WelshTitleButton") _welshButtonAnimator = childAnimator;
            else if (_englishButtonAnimator != null && _welshButtonAnimator != null) break;
        }

        UpdateLanguage(PersistenceController.GetLanguage()); 
    }

    /// <summary>
    /// Method to update language
    /// </summary>
    /// <param name="language"></param>
    public void UpdateLanguage(Language language)
    {

        if (language == Language.English)
        {
            _titleText.text = "Photonics Playground";
            _startGameText.text = "Start Game";
            _settingsText.text = "Settings";
            _acknowledgementsText.text = "Acknowledgements";

            _englishButtonText.text = "English";
            _welshButtonText.text = "Welsh";

            _welshButtonAnimator.SetBool("Selected",false);
            _englishButtonAnimator.SetBool("Selected",true);
        }
        else if (language == Language.Welsh)
        {
            _titleText.text = "Maes Chwarae Ffotoneg";
            _startGameText.text = "Dechrau Gêm";
            _settingsText.text = "Gosodiadau";
            _acknowledgementsText.text = "Cydnabyddiaethau";

            _englishButtonText.text = "Saesneg";
            _welshButtonText.text = "Cymraeg";

            _welshButtonAnimator.SetBool("Selected",true);
            _englishButtonAnimator.SetBool("Selected",false);
        }

    }


    public void SetLanguageEnglish()
    {
        UpdateLanguage(Language.English);
        PersistenceController.UpdateLanguage(Language.English);
    }

    /// <summary>
    /// Method to set language to Welsh
    /// Called by UI component
    /// </summary>
    public void SetLanguageWelsh()
    {
        UpdateLanguage(Language.Welsh);
        PersistenceController.UpdateLanguage(Language.Welsh);
    }


}
