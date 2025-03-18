using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanel : MonoBehaviour
{
    private Language _language = Language.English;
    private TMP_Text _languageText;
    private TMP_Text _englishButtonText;
    private Animator _englishButtonAnimator;
    private TMP_Text _welshButtonText;
    private Animator _welshButtonAnimator;


    private bool _forceNoChangeWhenReceivingCommunications = true;
    private TMP_Text _forceNoChangeWhenReceivingCommunicationsToggleText;

    private bool _disableConfirmations = true;
    private TMP_Text _disableConfirmationsToggleText;

    private bool _allowAdvancedInteractions = true;
    private TMP_Text _allowAdvancedInteractionsToggleText;

    private bool _allowSatelliteMovementParticles = true;
    private TMP_Text _allowSatelliteMovementParticlesToggleText;


    private TMP_Text _resetButtonText;
    private TMP_Text _returnButtonText;
    private TMP_Text _titleText;
    private TMP_Text _viewTeachingTransmissionText;
    private GameObject _viewTeachingTransmissionButton;

    private TMP_Text _exitLevelText;
    private GameObject _exitLevelButton;

    private GameController _gameController;
    private UIController _uiController;


    private bool _visible = false;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        _uiController = GameObject.FindGameObjectsWithTag("UI_Controller")[0].GetComponent<UIController>();

        var childRectTransformList = gameObject.GetComponentsInChildren<RectTransform>();

        // Loop through all children and filter for wanted objects
        foreach (RectTransform childTransform in childRectTransformList)
        {
            GameObject childObject = childTransform.gameObject;

            if (childObject.name == "Title") _titleText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "LanguageText") _languageText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "ResetText") _resetButtonText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "ReturnText") _returnButtonText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "ForceNoChangeText") _forceNoChangeWhenReceivingCommunicationsToggleText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "DisableConfirmationText") _disableConfirmationsToggleText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "AllowAdvancedInteractionText") _allowAdvancedInteractionsToggleText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "AllowSatelliteMovementParticlesText") _allowSatelliteMovementParticlesToggleText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "ViewTeachingText") _viewTeachingTransmissionText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "ViewTeaching") _viewTeachingTransmissionButton = childObject;

            else if (childObject.name == "EnglishText") _englishButtonText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "WelshText") _welshButtonText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "ExitLevelText") _exitLevelText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "ExitLevelButton") _exitLevelButton = childObject;

            else if (childObject.name == "EnglishButton") _englishButtonAnimator = childObject.GetComponent<Animator>();
            else if (childObject.name == "WelshButton") _welshButtonAnimator = childObject.GetComponent<Animator>();
        }


        //if (!_uiController.uiExpectations.expectConfirmationPanel) _exitLevelButton.active = false;
        if (!_uiController.uiExpectations.expectTeachingPanel) _viewTeachingTransmissionButton.active = false;
        

        ReadFile("settings.txt");
    }


    private void ChangeLanguage()
    {
        if (_language == Language.English)
        {
            _titleText.text = "Settings";
            _languageText.text = "Language";
            _englishButtonText.text = "English";
            _welshButtonText.text = "Welsh";
            _allowAdvancedInteractionsToggleText.text = "Allow Advanced Interactions";
            _allowSatelliteMovementParticlesToggleText.text = "Allow Satellite Movement Particles";
            _viewTeachingTransmissionText.text = "View Teaching Transmissions";
            _resetButtonText.text = "Reset";
            _returnButtonText.text = "Return";
            _exitLevelText.text = "Exit Level";

            _welshButtonAnimator.SetBool("Selected",false);
            _englishButtonAnimator.SetBool("Selected",true);

        }
        else if (_language == Language.Welsh)
        {
            _titleText.text = "(Not Translated)";
            _languageText.text = "Language";
            _englishButtonText.text = "English";
            _welshButtonText.text = "Welsh";
            _allowAdvancedInteractionsToggleText.text = "Allow Advanced Interactions";
            _allowSatelliteMovementParticlesToggleText.text = "Allow Satellite Movement Particles";
            _viewTeachingTransmissionText.text = "View Teaching Transmissions";
            _resetButtonText.text = "Reset";
            _returnButtonText.text = "Return";
            _exitLevelText.text = "Exit Level";

            _welshButtonAnimator.SetBool("Selected",true);
            _englishButtonAnimator.SetBool("Selected",false);
        }

    }
    
    private void ReadFile(string filename)
    {
        _language = Language.English;
        ChangeLanguage();
    }

    private void UpdateFile(string filename)
    {
        
    }

    public void SetLanguageEnglish()
    {
        _language = Language.English;
        ChangeLanguage();
    }

    public void SetLanguageWelsh()
    {
        _language = Language.Welsh;
        ChangeLanguage();
    }


    public void Reset()
    {
        ReadFile("settings.txt");
    }

    public void Return()
    {
        UpdateFile("settings.txt");

        _gameController.UpdateLanguage(_language);
        _uiController.UpdateLanguage(_language);

        _uiController.PresentFixedPanel(FixedUIPanel.Settings,false);
    }

    public void ViewTeachingTransmissions()
    {
        Return();


    }
}
