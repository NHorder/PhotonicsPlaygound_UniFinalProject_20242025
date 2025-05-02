using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Language settings, currently only English and Welsh are supported
/// Note: Support is hard coded, no files are used.
/// </summary>
public enum Language{
    English,
    Welsh
}


public class GameController : MonoBehaviour
{
    /// <summary>
    /// Core Game Controller, manages the level
    /// </summary>

    // Active Language
    public Language activeLanguage = Language.English;

    // Which level this is, used for resetting the level
    public Level thisLevel;


    // Level Information in both English and Welsh
    public string levelNameEnglish = " ";
    public string levelNameWelsh = " ";

    public string levelDescriptionEnglish = " ";
    public string levelDescriptionWelsh = " ";

    // Budgets for the level. 
    // Note the current budget is overwritten intially to the starting budget, it's 
    // public to allow other systems to check if a purchase can be made.
    public float startingBudget = 1000;
    public float currentBudget = 1000;

    public float minimumBudget = -300;

    public float expectedBudgetOnCompletion;

    // Base damage applied to all satellites
    public int baseDamage = 20;

    // Hidden are the number of active destinations, as this determines when the game is won
    // Hidden to prevent developers from modifying this value, as it may cause issues in level completion
    [HideInInspector]
    public int activeDestinations = 0;

    // Mark to end the game, hidden to prevent developers from toggling it too early. 
    [HideInInspector]
    public bool gameEnd = false;

    private bool _canPurchaseSatellite = true;

    
    // WorldInfo contains the number of satellites, origin, destinations and satellites destroyed.
    public WorldInfo worldInfo = new WorldInfo();

    // Framerate related settings
    // Mainly used for developer assistence, as this project is designed for WebGL - which provides a
    // default of 60 fps, hence it can greatly assist if the developer has the same fps.
    public FramerateRelatedSettings framerateRelatedSettings;

    // Minor settings for 'unique' interactions, I.e Fresnel Equations (See Refraction Satellite)
    public SpecializedInteractionSettings specializedInteractionSettings;

    private UIController _uiController;

    private SatelliteCreator _satelliteCreatorElysia;
    private CameraDrone _cameraDroneEyeOfZeta;

    /// <summary>
    /// Initalisation Method
    /// </summary>
    void Start()
    {
        // Retrieve the active language from the persistence controller
        activeLanguage = PersistenceController.GetLanguage();

        // Make sure there is an expected budget for score calclulation later on
        if (expectedBudgetOnCompletion == null || expectedBudgetOnCompletion == 0) expectedBudgetOnCompletion = (float)(0.5 * startingBudget);

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

    /// <summary>
    /// Method to set the UI controller.
    /// Needed in order to cascade updates, GameController is created before UI Controller, hence a search cannot be made
    /// </summary>
    /// <param name="providedUIController"></param>
    public void SetUIController(UIController providedUIController)
    {
        _uiController = providedUIController;
    }

    /// <summary>
    /// Method to get the UI Controller
    /// Used to reduce unneccary links / find methods to access the UI controller.
    /// </summary>
    /// <returns></returns>
    public UIController GetUIController()
    {
        return _uiController;
    }

    /// <summary>
    /// Method to purchase a satellite of a specific type. This is called from the Shop Panel.
    /// </summary>
    /// <param name="satelliteInfo"></param>
    public void PurchaseSatellite(SatelliteInfo satelliteInfo)
    {
        // Collect the satellite type and find it's price
        SatelliteType satType = satelliteInfo.satelliteType;
        var satPrice = satelliteInfo.satellitePurchasePrice;

        // Check that the satellite creator "Elysia" exists and the Eye of Zeta exists
        if (_satelliteCreatorElysia == null || _cameraDroneEyeOfZeta == null)
        {
            // Retrieve both game objects
            _satelliteCreatorElysia = GameObject.FindGameObjectsWithTag("SatelliteCreator")[0].GetComponent<SatelliteCreator>();
            _cameraDroneEyeOfZeta = GameObject.FindGameObjectsWithTag("EyeOfZeta")[0].GetComponent<CameraDrone>();
        }

        // If the price is more than 0 (meaning it can be purchased) identify which satellite to purchase.
        if (satPrice > 0 && _canPurchaseSatellite)
        {
            // Request Elysia to create a satellite
            bool purchased = _satelliteCreatorElysia.CreateSatellite(satType);
            // If it can it will update Purchased to True;

            // Attach drone to Elysia
            _cameraDroneEyeOfZeta.AttachDroneToSatellite(_satelliteCreatorElysia.gameObject.GetComponent<Transform>());

            // If successfuly, reduce curernt budget.
            // means if a satellite fails to be created, no budget is lost
            if (purchased)
            {
                // Decrease budget
                currentBudget -= satPrice;
            }
        }
    }

    /// <summary>
    /// Method to notify game controller of satellite destuction
    /// </summary>
    public void DestroyedSatellite()
    {
        worldInfo.numSatellitesDestroyed += 1;
    }

    /// <summary>
    /// Method to end the level.
    /// Includes unlocking of the next level and cascade of ending the level
    /// </summary>
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

    /// <summary>
    /// Method to reset the level, this is called from the ConfirmationController
    /// </summary>
    public void ResetLevel()
    {
        SceneController.ToLevel(thisLevel);
    }

    /// <summary>
    /// Method to notify gamecontroller of destination unlock, when all destinations are unlocked, end the level
    /// </summary>
    /// <param name="active"></param>
    public void DestinationTrigger(bool active)
    {

        // Update number of activated origins
        if (active) activeDestinations += 1;
        else activeDestinations -=1;

        // if the number of activated origins is more or equal to the number of origins in the world, then the game is complete
        if (activeDestinations >= worldInfo.numDestinations) LevelEnd();
    }


    /// <summary>
    /// Method to begin settings update cascade
    /// </summary>
    public void UpdateSettings()
    {
        // Update active language
        activeLanguage = PersistenceController.GetLanguage();

        // Update specialised interractions
        specializedInteractionSettings.allowReflectionDuringRefraction = PersistenceController.GetAllowAdvancedInteractions();
        specializedInteractionSettings.allowSatelliteParticleEffects = PersistenceController.GetAllowSatelliteMovementParticles();

        if (_satelliteCreatorElysia == null && thisLevel != Level.Titlescreen && thisLevel != Level.LevelSelection)
        {
            _satelliteCreatorElysia = GameObject.FindGameObjectsWithTag("SatelliteCreator")[0].GetComponent<SatelliteCreator>();
            _satelliteCreatorElysia.UpdateLanguage(activeLanguage);
        }
    }
}


/// <summary>
/// Framerate related settings, includes desttination lock time
/// Default of lock time is 3 seconds (I.e hold light beam over destination for 3 seconds to unlock it)
/// </summary>
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

/// <summary>
/// Specialised interaction settings: Particle effects, Fresnel Equations
/// and minimum light strength to trigger Fresnel Equations and destinations
/// </summary>
[System.Serializable]
public class SpecializedInteractionSettings
{
    public bool allowReflectionDuringRefraction = true;
    public bool allowSatelliteParticleEffects = true;
    public float minimumTransparencyNeededForDestinationRecognition = 0.5f;
    public float minimumTransparencyForReflectionDuringRefraction = 0.02f;
}


/// <summary>
/// World information, to track number of satellites, origins and destinations
/// </summary>
public class WorldInfo
{
    public int numOrigins = 0;
    public int numDestinations = 0;
    public int numSatellites = 0;
    public int numSatellitesDestroyed = 0;
}

