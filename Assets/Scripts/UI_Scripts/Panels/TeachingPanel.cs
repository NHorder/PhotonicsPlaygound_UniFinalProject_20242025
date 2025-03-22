using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeachingPanel : MonoBehaviour
{
    public List<TeachingElement> teachingElements;
    public TeachingSettings teachingSettings;

    private Language _language;
    private UIController _uiController;

    private int _currentTeachingElement = 0;

    private bool _previousTeachingElementButtonVisible = false;



    private TMP_Text _learningTitle;
    private TMP_Text _learningInfo;

    private GameObject _spriteBoxA;
    private Image _imageA;
    private TMP_Text _textA;

    private GameObject _spriteBoxB;
    private Image _imageB;
    private TMP_Text _textB;


    private GameObject _nextLearningButton;
    private GameObject _previousLearningButton;


    // Start is called before the first frame update
    void Start()
    {
        _language = PersistenceController.GetLanguage();
        _uiController = GameObject.FindGameObjectsWithTag("UI_Controller")[0].GetComponent<UIController>();

        if (teachingElements.Count > 0) PersistenceController.AddTeachingElements(teachingElements);

        var childTransformList = gameObject.GetComponentsInChildren<RectTransform>();
        foreach (RectTransform childTransform in childTransformList)
        {
            var childObject = childTransform.gameObject;

            if (childObject.name == "MainTitle") _learningTitle = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "MainText")  _learningInfo = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "SpriteBoxA") _spriteBoxA = childObject;
            else if (childObject.name == "ImageA") _imageA= childObject.GetComponent<Image>();
            else if (childObject.name == "TextA") _textA = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "SpriteBoxB") _spriteBoxB = childObject;
            else if (childObject.name == "ImageB") _imageB= childObject.GetComponent<Image>();
            else if (childObject.name == "TextB") _textB = childObject.GetComponent<TMP_Text>();
            else if (childObject.name == "NextElement") _nextLearningButton = childObject;
            else if (childObject.name == "PreviousElement") _previousLearningButton = childObject;
        }

        DisplayTeachingElement();
    }

    public void UpdateLanguage(Language newLanguage)
    {
        _language = newLanguage;
    }

    private void DisplayTeachingElement()
    {
        if (teachingElements.Count <= 0)
        {
            _currentTeachingElement = 0;
            _uiController.PresentFixedPanel(FixedUIPanel.Teaching,false);
        }

        // Check that current teaching element does not exist (exceed list size)
        // If so close panel.
        if (_currentTeachingElement >= teachingElements.Count)
        {
            _currentTeachingElement = 0;

            // Hide Panel if it's not already hidden
            TogglePreviousTeachingElementAppearence(false);
            _previousTeachingElementButtonVisible = false;

            // Close panel
            _uiController.PresentFixedPanel(FixedUIPanel.Teaching,false);
        }

        // if the teaching element is less than or equal to 0, then remove the ability to go further back. 
        if (_currentTeachingElement <= 0)
        {
            // Hide Panel if it's not already hidden
            TogglePreviousTeachingElementAppearence(false);

            // Just in case, force the teaching element to 0.
            _currentTeachingElement = 0;
        }


        if (_currentTeachingElement > 0)
        {
            TogglePreviousTeachingElementAppearence(true);
            _previousTeachingElementButtonVisible = true;
        }



        if (_currentTeachingElement >= 0 && _currentTeachingElement < teachingElements.Count)
        {
            TeachingElement teachingElement = teachingElements[_currentTeachingElement];

 
            
            if (teachingElement.usingSpriteA)
            {
                _spriteBoxA.active = true;

                // Updating the sprite here, as it's not supposed to be null if it's being used.
                // AND the sprite part is the only thing capable of being null. The related text has a default.
                _imageA.sprite = teachingElement.spriteA;

                var transform = _nextLearningButton.GetComponent<RectTransform>();
                transform.anchoredPosition = teachingSettings.nextElementLocation;
            }
            else
            {
                // Hide spriteA
                _spriteBoxA.active = false;

                // Move Next Item button
                // Move Previous Item Button
                var transform = _nextLearningButton.GetComponent<RectTransform>();
                transform.anchoredPosition = teachingSettings.nextElementLocationSpriteANotUsed;
            }


            if (teachingElement.usingSpriteB)
            {
                _spriteBoxB.active = true;

                // Updating the sprite here, as it's not supposed to be null if it's being used.
                // AND the sprite part is the only thing capable of being null. The related text has a default.
                _imageB.sprite = teachingElement.spriteB;

                var transform = _previousLearningButton.GetComponent<RectTransform>();
                transform.anchoredPosition = teachingSettings.previousElementLocation;
            }
            else
            {
                // Hide Sprite B
                _spriteBoxB.active = false;

                // Move Previous Item Button
                var transform = _previousLearningButton.GetComponent<RectTransform>();
                transform.anchoredPosition = teachingSettings.previousElementLocationSpriteBNotUsed;
            } 
            
            if (_language == Language.English)
            {
                _learningTitle.text = teachingElement.teachingTitle;
                _learningInfo.text = teachingElement.teachingInfo;
                _textA.text = teachingElement.spriteNameA;
                _textB.text = teachingElement.spirteNameB;
            }
            else if (_language == Language.Welsh)
            {
                _learningTitle.text = teachingElement.teachingTitleWelsh;
                _learningInfo.text = teachingElement.teachingInfoWelsh;
                _textA.text = teachingElement.spriteNameAWelsh;
                _textB.text = teachingElement.spriteNameBWelsh;
            }

        }

    }



    private void TogglePreviousTeachingElementAppearence(bool visible)
    {
        if (visible)
        {
            _previousTeachingElementButtonVisible = true;
            _previousLearningButton.active = true;
        }
        else
        {
            _previousTeachingElementButtonVisible = false;
            _previousLearningButton.active = false;
        }

    }


    public void NextTeachingElement()
    {
        _currentTeachingElement += 1;
        _previousTeachingElementButtonVisible = true;
        DisplayTeachingElement();

    }

    public void PreviousTeachingElement()
    {
        _currentTeachingElement -=1;
        DisplayTeachingElement();

    }

    public void CloseTeachingPanel()
    {
        _currentTeachingElement = 0;
        _uiController.PresentFixedPanel(FixedUIPanel.Teaching,false);
    }


    public void DisplayTeachingPanelFromSettings()
    {
        DisplayTeachingElement();
    }

}

[System.Serializable]
public class TeachingElement
{
    public string teachingTitle = " ";
    public string teachingTitleWelsh = "Not Translated";

    public string teachingInfo = " ";
    public string teachingInfoWelsh = " ";

    public bool usingSpriteA = false;
    public Sprite spriteA;
    public string spriteNameA = " ";
    public string spriteNameAWelsh = "Not Translated";

    public bool usingSpriteB = false;
    public Sprite spriteB;
    public string spirteNameB = " ";
    public string spriteNameBWelsh = "Not Translated";
}

[System.Serializable]
public class TeachingSettings
{
    public Vector2 nextElementLocation = new Vector2();
    public Vector2 nextElementLocationSpriteANotUsed = new Vector2();

    public Vector2 previousElementLocation = new Vector2();
    public Vector2 previousElementLocationSpriteBNotUsed = new Vector2();

}