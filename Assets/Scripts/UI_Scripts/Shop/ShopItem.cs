using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{

    private TMP_Dropdown _dropdown;
    private Image _shopSprite;

    private Satellite_Info _satelliteInfo;


    public List<Variant> variants;

    void Start()
    {
        var shopitemHandler = gameObject.GetComponentInParent<ShopDropDownHandler>();

        _satelliteInfo = gameObject.GetComponent<Satellite_Info>();
        shopitemHandler.Called(_satelliteInfo);


        _dropdown =  gameObject.GetComponentInChildren<TMP_Dropdown>();

        var childImageList = gameObject.GetComponentsInChildren<Image>();


        Image purchaseButton = null;

        Image startingImage = null;
        // Loop through all children and filter for wanted objects
        foreach (Image childImage in childImageList)
        {
            GameObject childObject = childImage.gameObject;
            if (childObject.name == "Shop_SatelliteSprite") _shopSprite = childImage;
            
            else if (childObject.name == "FrontImage") startingImage = childImage;

            else if (childObject.name == "Shop_PurchaseButton") purchaseButton = childImage;
            
            if (_shopSprite != null && startingImage != null && purchaseButton != null) break;
        }

        if (startingImage != null && variants.Count > 0) startingImage.sprite = variants[0].varientSprite;

        foreach (Variant variant in variants)
        {
            if (variant.varientSprite != null && variant.name != null)
            {
                var newOption = new TMP_Dropdown.OptionData(variant.name,variant.varientSprite);
                _dropdown.options.Add(newOption);
            }
            else
            {
                Debug.LogWarning("WARNING: Variant has no sprite or name!");
            }
        }

        _dropdown.value = 0;

        if (variants.Count <= 1 && purchaseButton != null)
        {
            _dropdown.gameObject.active = false;

            var rectTransform = purchaseButton.gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);
        }

    }

    public void NewSelectionMade()
    {
        if (_dropdown != null) 
        {
            Variant selectedVariant = variants[_dropdown.value];

            _satelliteInfo.satelliteType = selectedVariant.variantSatelliteType;
            _satelliteInfo.satelliteName = "";
            _satelliteInfo.satelliteDescription = "";
            _satelliteInfo.satelliteShortDescription = "";
            _satelliteInfo.RetreiveSatelliteText();
            _satelliteInfo.CreateShopItem();

            if (_shopSprite != null) _shopSprite.sprite = selectedVariant.varientSatelliteSprite;
        }
    }
}

[System.Serializable]
public class Variant
{
    public string name;
    public SatelliteType variantSatelliteType;
    public Sprite varientSprite;
    public Sprite varientSatelliteSprite;
}
