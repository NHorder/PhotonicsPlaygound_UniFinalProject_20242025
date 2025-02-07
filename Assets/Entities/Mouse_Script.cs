using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse_Script : MonoBehaviour
{
    public LayerMask layersToHit;
    
    private float currentMovementMultiplier;
    private float currentRotationMultiplier;
    private int movementCounter = 0;
    private int rotationCounter = 0;

    private bool horizontalHeld = false;
    private bool verticalHeld = false;
    private bool rotateHeld = false;

    private Rigidbody2D rigidBody2D = null;
    private Satellite_Info satellite_info = null;


    // Update is called once per frame
    void Update()
    {
        SelectInteraction();

        if (rigidBody2D != null)
        {
            KeyboardInteraction();
        }

        // Need a check to make sure it remains with the designated border
    }


    private void SelectInteraction()
    {
        var mouseLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            
            var listOfRayCasts= Physics2D.RaycastAll(mouseLoc,transform.up,0.1f,layersToHit);

            Debug.Log(listOfRayCasts.Length);

            foreach (RaycastHit2D rayCastInLoop in listOfRayCasts)
            {
                if (rayCastInLoop.distance > 0.1 && (rayCastInLoop.collider is BoxCollider))
                {
                    Debug.Log("Found Box collider!");
                    try{

                        Debug.Log("Selected a satellite!");

                        rigidBody2D = rayCastInLoop.collider.GetComponent<Rigidbody2D>();
                        satellite_info = rayCastInLoop.collider.GetComponent<Satellite_Info>();

                        currentMovementMultiplier = satellite_info.intialMovementMultiplier;
                        currentRotationMultiplier = satellite_info.intialRotationMultiplier;
                        satellite_info.IsSelected = true;

                    }
                    catch {
                        Debug.LogError( rayCastInLoop.collider.name+" has no satellite info!");
                    }
                    break;
                    
                }
            }

        }
    }


    private void KeyboardInteraction()
    {

        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        float rotationMovement = Input.GetAxisRaw("Rotation");

        rigidBody2D.AddForce(new Vector2(horizontalMovement * currentMovementMultiplier, verticalMovement * currentMovementMultiplier));
        rigidBody2D.AddTorque(rotationMovement * currentRotationMultiplier);

        if (Input.GetButton("Rotation")){

            if (rotationCounter > 60 && (currentRotationMultiplier < satellite_info.maxRotationMultiplier))   
            {
                rotationCounter = 0;
                currentRotationMultiplier += 0.01f;
            }
            rotationCounter += 1;
        }
        else currentRotationMultiplier = satellite_info.intialRotationMultiplier;

        if (Input.GetButton("Vertical") || Input.GetButton("Horizontal")){

            if (movementCounter > 60 && (currentMovementMultiplier < satellite_info.maxMovementMultiplier))   
            {
                movementCounter = 0;
                currentMovementMultiplier += 0.01f;
            }
            movementCounter += 1;
        }
        else currentMovementMultiplier = satellite_info.intialMovementMultiplier;

    }
}
