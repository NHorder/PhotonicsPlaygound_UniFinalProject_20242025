using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TeachingController : MonoBehaviour
{
    private UIController _uiController;


    public TeachingElement[] _teachingElementList;


    private int _currentElementNum = 0;
    private int _currentSpriteNum;


    private TMP_Text _title;
    private TMP_Text _description;
    private Image _image;


    private GameObject _previousTeachingElementButton;

    private GameObject _nextSpriteButton;
    private GameObject _previousSpriteButton;

    // Start is called before the first frame update
    void Start()
    {

        // If there are no teaching elements, disable this and give a warning
        if (_teachingElementList.Length == 0)
        {
            Debug.LogWarning("Warning: Teaching in use, but no elements to be taught!");
            if (_teachingElementList.Length == 0) this.gameObject.active = false;
        }
        else

        // Otherwise collect the neccarary components and begin!
        {

            var childTransformList = gameObject.GetComponentsInChildren<RectTransform>();

            // Loop through all children and filter for wanted objects
            foreach (RectTransform childTransform in childTransformList)
            {
                GameObject childObject = childTransform.gameObject;

                if (childObject.name == "TeachingName") _title = childObject.GetComponent<TMP_Text>();
                else if (childObject.name == "TeachingDescription") _description = childObject.GetComponent<TMP_Text>();
                else if (childObject.name == "TeachingImage") _image = childObject.GetComponent<Image>();
                else if (childObject.name =="TeachingNextSprite") _nextSpriteButton = childObject;
                else if (childObject.name =="TeachingPreviousSprite") _previousSpriteButton = childObject;
                else if (childObject.name =="TeachingPrevious") _previousTeachingElementButton = childObject;
            }

            // Displayt the teaching elements
            DisplayTeachingElement();
        }
    }
    
    public void SetUIController(UIController uiController)
    {
        this._uiController = uiController;
    }

    private void DisplayTeachingElement()
    {
        // If the current teaching element is 0, then disable the previous teaching element button
        if (_currentElementNum == 0)
        {
            _previousTeachingElementButton.active = false;
        }
        else
        {
            _previousTeachingElementButton.active = true;
        }

        // Check that the current teaching element number is less than the length
        if (_currentElementNum < _teachingElementList.Length)
        {
            // If so display elements

            // Display Elements
            _title.text = _teachingElementList[_currentElementNum].title;
            _description.text = _teachingElementList[_currentElementNum].description;

            // Set sprite to 0, then update the sprite
            // Dev Note: Teaching elements can contain 0 to many sprites
            _currentSpriteNum = 0;
            UpdateSprite();

            // If there are more than one sprite, show previous and next sprite buttons.
            if (_teachingElementList[_currentElementNum].teachingSprites.Length >= 1)
            {
                _nextSpriteButton.active = true;
                _previousSpriteButton.active = true;
            }
            else 
            {
                _nextSpriteButton.active = false;
                _previousSpriteButton.active = false;
            }
        
        }
        // Else assume the end of teaching elements
        else
        {
            HideTeaching();
        }

    }

    private void UpdateSprite()
    {
        // Updates the sprite if possible using current element
        // Dev Note: Specifically does not involve =, has to be less. As length can be 0 same with element which would throw errors.
        if (_currentElementNum < _teachingElementList.Length)
        {
            // Check current number is valid, and that the sprite exists, update the sprite
            if (_currentSpriteNum >= 0 && _currentSpriteNum < _teachingElementList[_currentElementNum].teachingSprites.Length)
            {
                _image.sprite = _teachingElementList[_currentElementNum].teachingSprites[_currentSpriteNum];
            }


            // The below two "else if" statements are used to create an array loop


            // Check if the number is more than the number of elements
            else if ( _currentSpriteNum >= _teachingElementList[_currentElementNum].teachingSprites.Length){
                // Set current sprite num to 0
                _currentSpriteNum = 0;
                _image.sprite = _teachingElementList[_currentElementNum].teachingSprites[_currentSpriteNum];
            }

            // Check number is less than 0
            else if (_currentSpriteNum < 0 && _teachingElementList[_currentElementNum].teachingSprites.Length > 0)
            {
                // Set current sprite to size -1, getting the last sprite
                _currentSpriteNum = _teachingElementList[_currentElementNum].teachingSprites.Length - 1;
                _image.sprite = _teachingElementList[_currentElementNum].teachingSprites[_currentSpriteNum];
            }

        }
    }

    public void HideTeaching()
    {
        // Hide teaching element, check to make sure ui controller is not null
        if (_uiController != null) _uiController.PresentPanel(UIPanel.Teaching,false);
    }

    public void NextTeachingElement()
    {
        // Called by a UI Button, hence no arguments

        // Update the teaching element
        _currentElementNum += 1;
        DisplayTeachingElement();
    }

    public void PreviousTeachingElement()
    {
        // Called by a UI Button, hence no arguments

        // Update the teaching element
        _currentElementNum -=1;
        DisplayTeachingElement();
    }

    public void NextSprite()
    {
        // Called by a UI Button, hence no arguments

        // Update the teaching sprite
        _currentSpriteNum += 1;
        UpdateSprite();
    }

    public void PreviousSprite()
    {
        // Called by a UI Button, hence no arguments

        // Update the teaching sprite
        _currentSpriteNum -=1;
        UpdateSprite();
    }

}



[System.Serializable]
public class TeachingElement
{
    public string title;
    public string description;
    public Sprite[] teachingSprites;

}