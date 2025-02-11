using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satellite_Info : MonoBehaviour
{

    public bool IsSelected = false;


    public Interaction interaction = Interaction.Reflection;

    public float intialMovementMultiplier = 1f;
    public float intialRotationMultiplier = 0.01f;

    public float maxMovementMultiplier = 5f;
    public float maxRotationMultiplier = 1f;


    public float reflectiveIndex;
    public float refractiveIndex;
    public float surfaceColor;
    public float absorbance;


    // Start is called before the first frame update
    void Start()
    {
        absorbance = Mathf.Clamp01(absorbance);
    }
}