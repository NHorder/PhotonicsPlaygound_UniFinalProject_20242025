using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LevelSelectManage: MonoBehaviour
{
    /// <summary>
    /// Class to handle level selection management
    /// This exists for the level unlock system
    /// </summary>
    public List<Level> unlockedLevels;

    private TMP_InputField _secretCodeInputField;

    /// <summary>
    /// Initialisation Method
    /// </summary>
    void Start()
    {
        PersistenceController.UnlockLevel(Level.LevelOne_Reflections);
        unlockedLevels = PersistenceController.GetUnlockedLevels();

        // Find all level buttons and secret code area, save them for later use
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

    /// <summary>
    /// Each button links to one level, which then checks if it's unlocked before attempting to send the user to said level
    /// </summary>
    public void ToLevelOne()
    {
        if (unlockedLevels.Contains(Level.LevelOne_Reflections)) SceneController.ToLevelOneReflections();
    }

    public void ToLevelTwo()
    {
        if (unlockedLevels.Contains(Level.LevelTwo_Refractions)) SceneController.ToLevelTwoRefractions();
    }

    public void ToLevelThree()
    {
        if (unlockedLevels.Contains(Level.LevelThree_Colour)) SceneController.ToLevelThreeColour();
    }

    public void ToLevelFour()
    {
        if (unlockedLevels.Contains(Level.LevelFour_ColourSplitting)) SceneController.ToLevelFourColourSplitting();
    }

    public void ToLevelFive()
    {
        if (unlockedLevels.Contains(Level.LevelFive_ColourCombinations)) SceneController.ToLevelFiveColourCombinations();
    }

    public void ToLevelSix()
    {
        if (unlockedLevels.Contains(Level.LevelSix_PromotionPrerequsite)) SceneController.ToLevelSixPromotionPrerequisite();
    }

    public void ToLevelSeven()
    {
        if (unlockedLevels.Contains(Level.LevelSeven_PromotionExam)) SceneController.ToLevelSevenPromotionExam();
    }

    public void ToLevelEight()
    {
        if (unlockedLevels.Contains(Level.LevelEight_Challange)) SceneController.ToLevelEightAHarderChallenge();
    }

    public void ToLevelNine()
    {
        if (unlockedLevels.Contains(Level.LevelNine_GravitationalAnomalies)) SceneController.ToLevelNineGravitationalAnomalies();
    }

    public void ToLevelTen()
    {
        if (unlockedLevels.Contains(Level.LevelTen_GravitationalCollapse)) SceneController.ToLevelTenGravitationalCollapse();
    }


    /// <summary>
    /// Secret codes have been included within this game
    /// Reason for this is to allow developers (and testers) to quickly unlock later levels without needing
    /// to replay the whole game in order to reach a specific level. 
    /// 
    /// The codes are:
    ///    - Photonics  (Unlocks all levels)
    ///    - Quasars (Unlocks level 9 and 10)
    ///    - Project Radiance (Directs user to testing level)
    /// 
    /// Note: Codes are case irrelevant, they are droped to lowercase then checked
    /// </summary>
    public void SecretCode()
    {
        if (_secretCodeInputField != null)
        {
            bool secretUnlocked = false;

            // Secret codes (photonics) or (elysian photonics) unlocks all levels
            if (_secretCodeInputField.text.ToLower() == "photonics")
            {
                Debug.Log("Everything unlocks...");
                secretUnlocked = true;

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
            }


            // Secret codes (quasars) or (quasar anomalies) unlock the last two levels
            else if (_secretCodeInputField.text.ToLower() == "quasars")
            {
                Debug.Log("Something unlocks related to quasars..");
                secretUnlocked = true;

                PersistenceController.UnlockLevel(Level.LevelNine_GravitationalAnomalies);
                PersistenceController.UnlockLevel(Level.LevelTen_GravitationalCollapse);
            }

            // Secret code (project radiance) directs the user to the test level
            else if (_secretCodeInputField.text.ToLower() == "project radiance") SceneController.ToTestLevel();

            // If any secret code is unlocked, then play unlock animations as needed
            if (secretUnlocked)
            {
                var childLevelList = gameObject.GetComponentsInChildren<LevelSelectButton>();
                var counter = 0;
                foreach (LevelSelectButton childLevel in childLevelList)
                {
                    if (secretUnlocked) childLevel.PlayUnlockAnimation();

                    if (secretUnlocked && (childLevel.level == Level.LevelNine_GravitationalAnomalies || childLevel.level == Level.LevelTen_GravitationalCollapse))
                    {
                        childLevel.PlayUnlockAnimation();
                    }
                }
            }
        }
    }
}