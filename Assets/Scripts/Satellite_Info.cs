using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum SatelliteType{
    Unknown,
    SingleSideReflector,
    DoubleSideReflector,
    GlassRefractor,
    SapphireRefractor,
    SiliconRefractor,
    WaterRefractor,
    Combiner,
    DuelSplitter,
    TrioSplitter,
    HexSplitter,
    WhiteBasicColourFilter,
    RedBasicColourFilter,
    BlueBasicColourFilter,
    GreenBasicColourFilter,
    YellowBasicColourFilter,
    CyanBasicColourFilter,
    MagentaBasicColourFilter,
    CustomColourFilter,

    Origin,
    Destination,
    SatelliteCreator,


    Debris_Absorb,
    Debris_Reflect,
    Debris_Splitter,
    Debris_Filter,


    CameraDrone,

}


public class Satellite_Info : MonoBehaviour
{
    public SatelliteType satelliteType = SatelliteType.Unknown;
    public Interaction interaction;
    public bool isDebris = false;
    public string satelliteName = "";

    public int satelliteHealth = 100;

    public string satelliteDescription= "";
    public string satelliteShortDescription = "";
    public int satellitePurchasePrice = 0;

    public bool canBeSold = true;
    public int satelliteSellPrice = 0;


    public bool canbeMoved = true;

    
    public AdvancedSatelliteInfo advanced_Satellite_Info;
    public SatelliteMovementInfo satellite_Movement_Info;
    public SatelliteShopInfo satellite_Shop_Info;

    private GameController _gameController;

    private Language _language = Language.English;

    // Start is called before the first frame update
    void Start()
    {
        // Retrieve game controller
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

        // Fill out satellite information
        this.RetreiveSatelliteText();

        // Clamp the absorbance of the satellite
        advanced_Satellite_Info.absorbance = Mathf.Clamp01(advanced_Satellite_Info.absorbance);

        // If intended to be a shop item, retrieve child objects
        if (satellite_Shop_Info.IsShopItem) CreateShopItem();
        else
        {
            // Update game controller of satellite, and it's type
            if (satelliteType == SatelliteType.Origin) _gameController.worldInfo.numOrigins += 1;
            else if (satelliteType == SatelliteType.Destination) _gameController.worldInfo.numDestinations += 1;
            else if (isDebris){
                // Do Nothing
            }
            else _gameController.worldInfo.numSatellites += 1;
        }
    }

    void Update()
    {
        if (_language != _gameController.activeLanguage)
        {
            _language = _gameController.activeLanguage;

            satelliteName = "";
            satelliteDescription = "";
            satelliteShortDescription = "";

            RetreiveSatelliteText();
            if (satellite_Shop_Info.IsShopItem) CreateShopItem();

        }

        // if the satellite health falls below or equal to 0, destroy the satellite in a flashy animation
        if (satelliteHealth <= 0)
        {
            // Play satellite destruction animation
            // The animation contains a destroy trigger - which when reached will call this DestroyObject function.
            // Forcing a animation delay before destruction.

            var animator = this.gameObject.GetComponent<Animator>();
            animator.SetBool("Destroy",true);

        }
    }

    public void RetreiveSatelliteText()
    {
        //// This method serves as the "database" where all satellites have their related information


        // Collect active language - important for defining descriptions
        if (_language == null) _language = _gameController.activeLanguage;

        // Collect the satellite number - used in naming conventions
        var satelliteNum = _gameController.worldInfo.numSatellites;

        if (satelliteType == SatelliteType.Origin)
        {
            canbeMoved = false;
            canBeSold = false;


            // if origin, name is determined randomly
            if (satelliteName == "") satelliteName = "Prometheus-"+Random.Range(1,384);

            // Origin satellites are not destroyable, but still have health in cases developers wish to destroy them
            if (satelliteHealth == 100) satelliteHealth = 1000;

            if (satelliteDescription == "" && _language == Language.English) satelliteDescription = "A Type XII Prometheus communication output, designed for deep space communciations it boasts a powerful beam of light to send messages into deep space. This is where your light laser begins.";
            else if (satelliteDescription == "" && _language == Language.Welsh) satelliteDescription = "NOT YET TRANSLATED";

            if (satelliteShortDescription == "" && _language == Language.English) satelliteShortDescription = "Not an Item in the Shop";
            else if (satelliteShortDescription == "" && _language == Language.Welsh) satelliteShortDescription = "NOT YET TRANSLATED";

            if (satellitePurchasePrice == 0) satellitePurchasePrice = 0;
            if (satelliteSellPrice == 0) satelliteSellPrice = 0;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0;
            

        }
        else if (satelliteType == SatelliteType.Destination)
        {
            canbeMoved = false;
            canBeSold = false;
            // If destination, name is generated with a random number
            if (satelliteName == "") satelliteName = "Fyrefly-"+Random.Range(1,384);

            // Destination satellites are destroyable, unlike origin
            if (satelliteHealth == 100) satelliteHealth = 700;

            if (satelliteDescription == "" && _language == Language.English) satelliteDescription = "A Type VI Fyrefly Deep Space Space Station, designed to withstand the harshest conditions in space. Your task is to get the light beam to this station's satellite dish.";
            else if (satelliteDescription == "" && _language == Language.Welsh) satelliteDescription = "NOT YET TRANSLATED";

            if (satelliteShortDescription == "" && _language == Language.English) satelliteShortDescription = "Not an Item in the Shop";
            else if (satelliteShortDescription == "" && _language == Language.Welsh) satelliteShortDescription = "NOT YET TRANSLATED";

            if (satellitePurchasePrice == 0) satellitePurchasePrice = 100;
            if (satelliteSellPrice == 0) satelliteSellPrice = 50;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0.1f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0;
            interaction = Interaction.Destination;
        }
        else if (satelliteType == SatelliteType.SatelliteCreator)
        {
            canbeMoved = false;
            canBeSold = false;
            // If destination, name is generated with a random number
            if (satelliteName == "") satelliteName = "Elysia";

            // Destination satellites are destroyable, unlike origin
            if (satelliteHealth == 100) satelliteHealth = 3000;

            if (satelliteDescription == "" && _language == Language.English) satelliteDescription = "Elysia is a cutting edge Elysian Matter Printer. One of the three in existence. It makes use of space-time manipulator lasers and drones to construct satellites of any type.";
            else if (satelliteDescription == "" && _language == Language.Welsh) satelliteDescription = "NOT YET TRANSLATED";

            if (satelliteShortDescription == "" && _language == Language.English) satelliteShortDescription = "Not an Item in the Shop";
            else if (satelliteShortDescription == "" && _language == Language.Welsh) satelliteShortDescription = "NOT YET TRANSLATED";

            if (satellitePurchasePrice == 0) satellitePurchasePrice = 12000000;
            if (satelliteSellPrice == 0) satelliteSellPrice = 12000000;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0;
            interaction = Interaction.Absorb;
        }
        else if (satelliteType == SatelliteType.SingleSideReflector)
        {
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Reflect-Single-LAM-{satelliteNum}-SAT";
                if (satelliteDescription == "") satelliteDescription = "A high grade reflectance satellite. The surface has no indents and is perfectly flat, providing optimal reflection of light, a true lambertian diffuse satellite.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A single surface reflection satellite.";
            }

            if (satelliteHealth == 100) satelliteHealth = 200;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 100;
            if (satelliteSellPrice == 0) satelliteSellPrice = 50;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.1f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Reflection;
            
        }
        else if (satelliteType == SatelliteType.DoubleSideReflector)
        {
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Reflect-Duo-LAM-{satelliteNum}-SAT";
                if (satelliteDescription == "") satelliteDescription = "A high grade reflectance satellite. Upgraded from it's predecessor, this includes two panels for lambertian reflection.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A two surface reflection satellite.";
            }

            if (satelliteHealth == 100) satelliteHealth = 250;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 150;
            if (satelliteSellPrice == 0) satelliteSellPrice = 75;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.1f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Reflection;

        }
        else if (satelliteType == SatelliteType.GlassRefractor)
        {
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Refract-GL-{satelliteNum}-SAT";
                if (satelliteDescription == "") satelliteDescription = "A high grade satellite designed for refracting light. The material is Glass and has a refractive index of 1.52, passing a laser through this object will alter the angle to a small degree.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A glass refraction satellite.";
            }

            
            if (satelliteHealth == 100) satelliteHealth = 90;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 150;
            if (satelliteSellPrice == 0) satelliteSellPrice = 75;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 1.52f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.033f;
            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Refraction;
        }
        else if (satelliteType == SatelliteType.SapphireRefractor){
             if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Refract-SAP-{satelliteNum}-SAT";
                if (satelliteDescription == "") satelliteDescription = "A high grade satellite designed for refracting light. The material is crystal sapphire and has a refractive index of 1.78, passing a laser through this object will alter the angle to a large degree.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A sapphire refraction satellite";
            }

            if (satelliteHealth == 100) satelliteHealth = 120;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 300;
            if (satelliteSellPrice == 0) satelliteSellPrice = 250;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 1.78f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.033f;
            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Refraction;
        }
        else if (satelliteType == SatelliteType.SiliconRefractor){
             if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Refract-S-{satelliteNum}-SAT";
                if (satelliteDescription == "") satelliteDescription = "A high grade satellite designed for refracting light. The material is silicon and has a refractive index of 3.4, passing a laser through this object will alter the angle to a large degree.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A silicon refraction satellite";
            }

            if (satelliteHealth == 100) satelliteHealth = 120;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 250;
            if (satelliteSellPrice == 0) satelliteSellPrice = 200;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 3.4f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.033f;
            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Refraction;
        }
        else if (satelliteType == SatelliteType.WaterRefractor){
             if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Refract-H2O-{satelliteNum}-SAT";
                if (satelliteDescription == "") satelliteDescription = "A cutting edge prototype satellite, capable of maintaining water in a liquid space in deep space and keeping it contained. It has a refractive index of 1.33, passing a laser through this object will alter the angle to a small degree. ";
                if (satelliteShortDescription == "") satelliteShortDescription = "A water refraction satellite.";
            }

            if (satelliteHealth == 100) satelliteHealth = 50;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 400;
            if (satelliteSellPrice == 0) satelliteSellPrice = 300;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 1.33f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.033f;
            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Refraction;
        }
        
        // If a colour filter then grant same name to all.
        else if ( satelliteType == SatelliteType.WhiteBasicColourFilter ||
                satelliteType == SatelliteType.RedBasicColourFilter ||
                satelliteType == SatelliteType.BlueBasicColourFilter ||
                satelliteType == SatelliteType.GreenBasicColourFilter ||
                satelliteType == SatelliteType.YellowBasicColourFilter ||
                satelliteType == SatelliteType.CyanBasicColourFilter ||
                satelliteType == SatelliteType.MagentaBasicColourFilter
        )
        {
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Colour-Filter-{satelliteNum}-SAT";
                if (satelliteDescription == "") satelliteDescription = "A simple colour filter. This will change the colour of light beams that pass through it. Cheaply made, not too durable but suitable for the task.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A fixed coloured filter";
            }
            else if (_language == Language.Welsh)
            {

            }

            if (satelliteHealth == 100) satelliteHealth = 125;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 75;
            if (satelliteSellPrice == 0) satelliteSellPrice = 50;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.1f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.ColourFilter;

            if (!satellite_Shop_Info.IsShopItem)
            {
                 // Get the animator and set the animation to be played (base animation is just colour change)
                Animator animator = gameObject.GetComponent<Animator>();
                // Get the colour filter script and the set the colour
                var colourFilterSatellite = gameObject.GetComponent<ColourFilterSatellite>();

                // Get the animator and set the animation to be played (base animation is just colour change)
                if (satelliteType == SatelliteType.WhiteBasicColourFilter) 
                {
                    animator.SetInteger("FilterID",0);
                    colourFilterSatellite.SetFilterColour(LaserColour.White);
                }

                else if (satelliteType == SatelliteType.RedBasicColourFilter) 
                {
                    animator.SetInteger("FilterID",1);
                    colourFilterSatellite.SetFilterColour(LaserColour.Red);
                }

                else if (satelliteType == SatelliteType.BlueBasicColourFilter)
                {
                    animator.SetInteger("FilterID",2);
                    colourFilterSatellite.SetFilterColour(LaserColour.Blue);
                }

                else if (satelliteType == SatelliteType.GreenBasicColourFilter) 
                {
                    animator.SetInteger("FilterID",3);
                    colourFilterSatellite.SetFilterColour(LaserColour.Green);
                }

                else if (satelliteType == SatelliteType.YellowBasicColourFilter) 
                {
                    animator.SetInteger("FilterID",4);
                    colourFilterSatellite.SetFilterColour(LaserColour.Yellow);
                }

                else if (satelliteType == SatelliteType.CyanBasicColourFilter) 
                {
                    animator.SetInteger("FilterID",5);
                    colourFilterSatellite.SetFilterColour(LaserColour.Cyan);
                }

                else if (satelliteType == SatelliteType.MagentaBasicColourFilter)
                {
                    animator.SetInteger("FilterID",6);
                    colourFilterSatellite.SetFilterColour(LaserColour.Magenta);
                }
            }
        }
        
        
        else if (satelliteType == SatelliteType.Combiner)
        {
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Combiner-{satelliteNum}-SAT";
                if (satelliteDescription == "") satelliteDescription = "A cutting edge satellite designed to combine two beams of light. Note: The output beam will be stronger and may vary in colour";
                if (satelliteShortDescription == "") satelliteShortDescription = "Combines two beams of light.";
            }

            if (satelliteHealth == 100) satelliteHealth = 400;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 400;
            if (satelliteSellPrice == 0) satelliteSellPrice = 250;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.0f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Combiner;

        }
        else if (satelliteType == SatelliteType.DuelSplitter)
        {
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Duel-Splitter-{satelliteNum}-SAT";
                if (satelliteDescription == "") satelliteDescription = "A must have in satellite communications, it can split a beam of light in two. Note: resulting beams will have less energy and may vary in colour.";
                if (satelliteShortDescription == "") satelliteShortDescription = "Splits a beam of light in two";
            }

            if (satelliteHealth == 100) satelliteHealth = 250;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 350;
            if (satelliteSellPrice == 0) satelliteSellPrice = 250;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Splitter;

        }
        else if (satelliteType == SatelliteType.TrioSplitter)
        {
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Trio-Splitter-{satelliteNum}-SAT";
                if (satelliteDescription == "") satelliteDescription = "An advanced variant of the Duel-Splitter, it can split a beam of light into three. Note: resulting beams will have less energy and may vary in colour";
                if (satelliteShortDescription == "") satelliteShortDescription = "Splits a beam of light into three";
            }

            if (satelliteHealth == 100) satelliteHealth = 250;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 400;
            if (satelliteSellPrice == 0) satelliteSellPrice = 300;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Splitter;
        }
        else if (satelliteType == SatelliteType.HexSplitter)
        {
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Hex-Splitter-{satelliteNum}-SAT";
                if (satelliteDescription == "") satelliteDescription = "A cutting edge prototype splitter, capable of splitting a single beam of light into six output lasers. Note: Resulting beams will have less energy and may vary in colour. ";
                if (satelliteShortDescription == "") satelliteShortDescription = "Splits a beam of light into six";
            }

            if (satelliteHealth == 100) satelliteHealth = 250;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 450;
            if (satelliteSellPrice == 0) satelliteSellPrice = 350;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Splitter;
        }

        // Satellite debris and asteroids are counted here, allowing user's to information about them
        else if (satelliteType == SatelliteType.Debris_Absorb)
        {
            canbeMoved = false;
            canBeSold = false;
            isDebris = true;
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Asteroid";
                if (satelliteDescription == "") satelliteDescription = "Scans indicate this asteroid will absorb all light that hits it. It is recommended to not direct light beams at this satellite.";
                if (satelliteShortDescription == "") satelliteShortDescription = "Absorbs all incoming light";
            }

            
            if (satelliteHealth == 100) satelliteHealth = 300;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 0;
            if (satelliteSellPrice == 0) satelliteSellPrice = 0;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 1f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Absorb;
            
        }
        else if (satelliteType == SatelliteType.Debris_Reflect)
        {
            canbeMoved = false;
            canBeSold = false;
            isDebris = true;
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Asteroid-Reflect";
                if (satelliteDescription == "") satelliteDescription = "Scans indicate this asteroid is partially reflective but will absorb a signficant amount of light. It is advised to not direct light beams at this satellite.";
                if (satelliteShortDescription == "") satelliteShortDescription = "Absorbs most incoming light, reflecting a small amount";
            }

            if (satelliteHealth == 100) satelliteHealth = 300;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 0;
            if (satelliteSellPrice == 0) satelliteSellPrice = 0;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.8f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Reflection;
            
        }
        else if (satelliteType == SatelliteType.Debris_Splitter)
        {
            canbeMoved = false;
            canBeSold = false;
            isDebris = true;
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Asteroid-Splitter";
                if (satelliteDescription == "") satelliteDescription = "Scans indicate this asteroid can split light and absorbs a signficant amount of energy. It is recommended to not direct light beams at this satellite.";
                if (satelliteShortDescription == "") satelliteShortDescription = "Absorbs most incoming light, splitting the remaining light";
            }

            if (satelliteHealth == 100) satelliteHealth = 300;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 0;
            if (satelliteSellPrice == 0) satelliteSellPrice = 0;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.8f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Absorb;
        }


        else if (satelliteType == SatelliteType.CameraDrone)
        {
            canBeSold = false;

            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"The Eye of Zeta";
                if (satelliteDescription == "") satelliteDescription = "The Eye of Zeta provides you with a real time visual feed, allowing you to traverse space within an allowed area. They contain highly classified technology and are astronomically expensive. Do not break it.";
                if (satelliteShortDescription == "") satelliteShortDescription = "Not an item in the shop";
            }
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = $"The Eye of Zeta";
                if (satelliteDescription == "") satelliteDescription = "NOT TRANSLATED";
                if (satelliteShortDescription == "") satelliteShortDescription = "NOT TRANSLATED";
            }
            
            if (satelliteHealth == 100) satelliteHealth = 900000;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 9999;
            if (satelliteSellPrice == 0) satelliteSellPrice = 0;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Absorb;
        }

        else
        {
            // if the satellite is unknown then set it to a default health and have it absorb all light, also notify developer

            Debug.LogWarning("WARNING: Unknown Satellite detected");

            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = "Unknown";
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

    public void DestroyObject()
    {
        _gameController.DestroyedSatellite();

        var satelliteScript = gameObject.GetComponent<SatelliteParent>();
        satelliteScript.DeleteLasers();
    
        Destroy(this.gameObject);
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

        if (opposingSatellite != null)
        {
            // Satellites cannot interact with origin
            if (opposingSatellite.satelliteType == SatelliteType.Origin || opposingSatellite.satelliteType == SatelliteType.SatelliteCreator || opposingSatellite.satelliteType == SatelliteType.CameraDrone ) {}

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


