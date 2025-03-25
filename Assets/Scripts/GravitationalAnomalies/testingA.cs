using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testingA : MonoBehaviour
{
    public float angle = 0.0f;

    public GameObject gm1;

    // Start is called before the first frame update
    void Start()
    {
        gm1 = GameObject.FindGameObjectsWithTag("EyeOfZeta")[0];
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.up = gm1.transform.position - transform.position;

        //Debug.Log(transform.eulerAngles.z);
    }
}
