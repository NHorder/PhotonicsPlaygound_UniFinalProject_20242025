using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanel : MonoBehaviour
{
    /// <summary>
    /// Class to handle settings panel
    /// </summary>
    
     
    private Language _language = Language.English;
    private TMP_Text _languageText;
    private TMP_Text _englishButtonText;
    private Animator _englishButtonAnimator;
    private TMP_Text _welshButtonText;
    private Animator _welshButtonAnimator;


    private bool _forceNoChangeWhenReceivingCommunications = true;
    private TMP_Text _forceNoChangeWhenReceivingCommunicationsToggleText;
    private Toggle _forceNoChangeWhenReceivingCommunicationsToggle;

    private bool _disableConfirmations = true;
    private TMP_Text _disableConfirmationsToggleText;
    private Toggle _disableConfirmmationsToggle;

    private bool _allowAdvancedInteractions = true;
    private TMP_Text _allowAdvancedInteractionsToggleText;
    private Toggle _allowAdvancedInteractionsToggle;

    private bool _allowSatelliteMovementParticles = true;
    private TMP_Text _allowSatelliteMovementParticlesToggleText;
    private Toggle _allowSatelliteMovementParticlesToggle;


    private TMP_Text _resetButtonText;
    private TMP_Text _returnButtonText;


    private TMP_Text _titleText;
    private TMP_Text _viewTeachingTransmissionText;
    private GameObject _viewTeachingTransmissionButton;

    private TMP_Text _exitLevelText;
    private GameObject _exitLevelButton;



    private GameController _gameController;
    private UIController _uiController;
    private ConfirmationPanel _confirmationPanel;


    private bool _visible = false;

    private Language _savedLanguage = Language.English;
    private TeachingPanel _teachingPanel;

    // Start is called before the first frame update
    /// <summary>
    /// Initialisation Method
    /// </summary>
    void Start()
    {
        _language = PersistenceController.GetLanguage();
        _savedLanguage = PersistenceController.GetLanguage();
        
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        _uiController = GameObject.FindGameObjectsWithTag("UI_Controller")[0].GetComponent<UIController>();
        _confirmationPanel = GameObject.FindGameObjectsWithTag("ConfirmationPanel")[0].GetComponent<ConfirmationPanel>();


        // Locate all needed information from child objects
        var childRectTransformList = gameObject.GetComponentsInChildren<RectTransform>();

        // Loop through all children and filter for wanted objects
        foreach (RectTransform childTransform in childRectTransformList)
        {
            GameObject childObject = childTransform.gameObject;

            if (childObject.name == "Title") _titleText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "LanguageText") _languageText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "ResetText") _resetButtonText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "ReturnText") _returnButtonText = childObject.GetComponent<TMP_Text>();

            else if (childObject.name == "ForceNoChangeOnCommunicationToggle") _forceNoChangeWhenReceivingCommunicationsToggle = childObject.GetComponent<Toggle>();
            else if (childObject.name == "ForceNoChangeText") _forceNoChangeWhenReceivingCommunicationsToggleText = childObject.GetComponent<TMP_Text>();

            else if (childObject.name == "AllowAdvancedInteractionToggle") _allowAdvancedInteractionsToggle = childObject.GetComponent<Toggle>();
            else if (childObject.name == "AllowAdvancedInteractionText") _allowAdvancedInteractionsToggleText = childObject.GetComponent<TMP_Text>();

            else if (childObject.name == "AllowSatelliteMovementParticles") _allowSatelliteMovementParticlesToggle = childObject.GetComponent<Toggle>();
            else if (childObject.name == "AllowSatelliteMovementParticlesText") _allowSatelliteMovementParticlesToggleText = childObject.GetComponent<TMP_Text>();

            else if (childObject.name == "DisableConfirmations") _disableConfirmmationsToggle = childObject.GetComponent<Toggle>();
            else if (childObject.name == "DisableConfirmationText") _disableConfirmationsToggleText = childObject.GetComponent<TMP_Text>();

            else if (childObject.name == "ViewTeachingText") _viewTeachingTransmissionText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "ViewTeaching") _viewTeachingTransmissionButton = childObject;

            else if (childObject.name == "EnglishText") _englishButtonText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "WelshText") _welshButtonText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "ExitLevelText") _exitLevelText = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "ExitLevelButton") _exitLevelButton = childObject;

            else if (childObject.name == "EnglishButton") _englishButtonAnimator = childObject.GetComponent<Animator>();
            else if (childObject.name == "WelshButton") _welshButtonAnimator = childObject.GetComponent<Animator>();
        }

        if (!_uiController.uiExpectations.expectTeachingPanel)_viewTeachingTransmissionButton.active = false;
        else _teachingPanel = GameObject.FindGameObjectsWithTag("TeachingPanel")[0].GetComponent<TeachingPanel>();


        // Call read settings to update to current information
        ReadSettings();
    }

    /// <summary>
    /// Method to read all settings from the Persistence Controller
    /// </summary>
    private void ReadSettings()
    {
        _language = PersistenceController.GetLanguage();
        _savedLanguage = PersistenceController.GetLanguage();
        ChangeLanguage();

        _forceNoChangeWhenReceivingCommunications = PersistenceController.GetForceNoChangeWhenReceivingCommunications();
        _forceNoChangeWhenReceivingCommunicationsToggle.isOn = _forceNoChangeWhenReceivingCommunications;

        _disableConfirmations = PersistenceController.GetDisableConfirmations();
        _disableConfirmmationsToggle.isOn = _disableConfirmations;

        _allowAdvancedInteractions = PersistenceController.GetAllowAdvancedInteractions();
        _allowAdvancedInteractionsToggle.isOn = _allowAdvancedInteractions;

        _allowSatelliteMovementParticles = PersistenceController.GetAllowSatelliteMovementParticles();
        _allowSatelliteMovementParticlesToggle.isOn = _allowSatelliteMovementParticles;
    }

    /// <summary>
    /// Method to update settings in Persistence Controller
    /// </summary>
    private void UpdateSettings()
    {
        PersistenceController.UpdateSettings(_language,_forceNoChangeWhenReceivingCommunications,_disableConfirmations,_allowAdvancedInteractions,_allowSatelliteMovementParticles);
    }

    
    /// <summary>
    /// Method to change the language of the components within the settings menu
    /// </summary>
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
            _disableConfirmationsToggleText.text = "Disabled Confirmations";
            _forceNoChangeWhenReceivingCommunicationsToggleText.text = "Force No Change When Receiving Communications";

            _viewTeachingTransmissionText.text = "View Teaching Transmissions";
            if (_resetButtonText != null) _resetButtonText.text = "Reset";
            _returnButtonText.text = "Return";
            if (_exitLevelButton != null) _exitLevelText.text = "Exit Level";

            _welshButtonAnimator.SetBool("Selected",false);
            _englishButtonAnimator.SetBool("Selected",true);

        }
        else if (_language == Language.Welsh)
        {
            _titleText.text = "Gosodiadau";
            _languageText.text = "Iaith";
            _englishButtonText.text = "Saesneg";
            _welshButtonText.text = "Cymraeg";
            _allowAdvancedInteractionsToggleText.text = "Caniatáu Rhyngweithiadau Uwch";
            _allowSatelliteMovementParticlesToggleText.text = "Caniatáu Gronynnau Symudiad Lloeren";
            _viewTeachingTransmissionText.text = "Gweld Darllediadau Addysgu";
            _disableConfirmationsToggleText.text = "Analluogi Cadarnhad";
            _forceNoChangeWhenReceivingCommunicationsToggleText.text = "Gorfodi dim newid wrth dderbyn cyfathrebiadau";

            if (_resetButtonText != null) _resetButtonText.text = "Ailosod";
            _returnButtonText.text = "Dychwelyd";
            if (_exitLevelButton != null) _exitLevelText.text = "Gadael Lefel";

            _welshButtonAnimator.SetBool("Selected",true);
            _englishButtonAnimator.SetBool("Selected",false);
        }

    }
    
    /// <summary>
    /// Method to set language to English
    /// Called by UI component
    /// </summary>
    public void SetLanguageEnglish()
    {
        _language = Language.English;
        ChangeLanguage();
    }

    /// <summary>
    /// Method to set language to Welsh
    /// Called by UI component
    /// </summary>
    public void SetLanguageWelsh()
    {
        _language = Language.Welsh;
        ChangeLanguage();
    }

    /// <summary>
    /// Method to toggle communication overwrite setting
    /// Called by UI component
    /// </summary>
    public void ToggleCommunicationOverwrite()
    {
        _forceNoChangeWhenReceivingCommunications = !_forceNoChangeWhenReceivingCommunications;
    }

    /// <summary>
    /// Method to toggle disable confirmations setting
    /// Called by UI component
    /// </summary>
    public void ToggleDisableConfirmations()
    {
        _disableConfirmations = !_disableConfirmations;
    }

    /// <summary>
    /// Method to toggle Advanced Interaction setting
    /// Called by UI component
    /// </summary>
    public void ToggleAdvancedInteractions()
    {
        _allowAdvancedInteractions = !_allowAdvancedInteractions;
    }

    /// <summary>
    /// Method to toggle satellite particle setting
    /// Called by UI Component
    /// </summary>
    public void ToggleSatelliteParticles()
    {
        _allowSatelliteMovementParticles = !_allowSatelliteMovementParticles;
    }

    /// <summary>
    /// Method to present the settings panel
    /// Called by UI Component
    /// </summary>
    public void ShowSettingsPanel()
    {
        _uiController.PresentFixedPanel(FixedUIPanel.Settings,true);
    }

    /// <summary>
    /// Method to reset the settings panel information
    /// Called by UI Component
    /// </summary>
    public void Reset()
    {
        ReadSettings();
    }

    /// <summary>
    /// Method to close and save changed setttings
    /// Called by UI Component
    /// </summary>
    public void Return()
    {
        UpdateSettings();

        _gameController.UpdateSettings();
        _uiController.UpdateSettings(_language != _savedLanguage);

        _uiController.PresentFixedPanel(FixedUIPanel.Settings,false);
    }

    /// <summary>
    /// Method to leave the level
    /// Called by UI Component
    /// </summary>
    public void LeaveLevel()
    {
        Return();
        _confirmationPanel.UpdateUIComponents(ConfirmAction.LeaveLevel);
        _uiController.PresentFixedPanel(FixedUIPanel.ConfirmAction,true);
    }

    /// <summary>
    /// Method to show the teaching transmissions - saves the settings in the process
    /// Called by UI Component
    /// </summary>
    public void ViewTeachingTransmissions()
    {
        if (_teachingPanel != null)
        {
            _teachingPanel.DisplayTeachingPanelFromSettings();
            Return();

            _uiController.PresentFixedPanel(FixedUIPanel.Teaching,true);
        }
        

    }

}
