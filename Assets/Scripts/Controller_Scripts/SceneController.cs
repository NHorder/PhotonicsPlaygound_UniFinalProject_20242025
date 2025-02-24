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
    public static void To_Level(Level level)
    {
        if (level == Level.TestingLevel) To_TestLevel();
        else if (level == Level.LevelOne_Reflections) To_LevelOne_Reflections();
        else if (level == Level.LevelTwo_Refractions) To_LevelTwo_Refractions();
        else Debug.LogError("ERROR: Level not recognised");
    }


    public static void To_TestLevel() {SceneManager.LoadScene("TestLevel");}

    public static void To_TitleScreen() {SceneManager.LoadScene("Titlescreen");}

    public static void To_LevelSelection() {SceneManager.LoadScene("LevelSelection");}

    public static void To_LevelOne_Reflections() {SceneManager.LoadScene("LevelOne_Reflections");}

    public static void To_LevelTwo_Refractions() {SceneManager.LoadScene("LevelTwo_Refractions");}
}
