using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


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
        // Retrieve game controller
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

        // Fill out satellite information
        this.RetreiveSatelliteText(this.satelliteType);

        // Clamp the absorbance of the satellite
        advanced_Satellite_Info.absorbance = Mathf.Clamp01(advanced_Satellite_Info.absorbance);

        // If intended to be a shop item, retrieve child objects
        if (satellite_Shop_Info.IsShopItem) CreateShopItem();
        else
        {
            // Update game controller of satellite, and it's type
            if (satelliteType == SatelliteType.Origin) _gameController.worldInfo.numOrigins += 1;
            else if (satelliteType == SatelliteType.Destination) _gameController.worldInfo.numDestinations += 1;
            else _gameController.worldInfo.numSatellites += 1;
        }
    }

    void Update()
    {
        // if the satellite health falls below or equal to 0, destroy the satellite in a flashy animation
        if (satelliteHealth <= 0)
        {
            // Play satellite destruction animation
            // This animation is forced, as it creates a delay before the object is deleted

            // Destroy satellite
            Destroy(this.gameObject);

            // Create fragments and scatter them
        }
    }

    public void RetreiveSatelliteText(SatelliteType satelliteType)
    {
        //// This method serves as the "database" where all satellites have their related information


        // Collect active language - important for defining descriptions
        Language language = _gameController.activeLanguage;

        // Collect the satellite number - used in naming conventions
        var satelliteNum = _gameController.worldInfo.numSatellites;

        if (satelliteType == SatelliteType.Origin)
        {
            // if origin, name is determined randomly
            if (satelliteName == "") satelliteName = "Prometheus-"+Random.Range(1,384);

            // Origin satellites are not destroyable, but still have health in cases developers wish to destroy them
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

            // If destination, name is generated with a random number
            if (satelliteName == "") satelliteName = "Fyrefly-"+Random.Range(1,384);

            // Destination satellites are destroyable, unlike origin
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

        else if (satelliteType == SatelliteType.SatelliteCreator)
        {
            // If destination, name is generated with a random number
            if (satelliteName == "") satelliteName = "Elysia-003";

            // Destination satellites are destroyable, unlike origin
            if (satelliteHealth == 100) satelliteHealth = 3000;

            if (satelliteDescription == "") satelliteDescription = "A cutting edge Elysian Matter Printer. One of the three exisitng matter printers, this space craft has been outfitted with the highest end equipment to create a number of satellites on request.";
            if (satelliteDescription == "" && language == Language.Welsh) satelliteDescription = "NOT YET TRANSLATED";

            if (satelliteShortDescription == "" && language == Language.English) satelliteShortDescription = "Not an Item in the Shop";
            if (satelliteShortDescription == "" && language == Language.Welsh) satelliteShortDescription = "NOT YET TRANSLATED";

            if (satellitePurchasePrice == 0) satellitePurchasePrice = 100;
            if (satelliteSellPrice == 0) satelliteSellPrice = 50;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0;
            interaction = Interaction.Absorb;
        }

        else if (satelliteType == SatelliteType.SingleSideReflector)
        {
            if (language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Reflect-LAM-{satelliteNum}-SAT";

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
                if (satelliteName == "") satelliteName = $"Refract-GL-{satelliteNum}-SAT";

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
            // if the satellite is unknown then set it to a default health and have it absorb all light, also notify developer

            Debug.LogWarning("WARNING: Unknown Satellite detected");

            if (language == Language.English)
            {
                if (satelliteName == "") satelliteName = "Unknown-SAT";
                if (satelliteHealth == 100) satelliteHealth = 100;

                if (satelliteDescription == "") satelliteDescription = "An unknown satellite with unknown interactions with light. Be cautious.";
                if (satelliteShortDescription == "") satelliteShortDescription = "Unknown satellite";
            }

            if (satellitePurchasePrice == 0) satellitePurchasePrice = 100;
            if (satelliteSellPrice == 0) satelliteSellPrice = 100;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 1f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0;
            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Absorb;
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
        // When collision occurs with the game object, decrease the health of this and the colliders satellite.
        // There is no guarentee that the hit object is a satellite.

        var colliderObject = collision.gameObject;

        // Prepare variable in case satellite on satellite collision occurred.
        Satellite_Info opposingSatellite = null;

        // Try to get satellite info of the object, may not be possible if it's an asteroid or boundary
        try
        {
            opposingSatellite = colliderObject.GetComponent<Satellite_Info>();
        }
        catch{
            // Do nothing, as expected occurance
        }

        // Satellites cannot interact with origin
        if (opposingSatellite.satelliteType == SatelliteType.Origin || opposingSatellite.satelliteType == SatelliteType.SatelliteCreator ) {}

        // Satellites are easily destroyed when interacting with destination
        else if (opposingSatellite.satelliteType == SatelliteType.Destination)
        {
            satelliteHealth -= 80;
            if (opposingSatellite != null) opposingSatellite.satelliteHealth -= 10;
        }

        // Glass refractors take more damage upon hitting opposing satellites
        else if (this.satelliteType == SatelliteType.GlassRefractor)
        {
            satelliteHealth -= 40;
            if (opposingSatellite != null) opposingSatellite.satelliteHealth -= 25;
        }

        else
        {
            satelliteHealth -= 25;
            if (opposingSatellite != null) opposingSatellite.satelliteHealth -= 25;
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


