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

        UpdateLanguage(PersistenceController.GetLanguage());
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
    

    public void Called(SatelliteInfo satelliteInfo)
    {
        if (currentIndex < shopItems.Count)
        {
            satelliteInfo.satelliteType = shopItems[currentIndex].satelliteType;
            satelliteInfo.satelliteName = "";
            satelliteInfo.satelliteDescription = "";
            satelliteInfo.satelliteShortDescription = "";
            satelliteInfo.satellitePurchasePrice = 0;

            satelliteInfo.RetreiveSatelliteText();
            satelliteInfo.satellite_Shop_Info.satelliteSprite = shopItems[currentIndex].shopItemSprite;
            satelliteInfo.CreateShopItem();
            

            var variantHandler = satelliteInfo.gameObject.GetComponent<ShopItem>();
            variantHandler.variants = shopItems[currentIndex].variants;

            currentIndex += 1;
        }
        else
        {
            currentIndex = 0;
            Called(satelliteInfo);
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
