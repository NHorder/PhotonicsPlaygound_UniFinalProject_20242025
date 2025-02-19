using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public enum Key{
    None,
    Vertical,
    Horizontal,
    Rotation
}

public class Satellite_Controller : MonoBehaviour
{

    UI_Controller uiController;

    private float currentMovementMultiplier;
    private float currentRotationMultiplier;
    private int movementCounter = 0;
    private int rotationCounter = 0;

    private Rigidbody2D lastFoundRigidBody2D = null;

    private Rigidbody2D selectedRigidbody2D = null;
    private Satellite_Info selected_satellite_info = null;

    private GameObject[] satelliteControlPanelKeys;

    private Key keyPressed = Key.None;


    void Start()
    {
        uiController = GameObject.FindGameObjectsWithTag("UI_Controller")[0].GetComponent<UI_Controller>();
        satelliteControlPanelKeys = GameObject.FindGameObjectsWithTag("Satellite_Control_Key");

    }

    // Update is called once per frame
    void Update()
    {
        bool levelComplete = uiController.GetLevelComplete();
        bool teachingPlayer = uiController.GetTeachingUser();

        if (levelComplete && selectedRigidbody2D != null)
        {
            selectedRigidbody2D.gameObject.GetComponent<Animator>().SetBool("Selected",false);
            selectedRigidbody2D = null;
            selected_satellite_info = null;
            uiController.PresentPanel(UIPanel.Satellite_Controls,false);
            uiController.PresentPanel(UIPanel.Satellite_Info_UI,false);
            uiController.selectedSatelliteInfo = null;
        }

        else if (!(levelComplete && teachingPlayer))
        {
            // Check to see if an mouse has been clicked
            SelectInteraction();

            // If there is a linked object (through it's rigidbody) then allow interaction
            if (selectedRigidbody2D != null)
            {
                // Allow for keyboard interactions
                KeyboardInteraction();

                // Allow for control panel interactions
                ControlPanelInteraction();
            }
        }
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
            // Check if there are any UI objects found, if there is don't select a satellite
            bool uiObjectFound = (MouseOverUIObject().Count != 0);

            if (!uiObjectFound && lastFoundRigidBody2D != null){

                // Make the previous selected and set to false (not selected)
                if (selected_satellite_info != null) selectedRigidbody2D.gameObject.GetComponent<Animator>().SetBool("Selected",false);

                selectedRigidbody2D = lastFoundRigidBody2D;
                selected_satellite_info = lastFoundRigidBody2D.gameObject.GetComponent<Satellite_Info>();

                // Update multipliers - Mutipliers are used to increase speed of rotation or movement based on how long 
                // they are held down to a maximum limit (defined in objects satellite information)
                currentMovementMultiplier = selected_satellite_info.satellite_Movement_Info.intialMovementMultiplier;
                currentRotationMultiplier = selected_satellite_info.satellite_Movement_Info.intialRotationMultiplier;

                // Will involve an animation update - as it needs to be clear which object the user has selected.
                selectedRigidbody2D.gameObject.GetComponent<Animator>().SetBool("Selected",true);

                uiController.selectedSatelliteInfo = selected_satellite_info;

                if (selected_satellite_info.satelliteType == SatelliteType.Origin || selected_satellite_info.satelliteType == SatelliteType.Destination)
                {
                    uiController.PresentPanel(UIPanel.Satellite_Controls,false);
                }
                else uiController.PresentPanel(UIPanel.Satellite_Controls,true);
                
                
                uiController.PresentPanel(UIPanel.Satellite_Info_UI,true);

            }
            // If it's null, set selected to null - this assumes an object that can't be rotated has been selected or empty space has been selected.
            else{
                if (selectedRigidbody2D != null && !uiObjectFound)
                {
                    selectedRigidbody2D.gameObject.GetComponent<Animator>().SetBool("Selected",false);
                    selectedRigidbody2D = null;
                    selected_satellite_info = null;
                    uiController.PresentPanel(UIPanel.Satellite_Controls,false);

                    uiController.PresentPanel(UIPanel.Satellite_Info_UI,false);

                    uiController.selectedSatelliteInfo = null;
                }

                
            }
            
            
        }
            
    }


    private List<RaycastResult> MouseOverUIObject()
    {
        // Code taken and adapted from Krishx007
        // within the discussion: https://discussions.unity.com/t/detect-mouseover-click-for-ui-canvas-object/152611/5 
        // Adaptions include comments and final return statement.

        // Create new pointer event data - needed for the EventSystem
        PointerEventData eventData = new PointerEventData(EventSystem.current);

        // Update eventdata position to that of the mouse position
        eventData.position =  Input.mousePosition;

        // Create a RaycastResult list
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        // Use the EventSystem RaycastAll to retrieve all UI elements beneath the mouse.
        EventSystem.current.RaycastAll( eventData, raycastResults );

        return raycastResults;
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

        if (selected_satellite_info.satelliteType != SatelliteType.Origin && selected_satellite_info.satelliteType != SatelliteType.Destination) InteractWithSatellite(horizontalMovement,verticalMovement,rotationMovement);
        
    }

    private void ControlPanelInteraction()
    {
        int horizontalMovement = 0;
        int verticalMovement = 0;
        int rotationMovement = 0;

        // Retrieve what objects the mouse is over
        List<RaycastResult> rayCastResults = MouseOverUIObject();

        // Instantiate new list for game objects
        List<GameObject> raycastKeysFound = new List<GameObject>();

        // As UI object lists can get very large, loop through all and check if the object tag is a control key, if not ignore it
        foreach (RaycastResult raycastResult in rayCastResults)
        {
            if (raycastResult.gameObject.tag == "Satellite_Control_Key") raycastKeysFound.Add(raycastResult.gameObject);
        }

        // Check if left mouse click is pressed down
        if (Input.GetMouseButton(0))
        {
            // Loop through all control key objects
            foreach (GameObject key in satelliteControlPanelKeys)
            {
                // Check if the key exists within the list
                if(raycastKeysFound.Contains(key))
                {
                    // Depending on the key name, increase respective movement and set keyPressed to the key axis
                    // Used Else if here, as only one key can be pressed at a given time, and if none are pressed, then set keyPressed to None.

                    if (key.name == "ForwardKey") {verticalMovement +=1; keyPressed = Key.Vertical;}
                    else if (key.name == "DownKey") {verticalMovement -=1; keyPressed = Key.Vertical;}

                    else if (key.name == "LeftKey") {horizontalMovement -=1; keyPressed = Key.Horizontal;}
                    else if (key.name == "RightKey") {horizontalMovement +=1; keyPressed = Key.Horizontal;}


                    else if (key.name == "RotateLeftKey") {rotationMovement +=1; keyPressed = Key.Rotation;}
                    else if (key.name == "RotateRightKey") {rotationMovement -=1; keyPressed = Key.Rotation;}

                    else keyPressed = Key.None;
                }
            }
        }

        // Call interact with satellite passing on the movement types
        InteractWithSatellite(horizontalMovement,verticalMovement,rotationMovement);

    }




    private void InteractWithSatellite(float horizontalMovement, float verticalMovement, float rotationMovement)
    {
        // if the selected body is not null
        if (selectedRigidbody2D != null)
        {
            // Add force based on movement multiplier
            selectedRigidbody2D.AddForce(new Vector2(horizontalMovement * currentMovementMultiplier, verticalMovement * currentMovementMultiplier));

            // Add torque (rotation) based on rotation multiplier
            selectedRigidbody2D.AddTorque(rotationMovement * currentRotationMultiplier);

            // If the input is rotation, gradually increase the speed of rotation to the object's defined limit (in satellite info)
            if (Input.GetButton("Rotation") ||  keyPressed == Key.Rotation){
                
                // Updates every 60 updates - Hardcoded update which appeared suitable, allows user good control
                if (rotationCounter > 60 && (currentRotationMultiplier < selected_satellite_info.satellite_Movement_Info.maxRotationMultiplier))   
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
            else currentRotationMultiplier = selected_satellite_info.satellite_Movement_Info.intialRotationMultiplier;

            // If the vertical or horizontal is held, then gradually increase the speed of movement to a limit (defined in satellite info)
            if (Input.GetButton("Vertical") || Input.GetButton("Horizontal") || keyPressed == Key.Vertical || keyPressed == Key.Horizontal){

                // Updates every 60 updates - Hardcoded update which appeared suitable, allows user good control
                if (movementCounter > 60 && (currentMovementMultiplier < selected_satellite_info.satellite_Movement_Info.maxMovementMultiplier))   
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
            else currentMovementMultiplier = selected_satellite_info.satellite_Movement_Info.intialMovementMultiplier;
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
