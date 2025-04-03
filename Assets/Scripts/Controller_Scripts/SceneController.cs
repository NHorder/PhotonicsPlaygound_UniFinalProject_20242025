using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum Level{
    TestingLevel,
    Titlescreen,
    LevelSelection,
    LevelOne_Reflections,
    LevelTwo_Refractions,
    LevelThree_Colour,
    LevelFour_ColourSplitting,
    LevelFive_ColourCombinations,
    LevelSix_PromotionPrerequsite,
    LevelSeven_PromotionExam,
    LevelEight_Challange,
    LevelNine_GravitationalAnomalies,
    LevelTen_GravitationalCollapse
}


public class SceneController: MonoBehaviour
{   
    public static void ToLevel(Level level)
    {
        /// Reason for this function, is for resetting the level - which only the game controller knows what the level is
        /// and for adaptability, so the game controller can reset scenes regardless of where it was created (as in developed)

        if (level == Level.TestingLevel) ToTestLevel();
        else if (level == Level.LevelOne_Reflections) ToLevelOneReflections();
        else if (level == Level.LevelTwo_Refractions) ToLevelTwoRefractions();
        else if (level == Level.LevelThree_Colour) ToLevelThreeColour();
        else if (level == Level.LevelFour_ColourSplitting) ToLevelFourColourSplitting();
        else if (level == Level.LevelFive_ColourCombinations) ToLevelFiveColourCombinations();
        else if (level == Level.LevelSix_PromotionPrerequsite)ToLevelSixPromotionPrerequisite();
        else if (level == Level.LevelSeven_PromotionExam) ToLevelSevenPromotionExam();
        else if (level == Level.LevelEight_Challange) ToLevelEightAHarderChallenge();
        else if (level == Level.LevelNine_GravitationalAnomalies) ToLevelNineGravitationalAnomalies();
        else if (level == Level.LevelTen_GravitationalCollapse) ToLevelTenGravitationalCollapse();

        else Debug.LogError("ERROR: Level not recognised");
    }

    public static void ToTestLevel() 
    {
        SceneManager.LoadScene("TestLevel");
    }

    public static void ToTitleScreen() 
    {
        SceneManager.LoadScene("Titlescreen");
    }

    public static void ToLevelSelection() 
    {
        SceneManager.LoadScene("LevelSelection");
    }

    public static void ToAcknowledgementsScreen()
    {
        SceneManager.LoadScene("Acknowledgements");
    }


    // Introductory Levels
    public static void ToLevelOneReflections() 
    {
        SceneManager.LoadScene("Level1_Reflections");
    }
    public static void ToLevelTwoRefractions() 
    {
        SceneManager.LoadScene("Level2_Refraction");
    }
    public static void ToLevelThreeColour()
    {
        SceneManager.LoadScene("Level3_Colour");
    }
    public static void ToLevelFourColourSplitting()
    {
        SceneManager.LoadScene("Level4_ColourSplitting");
    }
    public static void ToLevelFiveColourCombinations() 
    {
        SceneManager.LoadScene("Level5_ColourCombinations");
    }


    // Regular Levels - Gets the user familer with the interactions in various ways
    public static void ToLevelSixPromotionPrerequisite() 
    {
        SceneManager.LoadScene("Level6_PromotionPrerequisite");
    }
    public static void ToLevelSevenPromotionExam()
    {
        SceneManager.LoadScene("Level7_PromotionExam");
    }
    public static void ToLevelEightAHarderChallenge() 
    {
        SceneManager.LoadScene("Level8_AHarderChallenge");
    }


    // Higher Level Concept levels - breifly touches upon blackhole interactions
    public static void ToLevelNineGravitationalAnomalies() 
    {
        SceneManager.LoadScene("Level9_GravitationalAnomalies");
    }
    public static void ToLevelTenGravitationalCollapse()
    {
        SceneManager.LoadScene("Level10_GravitationalCollapse");
    }
    
}
