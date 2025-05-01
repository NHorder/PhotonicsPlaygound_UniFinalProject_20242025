using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


/// <summary>
/// Enum to restrict record type
/// </summary>
public enum RecordType
{
    GeneralInformation,
    Light,
    Reflection,
    Refraction,
    FresnelEquations,
    Colour,
    Blackholes
}

public class Record : MonoBehaviour
{
    /// <summary>
    /// Class to create and hold text for records within the Atheneaum
    /// </summary>

    public RecordType recordType;
    private string _englishTitle;
    private string _englishText;
    private string _welshTitle;
    private string _welshText;

    private TMP_Text[] _contentElements;

    // Start is called before the first frame update
    /// <summary>
    /// Initialisation Method
    /// </summary>
    void Start()
    {
        _contentElements = gameObject.GetComponentsInChildren<TMP_Text>();

        UpdateLanguage(PersistenceController.GetLanguage());
    }

    /// <summary>
    /// Method to update text based on record type and language
    /// </summary>
    private void GetText()
    {

        if (recordType == RecordType.GeneralInformation)
        {
            _englishTitle = "General Information";
            _englishText = "Goal of the Game\n\n\nThe goal of this game is to redirect the light from Prometheus to the corresponding Fyrefly satellite. Take note of the colour of both satellites, as Fyrefly will only respond to specific colours of light - White Fyrefly satellites respond to all light colours. \n\n\nGame Controls:\n\n\nMake use of the cursor to select satellites and the various objects within the scene, then use WASD to move selected satellites, QE to rotate. Or make use of the satellite control panel located at the bottom of the screen.\n\n\nEyes of Zeta\n\n\nThe Eye of Zeta is your view of the map, moving this satellite around will allow you to see different parts of the map, additionally it will move alongside any selected satellite.\n\n\nLevel Information\n\n\nThis describes the main purpose of the level, and has a settings and reset button should you need it.\n\n\nThe Shop:\n\n\nThe Shop is the basket icon on the lower right side of your screen, selecting this icon will open the shop, select a category and scroll to find the satellite you wish to purchase. Your budget is located at the top of the shop panel. Purchasing a satellite will send a request to Elysia to create it. Once created the satellite can be moved from Elysia’s printing bay anywhere you want it to.\n\n\nCommunications\n\n\nLocated in the top left of your screen, the communications panel displays all communications with Elysia and Fyrefly satellites as well as showing level progress. This panel will automatically open whenever you receive communications from these satellites, however this can be disabled in the settings menu. \n\n";
            _welshTitle = "Gwybodaeth Gyffredinol";
            _welshText = "Nod y Gêm\n\n\nNod y gêm hon yw ailgyfeirio'r golau o Prometheus i'r lloeren Fyrefly gyfatebol. Cymerwch sylw o liw y ddwy loeren, gan y bydd Fyrefly yn ymateb i liwiau penodol o olau yn unig - mae lloerennau Fyrefly Gwyn yn ymateb i bob lliw golau.\n\n\nRheolaethau Gêm:\n\n\nDefnyddiwch y cyrchwr i ddewis lloerennau a'r gwahanol wrthrychau o fewn yr olygfa, yna defnyddio WASD i symud lloerennau dewisedig, QE i gylchdroi. Neu defnyddiwch y panel rheoli lloeren sydd wedi'i leoli ar waelod y sgrin.\n\n\nLlygaid Zeta\n\n\nLlygad Zeta yw eich golwg o'r map, bydd symud y lloeren hon o gwmpas yn eich galluogi i weld gwahanol rannau o'r map, yn ychwanegol bydd yn symud ochr yn ochr ag unrhyw loeren a ddewiswyd.\n\nGwybodaeth Lefel\n\n\nMae hyn yn disgrifio prif bwrpas y lefel, ac mae ganddo fotwm gosodiadau ac ailosod os oes ei angen arnoch.\n\n\nY Siop:\n\n\nY Siop yw'r eicon basged ar ochr dde isaf eich sgrin, bydd dewis yr eicon hwn yn agor y siop, dewiswch gategori a sgroliwch i ddod o hyd i'r lloeren rydych chi am ei brynu. Mae eich cyllideb wedi'i lleoli ar frig panel y siop. Bydd prynu lloeren yn anfon cais at Elysia i'w chreu. Ar ôl ei chreu, gellir symud y lloeren o fae argraffu Elysia unrhyw le rydych chi ei eisiau.\n\n\nCyfathrebiadau\n\n\nWedi'i leoli ar y chwith uchaf o'ch sgrin, mae'r panel cyfathrebu yn arddangos yr holl gyfathrebu â lloerennau Elysia a Fyrefly yn ogystal â dangos cynnydd lefel. Bydd y panel hwn yn agor yn awtomatig pryd bynnag y byddwch yn derbyn cyfathrebiadau gan y lloerennau hyn, ond gellir analluogi hyn yn y ddewislen gosodiadau.";
        }
        else if (recordType == RecordType.Light)
        {
            _englishTitle = "Light: How it works (Simplified)";
            _englishText = "Visible light describes the range of which light can be seen by humans, often resulting in numerous colours. \n\nLight itself consists of photons, small packets of energy that follow a wave pattern. Photons are often used to describe light and can provide insights into how light interacts with different materials.\n\nReferences:\n\thttps://en.wikipedia.org/wiki/Light \n\t- https://www.bbc.co.uk/bitesize/topics/z3nnb9q ";
            _welshTitle = "Golau: Sut mae'n gweithio (Wedi’u symleiddio)";
            _welshText = "Mae golau gweladwy yn disgrifio'r ystod y gall golau gael ei weld gan bobl, yn aml yn arwain at nifer o liwiau. Mae golau ei hun yn cynnwys ffotonau, pecynnau bach o egni sy'n dilyn patrwm tonnau. Defnyddir ffotonau yn aml i ddisgrifio golau a gallant ddarparu mewnwelediadau i sut mae golau yn rhyngweithio â gwahanol ddeunyddiau. Cyfeirnodau:\n\thttps://en.wikipedia.org/wiki/Light \n\t- https://www.bbc.co.uk/bitesize/topics/z3nnb9q";
        }
        else if (recordType == RecordType.Reflection)
        {
            _englishTitle = "Reflection: How it works (Simplified)";
            _englishText = "Reflection of light in this game follows a type of reflection called “specular reflection”. This type of reflection states that light hitting a surface is reflected at the same angle as the incoming light, in relation to the orientation (the normal) of a surface.\n\n\n\n\n\n\n\n\n\n\n\n\n\nReflection Diagram\n\n\nIn the above diagram, the light hits the mirror at the angle i, as such it would leave at the same angle, r. \n\nAs this is reflected on a mirror, some of the light’s energy is absorbed by the mirror, resulting in the reflected light being slightly weaker. This is why light becomes darker after many many reflections.\n\nReferences\n\t- https://en.wikipedia.org/wiki/Reflection_(physics) \n\t- https://www.bbc.co.uk/bitesize/topics/z3nnb9q/articles/zy34r2p";
            _welshTitle = "Adlewyrchiad: Sut mae'n gweithio (Wedi’u symleiddio)";
            _welshText = "Mae adlewyrchiad golau yn y gêm hon yn dilyn math o adlewyrchiad o'r enw 'adlewyrchiad speciwla'. Mae'r math hwn o adlewyrchiad yn nodi bod golau sy'n taro arwyneb yn cael ei adlewyrchu ar yr un ongl â'r golau sy'n dod i mewn, mewn perthynas â chyfeiriadedd (y normal) o arwyneb.\n\n\n\n\n\n\n\n\n\n\n\n\n\nDiagram Adlewyrchiant\n\n\nYn y diagram uchod, mae'r golau yn taro'r drych ar yr ongl i, felly byddai'n gadael ar yr un ongl,r.\n\nGan fod hyn yn cael ei adlewyrchu ar ddrych, mae rhywfaint o egni'r golau yn cael ei amsugno gan y drych, gan arwain at y golau a adlewyrchir ychydig yn wannach. Dyma pam mae golau yn dod yn dywyllach ar ôl llawer o fyfyrdodau.\n\nCyfeirnodau\n\t- https://en.wikipedia.org/wiki/Reflection_(physics) \n\t- https://www.bbc.co.uk/bitesize/topics/z3nnb9q/articles/zy34r2p";
        }
        else if (recordType == RecordType.Refraction)
        {
            _englishTitle = "Refraction: How it works (Simplified)";
            _englishText = "When light enters a material it is redirected, as there is a difference in the number of particles. A good comparison is walking, walking through the air is easy, walking the water is much harder. It’s similar for light, and is instead shown through a change in angle. \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nRefraction Diagram: \n\nAs shown in the above diagram, the angle of this change can be calculated through the use of the equation below. A different way of understanding would be viewing everything as in something, for example we are “in” air, just as light is. Meaning when light hits glass it hits the boundary between air and glass. \n\n\nn1 sin(Angle I) = n2 sin(Angle R)\n\n  - Angle i is the angle of incidence, the angle in which the light hits the boundary of the material.\n-  Angle r is the angle of refraction, the angle in which the light is redirected.\n  - n1 Is the refractive index of the first material, I.e air.\n  - n2 is the refractive index of the second material, I.e glass\n\nRefraction also occurs when light leaves a material, I.e glass -> air. \n\nThe game takes place within a vacuum that has a refractive index of 1, materials used are glass (refractive index of 1.52), silicon (refractive index of 3.4), sapphire (refractive index of 1.78) and water (refractive index of 1.33).\n\nReferences:\n\t- https://en.wikipedia.org/wiki/Refraction\n\thttps://en.wikipedia.org/wiki/Snell%27s_law\n\t-https://en.wikipedia.org/wiki/List_of_refractive_indices ";
            _welshTitle = "Plygiant: Sut mae'n gweithio (Wedi’u symleiddio)";
            _welshText = "Pan fydd golau yn mynd i mewn i ddeunydd mae'n cael ei ailgyfeirio, gan fod gwahaniaeth yn nifer y gronynnau. Cymhariaeth dda yw cerdded, mae cerdded drwy'r awyr yn hawdd, ond mae cerdded y dŵr yn llawer anoddach. Mae'n debyg ar gyfer golau, ac yn hytrach yn cael ei ddangos trwy newid mewn ongl.\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nDiagram Plygian\n\nFel y dangosir yn y diagram uchod, gellir cyfrifo ongl y newid hwn trwy ddefnyddio'r hafaliad isod. Ffordd wahanol o ddeall fyddai gweld popeth fel mae e tu fewn i rhywbeth, er enghraifft rydyn ni'n 'mewn' yr awyr, yn union fel y mae golau. Pan fydd golau yn taro gwydr mae'n taro'r ffin rhwng aer a gwydr.\n\nn1 sin(Ongl I) = n2 sin(Ongl R)\n\n  - Ongle i yw’r ongl drawiad, yr ongl lle mae'r golau yn taro ffin y deunydd\n - Ongle R yw’r ongl plygiant, yr ongl y mae'r golau yn cael ei ailgyfeirio ynddi.\n - n1 yw indecs plygiannol y deunydd cyntaf, h.y. aer.\n - n2 yw indecs plygiannol yr ail ddeunydd, h.y. gwydr\n\nMae plygiant hefyd yn digwydd pan fydd golau yn gadael deunydd, h.y. gwydr > aer\n\nMae'r gêm yn digwydd o fewn gwactod sydd â indecs plygiannol o 1, Deunyddiau a ddefnyddir yw gwydr (indecs plygiannol o 1.52), silicon (indecs plygiannol o 3.4), saffir (indecs plygiannol o 1.78) a dŵr (indecs plygiannol o 1.33).\n\nCyfeirnodau\n\t- https://en.wikipedia.org/wiki/Refraction\n\thttps://en.wikipedia.org/wiki/Snell%27s_law\n\t-https://en.wikipedia.org/wiki/List_of_refractive_indices ";
        }
        else if (recordType == RecordType.FresnelEquations)
        {
            _englishTitle = "Fresnel Equations: How it works (Simplified) (Advanced Interaction)";
            _englishText = "Fresnel Equations describe the particular case of light being partially reflected during the process of refraction. However, the strength of this light is very low (about 4% for glass!). As such, it has been included in the game as an “advanced interaction”. \n\nThe Fresnel Equation used in this game is:\n\nR0 = | (n1-n2) / (n1+n2) | ^2\n\n - n1 is the refractive index of the first material, I.e air\n- n2 is the refractive index of the second material, i.e glass\n\nThe above equation is used to calculate the power of the reflected light, as there is a small number of refractive materials. We then follow the reflection rule to calculate the angle of reflection\n\nReferences:\n\t- https://en.wikipedia.org/wiki/Fresnel_equations \n\t- https://en.wikipedia.org/wiki/Fresnel_equations#Normal_incidence ";
            _welshTitle = "Hafaliadau Fresnel: Sut mae'n gweithio (Wedi’u symleiddio) (Rhyngweithiad Uwch)";
            _welshText = "Mae Hafaliadau Fresnel yn disgrifio'r achos penodol o olau yn cael ei adlewyrchu'n rhannol yn ystod y broses o plygiant. Ond, mae cryfder y golau hwn yn isel iawn (tua 4% ar gyfer gwydr!). Felly, mae wedi cael ei gynnwys yn y gêm fel 'rhyngweithiad datblygedig'.\n\nYr Hafaliad Fresnel a ddefnyddir yn y gêm hon yw::\n\nR0 = | (n1-n2) / (n1+n2) | ^2\n\n - n1 yw indecs plygiannol y deunydd cyntaf, h.y. aer.\n\n- n2 yw indecs plygiannol yr ail ddeunydd, h.y. gwydr\n\nDefnyddir yr hafaliad uchod i gyfrifo pŵer y golau a adlewyrchir, gan fod nifer fach o ddeunyddiau plygiannol. Yna rydym yn dilyn y rheol adlewyrchiad i gyfrifo'r ongl adlewyrchiad\n\nCyfeirnodau:\n\t- https://en.wikipedia.org/wiki/Fresnel_equations \n\t- https://en.wikipedia.org/wiki/Fresnel_equations#Normal_incidence ";
        }
        else if (recordType == RecordType.Colour)
        {
            _englishTitle = "Colour: How it works (Simplified)";
            _englishText = "Light can be many colours and many people have developed many ways of representing this. In this game, however, it is represented through 7 colours: White, red, blue, green, yellow, cyan and magenta. \n\nWhite light contains all possible colours, as such it can be split into each component. This can be done as each colour of light has a particular wavelength, longer waves produce a more red colour, shorter waves produce more blue, with green lying somewhere in the middle. \n\nSplitting a white light into its various colours is natural and can be done through the use of coloured surfaces. If a white light hits a red surface, it will reflect red light! \n\nThe satellites in this game, the splitters, handle this colour splitting for you, allowing you a range of colours to play with. This can also be done through the use of prisms and refractions.\n\nColours can also be combined, creating unique combinations. For example, overlapping a red and blue light (under the right circumstances) can create the magenta (pink) light. Green and red to make yellow,  green and blue to make cyan. \n\n\n\n\n\n\n\n\n\n\n\n\n\nColour Chart\n\nReferences\n-  https://en.wikipedia.org/wiki/Color\n-  https://en.wikipedia.org/wiki/Color_space\n-  https://science.nasa.gov/ems/09_visiblelight/\n-  https://www.bbc.co.uk/bitesize/guides/z7ftnrd/revision/6";
            _welshTitle = "Lliw: Sut mae'n gweithio (Wedi’u symleiddio)";
            _welshText = "Gall golau fod yn llawer o liwiau ac mae llawer o bobl wedi datblygu sawl ffordd o gynrychioli hyn. Yn y gêm hon, mae'n cael ei gynrychioli trwy 7 lliw: Gwyn, coch, glas, gwyrdd, melyn, gwyrddlas a magenta.\n\nMae golau gwyn yn cynnwys pob lliw posibl, felly gellir ei rannu'n bob cydran. Gellir gwneud hyn gan fod gan bob lliw golau donfedd benodol, tonnau hirach yn cynhyrchu lliw mwy coch, tonnau byrrach yn cynhyrchu mwy o las, gyda gwyrdd yn gorwedd rhywle yn y canol\n\nMae rhannu golau gwyn yn ei wahanol liwiau yn naturiol a gellir ei wneud trwy ddefnyddio arwynebau lliw. Os bydd golau gwyn yn taro wyneb coch, bydd yn adlewyrchu golau coch!\n\nMae'r lloerennau yn y gêm hon, y holltydd, yn trin y hollti lliw hwn i chi, gan ganiatáu i chi amrywiaeth o liwiau i chwarae gyda nhw. Gellir gwneud hyn hefyd trwy ddefnyddio prismau a plygiant.\n\nGellir cyfuno lliwiau hefyd, gan greu cyfuniadau unigryw. Er enghraifft, gall gorgyffwrdd golau coch a glas (o dan yr amgylchiadau cywir) greu'r golau magenta (pinc). Gwyrdd a choch i wneud melyn, gwyrdd a glas i wneud gwyrddlas.\n\n\n\n\n\n\n\n\n\n\n\n\n\nOlwyn Lliw\n\nCyfeirnodau\n-  https://en.wikipedia.org/wiki/Color\n-  https://en.wikipedia.org/wiki/Color_space\n-  https://science.nasa.gov/ems/09_visiblelight/\n-  https://www.bbc.co.uk/bitesize/guides/z7ftnrd/revision/6";
        }
        else if (recordType == RecordType.Blackholes)
        {
            _englishTitle = "Gravitational Anomalies: How it works (Simplified)";
            _englishText = "Gravitational Anomalies, in other words blackholes! \n\nBlack holes are incredibly dense celestial objects that are formed when specific types of stars die. They exert a significantly higher gravitational pull on nearby objects compared to stars and planets despite their smaller size (in some cases). This gravitational strength is so strong that the inner ‘ring’ within a blackhole, called the Event Horizon, results in light not being able to escape once passing through this ‘ring’. \n\nDue to the strength of gravity, black holes can warp space-time. This allows light to curve around the black hole. \n\nFor the specifics, this game utilises the Angular Deflection equation (with some exaggerations) to visually reflect how light curves with the warping of spacetime.\n\n\n The equation to calculate the Angular Deflection is as follows:\n\t- ad  2rs / b\n\tRs = Schwarzschild Radius:\n\t- b = Impact Parameter\n\n\nFurther divided:\n- Rs = 2GM / c^2\n- G is the gravitational constant\n- M is the mass of the blackhole, measured in solar masses (How many times does the blackhole weigh compared to our Sun)\n\n- Impact parameter = r3 Sqrt(r3 / r3-rs)\n- R3 is the distance between the centre of the blackhole, called the Singularity, and the centre of the light photon.\n- Rs is the Schwarzschild Radius\n\n\nAn example:\n- M = 10 SM (Solar Masses)\n- R3 = 29600;\n- Rs =(2 * (6.67430E-11) * (10 * 1.959E30) ) / (299 792 458)^2  = 29 095\n- b = (29600) Sqrt( (29600) / (29600 - Rs) ) = 226 772.99\n- ad = 0.25748\n\n The angular deflection for a single photon of light at a distance of 505m from the centre of the black hole would be: 0.25748.\n\nNormally, the angular deflection would be incredibly small, even at seemingly large distances, as such it has been exaggerated in order to show how the equation works and to make it usable in game.\n\nAdditionally, this game limits the influence of a blackhole on the surrounding environment. In reality, blackholes would have an effect on all satellites and celestial objects within a significantly large distance from its centre. For example, the centre of the Milky Way (our galaxy) is a massive black hole that our planet, and many others orbit.\n\nReferences\n- Original Schwarzschild Paper (in German):  https://www.scribd.com/doc/25310028/Ij-i-3-o-j-c \n- Original Schwarzschild Paper (Translated into English) https://arxiv.org/abs/physics/9905030 \n- Interpretation of the Schwarzschild  Paper: https://arxiv.org/abs/0709.2257\n- https://en.wikipedia.org/wiki/Schwarzschild_geodesics#Bending_of_light_by_gravity\n- https://en.wikipedia.org/wiki/Schwarzschild_radius\n";
            _welshTitle = "Anghysondebau Disgyrchiant: Sut mae'n gweithio (Wedi’u symleiddio)";
            _welshText = "Anghysondebau Disgyrchiant, mewn geiriau eraill tyllau duon!\n\nMae tyllau duon yn wrthrychau nefol anhygoel o drwchus sy'n cael eu ffurfio pan fydd mathau penodol o sêr yn marw. Maent yn gweithredu tynnu disgyrchiant sylweddol uwch ar wrthrychau cyfagos o'i gymharu â sêr a phlanedau er gwaethaf eu maint llai (mewn rhai achosion). Mae'r cryfder disgyrchiant hwn mor gryf fel bod y 'cylch' mewnol o fewn twll du, o'r enw Gorwel Digwyddiadau, yn arwain at olau na allai ddianc unwaith y bydd yn pasio trwy'r 'cylch' hwn.\n\nOherwydd cryfder disgyrchiant, gall tyllau duon ystumio gofod-amser. Mae hyn yn caniatáu i olau gromlin o amgylch y twll du.\n\nAr gyfer y manylion, mae'r gêm hon yn defnyddio'r hafaliad Angular Deflection (gyda rhywfaint o orliwio) i adlewyrchu'n weledol sut mae golau yn cromliniau gyda ystumio gofod-amser. Mae'r hafaliad i gyfrifo'r Gwyriad\n\nOnglog fel a ganlyn:\n\t- ad  2rs / b\n\tRs =Radiws Schwarzschild:\n\t- b = Paramedr Effaith\n\nRhannu ymhellach:\n- Rs = 2GM / c^2\n- G yw'r cysonyn disgyrchiant\n- M yw màs y twll du, wedi'i fesur mewn masau solar (Sawl gwaith mae'r twll du yn pwyso o'i gymharu â'n haul)\n\n- Paramedr effaith = r3 Sqrt(r3 / r3-rs)\n- R3 yw'r pellter rhwng canol y twll du, a elwir yn Hynodyn, a chanol y ffoton golau\n- Rs yw Radiws Schwarzschild\n\n\nEnghraifft:\n- M = 10 SM (Màs Solar)\n- R3 = 29600;\n- Rs =(2 * (6.67430E-11) * (10 * 1.959E30) ) / (299 792 458)^2  = 29 095\n- b = (29600) Sqrt( (29600) / (29600 - Rs) ) = 226 772.99\n- ad = 0.25748\n\n Y gwyriad onglog ar gyfer un ffoton o olau ar bellter o 500m o ganol y twll du byddai: 0.25748.\n\nFel arfer, byddai'r gwyriad onglog yn anhygoel o fach, hyd yn oed ar bellteroedd sy'n ymddangos yn fawr, felly mae wedi cael ei orliwio er mwyn dangos sut mae'r hafaliad yn gweithio ac i'w wneud yn ddefnyddiol yn y gêm\n\nYn ogystal, mae'r gêm hon yn cyfyngu ar ddylanwad twll du ar yr amgylchedd cyfagos. Mewn gwirionedd, byddai tyllau duon yn cael effaith ar bob lloeren a gwrthrych nefol o fewn pellter sylweddol fawr o'i ganolfan. Er enghraifft, mae canol y Llwybr Llaethog (ein galaeth) yn dwll du enfawr y mae ein planed, a llawer o rai eraill yn ei gylchdroi.\n\nCyfeirnodau:\n- Papur Schwarzschild Gwreiddiol (yn Almaeneg): https://www.scribd.com/doc/25310028/Ij-i-3-o-j-c \n- Papur Schwarzschild Gwreiddiol (Cyfieithwyd i'r Saesneg): https://arxiv.org/abs/physics/9905030 \n- Dehongliad o’r Papur Schwarzschild: https://arxiv.org/abs/0709.2257\n- https://en.wikipedia.org/wiki/Schwarzschild_geodesics#Bending_of_light_by_gravity\n- https://en.wikipedia.org/wiki/Schwarzschild_radius\n";
        }
    }

    /// <summary>
    /// Method to update language
    /// </summary>
    /// <param name="language"></param>
    public void UpdateLanguage(Language language)
    {
        // Determine which text to use
        GetText();

        // Loop through all text components and update their text
        foreach (TMP_Text contentText in _contentElements)
        {
            if (language == Language.English)
            {
                if (contentText.gameObject.name == "Title") contentText.text = _englishTitle;
                if (contentText.gameObject.name == "CoreText") contentText.text = _englishText;
            }

            else if (language == Language.Welsh)
            {
                if (contentText.gameObject.name == "Title") contentText.text = _welshTitle;
                if (contentText.gameObject.name == "CoreText") contentText.text = _welshText;
            }
        }
        
    }

}
