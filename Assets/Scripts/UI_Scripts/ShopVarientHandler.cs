using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopVarientHandler : MonoBehaviour
{

    private TMP_Dropdown _dropdown;
    private Image _shopSprite;

    private Satellite_Info _satelliteInfo;


    public List<Variant> variants;

    void Start()
    {
        _satelliteInfo = gameObject.GetComponent<Satellite_Info>();

        _dropdown =  gameObject.GetComponentInChildren<TMP_Dropdown>();

        var childImageList = gameObject.GetComponentsInChildren<Image>();


        Image startingImage = null;
        // Loop through all children and filter for wanted objects
        foreach (Image childImage in childImageList)
        {
            GameObject childObject = childImage.gameObject;
            if (childObject.name == "Shop_SatelliteSprite") _shopSprite = childImage;
            
            else if (childObject.name == "FrontImage") startingImage = childImage;
            
            if (_shopSprite != null && startingImage != null) break;
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

    }

    public void NewSelectionMade()
    {

        Debug.Log("VALUE has changed!");
        if (_dropdown != null) 
        {

            Variant selectedVariant = variants[_dropdown.value];

            _satelliteInfo.satelliteType = selectedVariant.variantSatelliteType;
            _satelliteInfo.RetreiveSatelliteText();

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
