using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Language{
    English,
    Welsh
}




public class GameController : MonoBehaviour
{

    public Language activeLanguage = Language.English;

    public Level thisLevel;

    public string levelNameEnglish = " ";
    public string levelNameWelsh = " ";

    public string levelDescriptionEnglish = " ";
    public string levelDescriptionWelsh = " ";


    public int startingBudget = 1000;
    public int currentBudget = 1000;
    public int maxBudget = 9999;


    [HideInInspector]
    public int activeDestinations = 0;
    
    [HideInInspector]
    public bool gameEnd = false;


    private bool _canPurchaseSatellite = true;


    public WorldInfo worldInfo = new WorldInfo();

    public FramerateRelatedSettings framerateRelatedSettings;

    public SpecializedInteractionSettings specializedInteractionSettings;

    public EyeOfZetaCameraDroneSettings eyeOfZetaCameraDroneSettings;

    private UIController _uiController;

    private SatelliteCreator satelliteCreator;


    void Start()
    {
        activeLanguage = PersistenceController.GetLanguage();

        // Check to make sure the desired framerate is not less than or equal to 0. 
        if (framerateRelatedSettings.desiredFramerate <= 0) 
        {
            Debug.LogWarning("WARNING: DesiredFramerate is less than or equal to 0. Overwriting to default.");

            // Default is 60, Unity defaults to 60 for webGL projects - which this is intended to be used for.
            framerateRelatedSettings.desiredFramerate = 60;
        }

        // Let Unity know that the ideal framerate - 60 is the default and Unity defaults to 60 for web browsers.
        Application.targetFrameRate = framerateRelatedSettings.desiredFramerate;

        if (framerateRelatedSettings.counterToHoldForLevelCompletion < 0)
        {

            // Necessary checks to prevent users from potentially having a 0 division error.

            if (framerateRelatedSettings.laserCycleDelay <= 0)
            {
                Debug.LogWarning("WARNING: LaserCycleDelay is less than or equal to 0. Overwriting to default.");
                framerateRelatedSettings.laserCycleDelay = 1;
            }

            if (framerateRelatedSettings.numLocksForLevelCompletion <= 1)
            {
                Debug.LogWarning("WARNING: Number of Locks for level completion cannot be less than 1. Overwriting to default");
                framerateRelatedSettings.numLocksForLevelCompletion = 1;
            }

            if (framerateRelatedSettings.timeToHoldForLevelCompletion <= 0)
            {
                Debug.LogWarning("WARNING: TimeToHoldForLevelCompletion is less than or equal to 0. Overwriting to default");
                framerateRelatedSettings.timeToHoldForLevelCompletion = 1;
            }

            // Calcualte the number of laser interactions per second - round to nearest full number. This is used for destination interactions
            int laserInteractionsPerSecond = framerateRelatedSettings.desiredFramerate / (framerateRelatedSettings.laserCycleDelay + 1);

            // Calculate the time (s) needed per lock
            float timePerLock = framerateRelatedSettings.numLocksForLevelCompletion / framerateRelatedSettings.timeToHoldForLevelCompletion;

            // If time per lock is less than or equal to 0, something has gone wrong, log error
            if (timePerLock <= 0) 
            {
                Debug.LogError("ERROR: Problem with completion locks, results in timePerLock to be less than 0. Using defaults");
                // Defaults to 1s per lock.
                timePerLock = 1;
            } 

            // Using Mathf.ToRoundInt to make sure it rounds to the nearest correctly - if just simple cast it will ignore the
            // float points, and just take the whole number. I.e 3.9999 would be considered 3 using casting.
            framerateRelatedSettings.counterToHoldForLevelCompletion = Mathf.RoundToInt(laserInteractionsPerSecond * timePerLock);

        }
    }

    public void SetUIController(UIController providedUIController)
    {
        _uiController = providedUIController;
    }

    public UIController GetUIController()
    {
        return _uiController;
    }

    public void PurchaseSatellite(Satellite_Info satelliteInfo)
    {
        // Collect the satellite type and find it's price
        SatelliteType satType = satelliteInfo.satelliteType;
        var satPrice = satelliteInfo.satellitePurchasePrice;



        if (satelliteCreator == null)
        {
            satelliteCreator = GameObject.FindGameObjectsWithTag("SatelliteCreator")[0].GetComponent<SatelliteCreator>();
        }

        // If the price is more than 0 (meaning it can be purchased) identify which satellite to purchase.
        if (satPrice > 0 && _canPurchaseSatellite)
        {
            bool purchased = false;

            if (satelliteCreator != null) purchased = satelliteCreator.CreateSatellite(satType);

            if (purchased)
            {
                // Decrease budget
                currentBudget -= satPrice;
            }
        }
    }

    public void DestroyedSatellite()
    {
        worldInfo.numSatellitesDestroyed += 1;
    }

    public void LevelEnd()
    {
        // Notify persistence controller to unlock the next level
        if (thisLevel == Level.LevelOne_Reflections) PersistenceController.UnlockLevel(Level.LevelTwo_Refractions);
        else if (thisLevel == Level.LevelTwo_Refractions) PersistenceController.UnlockLevel(Level.LevelThree_Colour);
        else if (thisLevel == Level.LevelThree_Colour) PersistenceController.UnlockLevel(Level.LevelFour_ColourSplitting);
        else if (thisLevel == Level.LevelFour_ColourSplitting) PersistenceController.UnlockLevel(Level.LevelFive_ColourCombinations);
        else if (thisLevel == Level.LevelFive_ColourCombinations) PersistenceController.UnlockLevel(Level.LevelSix_PromotionPrerequsite);
        else if (thisLevel == Level.LevelSix_PromotionPrerequsite) PersistenceController.UnlockLevel(Level.LevelSeven_PromotionExam);
        else if (thisLevel == Level.LevelSeven_PromotionExam) PersistenceController.UnlockLevel(Level.LevelEight_Challange);
        else if (thisLevel == Level.LevelEight_Challange) PersistenceController.UnlockLevel( Level.LevelNine_GravitationalAnomalies);
        else if (thisLevel == Level.LevelNine_GravitationalAnomalies) PersistenceController.UnlockLevel(Level.LevelTen_GravitationalCollapse);

        gameEnd = true;
        
        // Notify UI controller of level won, and pass the score
        if (_uiController != null) _uiController.LevelHasEnded();
        else Debug.LogError("ERROR: Level cannot be completed. GameManager does not have link to UI controller");
    }

    public void ResetLevel()
    {
        SceneController.ToLevel(thisLevel);
    }

    public void DestinationTrigger(bool active)
    {

        // Update number of activated origins
        if (active) activeDestinations += 1;
        else activeDestinations -=1;

        // if the number of activated origins is more or equal to the number of origins in the world, then the game is complete
        if (activeDestinations >= worldInfo.numDestinations) LevelEnd();
    }



    public void UpdateSettings()
    {
        activeLanguage = PersistenceController.GetLanguage();

        bool allowAdvancedInteraction = PersistenceController.GetAllowAdvancedInteractions();

        specializedInteractionSettings.allowReflectionDuringRefraction = allowAdvancedInteraction;
    }
}



[System.Serializable]
public class FramerateRelatedSettings
{
    public int desiredFramerate = 60;
    public int laserCycleDelay = 1;

    public int numLocksForLevelCompletion = 3;

    public float timeToHoldForLevelCompletion = 3;

    [HideInInspector]
    public int counterToHoldForLevelCompletion = -1;

}


[System.Serializable]
public class SpecializedInteractionSettings
{
    public bool allowReflectionDuringRefraction = false;
    public float minimumTransparencyNeededForDestinationRecognition = 0.5f;
    public float minimumTransparencyForReflectionDuringRefraction = 0.02f;
}

[System.Serializable]
public class EyeOfZetaCameraDroneSettings
{
    public float minimumXPosition = -20f;
    public float maximumXPosition = 20f;
    public float minimumYPosition = -20f;
    public float maximumYPosition = 20f;

    public float cameraDroneMovementSpeed = 20;

}

public class WorldInfo
{
    public int numOrigins = 0;
    public int numDestinations = 0;
    public int numSatellites = 0;
    public int numSatellitesDestroyed = 0;


    public float newSatelliteLocationX;
    public float newSatelliteLocationY;
    
    public GameObject satelliteBuilder;
}

