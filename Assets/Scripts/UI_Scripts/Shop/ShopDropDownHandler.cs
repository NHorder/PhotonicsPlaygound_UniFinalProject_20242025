using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopDropDownHandler : MonoBehaviour
{
    public string dropDownName = "";
    public string dropDownNameWelsh = "";
    public List<ShopItemInfomation> shopItems;


    private int currentIndex = 0;

    private TMP_Dropdown _tmpDropdown;

    // Start is called before the first frame update
    void Start()
    {
        
        _tmpDropdown = gameObject.GetComponent<TMP_Dropdown>();
        _tmpDropdown.transform.SetAsLastSibling();

        List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();

        foreach (ShopItemInfomation shopItem in shopItems)
        {
            TMP_Dropdown.OptionData temp = new TMP_Dropdown.OptionData();
            temp.text = dropDownName;
            list.Add(temp);
        }

        _tmpDropdown.AddOptions(list);

        UpdateLanguage(SettingsController.GetLanguage());
    }

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
    

    public void Called(Satellite_Info satellite_Info)
    {
        if (currentIndex < shopItems.Count)
        {
            satellite_Info.satelliteType = shopItems[currentIndex].satelliteType;
            satellite_Info.satelliteName = "";
            satellite_Info.satelliteDescription = "";
            satellite_Info.satelliteShortDescription = "";
            satellite_Info.satellitePurchasePrice = 0;

            satellite_Info.RetreiveSatelliteText();
            satellite_Info.satellite_Shop_Info.satelliteSprite = shopItems[currentIndex].shopItemSprite;
            satellite_Info.CreateShopItem();
            

            var variantHandler = satellite_Info.gameObject.GetComponent<ShopItem>();
            variantHandler.variants = shopItems[currentIndex].variants;

            currentIndex += 1;
        }
        else
        {
            currentIndex = 0;
            Called(satellite_Info);
        }
    }
}

[System.Serializable]
public class ShopItemInfomation
{
    public SatelliteType satelliteType;
    public Sprite shopItemSprite;
    bool hasVariant = false;

    public List<Variant> variants;

}
