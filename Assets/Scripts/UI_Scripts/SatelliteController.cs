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

public class SatelliteController : MonoBehaviour
{

    private UIController _uiController;

    private float _currentMovementMultiplier;
    private float _currentRotationMultiplier;
    private int _movementCounter = 0;
    private int _rotationCounter = 0;

    private Rigidbody2D _lastFoundRigidBody2D = null;

    private Rigidbody2D _selectedRigidbody2D = null;
    private Satellite_Info _selectedSatelliteInfo = null;

    private Key _keyPressed = Key.None;


    void Start()
    {
        // Retrieve UI controller
        //_uiController = GameObject.FindGameObjectsWithTag("UI_Controller")[0].GetComponent<UIController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check that interaction is enabled
        var interactionEnabled = true;//_uiController.GetInteractionEnabled();
        if (interactionEnabled)
        {
            // Check to see if an mouse has been clicked
            SelectInteraction();

            // If there is a linked object (through it's rigidbody) then allow interaction
            if (_selectedRigidbody2D != null && _selectedSatelliteInfo.canbeMoved)
            {
                // Allow for keyboard interactions
                KeyboardInteraction();

                // Allow for control panel interactions if the panel is expeccted
                if (_uiController.uiExpectations.expectSatelliteControlsAndInfoPanels) ControlPanelInteraction();
            }
        }

        // If interaction is disabled, but the selected rigid body is not null
        else if (!interactionEnabled && _selectedRigidbody2D != null) 
        {
            // Update the items animator to show unselected
            _selectedRigidbody2D.gameObject.GetComponent<Animator>().SetBool("Selected",false);

            // Disconnect from the selected satellite
            _selectedRigidbody2D = null;
            _selectedSatelliteInfo = null;

            // Hide the controls and information UI, as not item is selected
            _uiController.PresentPanel(UIPanel.Satellite_Controls,false);
            _uiController.PresentPanel(UIPanel.Satellite_Info_UI,false);
            
            // Notify UI Controller that nothing is selected
            _uiController.selectedSatelliteInfo = null;
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
            var uiObjectFound = (MouseOverUIObject().Count != 0);

            // Check that there are no UI objects and the last found rigidbody is not null
            if (!uiObjectFound && _lastFoundRigidBody2D != null){

                // Make the previous selected and set to false (not selected)
                if (_selectedSatelliteInfo != null) _selectedRigidbody2D.gameObject.GetComponent<Animator>().SetBool("Selected",false);

                // Collect the satellite info for the last found rigid body.
                Satellite_Info lastFoundSatelliteInfo = _lastFoundRigidBody2D.gameObject.GetComponent<Satellite_Info>();

                // If this satellite info says that it's a selectable object, then proceed.
                // This is checked, as some satellites may not be selectable.
                if (lastFoundSatelliteInfo.advanced_Satellite_Info.isSelectable)
                {
                    _selectedRigidbody2D = _lastFoundRigidBody2D;
                    _selectedSatelliteInfo = _lastFoundRigidBody2D.gameObject.GetComponent<Satellite_Info>();

                    // Update multipliers - Mutipliers are used to increase speed of rotation or movement based on how long 
                    // they are held down to a maximum limit (defined in objects satellite information)
                    _currentMovementMultiplier = _selectedSatelliteInfo.satellite_Movement_Info.intialMovementMultiplier;
                    _currentRotationMultiplier = _selectedSatelliteInfo.satellite_Movement_Info.intialRotationMultiplier;

                    // Will involve an animation update - as it needs to be clear which object the user has selected.
                    _selectedRigidbody2D.gameObject.GetComponent<Animator>().SetBool("Selected",true);

                    // Notify ui controller of new satellite selection
                    _uiController.selectedSatelliteInfo = _selectedSatelliteInfo;

                    // If expecting the controls and info panel, update and present the panels
                    if (_uiController.uiExpectations.expectSatelliteControlsAndInfoPanels)
                    {

                        // If the selected satellite can be moved, then present the movement panel, else hide it.
                        if (_selectedSatelliteInfo.canbeMoved) _uiController.PresentPanel(UIPanel.Satellite_Controls,true);
                        
                        else _uiController.PresentPanel(UIPanel.Satellite_Controls,false);

                        // Present the satellite info panel
                        _uiController.PresentPanel(UIPanel.Satellite_Info_UI,true);
                    }
                }
            }
            // If it's null, set selected to null - this assumes an object that can't be rotated has been selected or empty space has been selected.
            else{
                
                CloseInformation(uiObjectFound);
            }
        }
    }

    private void CloseInformation(bool uiObjectFound = false)
    {
        if (_selectedRigidbody2D != null && !uiObjectFound)
        {
            _selectedRigidbody2D.gameObject.GetComponent<Animator>().SetBool("Selected",false);
            _selectedRigidbody2D = null;
            _selectedSatelliteInfo = null;

            if (_uiController.uiExpectations.expectSatelliteControlsAndInfoPanels)
            {
                _uiController.PresentPanel(UIPanel.Satellite_Controls,false);
                _uiController.PresentPanel(UIPanel.Satellite_Info_UI,false);

            }

            _uiController.selectedSatelliteInfo = null;
        }
    }

    private List<RaycastResult> MouseOverUIObject()
    {
        // Code taken and adapted from Krishx007  (last viewed 2025/02/12)
        // within the discussion: https://discussions.unity.com/t/detect-mouseover-click-for-ui-canvas-object/152611/5 
        // Adaptions include comments and final return statement.

        // Create new pointer event data - needed for the EventSystem
        var eventData = new PointerEventData(EventSystem.current);

        // Update eventdata position to that of the mouse position
        eventData.position =  Input.mousePosition;

        // Create a RaycastResult list
        var raycastResults = new List<RaycastResult>();

        // Use the EventSystem RaycastAll to retrieve all UI elements beneath the mouse.
        EventSystem.current.RaycastAll(eventData, raycastResults );

        // Return the results
        return raycastResults;
    }

    

    private void KeyboardInteraction()
    {
        // Method to handle keyboard interactions

        // Retrieve movement for horizontal and vertical (WASD or arrow keys)
        var horizontalMovement = Input.GetAxisRaw("Horizontal");
        var verticalMovement = Input.GetAxisRaw("Vertical");

        // Custom keybinds to handle rotation, replaces Fire1 - keys are Q E (with Q being positive, E being negative)
        // Reason is unity rotation is postive left, negative right.
        var rotationMovement = Input.GetAxisRaw("Rotation");

        // If the seleccted satellite IS NOT Origin or Destination, allow movement
        // Reasoning, it would be way to easy to win if you move the destination to the origin or vice versa.
        if (_selectedSatelliteInfo.satelliteType != SatelliteType.Origin && _selectedSatelliteInfo.satelliteType != SatelliteType.Destination)
        {
            InteractWithSatellite(horizontalMovement,verticalMovement,rotationMovement);
        }
    }

    private void ControlPanelInteraction()
    {
        var horizontalMovement = 0;
        var verticalMovement = 0;
        var rotationMovement = 0;

        // Retrieve what objects the mouse is over
        List<RaycastResult> rayCastResults = MouseOverUIObject();

        // As UI object lists can get very large, loop through all and check if the object tag is a control key, if not ignore it
        foreach (RaycastResult raycastResult in rayCastResults)
        {
            // If the raycast result has a control key AND the mouse is down
            if (raycastResult.gameObject.tag == "Satellite_Control_Key" && Input.GetMouseButton(0))
            {
                // Collect the game object
                var key = raycastResult.gameObject;

                // Depending on the key name, increase respective movement and set keyPressed to the key axis
                // Used Else if here, as only one key can be pressed at a given time, and if none are pressed, then set keyPressed to None.
                if (key.name == "ForwardKey") {verticalMovement +=1; _keyPressed = Key.Vertical;}
                else if (key.name == "DownKey") {verticalMovement -=1; _keyPressed = Key.Vertical;}

                else if (key.name == "LeftKey") {horizontalMovement -=1; _keyPressed = Key.Horizontal;}
                else if (key.name == "RightKey") {horizontalMovement +=1; _keyPressed = Key.Horizontal;}

                else if (key.name == "RotateLeftKey") {rotationMovement +=1; _keyPressed = Key.Rotation;}
                else if (key.name == "RotateRightKey") {rotationMovement -=1; _keyPressed = Key.Rotation;}

                // Else nothing was pressed
                else _keyPressed = Key.None;   

                // Break Loop
                // Assumes the player cannot have multiple mice on the screen at once
                break;
            }

            // Else if the key was found but the mouse button was not down, break the loop early.
            // Assumes the player cannot have multiple miceon the screen at once
            else if (raycastResult.gameObject.tag == "Satellite_Control_Key" && !Input.GetMouseButton(0))
            {
                // Break Loop
                break;
            }
        }
       
        // Call interact with satellite passing on the movement types
        InteractWithSatellite(horizontalMovement,verticalMovement,rotationMovement);
    }

    private void InteractWithSatellite(float horizontalMovement, float verticalMovement, float rotationMovement)
    {
        // if the selected body is not null
        if (_selectedRigidbody2D != null)
        {
            // Vertical, Horizontal and Rotation movement are either -1, 0 or 1. As that is how Unity input works



            // Add force based on movement multiplier
            _selectedRigidbody2D.AddForce(new Vector2(horizontalMovement * _currentMovementMultiplier, verticalMovement * _currentMovementMultiplier));

            // Add torque (rotation) based on rotation multiplier
            _selectedRigidbody2D.AddTorque(rotationMovement * _currentRotationMultiplier);

            // If the input is rotation, gradually increase the speed of rotation to the object's defined limit (in satellite info)
            if (Input.GetButton("Rotation") ||  _keyPressed == Key.Rotation){
                
                // Updates every 60 updates - Hardcoded update which appeared suitable, allows user good control
                if (_rotationCounter > 60 && (_currentRotationMultiplier < _selectedSatelliteInfo.satellite_Movement_Info.maxRotationMultiplier))   
                {
                    // Reset the rotation counter
                    _rotationCounter = 0;

                    // As 60 frames is still small, it can rapidly increase speeds if increment is 1, hence small increment allows for
                    // more precise user control
                    _currentRotationMultiplier += 0.01f;
                }

                _rotationCounter += 1;
            }

            // Else if the button is let go, reset it the rotation multipler back to it's intial (defined in satellite info)
            else _currentRotationMultiplier = _selectedSatelliteInfo.satellite_Movement_Info.intialRotationMultiplier;

            // If the vertical or horizontal is held, then gradually increase the speed of movement to a limit (defined in satellite info)
            if (Input.GetButton("Vertical") || Input.GetButton("Horizontal") || _keyPressed == Key.Vertical || _keyPressed == Key.Horizontal){

                // Updates every 60 updates - Hardcoded update which appeared suitable, allows user good control
                if (_movementCounter > 60 && (_currentMovementMultiplier < _selectedSatelliteInfo.satellite_Movement_Info.maxMovementMultiplier))   
                {
                    // Reset the rotation counter
                    _movementCounter = 0;

                    // As 60 frames is still small, it can rapidly increase speeds if increment is 1, hence small increment allows for
                    // more precise user control
                    _currentMovementMultiplier += 0.01f;
                }

                _movementCounter += 1;
            }

            // Else if the button is let go, reset it the rotation multipler back to it's intial (defined in satellite info)
            else _currentMovementMultiplier = _selectedSatelliteInfo.satellite_Movement_Info.intialMovementMultiplier;
        }
        
        // If nothing is selected, but attempts to interact with a selected item occurs, throw an error, this should not be reached
        else Debug.LogError("ERROR: An error has occurred with control over selected satellites");
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // When passing through another collider, set last rigid body found.
        // Specifically set to watch for box colliders, as all light interactions interact with polygon colliders

        if (collider is BoxCollider2D) _lastFoundRigidBody2D = collider.gameObject.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
         if (_lastFoundRigidBody2D == null && collider is BoxCollider2D) _lastFoundRigidBody2D = collider.gameObject.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        // When exiting another collider, reset the last rigidbody found
        // Specifically set to watch for box colliders, as all light interactions interact with polygon colliders
        if (collider is BoxCollider2D) _lastFoundRigidBody2D = null;
    }


    public void SellSatellite()
    {
        if (_selectedSatelliteInfo.canBeSold)
        {
            int price = _selectedSatelliteInfo.satelliteSellPrice;

            var gameController = _uiController.GetGameController();

            if (gameController.currentBudget + price <= gameController.maxBudget)
            {
                gameController.currentBudget += price;
            }

            _selectedSatelliteInfo.DestroyObject();
            CloseInformation();

        }
    }

}
