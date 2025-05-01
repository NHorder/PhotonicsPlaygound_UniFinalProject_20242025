using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    /// <summary>
    /// Class to handle the shop item interactions
    /// These are typically single satellites
    /// </summary>

    private TMP_Dropdown _dropdown;
    private Image _shopSprite;

    private SatelliteInfo _satelliteInfo;


    public List<Variant> variants;

    /// <summary>
    /// Initialisation Method
    /// </summary>
    void Start()
    {
        var shopitemHandler = gameObject.GetComponentInParent<ShopDropDownHandler>();
        _satelliteInfo = gameObject.GetComponent<SatelliteInfo>();
        shopitemHandler.DetermineInformation(_satelliteInfo);
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

        // If there are variants of this shop item, then change sprite to that of the first variant
        if (startingImage != null && variants.Count > 0) startingImage.sprite = variants[0].varientSprite;

        // Loop through all variants and save them for later use
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

        // If there are only one or less variants and the purchase button exists, move the purchase button
        // and hide the variant dropdown option
        if (variants.Count <= 1 && purchaseButton != null)
        {
            _dropdown.gameObject.active = false;

            var rectTransform = purchaseButton.gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);
        }

    }

    /// <summary>
    /// Method to change variant selected
    /// </summary>
    public void NewSelectionMade()
    {
        if (_dropdown != null) 
        {
            // Find which variant has been selected
            Variant selectedVariant = variants[_dropdown.value];

            // Reset satellite information
            _satelliteInfo.satelliteType = selectedVariant.variantSatelliteType;
            _satelliteInfo.satelliteName = "";
            _satelliteInfo.satelliteDescription = "";
            _satelliteInfo.satelliteShortDescription = "";
            // Call retrievie satellite information - updates name, description and short description
            _satelliteInfo.RetreiveSatelliteText();

            // Creates a shop item, so it's prepared for the shop
            _satelliteInfo.CreateShopItem();

            // If it doesn't have a known sprite, then select a variant sprite
            if (_shopSprite != null) _shopSprite.sprite = selectedVariant.varientSatelliteSprite;
        }
    }
}

/// <summary>
/// Class for containing Variant information, includes name, type, sprite indicator and satellite sprite
/// Example of a sprite indicator would be a coloured circle for colour filters
/// </summary>
[System.Serializable]
public class Variant
{
    public string name;
    public SatelliteType variantSatelliteType;
    public Sprite varientSprite;
    public Sprite varientSatelliteSprite;
}
