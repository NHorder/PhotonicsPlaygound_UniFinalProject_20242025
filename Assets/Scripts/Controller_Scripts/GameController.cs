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
    public string levelName = "";
    public string levelDescription = "";
    public int startingBudget = 1000;
    public int currentBudget = 1000;


    [HideInInspector]
    public int activeDestinations = 0;
    
    [HideInInspector]
    public bool gameEnd = false;


    private bool _canPurchaseSatellite = true;


    public WorldInfo worldInfo = new WorldInfo();
    public SatellitePrefabs satellitePrefabs;

    public FramerateRelatedSettings framerateRelatedSettings;

    public SpecializedInteractionSettings specializedInteractionSettings;

    private UIController _uiController;


    void Start()
    {

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

        // If the price is more than 0 (meaning it can be purchased) identify which satellite to purchase.
        if (satPrice > 0 && _canPurchaseSatellite)
        {
            // Lock the ability to purchase a satellite until this program has executed.
            _canPurchaseSatellite = false;

            // Prepare variable
            GameObject purchasedSatellite = null;

            // Determine the satellite
            if (satType == SatelliteType.SingleSideReflector)
            {
                if (satellitePrefabs.singlePanelReflectionSatellite != null) purchasedSatellite = satellitePrefabs.singlePanelReflectionSatellite; 
                else Debug.LogError("ERROR: No linked Single Panel Reflection Satellite found");
            }

            else if (satType == SatelliteType.GlassRefractor)
            {
                if (satellitePrefabs.glassRefractionSatellite != null) purchasedSatellite = satellitePrefabs.glassRefractionSatellite; 
                else Debug.LogError("ERROR: No linked Glass Refraction Satellite found");
            }


            if (purchasedSatellite != null)
            {
                // Decrease budget
                currentBudget -= satPrice;

                // Instantiate new satellite and set it's location and collision layers.
                var newSatellite = Instantiate(purchasedSatellite);
                newSatellite.layer = LayerMask.NameToLayer("Object");

                newSatellite.transform.position = new Vector3(worldInfo.newSatelliteLocationX,worldInfo.newSatelliteLocationY,0f);

                // Apply a force to move the satellite away from spawn to prevent satellite clipping.
                newSatellite.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f,100f));
            }
            else
            {
                Debug.LogWarning("WARNING: Satellite Type not recognised (Purchase)");
            }

        }
    }

    public void DestroyedSatellite()
    {
        worldInfo.numSatellitesDestroyed += 1;
    }

    public void LevelEnd()
    {
        gameEnd = true;
        // Notify UI controller of level won, and pass the score
        if (_uiController != null) _uiController.SetCompletedModeActive(true);
        else Debug.LogError("ERROR: Level cannot be completed. GameManager does not have link to UI controller");
    }

    public void ResetLevel()
    {
        // Would collect user confirmation from a menu
        var userConfirmation = true;

        if (userConfirmation) SceneController.ToLevel(thisLevel);
    }

    public void DestinationTrigger(bool active)
    {

        // Update number of activated origins
        if (active) activeDestinations += 1;
        else activeDestinations -=1;

        // if the number of activated origins is more or equal to the number of origins in the world, then the game is complete
        if (activeDestinations >= worldInfo.numDestinations) LevelEnd();
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

[System.Serializable]
public class SatellitePrefabs
{
    public GameObject singlePanelReflectionSatellite;
    public GameObject glassRefractionSatellite;
}