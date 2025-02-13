using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SatelliteType{
    Unknown,
    SingleSideReflector,
    GlassRefractor
}



public class Satellite_Info : MonoBehaviour
{
    public SatelliteType satelliteType = SatelliteType.Unknown;

    public string satelliteName = "";
    public string satelliteDescription= "";
    public int satellitePurchasePrice = 0;
    public int satelliteSellPrice = 0;

    
    public Interaction interaction;

    public float refractiveIndex = 0;
    public float surfaceColor;
    public float absorbance = 0;
    
    public float intialMovementMultiplier = 1f;
    public float intialRotationMultiplier = 0.01f;

    public float maxMovementMultiplier = 5f;
    public float maxRotationMultiplier = 1f;

    // Start is called before the first frame update
    void Start()
    {
        this.RetreiveSatelliteText(this.satelliteType);
        absorbance = Mathf.Clamp01(absorbance);

        Debug.Log(interaction);
    }




    public void RetreiveSatelliteText(SatelliteType satelliteType)
    {
        Language language = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>().activeLanguage;

        if (satelliteType == SatelliteType.SingleSideReflector)
        {
            if (language == Language.English)
            {
                if (satelliteName == "") satelliteName = "Reflect-LAM-SAT";
                if (satelliteDescription == "") satelliteDescription = "A high grade reflactance satellite. The surface has no indents and is perfectly flat, providing optimal reflection of light.";
                if (satellitePurchasePrice == 0) satellitePurchasePrice = 100;
                if (satelliteSellPrice == 0) satelliteSellPrice = 50;
                if (refractiveIndex == 0f) refractiveIndex = 0f;
                if (absorbance == 0) absorbance = 0;

                if (interaction == null) interaction = Interaction.Reflection;
            }
            
        }

        else if (satelliteType == SatelliteType.GlassRefractor)
        {
            if (language == Language.English)
            {
                if (satelliteName == "") satelliteName = "Refract-GL-SAT";
                if (satelliteDescription == "") satelliteDescription = "A high grade satellite designed for refracting light. The material is Glass and has a refractive index of 1.52, passing a laser through this object will alter the angle to a small degree.";
                if (satellitePurchasePrice == 0) satellitePurchasePrice = 150;
                if (satelliteSellPrice == 0) satelliteSellPrice = 75;
                if (refractiveIndex == 0f) refractiveIndex = 1.52f;
                if (absorbance == 0) absorbance = 0;


                if (interaction == null) interaction = Interaction.Refraction;
            }
        }

        else
        {
            if (language == Language.English)
            {
                if (satelliteName == "") satelliteName = "Unknown-SAT";
                if (satelliteDescription == "") satelliteDescription = "An unknown satellite with unknown interactions with light. Be cautious.";
                if (satellitePurchasePrice == 0) satellitePurchasePrice = 100;
                if (satelliteSellPrice == 0) satelliteSellPrice = 100;
                if (refractiveIndex == 0f) refractiveIndex = 1f;
                if (absorbance == 0) absorbance = 0;
                if (interaction == null) interaction = Interaction.Absorb;
            }
        }
    }
}




