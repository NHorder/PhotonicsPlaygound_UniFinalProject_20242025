using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public string satelliteName = "";
    public string satelliteDescription= "";
    public int satellitePurchasePrice = 0;
    public int satelliteSellPrice = 0;

    private int satelliteNum = 0;

    public LaserColour lightColor = LaserColour.White;

    public bool IsSatelliteReceivedCorrectLaser = false;

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
        gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

        this.RetreiveSatelliteText(this.satelliteType);
        absorbance = Mathf.Clamp01(absorbance);

        Debug.Log(interaction);
    }

    public void RetreiveSatelliteText(SatelliteType satelliteType)
    {
        Language language = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>().activeLanguage;


        if (satelliteType == SatelliteType.Origin)
        {
            
            if (satelliteName == "") satelliteName = "Prometheus-"+Random.Range(1,384);
            if (satelliteDescription == "" && language == Language.English) satelliteDescription = "A Type XII Prometheus communication output, designed for deep space communciations it boasts a powerful beam of light to send messages into deep space. This is where your light laser begins.";
            if (satelliteDescription == "" && language == Language.Welsh) satelliteDescription = "NOT YET TRANSLATED";
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 0;
            if (satelliteSellPrice == 0) satelliteSellPrice = 0;
            if (refractiveIndex == 0f) refractiveIndex = 0f;
            if (absorbance == 0) absorbance = 0;

        }

        else if (satelliteType == SatelliteType.Destination)
        {
            if (satelliteName == "") satelliteName = "Fyrefly-"+Random.Range(1,384);
            if (satelliteDescription == "") satelliteDescription = "A Type VI Fyrefly Deep Space Space Station, designed to withstand the harshest conditions in space. Your task is to get the light beam to this station's satellite dish.";
            if (satelliteDescription == "" && language == Language.Welsh) satelliteDescription = "NOT YET TRANSLATED";
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 100;
            if (satelliteSellPrice == 0) satelliteSellPrice = 50;
            if (refractiveIndex == 0f) refractiveIndex = 0f;
            if (absorbance == 0) absorbance = 0;

            interaction = Interaction.Absorb;
        }

        else if (satelliteType == SatelliteType.SingleSideReflector)
        {
            if (language == Language.English)
            {
                if (satelliteName == "") satelliteName = "Reflect-LAM-"+satelliteNum+"-SAT";
                if (satelliteDescription == "") satelliteDescription = "A high grade reflactance satellite. The surface has no indents and is perfectly flat, providing optimal reflection of light, a true lambertian diffuse satellite.";
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
                if (satelliteName == "") satelliteName = "Refract-GL-"+satelliteNum+"SAT";
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




