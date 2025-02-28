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
