using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    public int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        counter += 1;

        if (counter >= 60)
        {
            Debug.Log("Time!");
            counter = 0;
        }
    }
}
