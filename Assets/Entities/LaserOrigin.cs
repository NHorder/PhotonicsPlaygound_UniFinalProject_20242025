using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LaserOrigin : MonoBehaviour
{

    public LayerMask layersToHit;
    public float startingEnergy = 50f;


    private int updateDelay = 32;
    private int currentUpdateCount = 0;

    public bool fireOnce = true;

    // First laser is placed manually, then attached
    public GameObject firstLaser;

    // Laser list is created from connections to 
    private List<GameObject> listOfLasers;

    public List<Vector2> listOfLaserOrigins;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Origin has renderer: "+gameObject.GetComponent<SpriteRenderer>() != null);

        listOfLasers = new List<GameObject>();
        listOfLaserOrigins = new List<Vector2>();
        
    }
 
    // Update is called once per frame
    void Update()
    {

        if (currentUpdateCount > updateDelay)
        {
            currentUpdateCount = 0;
            Debug.Log("Laser Count "+listOfLaserOrigins.Count);
            string text = "";

            // Delete all lasers
            foreach (GameObject laser in listOfLasers)
            {
                text += laser.name + " : " + laser.transform.localScale  + "| Has Renderer: "+ (laser.GetComponent<SpriteRenderer>() != null) + "| ";
                Destroy(laser);
            }

            listOfLasers.Clear();
            listOfLaserOrigins.Clear();

            Fire_Initial_Laser();
        }
        else
        {
            currentUpdateCount++;
        }
    }

    private void Fire_Initial_Laser()
    {
        // Create Laser at coordinates
        // Add to List

        //Debug.Log("Firing Initial Laser");

        GameObject newLaser = Instantiate(firstLaser);
        newLaser.transform.position = this.transform.position;
        newLaser.transform.rotation = this.transform.rotation;
        newLaser.transform.localScale = new Vector3(1f,1f,1f);


        AddLaser(newLaser,this.transform.position);

    }

    public void AddLaser(GameObject newLaser,Vector2 raycastPos)
    {

        if (!listOfLaserOrigins.Contains(raycastPos))
        {
            listOfLasers.Add(newLaser);
            listOfLaserOrigins.Add(raycastPos);

            newLaser.name = "Laser_"+listOfLasers.Count;
        }
        else
        {
            Destroy(newLaser);
        }

    }

}
