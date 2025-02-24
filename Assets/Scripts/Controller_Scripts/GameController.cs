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



    private bool canPurchaseSatellite = true;

    public WorldInfo worldInfo;
    public Satellite_Prefabs satellite_Prefabs;

    private UI_Controller ui_controller;

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
        // Notify UI controller of level won, and pass the score
        if (ui_controller != null) ui_controller.SetCompletedModeActive(true);
        else Debug.LogError("ERROR: Level cannot be completed. GameManager does not have link to UI controller");

    }

    public void ResetLevel()
    {
        bool userConfirmation = true;

        if (userConfirmation) SceneController.To_Level(thisLevel);
    }

}





[System.Serializable]
public class WorldInfo
{
    public int numOrigins = 1;
    public int numDestinations = 1;
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