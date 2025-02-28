using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum SatelliteType{
    Unknown,
    SingleSideReflector,
    GlassRefractor,
    Origin,
    Destination,

}

public class Satellite_Info : MonoBehaviour
{
    public SatelliteType satelliteType = SatelliteType.Unknown;
    public Interaction interaction;
    public string satelliteName = "";

    public int satelliteHealth = 100;

    public string satelliteDescription= "";
    public string satelliteShortDescription = "";
    public int satellitePurchasePrice = 0;
    public int satelliteSellPrice = 0;

    
    public AdvancedSatelliteInfo advanced_Satellite_Info;
    public SatelliteMovementInfo satellite_Movement_Info;
    public SatelliteShopInfo satellite_Shop_Info;

    private GameController _gameController;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

        this.RetreiveSatelliteText(this.satelliteType);
        advanced_Satellite_Info.absorbance = Mathf.Clamp01(advanced_Satellite_Info.absorbance);

        if (satellite_Shop_Info.IsShopItem) CreateShopItem();
        else
        {
            if (satelliteType == SatelliteType.Origin) _gameController.worldInfo.numOrigins += 1;
            else if (satelliteType == SatelliteType.Destination) _gameController.worldInfo.numDestinations += 1;
            else _gameController.worldInfo.numSatellites += 1;
        }
    }

    void Update()
    {
        if (satelliteHealth <= 0)
        {
            // Play satellite destruction animation

            // Destroy satellite
            Destroy(this.gameObject);
        }
    }

    public void RetreiveSatelliteText(SatelliteType satelliteType)
    {
        Language language = _gameController.activeLanguage;

        var satelliteNum = _gameController.worldInfo.numSatellites;

        if (satelliteType == SatelliteType.Origin)
        {
            
            if (satelliteName == "") satelliteName = "Prometheus-"+Random.Range(1,384);

            if (satelliteHealth == 100) satelliteHealth = 1000;

            if (satelliteDescription == "" && language == Language.English) satelliteDescription = "A Type XII Prometheus communication output, designed for deep space communciations it boasts a powerful beam of light to send messages into deep space. This is where your light laser begins.";
            if (satelliteDescription == "" && language == Language.Welsh) satelliteDescription = "NOT YET TRANSLATED";

            if (satelliteShortDescription == "" && language == Language.English) satelliteShortDescription = "Not an Item in the Shop";
            if (satelliteShortDescription == "" && language == Language.Welsh) satelliteShortDescription = "NOT YET TRANSLATED";

            if (satellitePurchasePrice == 0) satellitePurchasePrice = 0;
            if (satelliteSellPrice == 0) satelliteSellPrice = 0;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0;
            

        }

        else if (satelliteType == SatelliteType.Destination)
        {
            if (satelliteName == "") satelliteName = "Fyrefly-"+Random.Range(1,384);

            if (satelliteHealth == 100) satelliteHealth = 700;

            if (satelliteDescription == "") satelliteDescription = "A Type VI Fyrefly Deep Space Space Station, designed to withstand the harshest conditions in space. Your task is to get the light beam to this station's satellite dish.";
            if (satelliteDescription == "" && language == Language.Welsh) satelliteDescription = "NOT YET TRANSLATED";

            if (satelliteShortDescription == "" && language == Language.English) satelliteShortDescription = "Not an Item in the Shop";
            if (satelliteShortDescription == "" && language == Language.Welsh) satelliteShortDescription = "NOT YET TRANSLATED";

            if (satellitePurchasePrice == 0) satellitePurchasePrice = 100;
            if (satelliteSellPrice == 0) satelliteSellPrice = 50;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0;
            interaction = Interaction.Destination;
        }

        else if (satelliteType == SatelliteType.SingleSideReflector)
        {
            if (language == Language.English)
            {
                if (satelliteName == "") satelliteName = "Reflect-LAM-"+satelliteNum+"-SAT";

                if (satelliteHealth == 100) satelliteHealth = 200;

                if (satelliteDescription == "") satelliteDescription = "A high grade reflactance satellite. The surface has no indents and is perfectly flat, providing optimal reflection of light, a true lambertian diffuse satellite.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A single surface reflection satellite.";
            }

            if (satellitePurchasePrice == 0) satellitePurchasePrice = 100;
            if (satelliteSellPrice == 0) satelliteSellPrice = 50;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Reflection;
            
        }

        else if (satelliteType == SatelliteType.GlassRefractor)
        {
            if (language == Language.English)
            {
                if (satelliteName == "") satelliteName = "Refract-GL-"+satelliteNum+"-SAT";

                if (satelliteHealth == 100) satelliteHealth = 90;

                if (satelliteDescription == "") satelliteDescription = "A high grade satellite designed for refracting light. The material is Glass and has a refractive index of 1.52, passing a laser through this object will alter the angle to a small degree.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A glass refraction satellite.";
            }

            if (satellitePurchasePrice == 0) satellitePurchasePrice = 150;
            if (satelliteSellPrice == 0) satelliteSellPrice = 75;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 1.52f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0;
            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Refraction;
        }

        else
        {
            if (language == Language.English)
            {
                if (satelliteName == "") satelliteName = "Unknown-SAT";
                if (satelliteHealth == 100) satelliteHealth = 100;

                if (satelliteDescription == "") satelliteDescription = "An unknown satellite with unknown interactions with light. Be cautious.";
                if (satelliteShortDescription == "") satelliteShortDescription = "Unknown satellite";
                if (satellitePurchasePrice == 0) satellitePurchasePrice = 100;
                if (satelliteSellPrice == 0) satelliteSellPrice = 100;
                if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 1f;
                if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0;
                if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Absorb;
            }
        }
    }


    private void CreateShopItem()
    {
        // Get common component across children
        var childrenTransformList = gameObject.GetComponentsInChildren<RectTransform>();


        // Execute creation on all child transform
        foreach(RectTransform childTransform in childrenTransformList)
        {
            var childObject = childTransform.gameObject;

            if (childObject.tag != "Prefab_ShopContent" && childObject.name != "Shop_SalePriceText")
            {

                // Remove Clone brackets from the new child objects - more of a personal preference thing
                childObject.name = childObject.name.Replace("(Clone)","");


                // Retrieve text component
                var textComponent = childObject.GetComponent<TMP_Text>();

                if (childObject.name == "Shop_SatelliteName") textComponent.text = satelliteName;
                else if (childObject.name == "Shop_ShortDescription") textComponent.text = satelliteShortDescription;

                else if (childObject.name == "Shop_PurchaseButton")
                {
                    // Assign an execution script
                    var purchasePriceText = childObject.GetComponentInChildren<TMP_Text>();
                    purchasePriceText.text = "£"+satellitePurchasePrice;
                    
                    childObject.GetComponent<Button>().onClick.AddListener(PurchaseSatellite);
                }

                else if (childObject.name == "Shop_SatelliteSprite")
                {
                    var imageComponent = childObject.GetComponent<Image>();
                    if (satellite_Shop_Info.satelliteSprite != null) imageComponent.sprite = satellite_Shop_Info.satelliteSprite;
                }
            }
        }

    }

    public void PurchaseSatellite()
    {
        _gameController.PurchaseSatellite(this);
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        var colliderObject = collision.gameObject;


        // Try to get satellite info of the object.
        try
        {
            var satInfo = colliderObject.GetComponent<Satellite_Info>();

            // Satellites cannot interact with origin
            if (satInfo.satelliteType == SatelliteType.Origin) {}

            // Satellites are easily destroyed when interacting with destination
            else if (satInfo.satelliteType == SatelliteType.Destination)
            {
                satelliteHealth -= 80;
                satInfo.satelliteHealth -= 10;
            }

            else
            {
                // Glass refractors take more damage upon hitting opposing satellites
                if (this.satelliteType == SatelliteType.GlassRefractor)
                {
                    satelliteHealth -= 40;
                    satInfo.satelliteHealth -= 25;
                }
                else
                {
                    satelliteHealth -= 25;
                    satInfo.satelliteHealth -= 25;
                }
            }


        }
        catch{

            // Try to get asteroid info from the object
            try {

            }

            // Else do nothing
            catch{}

        }
    }
}




[System.Serializable]
public class AdvancedSatelliteInfo
{
    public bool isSelectable = true;
    public float refractiveIndex = 0;
    public float percentageOfReflectedLightWhenRefracted = 0;
    public float surfaceColor;
    public float absorbance = 0;
    public LaserColour lightColor = LaserColour.White;

    public bool IsSatelliteReceivedCorrectLaser = false;
}

[System.Serializable]
public class SatelliteMovementInfo
{
    public float intialMovementMultiplier = 2f;
    public float intialRotationMultiplier = 2f;

    public float maxMovementMultiplier = 20f;
    public float maxRotationMultiplier = 20f;
}

[System.Serializable]
public class SatelliteShopInfo
{
    public bool IsShopItem = false;

    public Sprite satelliteSprite;
    public float ShopSpriteWidth;
    public float ShopSpriteHeight;
    public float ShopItemXLoc;
    public float ShopItemYLoc;
}


