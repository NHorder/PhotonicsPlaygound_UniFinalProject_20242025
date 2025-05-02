using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopDropDownHandler : MonoBehaviour
{
    /// <summary>
    /// Class to handle shop drop down menus
    /// </summary>
    public string dropDownName = "";
    public string dropDownNameWelsh = "";

    // List contianining all shop items for the given dropdown
    public List<ShopItemInfomation> shopItems;


    private int currentIndex = 0;

    private TMP_Dropdown _tmpDropdown;

    // Start is called before the first frame update
    /// <summary>
    /// Initialisation Method
    /// </summary>
    void Start()
    {
        
        _tmpDropdown = gameObject.GetComponent<TMP_Dropdown>();
        _tmpDropdown.transform.SetAsLastSibling();

        // Identify and locate needed information
        List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
        foreach (ShopItemInfomation shopItem in shopItems)
        {
            TMP_Dropdown.OptionData temp = new TMP_Dropdown.OptionData();
            temp.text = dropDownName;
            list.Add(temp);
        }

        _tmpDropdown.AddOptions(list);

        UpdateLanguage(PersistenceController.GetLanguage());
    }

    /// <summary>
    /// Method to update language - this only counts for the dropdown title, I.e Splitters and Combiners
    /// </summary>
    /// <param name="language"></param>
    public void UpdateLanguage(Language language)
    {
        if (language == Language.English)
        {
            _tmpDropdown.captionText.text = dropDownName;
        }
        else if (language == Language.Welsh)
        {
            _tmpDropdown.captionText.text = dropDownNameWelsh;
        }
        
    }
    
    /// <summary>
    /// Method used to create shop items, and display their correct information
    /// This is typically used to handle variants of a type of item
    /// Note: Variants are not fully implemented due to issue where the dropdown appeared below 
    /// a certain UI element making it hard to select a given variant type
    /// </summary>
    /// <param name="satelliteInfo"></param>
    public void DetermineInformation(SatelliteInfo satelliteInfo)
    {
        if (currentIndex < shopItems.Count)
        {
            satelliteInfo.satelliteType = shopItems[currentIndex].satelliteType;

            // Reset information
            satelliteInfo.satelliteName = "";
            satelliteInfo.satelliteDescription = "";
            satelliteInfo.satelliteShortDescription = "";
            satelliteInfo.satellitePurchasePrice = 0;

            // Force text update
            satelliteInfo.RetreiveSatelliteText();

            // Update shop sprite
            satelliteInfo.satellite_Shop_Info.satelliteSprite = shopItems[currentIndex].shopItemSprite;
            satelliteInfo.CreateShopItem();
            
            // Update current value
            var variantHandler = satelliteInfo.gameObject.GetComponent<ShopItem>();
            variantHandler.variants = shopItems[currentIndex].variants;

            currentIndex += 1;
        }
        else
        {
            currentIndex = 0;
            DetermineInformation(satelliteInfo);
        }
    }
}


/// <summary>
/// Class to store shop item information, includes satellite type, shop item sprite and variants 
/// </summary>
[System.Serializable]
public class ShopItemInfomation
{
    public SatelliteType satelliteType;
    public Sprite shopItemSprite;
    bool hasVariant = false;

    public List<Variant> variants;

}
