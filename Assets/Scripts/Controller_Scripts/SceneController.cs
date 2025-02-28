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
        else Debug.LogError("ERROR: Level not recognised");
    }


    public static void ToTestLevel() {SceneManager.LoadScene("TestLevel");}

    public static void ToTitleScreen() {SceneManager.LoadScene("Titlescreen");}

    public static void ToLevelSelection() {SceneManager.LoadScene("LevelSelection");}

    public static void ToLevelOneReflections() {SceneManager.LoadScene("LevelOne_Reflections");}

    public static void ToLevelTwoRefractions() {SceneManager.LoadScene("LevelTwo_Refractions");}
}
