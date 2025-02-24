using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TeachingController : MonoBehaviour
{
    private UI_Controller ui_Controller;


    public Teaching_Element[] teaching_Elements;


    private int currentElementNum = 0;
    private int currentSpriteNum;


    private TMP_Text title;
    private TMP_Text description;
    private Image image;


    private GameObject previousTeachingElementButton;

    private GameObject nextSpriteButton;
    private GameObject previousSpriteButton;

    // Start is called before the first frame update
    void Start()
    {

        // If there are no teaching elements, disable this and give a warning
        if (teaching_Elements.Length == 0)
        {
            Debug.LogWarning("Warning: Teaching in use, but no elements to be taught!");
            if (teaching_Elements.Length == 0) this.gameObject.active = false;
        }
        else

        // Otherwise collect the neccarary components and begin!
        {
            RectTransform[] childTransforms = gameObject.GetComponentsInChildren<RectTransform>();

            foreach (RectTransform childTransform in childTransforms)
            {
                GameObject childObject = childTransform.gameObject;

                if (childObject.name == "TeachingName") title = childObject.GetComponent<TMP_Text>();
                else if (childObject.name == "TeachingDescription") description = childObject.GetComponent<TMP_Text>();
                else if (childObject.name == "TeachingImage") image = childObject.GetComponent<Image>();
                else if (childObject.name =="TeachingNextSprite") nextSpriteButton = childObject;
                else if (childObject.name =="TeachingPreviousSprite") previousSpriteButton = childObject;
                else if (childObject.name =="TeachingPrevious") previousTeachingElementButton = childObject;

            }


            DisplayTeachingElement();
        }
    }
    
    public void SetUIController(UI_Controller ui_Controller)
    {
        this.ui_Controller = ui_Controller;
    }

    private void DisplayTeachingElement()
    {
        if (currentElementNum == 0)
        {
            previousTeachingElementButton.active = false;
        }
        else
        {
            previousTeachingElementButton.active = true;
        }

        if (currentElementNum < teaching_Elements.Length)
        {
            // Display Elements
            title.text = teaching_Elements[currentElementNum].title;
            description.text = teaching_Elements[currentElementNum].description;

            currentSpriteNum = 0;
            UpdateSprite();

            if (teaching_Elements[currentElementNum].teachingSprites.Length > 1)
            {
                nextSpriteButton.active = true;
                previousSpriteButton.active = true;
            }
            else 
            {
                nextSpriteButton.active = false;
                previousSpriteButton.active = false;
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
        if (currentElementNum < teaching_Elements.Length)
        {
            

            if (currentSpriteNum >= 0 && currentSpriteNum < teaching_Elements[currentElementNum].teachingSprites.Length)
            {
                image.sprite = teaching_Elements[currentElementNum].teachingSprites[currentSpriteNum];
            }

            else if ( currentSpriteNum >= teaching_Elements[currentElementNum].teachingSprites.Length){
                currentSpriteNum = 0;
                image.sprite = teaching_Elements[currentElementNum].teachingSprites[currentSpriteNum];
            }

            else if (currentSpriteNum < 0 && teaching_Elements[currentElementNum].teachingSprites.Length > 0)
            {
                currentSpriteNum = teaching_Elements[currentElementNum].teachingSprites.Length - 1;
                image.sprite = teaching_Elements[currentElementNum].teachingSprites[currentSpriteNum];
            }

        }
    }





    public void HideTeaching()
    {
        if (ui_Controller != null) ui_Controller.PresentPanel(UIPanel.Teaching,false);
    }

    public void NextTeachingElement()
    {
        currentElementNum += 1;
        DisplayTeachingElement();
    }

    public void PreviousTeachingElement()
    {
        currentElementNum -=1;
        DisplayTeachingElement();
    }

    public void NextSprite()
    {
        currentSpriteNum += 1;
        UpdateSprite();
    }

    public void PreviousSprite()
    {
        currentSpriteNum -=1;
        UpdateSprite();
    }




}



[System.Serializable]
public class Teaching_Element
{
    public string title;
    public string description;
    public Sprite[] teachingSprites;

}