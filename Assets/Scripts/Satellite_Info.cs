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


    GravitationalAnomaly,


    CameraDrone,

}

public enum SatelliteTypeModifier
{
    Weak,
    SlightlyWeak,
    Middle,
    SlightStrong,
    Strong,
    Indestructible
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

    private int _numberImmunityFrames = 0;
    private int _remainingImmunityFrames = 0;

    private SatelliteTypeModifier satelliteTypeModifier;

    public ParticleSystem statelliteParticleSystem;


    private SatelliteController _satelliteControlsPanel;


    // Start is called before the first frame update
    void Start()
    {
        _language = PersistenceController.GetLanguage(); 
        // Retrieve game controller
        _gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

        // Immunity frames to prevent instant destruction of satellites on collision, includes a brief delay of immunity.
        // As we are dealing with immunity frames, we round down. Hence the use of RoundToInt - which cuts of the decimals
        // so, 3.9999 would be considered 3 by this rounding function. 
        // It's harsh due to it being immunity related, note that black holes are excempt from immunity. It's guarenteed destruction if you 
        // send a satellite into a black hole
        _numberImmunityFrames = Mathf.RoundToInt(_gameController.framerateRelatedSettings.desiredFramerate * advanced_Satellite_Info.numberImmunityFrames);

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
            else if (satelliteType == SatelliteType.CameraDrone || satelliteType == SatelliteType.SatelliteCreator 
            || satelliteType == SatelliteType.GravitationalAnomaly || isDebris)
            {
                // Do Nothing
            }
            else _gameController.worldInfo.numSatellites += 1;
        }

         // If the satellite is not a: Drone, Creator, Destination or blackhole or Origin, then retrieve it's particle system
        if (canbeMoved && satelliteType != SatelliteType.CameraDrone && !satellite_Shop_Info.IsShopItem)
        {
            statelliteParticleSystem = gameObject.GetComponent<ParticleSystem>();
            statelliteParticleSystem.Stop();
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

        if (_remainingImmunityFrames > 0) _remainingImmunityFrames -= 1;

        // if the satellite health falls below or equal to 0, destroy the satellite in a flashy animation
        if (satelliteHealth <= 0 && satelliteTypeModifier != SatelliteTypeModifier.Indestructible && !satellite_Shop_Info.IsShopItem)
        {
            Debug.Log("??");
            Debug.Log(satelliteHealth);

            // Play satellite destruction animation
            // The animation contains a destroy trigger - which when reached will call this DestroyObject function.
            // Forcing a animation delay before destruction.

            var animator = this.gameObject.GetComponent<Animator>();
            animator.SetBool("Destroy",true);


            // If the animation has not already triggered by this point, destroy the satellite and skip the animation
            // This should only occur when satellites do not yet have a destruction animation. 
            if (satelliteHealth <= -100)
            {
                DestroyObject();
            }

        }
    }

    public void RetreiveSatelliteText()
    {
        //// This method serves as the "database" where all satellites have their related information

        // Collect active language - important for defining descriptions
        if (_language == null) _language = _gameController.activeLanguage;
        satelliteHealth = 100;

        if (satelliteType == SatelliteType.Origin)
        {
            canbeMoved = false;
            canBeSold = false;

            satelliteTypeModifier = SatelliteTypeModifier.Indestructible;


            // if origin, name is determined randomly
            if (satelliteName == "") satelliteName = "Prometheus-"+Random.Range(1,384);

            if (satelliteDescription == "" && _language == Language.English) satelliteDescription = "A Type XII Prometheus communication output, designed for deep space communciations it boasts a powerful beam of light to send messages into deep space. This is where your light laser begins.";
            else if (satelliteDescription == "" && _language == Language.Welsh) satelliteDescription = "Mae canolfan gyfathrebu Math XII Prometheus, a gynlluniwyd ar gyfer cyfathrebu gofod dwfn, yn ymfalchïo mewn pelydr pwerus o olau i anfon negeseuon i'r gofod dwfn. Dyma lle mae eich laser golau yn dechrau.";

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


            if (satelliteDescription == "" && _language == Language.English) satelliteDescription = "A Type VI Fyrefly Deep Space Space Station, designed to withstand the harshest conditions in space. Your task is to get the light beam to this station's satellite dish.";
            else if (satelliteDescription == "" && _language == Language.Welsh) satelliteDescription = "Gorsaf Ymchwil Gofod Dwfn Math VI Fyrefly, a gynlluniwyd i wrthsefyll amodau llymaf y gofod. Eich tasg yw cael y pelydr golau i ddysgl lloeren yr orsaf hon.";

            if (satelliteShortDescription == "" && _language == Language.English) satelliteShortDescription = "Not an Item in the Shop";
            else if (satelliteShortDescription == "" && _language == Language.Welsh) satelliteShortDescription = "NOT YET TRANSLATED";

            satelliteTypeModifier = SatelliteTypeModifier.Indestructible;
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


            if (satelliteDescription == "" && _language == Language.English) satelliteDescription = "Elysia is a cutting edge Elysian Matter Printer. One of the three in existence. It makes use of space-time manipulator lasers and drones to construct satellites of any type.";
            else if (satelliteDescription == "" && _language == Language.Welsh) satelliteDescription = "Mae Elysia yn Argraffydd Mater Elysian arloesol. Un o dri sy'n bodoli, mae'n defnyddio laserau a dronau manipulator gofod-amser i adeiladu lloerennau o unrhyw fath";

            if (satelliteShortDescription == "" && _language == Language.English) satelliteShortDescription = "Not an Item in the Shop";
            else if (satelliteShortDescription == "" && _language == Language.Welsh) satelliteShortDescription = "NOT YET TRANSLATED";

            satelliteTypeModifier = SatelliteTypeModifier.Indestructible;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 12000000;
            if (satelliteSellPrice == 0) satelliteSellPrice = 12000000;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0;
            interaction = Interaction.Absorb;
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
                if (satelliteName == "") satelliteName = $"Llygad Zeta";
                if (satelliteDescription == "") satelliteDescription = "Mae Llygad Zeta yn darparu porthiant gweledol amser real i chi, gan eich galluogi i groesi gofod o fewn ardal a ganiateir. Maent yn cynnwys technoleg hynod ddosbarthedig ac yn seryddol ddrud. Peidiwch â'i dorri";
                if (satelliteShortDescription == "") satelliteShortDescription = "NOT TRANSLATED";
            }

            satelliteTypeModifier = SatelliteTypeModifier.Indestructible;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 9999;
            if (satelliteSellPrice == 0) satelliteSellPrice = 0;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Absorb;
        }

        
        else if (satelliteType == SatelliteType.SingleSideReflector)
        {
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Reflect-Single-LAM-SAT";
                if (satelliteDescription == "") satelliteDescription = "A high grade reflectance satellite. The surface has no indents and is perfectly flat, providing optimal reflection of light, a true lambertian diffuse satellite.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A single surface reflection satellite.";
            }
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "SAT-Adlewyrchu-Sengl-LAM";
                if (satelliteDescription == "") satelliteDescription ="Lloeren adlewyrchiad gradd uchel. Nid oes gan yr wyneb unrhyw fewnoliadau ac mae'n berffaith wastad, gan ddarparu adlewyrchiad gorau posibl o olau, lloeren gwasgaredig lambertian go iawn";
                if (satelliteShortDescription == "") satelliteShortDescription = "Lloeren adlewyrchu un arwyneb";
            }


            satelliteTypeModifier = SatelliteTypeModifier.Middle;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 200;
            if (satelliteSellPrice == 0) satelliteSellPrice = 100;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.1f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Reflection;
            
        }
        else if (satelliteType == SatelliteType.DoubleSideReflector)
        {
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Reflect-Duo-LAM-SAT";
                if (satelliteDescription == "") satelliteDescription = "A high grade reflectance satellite. Upgraded from it's predecessor, this includes two panels for lambertian reflection.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A two surface reflection satellite.";
            }
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "SAT-Adlewyrchu-Deuawd-LAM";
                if (satelliteDescription == "") satelliteDescription ="Lloeren adlewyrchiad gradd uchel. Uwchradd o'i ragflaenydd, mae hyn yn cynnwys dau banel ar gyfer adlewyrchiad lambertian";
                if (satelliteShortDescription == "") satelliteShortDescription = "Lloeren adlewyrchiad dau wyneb";
            }
            satelliteTypeModifier = SatelliteTypeModifier.SlightStrong;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 225;
            if (satelliteSellPrice == 0) satelliteSellPrice = 150;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.1f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Reflection;

        }
        else if (satelliteType == SatelliteType.GlassRefractor)
        {
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Refract-GL-SAT";
                if (satelliteDescription == "") satelliteDescription = "A high grade satellite designed for refracting light. The material is Glass and has a refractive index of 1.52, passing a laser through this object will alter the angle to a small degree.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A glass refraction satellite.";
            }
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "SAT-Plygu-GL";
                if (satelliteDescription == "") satelliteDescription ="Lloeren gradd uchel wedi'i chynllunio ar gyfer plygiannu golau. Mae'r deunydd yn wydr ac mae ganddo indecs plygiannol o 1.52, bydd pasio laser trwy'r gwrthrych hwn yn newid yr ongl i raddau bach";
                if (satelliteShortDescription == "") satelliteShortDescription = "Lloeren plygiant gwydr";
            }

            satelliteTypeModifier = SatelliteTypeModifier.Weak;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 200;
            if (satelliteSellPrice == 0) satelliteSellPrice = 150;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 1.52f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.033f;
            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Refraction;
        }
        else if (satelliteType == SatelliteType.SapphireRefractor){
             if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Refract-SAP-SAT";
                if (satelliteDescription == "") satelliteDescription = "A high grade satellite designed for refracting light. The material is crystal sapphire and has a refractive index of 1.78, passing a laser through this object will alter the angle to a large degree.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A sapphire refraction satellite";
            }
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "SAT-Plygu-SAP";
                if (satelliteDescription == "") satelliteDescription ="Lloeren gradd uchel wedi'i chynllunio ar gyfer plygiannu golau. Mae'r deunydd yn saffir grisial ac mae ganddo indecs plygiannol o 1.78, bydd pasio laser trwy'r gwrthrych hwn yn newid yr ongl i raddau helaeth. Mae'r saffir ei hun yn synthetig, wedi'i gynllunio i gynnal ei eglurder heb ei lliw unigryw.";
                if (satelliteShortDescription == "") satelliteShortDescription = "Lloeren plygiant saffir";
            }

            satelliteTypeModifier = SatelliteTypeModifier.Strong;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 300;
            if (satelliteSellPrice == 0) satelliteSellPrice = 250;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 1.78f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.033f;
            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Refraction;
        }
        else if (satelliteType == SatelliteType.SiliconRefractor){
             if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Refract-S-SAT";
                if (satelliteDescription == "") satelliteDescription = "A high grade satellite designed for refracting light. The material is silicon and has a refractive index of 3.4, passing a laser through this object will alter the angle to a large degree.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A silicon refraction satellite";
            }
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "SAT-Plygu-S";
                if (satelliteDescription == "") satelliteDescription ="Lloeren gradd uchel wedi'i chynllunio ar gyfer plygiannu golau. Mae'r deunydd yn synthesis arbenigol o silicon ac mae ganddo indecs plygiannol o 3.4, bydd pasio laser trwy'r gwrthrych hwn yn newid yr ongl i raddau helaeth.";
                if (satelliteShortDescription == "") satelliteShortDescription = "Lloeren plygiant silicon";
            }

            satelliteTypeModifier = SatelliteTypeModifier.SlightStrong;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 250;
            if (satelliteSellPrice == 0) satelliteSellPrice = 200;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 3.4f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.033f;
            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Refraction;
        }
        else if (satelliteType == SatelliteType.WaterRefractor){
             if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Refract-H2O-SAT";
                if (satelliteDescription == "") satelliteDescription = "A cutting edge prototype satellite, capable of maintaining water in a liquid space in deep space and keeping it contained. It has a refractive index of 1.33, passing a laser through this object will alter the angle to a small degree. ";
                if (satelliteShortDescription == "") satelliteShortDescription = "A water refraction satellite.";
            }
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "SAT-Plygu-H2O";
                if (satelliteDescription == "") satelliteDescription ="Lloeren prototeip arloesol, sy'n gallu cynnal dŵr mewn gofod hylif yn y gofod dwfn a'i gadw. Mae ganddo indecs plygiannol o 1.33, bydd pasio laser trwy'r gwrthrych hwn yn newid yr ongl i raddau bach.";
                if (satelliteShortDescription == "") satelliteShortDescription = "Lloeren plygiant dŵr";
            }

            satelliteTypeModifier = SatelliteTypeModifier.Weak;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 375;
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
                if (satelliteName == "") satelliteName = $"Colour-Filter-SAT";
                if (satelliteDescription == "") satelliteDescription = "A simple colour filter. This will change the colour of light beams that pass through it. Cheaply made, not too durable but suitable for the task.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A fixed coloured filter";
            }
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "SAT-Hidlydd-Lliw";
                if (satelliteDescription == "") satelliteDescription ="Hidlydd lliw syml. Bydd hyn yn newid lliw pelydrau golau sy'n pasio drwyddo. Wedi'i wneud yn rhad, ddim yn rhy gadarn ond yn addas ar gyfer y dasg";
                if (satelliteShortDescription == "") satelliteShortDescription = "Hidlydd lliw sefydlog";
            }


            satelliteTypeModifier = SatelliteTypeModifier.Weak;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 100;
            if (satelliteSellPrice == 0) satelliteSellPrice = 75;
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
        else if (satelliteType == SatelliteType.CustomColourFilter)
        {
           if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Custom-Colour-Filter-SAT";
                if (satelliteDescription == "") satelliteDescription = "A high grade colour filter, allows swapping between up to 7 colours on the fly. Suitable for all tasks.";
                if (satelliteShortDescription == "") satelliteShortDescription = "A customisable colour filter";
            }
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "SAT-Hidlydd-Lliw-Addasedig";
                if (satelliteDescription == "") satelliteDescription ="Hidlydd lliw gradd uchel, yn caniatáu cyfnewid rhwng hyd at 7 lliw ar y hedfan. Addas ar gyfer pob tasg.";
                if (satelliteShortDescription == "") satelliteShortDescription = "Hidlydd lliw y gellir ei addasu";
            }

            satelliteTypeModifier = SatelliteTypeModifier.Strong;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 350;
            if (satelliteSellPrice == 0) satelliteSellPrice = 200;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.1f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.ColourFilter; 
        }
        
        else if (satelliteType == SatelliteType.Combiner)
        {
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Combiner-SAT";
                if (satelliteDescription == "") satelliteDescription = "A cutting edge satellite designed to combine two beams of light. Note: The output beam will be stronger and may vary in colour";
                if (satelliteShortDescription == "") satelliteShortDescription = "Combines two beams of light.";
            }
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "SAT-Cyfuno";
                if (satelliteDescription == "") satelliteDescription ="Lloeren flaengar wedi'i chynllunio i gyfuno dau belydryn o olau. Nodyn: Bydd y pelydr allbwn yn gryfach a gall amrywio o ran lliw.";
                if (satelliteShortDescription == "") satelliteShortDescription = "Cyfuno dau belydryn o olau";
            }

            satelliteTypeModifier = SatelliteTypeModifier.SlightlyWeak;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 400;
            if (satelliteSellPrice == 0) satelliteSellPrice = 275;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.0f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Combiner;

        }
        else if (satelliteType == SatelliteType.DuelSplitter)
        {
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Duel-Splitter-SAT";
                if (satelliteDescription == "") satelliteDescription = "A must have in satellite communications, it can split a beam of light in two. Note: resulting beams will have less energy and may vary in colour.";
                if (satelliteShortDescription == "") satelliteShortDescription = "Splits a beam of light in two";
            }
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "SAT-Holltydd-Deuol";
                if (satelliteDescription == "") satelliteDescription ="Yn ddefnyddiol iawn mewn cyfathrebu lloeren, gall rannu pelydr o olau yn ddau. Nodyn: Bydd gan pelydrau sy'n deillio o hyn lai o egni a gallant amrywio o ran lliw";
                if (satelliteShortDescription == "") satelliteShortDescription = "Rhannu pelydr o olau yn ddwy";
            }
            satelliteTypeModifier = SatelliteTypeModifier.Middle;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 300;
            if (satelliteSellPrice == 0) satelliteSellPrice = 225;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Splitter;

        }
        else if (satelliteType == SatelliteType.TrioSplitter)
        {
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Trio-Splitter-SAT";
                if (satelliteDescription == "") satelliteDescription = "An advanced variant of the Duel-Splitter, it can split a beam of light into three. Note: resulting beams will have less energy and may vary in colour";
                if (satelliteShortDescription == "") satelliteShortDescription = "Splits a beam of light into three";
            }
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "SAT-Holltydd-Triawd";
                if (satelliteDescription == "") satelliteDescription ="Amrywiad datblygedig o'r Holltydd-Deuawd, gall rannu pelydr o olau yn dri. Nodyn: bydd gan pelydrau sy'n deillio o hyn lai o egni a gallant amrywio o ran lliw.";
                if (satelliteShortDescription == "") satelliteShortDescription = "Rhannu pelydr o olau yn dri";
            }
            satelliteTypeModifier = SatelliteTypeModifier.Middle;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 375;
            if (satelliteSellPrice == 0) satelliteSellPrice = 300;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Splitter;
        }
        else if (satelliteType == SatelliteType.HexSplitter)
        {
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Hex-Splitter-SAT";
                if (satelliteDescription == "") satelliteDescription = "A cutting edge prototype splitter, capable of splitting a single beam of light into six output lasers. Note: Resulting beams will have less energy and may vary in colour. ";
                if (satelliteShortDescription == "") satelliteShortDescription = "Splits a beam of light into six";
            }
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "SAT-Holltydd-Hecs";
                if (satelliteDescription == "") satelliteDescription ="Holltydd prototeip blaengar, sy'n gallu rhannu un pelydr o olau yn chwe laser allbwn. Nodyn: Bydd gan pelydrau sy'n deillio o hyn lai o egni a gallant amrywio o ran lliw";
                if (satelliteShortDescription == "") satelliteShortDescription = "Rhannu pelydr o olau yn chwech";
            }
            satelliteTypeModifier = SatelliteTypeModifier.SlightlyWeak;
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
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "Asteroid";
                if (satelliteDescription == "") satelliteDescription ="Mae sganiau'n dangos y bydd yr asteroid hwn yn amsugno'r holl olau sy'n ei daro. Argymhellir peidio â chyfeirio pelydrau golau at y lloeren hon.";
                if (satelliteShortDescription == "") satelliteShortDescription = "";
            }

            satelliteTypeModifier = SatelliteTypeModifier.SlightStrong;
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
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "Asteroid Adlewyrchydd";
                if (satelliteDescription == "") satelliteDescription ="Mae sganiau'n dangos bod yr asteroid hwn yn rhannol adlewyrchol ond bydd yn amsugno llawer iawn o olau. Cynghorir peidio â chyfeirio pelydrau golau at y lloeren hon";
                if (satelliteShortDescription == "") satelliteShortDescription = "";
            }

            satelliteTypeModifier = SatelliteTypeModifier.Middle;
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
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "Asteroid Holltydd";
                if (satelliteDescription == "") satelliteDescription ="Mae sganiau'n dangos y gall yr asteroid hwn rannu golau ac amsugno swm sylweddol o egni. Argymhellir peidio â chyfeirio pelydrau golau at y lloeren hon.";
                if (satelliteShortDescription == "") satelliteShortDescription = "";
            }

            satelliteTypeModifier = SatelliteTypeModifier.Middle;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 0;
            if (satelliteSellPrice == 0) satelliteSellPrice = 0;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0.8f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Absorb;
        }

        else if (satelliteType == SatelliteType.GravitationalAnomaly)
        {
            canbeMoved = false;
            canBeSold = false;
            isDebris = true;
            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = $"Gravitational Anomaly";
                if (satelliteDescription == "") satelliteDescription = "WARNING! Scans indicate significant gravitational disturbance within this region. Satellites entering this region may be lost! It may have an unexpected interference when light passes near it….";
                if (satelliteShortDescription == "") satelliteShortDescription = "";
            }
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "Anghysondeb Disgyrchiant";
                if (satelliteDescription == "") satelliteDescription ="RHYBUDD! Mae sganiau yn dangos aflonyddwch disgyrchiant sylweddol yn y rhanbarth hwn. Efallai y bydd lloerennau sy'n mynd i mewn i'r rhanbarth hwn yn cael eu colli! Efallai y bydd ganddo ymyrraeth annisgwyl pan fydd golau yn pasio yn agos ato...";
                if (satelliteShortDescription == "") satelliteShortDescription = "";
            }

            satelliteTypeModifier = SatelliteTypeModifier.Indestructible;
            if (satelliteHealth == 100) satelliteHealth = 9999999;
            if (satellitePurchasePrice == 0) satellitePurchasePrice = 0;
            if (satelliteSellPrice == 0) satelliteSellPrice = 0;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 0f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0f;

            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.GravitationalAnomaly;
        }

        
        else
        {
            // if the satellite is unknown then set it to a default health and have it absorb all light, also notify developer

            Debug.LogWarning("WARNING: Unknown Satellite detected");

            if (_language == Language.English)
            {
                if (satelliteName == "") satelliteName = "Unknown";

                if (satelliteDescription == "") satelliteDescription = "An unknown satellite with unknown interactions with light. Be cautious.";
                if (satelliteShortDescription == "") satelliteShortDescription = "Unknown satellite";
            }
            else if (_language == Language.Welsh)
            {
                if (satelliteName == "") satelliteName = "";
                if (satelliteDescription == "") satelliteDescription ="";
                if (satelliteShortDescription == "") satelliteShortDescription = "";
            }

            if (satellitePurchasePrice == 0) satellitePurchasePrice = 100;
            if (satelliteSellPrice == 0) satelliteSellPrice = 100;
            if (advanced_Satellite_Info.refractiveIndex == 0f) advanced_Satellite_Info.refractiveIndex = 1f;
            if (advanced_Satellite_Info.absorbance == 0) advanced_Satellite_Info.absorbance = 0;
            if (interaction == null || interaction == Interaction.SelfDetermine) interaction = Interaction.Absorb;
        }
    }


    public void CreateShopItem()
    {
        // Get common component across children
        var childrenTransformList = gameObject.GetComponentsInChildren<RectTransform>();

        // Execute creation on all child transform
        foreach(RectTransform childTransform in childrenTransformList)
        {
            var childObject = childTransform.gameObject;

            
            // Remove Clone brackets from the new child objects - more of a personal preference thing
            //childObject.name = childObject.name.Replace("(Clone)","");


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

    public void PurchaseSatellite()
    {
        _gameController.PurchaseSatellite(this);
    }

    public void DestroyObject()
    {
        _gameController.DestroyedSatellite();

        var satelliteScript = gameObject.GetComponent<SatelliteParent>();
        satelliteScript.DeleteLasers();

        // Check to see if the satellite being destroyed has the Eye of Zeta attached, if so remove the attachment
        CameraDrone eyeOfZeta = gameObject.GetComponentInChildren<CameraDrone>();
        if (eyeOfZeta != null) eyeOfZeta.DetachDroneFromSatellite();
    
        Destroy(this.gameObject);
    }

    private void DamageSatellite(Collision2D collision = null, Collider2D collider = null)
    {  

        // When collision occurs with the game object, decrease the health of this and the colliders satellite.
        // There is no guarentee that the hit object is a satellite.

        GameObject colliderObject;

        if (collision != null) colliderObject = collision.gameObject;
        else colliderObject = collider.gameObject;

        // Prepare variable in case satellite on satellite collision occurred.
        Satellite_Info opposingSatellite = null;

        // Try to get satellite info of the object, may not be possible if it's an asteroid or boundary
        colliderObject.TryGetComponent<Satellite_Info>(out opposingSatellite);

        // If the remainingImmunityDuration is more 0, negate one from it. Otherwise take damage
        if (_remainingImmunityFrames <= 0 && opposingSatellite != null)
        {
            var baseDamage = _gameController.baseDamage;

            var typeModifier = 1.0f;

            if (satelliteTypeModifier == SatelliteTypeModifier.Weak) typeModifier = 3.0f;
            else if (satelliteTypeModifier == SatelliteTypeModifier.SlightlyWeak) typeModifier = 2.0f; 
            else if (satelliteTypeModifier == SatelliteTypeModifier.Middle) typeModifier = 1.0f;
            else if (satelliteTypeModifier == SatelliteTypeModifier.SlightStrong) typeModifier = 0.75f;
            else if (satelliteTypeModifier == SatelliteTypeModifier.Strong) typeModifier = 0.5f;

            // This prevents damage from destinations, creators, origins, etc
            else typeModifier = 0.0f;


            var speedModifier = 1.0f;
            var currentSpeed = 1.0f;
            var maxSpeed = 1.0f;

            if (_satelliteControlsPanel != null)
            {
                currentSpeed = _satelliteControlsPanel.currentMovementMultiplier;
                maxSpeed = satellite_Movement_Info.maxRotationMultiplier;
            }
            else if (_gameController.GetUIController().uiExpectations.expectSatelliteControlPanel) 
            {
                _satelliteControlsPanel = GameObject.FindGameObjectsWithTag("SatelliteControlsPanel")[0].GetComponent<SatelliteController>();
            }


            if (currentSpeed / maxSpeed  == 1) speedModifier = 1.75f;
            else if (currentSpeed / maxSpeed  >= 80) speedModifier = 1.5f;
            else if (currentSpeed / maxSpeed  >= 60) speedModifier = 1.25f;
            else if (currentSpeed / maxSpeed  >= 40) speedModifier = 1.0f;
            else if (currentSpeed / maxSpeed  >= 20) speedModifier = 0.75f;
            else speedModifier = 0.5f;

            // If the other satellite type is industructable, don't do damage to either.
            if (opposingSatellite.satelliteTypeModifier != SatelliteTypeModifier.Indestructible)
            {
                satelliteHealth = (int)(satelliteHealth - (baseDamage * typeModifier * speedModifier));
            }


            _remainingImmunityFrames = _numberImmunityFrames;
        }
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        DamageSatellite(collision);
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        DamageSatellite(collision);
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Singularity" && this.gameObject.tag != "Singularity" && !(collider is BoxCollider2D))
        {
            var animator = this.gameObject.GetComponent<Animator>();
            animator.SetBool("Destroy",true);
        }
        else DamageSatellite(null,collider);
        
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag == "Singularity" && this.gameObject.tag != "Singularity" && !(collider is BoxCollider2D))
        {
            // Do nothing, it should be either in the process of being destroyed, or has been destroyed
        }
        else DamageSatellite(null,collider);
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

    public float numberImmunityFrames = 1;

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


