using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LevelSelectManage: MonoBehaviour
{

    public List<Level> unlockedLevels;

    private TMP_InputField _secretCodeInputField;

    void Start()
    {
        PersistenceController.UnlockLevel(Level.LevelOne_Reflections);
        unlockedLevels = PersistenceController.GetUnlockedLevels();

        var childLevelList = gameObject.GetComponentsInChildren<LevelSelectButton>();
        var counter = 0;
        foreach (LevelSelectButton childLevel in childLevelList)
        {
            counter += 1;

            if (unlockedLevels.Contains(childLevel.level))
            {
                // If most recently unlocked, play an animation
                if (counter >= unlockedLevels.Count) childLevel.PlayUnlockAnimation();

                // Else just straight unlock the level.
                else childLevel.UnlockLevel();
            }
        }

        _secretCodeInputField = gameObject.GetComponentInChildren<TMP_InputField>();
    }

    public void ToLevelOne()
    {
        if (unlockedLevels.Contains(Level.LevelOne_Reflections)) SceneController.ToLevelOneReflections();
    }

    public void ToLevelTwo()
    {
        if (unlockedLevels.Contains(Level.LevelTwo_Refractions)) SceneController.ToLevelOneReflections();
    }

    public void ToLevelThree()
    {
        if (unlockedLevels.Contains(Level.LevelThree_Colour)) SceneController.ToLevelOneReflections();
    }

    public void ToLevelFour()
    {
        if (unlockedLevels.Contains(Level.LevelFour_ColourSplitting)) SceneController.ToLevelOneReflections();
    }

    public void ToLevelFive()
    {
        if (unlockedLevels.Contains(Level.LevelFive_ColourCombinations)) SceneController.ToLevelOneReflections();
    }

    public void ToLevelSix()
    {
        if (unlockedLevels.Contains(Level.LevelSix_PromotionPrerequsite)) SceneController.ToLevelOneReflections();
    }

    public void ToLevelSeven()
    {
        if (unlockedLevels.Contains(Level.LevelSeven_PromotionExam)) SceneController.ToLevelOneReflections();
    }

    public void ToLevelEight()
    {
        if (unlockedLevels.Contains(Level.LevelEight_Challange)) SceneController.ToLevelOneReflections();
    }

    public void ToLevelNine()
    {
        if (unlockedLevels.Contains(Level.LevelNine_GravitationalAnomalies)) SceneController.ToLevelOneReflections();
    }

    public void ToLevelTen()
    {
        if (unlockedLevels.Contains(Level.LevelTen_GravitationalCollapse)) SceneController.ToLevelOneReflections();
    }


    public void SecretCode()
    {
        if (_secretCodeInputField != null)
        {
            bool unlockAll = false;
            bool unlockQuasars = false;

            if (_secretCodeInputField.text.ToLower() == "photonics" || _secretCodeInputField.text.ToLower() == "elysian photonics")
            {
                Debug.Log("Everything unlocks...");
                unlockAll = true;

                // Unlock all levels for this instance
                PersistenceController.UnlockLevel(Level.LevelTwo_Refractions);
                PersistenceController.UnlockLevel(Level.LevelThree_Colour);
                PersistenceController.UnlockLevel(Level.LevelFour_ColourSplitting);
                PersistenceController.UnlockLevel(Level.LevelFive_ColourCombinations);
                PersistenceController.UnlockLevel(Level.LevelSix_PromotionPrerequsite);
                PersistenceController.UnlockLevel(Level.LevelSeven_PromotionExam);
                PersistenceController.UnlockLevel(Level.LevelEight_Challange);
                PersistenceController.UnlockLevel(Level.LevelNine_GravitationalAnomalies);
                PersistenceController.UnlockLevel(Level.LevelTen_GravitationalCollapse);


                if (_secretCodeInputField.text.ToLower() == "elysian photonics")
                {
                    // Rapidly switch between each level regardless of unlock condition to update the 
                    // PersistenceControllers Teaching elements - so all teaching info is available.
                    SceneController.ToTestLevel();
                    SceneController.ToLevelSelection();
                }
            }

            else if (_secretCodeInputField.text.ToLower() == "quasars" || _secretCodeInputField.text.ToLower() == "quasar anomalies")
            {
                Debug.Log("Something unlocks related to quasars..");
                unlockQuasars = true;

                PersistenceController.UnlockLevel(Level.LevelNine_GravitationalAnomalies);
                PersistenceController.UnlockLevel(Level.LevelTen_GravitationalCollapse);

                if (_secretCodeInputField.text.ToLower() == "quasar anomalies")
                {
                    // Rapidly switch between both levels to update the 
                    // PersistenceControllers Teaching elements - so all teaching info is available.

                    SceneController.ToLevelSelection();
                }
            }

            if (unlockAll || unlockQuasars)
            {
                var childLevelList = gameObject.GetComponentsInChildren<LevelSelectButton>();
                var counter = 0;
                foreach (LevelSelectButton childLevel in childLevelList)
                {
                    if (unlockAll) childLevel.PlayUnlockAnimation();

                    if (unlockQuasars && (childLevel.level == Level.LevelNine_GravitationalAnomalies || childLevel.level == Level.LevelTen_GravitationalCollapse))
                    {
                        childLevel.PlayUnlockAnimation();
                    }
                    
                }
            }
        }
    }
}