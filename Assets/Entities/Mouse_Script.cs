using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse_Script : MonoBehaviour
{
    
    private float currentMovementMultiplier;
    private float currentRotationMultiplier;
    private int movementCounter = 0;
    private int rotationCounter = 0;

    private Rigidbody2D lastFoundRigidBody2D = null;

    private Rigidbody2D selectedRigidbody2D = null;
    private Satellite_Info selected_satellite_info = null;


    // Update is called once per frame
    void Update()
    {
        // Check to see if an mouse has been clicked
        SelectInteraction();

        // If there is a linked object (through it's rigidbody) then allow interaction
        if (selectedRigidbody2D != null)
        {
            // Allow for keyboard interactions
            KeyboardInteraction();
        }

        // Need a check to make sure it remains with the designated border
    }

    private void SelectInteraction()
    {
        // Calculate the mouse location in world position - Note: Input.mousePosition defaults to screen space, hence a conversion is needed using the main camera
        var mouseLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Update this objects transform to the mouse location - this is used for the collider physics
        this.transform.position = mouseLoc;

        // If the user hits left click
        if (Input.GetMouseButtonDown(0))
        {
            // If the last found rigid body is not null, select the object and retrieve needed information.
            if (lastFoundRigidBody2D != null){

                // Make the previous selected and set to false (not selected)
                if (selected_satellite_info != null) selectedRigidbody2D.gameObject.GetComponent<Animator>().SetBool("Selected",false);

                selectedRigidbody2D = lastFoundRigidBody2D;
                selected_satellite_info = lastFoundRigidBody2D.gameObject.GetComponent<Satellite_Info>();

                // Update multipliers - Mutipliers are used to increase speed of rotation or movement based on how long 
                // they are held down to a maximum limit (defined in objects satellite information)
                currentMovementMultiplier = selected_satellite_info.intialMovementMultiplier;
                currentRotationMultiplier = selected_satellite_info.intialRotationMultiplier;

                // Will involve an animation update - as it needs to be clear which object the user has selected.
                selectedRigidbody2D.gameObject.GetComponent<Animator>().SetBool("Selected",true);

            }
            // If it's null, set selected to null - this assumes an object that can't be rotated has been selected or empty space has been selected.
            else{
                selectedRigidbody2D.gameObject.GetComponent<Animator>().SetBool("Selected",false);

                selectedRigidbody2D = null;
                selected_satellite_info = null;
            }
            
            
        }
    }

    private void KeyboardInteraction()
    {
        // Method to handle keyboard interactions

        // Retrieve movement for horizontal and vertical (WASD or arrow keys)
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        // Custom keybinds to handle rotation, replaces Fire1 - keys are Q E (with Q being positive, E being negative)
        // Reason is unity rotation is postive left, negative right.
        float rotationMovement = Input.GetAxisRaw("Rotation");

        // if the selected body is not null
        if (selectedRigidbody2D != null)
        {
            // Add force based on movement multiplier
            selectedRigidbody2D.AddForce(new Vector2(horizontalMovement * currentMovementMultiplier, verticalMovement * currentMovementMultiplier));

            // Add torque (rotation) based on rotation multiplier
            selectedRigidbody2D.AddTorque(rotationMovement * currentRotationMultiplier);

            // If the input is rotation, gradually increase the speed of rotation to the object's defined limit (in satellite info)
            if (Input.GetButton("Rotation")){
                
                // Updates every 60 updates - Hardcoded update which appeared suitable, allows user good control
                if (rotationCounter > 60 && (currentRotationMultiplier < selected_satellite_info.maxRotationMultiplier))   
                {
                    // Reset the rotation counter
                    rotationCounter = 0;

                    // As 60 frames is still small, it can rapidly increase speeds if increment is 1, hence small increment allows for
                    // more precise user control
                    currentRotationMultiplier += 0.01f;
                }

                rotationCounter += 1;
            }

            // Else if the button is let go, reset it the rotation multipler back to it's intial (defined in satellite info)
            else currentRotationMultiplier = selected_satellite_info.intialRotationMultiplier;



            // If the vertical or horizontal is held, then gradually increase the speed of movement to a limit (defined in satellite info)
            if (Input.GetButton("Vertical") || Input.GetButton("Horizontal")){

                // Updates every 60 updates - Hardcoded update which appeared suitable, allows user good control
                if (movementCounter > 60 && (currentMovementMultiplier < selected_satellite_info.maxMovementMultiplier))   
                {
                    // Reset the rotation counter
                    movementCounter = 0;

                    // As 60 frames is still small, it can rapidly increase speeds if increment is 1, hence small increment allows for
                    // more precise user control
                    currentMovementMultiplier += 0.01f;
                }

                movementCounter += 1;
            }
            // Else if the button is let go, reset it the rotation multipler back to it's intial (defined in satellite info)
            else currentMovementMultiplier = selected_satellite_info.intialMovementMultiplier;
        }
        
        else Debug.LogError("ERROR: An error has occurred with control over selected satellites");
        
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        // When passing through another collider, set last rigid body found.
        // Specifically set to watch for box colliders, as all light interactions interact with polygon colliders

        if (collider is BoxCollider2D) lastFoundRigidBody2D = collider.gameObject.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        // When exiting another collider, reset the last rigidbody found
        // Specifically set to watch for box colliders, as all light interactions interact with polygon colliders
        if (collider is BoxCollider2D) lastFoundRigidBody2D = null;
    }


}
