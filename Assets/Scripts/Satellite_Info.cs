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
    private GameController gameController;

    public SatelliteType satelliteType = SatelliteType.Unknown;
    public Interaction interaction;

    public string satelliteName = "";
    public string satelliteDescription= "";
    public string satelliteShortDescription = "";
    public int satellitePurchasePrice = 0;
    public int satelliteSellPrice = 0;

    
    public Advanced_Satellite_Info advanced_Satellite_Info;
    public Satellite_Movement_Info satellite_Movement_Info;
    public Satellite_Shop_Info satellite_Shop_Info;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

        this.RetreiveSatelliteText(this.satelliteType);
        advanced_Satellite_Info.absorbance = Mathf.Clamp01(advanced_Satellite_Info.absorbance);

        if (satellite_Shop_Info.IsShopItem) CreateShopItem();
    }

    public void RetreiveSatelliteText(SatelliteType satelliteType)
    {
        Language language = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>().activeLanguage;

        int satelliteNum = gameController.worldInfo.numSatellites;

        if (satelliteType == SatelliteType.Origin)
        {
            
            if (satelliteName == "") satelliteName = "Prometheus-"+Random.Range(1,384);
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
            if (satelliteDescription == "") satelliteDescription = "A Type VI Fyrefly Deep Space Space Station, designed to withstand the harshest conditions in space. Your task is to get the light beam to this station's satellite dish.";
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
                if (satelliteName == "") satelliteName = "Reflect-LAM-"+satelliteNum+"-SAT";
                if (satelliteDescription == "") satelliteDescription = "A high grade reflactance satellite. The surface has no indents and is perfectly flat, providing optimal reflection of light, a true lambertian diffuse satellite.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A single surface reflection satellite.";
            }

            if (satellitePurchasePrice == 0) satellitePurchasePrice = 100;
            if (satelliteSellPrice == 0) satelliteSellPrice = 50;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0;

            if (interaction == null || interaction == Interaction.Self_Determine) interaction = Interaction.Reflection;
            
        }

        else if (satelliteType == SatelliteType.GlassRefractor)
        {
            if (language == Language.English)
            {
                if (satelliteName == "") satelliteName = "Refract-GL-"+satelliteNum+"-SAT";
                if (satelliteDescription == "") satelliteDescription = "A high grade satellite designed for refracting light. The material is Glass and has a refractive index of 1.52, passing a laser through this object will alter the angle to a small degree.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A glass refraction satellite.";
            }

            if (satellitePurchasePrice == 0) satellitePurchasePrice = 150;
            if (satelliteSellPrice == 0) satelliteSellPrice = 75;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 1.52f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0;
            if (interaction == null || interaction == Interaction.Self_Determine) interaction = Interaction.Refraction;
        }

        else
        {
            if (language == Language.English)
            {
                if (satelliteName == "") satelliteName = "Unknown-SAT";
                if (satelliteDescription == "") satelliteDescription = "An unknown satellite with unknown interactions with light. Be cautious.";
                if (satelliteShortDescription == "") satelliteShortDescription = "Unknown satellite";
                if (satellitePurchasePrice == 0) satellitePurchasePrice = 100;
                if (satelliteSellPrice == 0) satelliteSellPrice = 100;
                if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 1f;
                if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0;
                if (interaction == null || interaction == Interaction.Self_Determine) interaction = Interaction.Absorb;
            }
        }
    }


    private void CreateShopItem()
    {
        // Get common component across children
        RectTransform[] childrenTransforms = gameObject.GetComponentsInChildren<RectTransform>();


        // Execute creation on all child transform
        foreach(RectTransform childTransform in childrenTransforms)
        {
            GameObject childObject = childTransform.gameObject;

            if (childObject.tag != "Prefab_ShopContent" && childObject.name != "Shop_SalePriceText")
            {

                // Remove Clone brackets from the new child objects - more of a personal preference thing
                childObject.name = childObject.name.Replace("(Clone)","");


                // Retrieve text component
                TMP_Text textComponent = childObject.GetComponent<TMP_Text>();

                if (childObject.name == "Shop_SatelliteName") textComponent.text = satelliteName;
                else if (childObject.name == "Shop_ShortDescription") textComponent.text = satelliteShortDescription;

                else if (childObject.name == "Shop_PurchaseButton")
                {
                    // Assign an execution script
                    TMP_Text purchasePriceText = childObject.GetComponentInChildren<TMP_Text>();
                    purchasePriceText.text = "£"+satellitePurchasePrice;
                    
                    childObject.GetComponent<Button>().onClick.AddListener(PurchaseSatellite);
                }

                else if (childObject.name == "Shop_SatelliteSprite")
                {
                    Image imageComponent = childObject.GetComponent<Image>();
                    if (satellite_Shop_Info.satelliteSprite != null) imageComponent.sprite = satellite_Shop_Info.satelliteSprite;
                }
            }
        }

        

        
        
    }

    public void PurchaseSatellite()
    {
        gameController.PurchaseSatellite(this);
    }
}




[System.Serializable]
public class Advanced_Satellite_Info
{
    public float refractiveIndex = 0;
    public float surfaceColor;
    public float absorbance = 0;
    public LaserColour lightColor = LaserColour.White;

    public bool IsSatelliteReceivedCorrectLaser = false;
}

[System.Serializable]
public class Satellite_Movement_Info
{
    public float intialMovementMultiplier = 1f;
    public float intialRotationMultiplier = 0.01f;

    public float maxMovementMultiplier = 5f;
    public float maxRotationMultiplier = 1f;
}

[System.Serializable]
public class Satellite_Shop_Info
{
    public bool IsShopItem = false;

    public Sprite satelliteSprite;
    public float ShopSpriteWidth;
    public float ShopSpriteHeight;
    public float ShopItemXLoc;
    public float ShopItemYLoc;
}


