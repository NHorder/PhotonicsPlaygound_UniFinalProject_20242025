using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    private bool canPurchaseSatellite = true;


    public WorldInfo worldInfo;
    public Satellite_Prefabs satellite_Prefabs;

    public FramerateRelatedSettings framerateRelatedSettings;

    private UI_Controller ui_controller;


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


            int laserInteractionsPerSecond = framerateRelatedSettings.desiredFramerate / (framerateRelatedSettings.laserCycleDelay + 1);

            float timePerLock = framerateRelatedSettings.numLocksForLevelCompletion / framerateRelatedSettings.timeToHoldForLevelCompletion;

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


    public void SetUIController(UI_Controller providedUIController)
    {
        ui_controller = providedUIController;
    }


    public void PurchaseSatellite(Satellite_Info satellite_Info)
    {

        Debug.Log("Satellite Purchase Request Received!");

        SatelliteType satType = satellite_Info.satelliteType;
        int satPrice = satellite_Info.satellitePurchasePrice;

        if (satPrice > 0 && canPurchaseSatellite)
        {
            canPurchaseSatellite = false;

            GameObject purchasedSatellite = null;

            if (satType == SatelliteType.SingleSideReflector)
            {
                if (satellite_Prefabs.singlePanelReflectionSatellite != null) purchasedSatellite = satellite_Prefabs.singlePanelReflectionSatellite; 
                else Debug.LogError("ERROR: No linked Single Panel Reflection Satellite found");
            }

            else if (satType == SatelliteType.GlassRefractor)
            {
                if (satellite_Prefabs.glassRefractionSatellite != null) purchasedSatellite = satellite_Prefabs.glassRefractionSatellite; 
                else Debug.LogError("ERROR: No linked Glass Refraction Satellite found");
            }


            if (purchasedSatellite != null)
            {
                Debug.Log("Making new Satellite!");
                currentBudget -= satPrice;
                worldInfo.numSatellites += 1;

                GameObject newSatellite = Instantiate(purchasedSatellite);
                newSatellite.layer = LayerMask.NameToLayer("Object");

                newSatellite.transform.position = new Vector3(worldInfo.newSatelliteLocationX,worldInfo.newSatelliteLocationY,0f);

                newSatellite.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f,100f));
            }

            // Coroutine follows very similarly to https://docs.unity3d.com/6000.0/Documentation/ScriptReference/WaitForSeconds.html#:~:text=Start%20waiting%20at%20the%20end,seconds%20after%20it%20was%20called. 
            // This is not my own coroutine activation code
            StartCoroutine(LockSatellitePurchase_Coroutine());
        }
    }

    private IEnumerator  LockSatellitePurchase_Coroutine()
    {
        // Coroutine follows very similarly to https://docs.unity3d.com/6000.0/Documentation/ScriptReference/WaitForSeconds.html#:~:text=Start%20waiting%20at%20the%20end,seconds%20after%20it%20was%20called. 
        // This is not my own coroutine activation code, adapted to wait for 1 second which is all I need
        yield return new WaitForSeconds(1);

        // Set can purchase satellite to true;
        canPurchaseSatellite = true;
    }

    public void LevelEnd()
    {
        gameEnd = true;
        // Notify UI controller of level won, and pass the score
        if (ui_controller != null) ui_controller.SetCompletedModeActive(true);
        else Debug.LogError("ERROR: Level cannot be completed. GameManager does not have link to UI controller");

    }

    public void ResetLevel()
    {
        bool userConfirmation = true;

        if (userConfirmation) SceneController.To_Level(thisLevel);
    }

    public void DestinationTrigger(bool active)
    {

        // Update number of activated origins
        if (active) activeDestinations += 1;
        else activeDestinations -=1;

        // if the number of activated origins is more or equal to the number of origins in the world, then the game is complete
        if (activeDestinations >= worldInfo.numDestinations) 
        {
            LevelEnd();
        }
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
public class WorldInfo
{
    public int numOrigins;
    public int numDestinations;
    public int numSatellites;
    public int numSatellitesDestroyed;


    public float newSatelliteLocationX;
    public float newSatelliteLocationY;
    
    public GameObject satelliteBuilder;
}

[System.Serializable]
public class Satellite_Prefabs
{
    public GameObject singlePanelReflectionSatellite;
    public GameObject glassRefractionSatellite;
}