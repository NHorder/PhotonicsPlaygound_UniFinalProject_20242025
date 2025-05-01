using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectButton : MonoBehaviour
{
    /// <summary>
    /// Class to handle Level Select buttons in the Level Selection Screen
    /// </summary>

    public Level level;

    private TMP_Text _buttonText;

    /// <summary>
    /// Initialisation Method
    /// </summary>
    void Start()
    {
        _buttonText = gameObject.GetComponentInChildren<TMP_Text>();

        UpdateLanguage();
    }

    /// <summary>
    /// Method to update the language of the button based on language and level
    /// </summary>
    public void UpdateLanguage()
    {
        var _language = PersistenceController.GetLanguage();

        if (_language == Language.English)
        {
            if (level == Level.LevelOne_Reflections) _buttonText.text = "Level 1: Reflections";
            else if (level == Level.LevelTwo_Refractions) _buttonText.text = "Level 2: Refractions";
            else if (level == Level.LevelThree_Colour) _buttonText.text = "Level 3:   Colour";
            else if (level == Level.LevelFour_ColourSplitting) _buttonText.text = "Level 4: Colour Splitting";
            else if (level == Level.LevelFive_ColourCombinations) _buttonText.text = "Level 5: Colour Combinations";
            else if (level == Level.LevelSix_PromotionPrerequsite) _buttonText.text = "Level 6: Promotion Prerequisite";
            else if (level == Level.LevelSeven_PromotionExam) _buttonText.text =  "Level 7: Promotion Exam";
            else if (level == Level.LevelEight_Challange) _buttonText.text = "Level 8: A Harder Challenge";
            else if (level == Level.LevelNine_GravitationalAnomalies) _buttonText.text = "Level 9: Gravitational Anomalies";
            else if (level == Level.LevelTen_GravitationalCollapse) _buttonText.text = "Level 10: Gravitational Collapse";

        }
        else if (_language == Language.Welsh)
        {
            if (level == Level.LevelOne_Reflections) _buttonText.text = "Lefel 1: Adlewyrchiad";
            else if (level == Level.LevelTwo_Refractions) _buttonText.text = "Lefel 2: Plygiannau";
            else if (level == Level.LevelThree_Colour) _buttonText.text = "Lefel 3: Lliw";
            else if (level == Level.LevelFour_ColourSplitting) _buttonText.text = "Lefel 4: Hollti Lliw";
            else if (level == Level.LevelFive_ColourCombinations) _buttonText.text = "Lefel 5: Cyfuniadau Lliwiau";
            else if (level == Level.LevelSix_PromotionPrerequsite) _buttonText.text = "Lefel 6: Rhagofyniad Dyrchafiad";
            else if (level == Level.LevelSeven_PromotionExam) _buttonText.text = "Lefel 7: Arholiad Dyrchafiad";
            else if (level == Level.LevelEight_Challange) _buttonText.text = "Lefel 8: Her Anoddach";
            else if (level == Level.LevelNine_GravitationalAnomalies) _buttonText.text = "Lefel 9: Anghysondebau Disgyrchiant";
            else if (level == Level.LevelTen_GravitationalCollapse) _buttonText.text = "Lefel 10: Cwymp Disgyrchian";
        }

    }

    /// <summary>
    /// Method to progress call animation when a level has been unlocked prior
    /// </summary>
    public void UnlockLevel()
    {
        // Play an unlock animation, cause why not.
        var animator = gameObject.GetComponentInChildren<Animator>();
        if (animator != null) animator.SetBool("AlreadyUnlocked",true);
    }

    /// <summary>
    /// Method to play the unlock animation
    /// </summary>
    public void PlayUnlockAnimation()
    { 
        // Play an unlock animation, cause why not.
        var animator = gameObject.GetComponentInChildren<Animator>();
        if (animator != null) animator.SetBool("Unlocked",true);

    }

}
