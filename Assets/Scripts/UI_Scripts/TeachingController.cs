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


            DisplayTeachingElement();
        }
    }
    
    public void SetUIController(UIController uiController)
    {
        this._uiController = uiController;
    }

    private void DisplayTeachingElement()
    {
        if (_currentElementNum == 0)
        {
            _previousTeachingElementButton.active = false;
        }
        else
        {
            _previousTeachingElementButton.active = true;
        }

        if (_currentElementNum < _teachingElementList.Length)
        {
            // Display Elements
            _title.text = _teachingElementList[_currentElementNum].title;
            _description.text = _teachingElementList[_currentElementNum].description;

            _currentSpriteNum = 0;
            UpdateSprite();

            if (_teachingElementList[_currentElementNum].teachingSprites.Length > 1)
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
        // Assumes end of teaching elements
        else
        {
            HideTeaching();
        }

    }

    private void UpdateSprite()
    {
        // Updates the sprite if possible using current element
        if (_currentElementNum < _teachingElementList.Length)
        {
            if (_currentSpriteNum >= 0 && _currentSpriteNum < _teachingElementList[_currentElementNum].teachingSprites.Length)
            {
                _image.sprite = _teachingElementList[_currentElementNum].teachingSprites[_currentSpriteNum];
            }

            else if ( _currentSpriteNum >= _teachingElementList[_currentElementNum].teachingSprites.Length){
                _currentSpriteNum = 0;
                _image.sprite = _teachingElementList[_currentElementNum].teachingSprites[_currentSpriteNum];
            }

            else if (_currentSpriteNum < 0 && _teachingElementList[_currentElementNum].teachingSprites.Length > 0)
            {
                _currentSpriteNum = _teachingElementList[_currentElementNum].teachingSprites.Length - 1;
                _image.sprite = _teachingElementList[_currentElementNum].teachingSprites[_currentSpriteNum];
            }

        }
    }

    public void HideTeaching()
    {
        if (_uiController != null) _uiController.PresentPanel(UIPanel.Teaching,false);
    }

    public void NextTeachingElement()
    {
        _currentElementNum += 1;
        DisplayTeachingElement();
    }

    public void PreviousTeachingElement()
    {
        _currentElementNum -=1;
        DisplayTeachingElement();
    }

    public void NextSprite()
    {
        _currentSpriteNum += 1;
        UpdateSprite();
    }

    public void PreviousSprite()
    {
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