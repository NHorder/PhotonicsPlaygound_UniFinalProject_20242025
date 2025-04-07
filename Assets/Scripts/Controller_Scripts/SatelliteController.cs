using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// Enumerator for satellite control keys (Up, down, left, right, rotate left, rotate right)
/// Used for UI elements of the satellite control panel
/// </summary>
public enum Key{
    None,
    Vertical,
    Horizontal,
    Rotation
}


public class SatelliteController : MonoBehaviour
{
    /// <summary>
    /// Method to apply force and torque to satellites and display related information
    /// Note: This class is attached to a gameObject continaing a Trigger collider
    /// </summary>
    

    // When key is held, increment movement and rotation by amount
    public float heldMovementSpeedIncrease = 0.01f;
    public float heldRotationSpeedIncrease = 0.01f;


    private UIController _uiController;

    // Movement multipliers
    public float currentMovementMultiplier;
    public float currentRotationMultiplier;

    // Movement counters for when to increment satellite speed
    private int _movementCounter = 0;
    private int _rotationCounter = 0;


    // Last found rigidbody for selection of satellites
    private Rigidbody2D _lastFoundRigidBody2D = null;

    // Selected saved information
    private Rigidbody2D _selectedRigidbody2D = null;
    public SatelliteInfo selectedSatelliteInfo = null;


    // Which key is pressed - Indicated pressed key from the UI elements, does not apply to keyboard interaction
    private Key _keyPressed = Key.None;

    // Is the  information panel visible
    private bool _satInfoPanelVisible = false;

    // If first selection of a satellite
    private bool _firstSelection = true;

    // Link to Eye of Zeta drone
    private CameraDrone _eyeOfZeta;

    /// <summary>
    /// Initalisation Method
    /// </summary>
    void Start()
    {
        // Retrieve UI controller and Eye of Zeta
        _uiController = GameObject.FindGameObjectsWithTag("UI_Controller")[0].GetComponent<UIController>();
        _eyeOfZeta = GameObject.FindGameObjectsWithTag("EyeOfZeta")[0].GetComponent<CameraDrone>();

        // Set the intial selected item to the Eye of Zeta
        // Makes movement more intuitive
        _selectedRigidbody2D = _eyeOfZeta.GetComponent<Rigidbody2D>();
        _selectedRigidbody2D.gameObject.GetComponent<Animator>().SetBool("Selected",true);
        selectedSatelliteInfo = _eyeOfZeta.GetComponent<SatelliteInfo>();
    }

    /// <summary>
    /// Method called once per system tick - 1 per frame for 60fps
    /// </summary>
    void Update()
    {
        // Check to see if an mouse has been clicked
        SelectInteraction();

        // If there is a linked object (through it's rigidbody) then allow interaction
        if (_selectedRigidbody2D != null && selectedSatelliteInfo.canbeMoved)
        {
            // Allow for keyboard interactions
            KeyboardInteraction();

            // Allow for control panel interactions if the panel is expeccted
            if (_uiController.uiExpectations.expectSatelliteControlPanel) ControlPanelInteraction();
        }

    }

    /// <summary>
    /// Method to handle the selection of satellites
    /// </summary>
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
                if (selectedSatelliteInfo != null) _selectedRigidbody2D.gameObject.GetComponent<Animator>().SetBool("Selected",false);

                // Collect the satellite info for the last found rigid body.
                SatelliteInfo lastFoundSatelliteInfo = _lastFoundRigidBody2D.gameObject.GetComponent<SatelliteInfo>();

                // If this satellite info says that it's a selectable object, then proceed.
                // This is checked, as some satellites may not be selectable.
                if (lastFoundSatelliteInfo.advanced_Satellite_Info.isSelectable)
                {
                    _selectedRigidbody2D = _lastFoundRigidBody2D;
                    selectedSatelliteInfo = _lastFoundRigidBody2D.gameObject.GetComponent<SatelliteInfo>();

                    // Update multipliers - Mutipliers are used to increase speed of rotation or movement based on how long 
                    // they are held down to a maximum limit (defined in objects satellite information)
                    currentMovementMultiplier = selectedSatelliteInfo.satellite_Movement_Info.intialMovementMultiplier;
                    currentRotationMultiplier = selectedSatelliteInfo.satellite_Movement_Info.intialRotationMultiplier;

                    // Will involve an animation update - as it needs to be clear which object the user has selected.
                    _selectedRigidbody2D.gameObject.GetComponent<Animator>().SetBool("Selected",true);

                    //If expecting the controls and info panel, update and present the panels
                    if (_uiController.uiExpectations.expectSatelliteControlPanel)
                    {

                        // If Particle effects are allowed, activate the particle system of that satellite
                        if (_uiController.GetGameController().specializedInteractionSettings.allowSatelliteParticleEffects &&
                        selectedSatelliteInfo.canbeMoved && selectedSatelliteInfo.satelliteType != SatelliteType.CameraDrone)
                        {
                            selectedSatelliteInfo.statelliteParticleSystem.Play();
                        }

                        // If the selected satellite can be moved, then present the movement panel, else hide it.
                        if (selectedSatelliteInfo.canbeMoved && !_satInfoPanelVisible)
                        {
                            _uiController.ToggleVisibleSatelliteControls();
                            _satInfoPanelVisible = true;
                        }
                        else if (!selectedSatelliteInfo.canbeMoved && _uiController.CloseSatelliteControlsIfOpen())
                        {
                            _satInfoPanelVisible = false;
                        }

                        // Prevent the Eye of Zeta drone from attaching to itself
                        if (selectedSatelliteInfo.satelliteType != SatelliteType.CameraDrone)
                        {
                            _eyeOfZeta.AttachDroneToSatellite(selectedSatelliteInfo.gameObject.GetComponent<Transform>());
                        }
                        else _eyeOfZeta.DetachDroneFromSatellite();

                        // Open the shop when the satellite creator is selected
                        if (selectedSatelliteInfo.satelliteType == SatelliteType.SatelliteCreator)
                        {
                            _uiController.ToggleVisibleShopIfClosed();
                        }
                        
                        // Present the satellite info panel if opened for the first time
                        if (_firstSelection)
                        {
                            _uiController.ToggleVisibleSatelliteInformation();
                            _firstSelection = false;
                        }
                    }
                }
            }
            
            
            // If it's null, set selected to null - this assumes an object that can't be rotated has been selected or empty space has been selected.
            else if (!uiObjectFound)
            {    
                _eyeOfZeta.DetachDroneFromSatellite();
                CloseInformation();
            }
        }
    }

    /// <summary>
    /// Method to deselect items - close the information for hte selected satellite
    /// </summary>
    private void CloseInformation()
    {
        if (_selectedRigidbody2D != null)
        {
            // If the satellite is not the camera drone and can be move, stop it's particle system
            // Results in particle systems only being active when selected and moved by users
            if (selectedSatelliteInfo.canbeMoved && selectedSatelliteInfo.satelliteType != SatelliteType.CameraDrone)
            {
                selectedSatelliteInfo.statelliteParticleSystem.Stop();
            }

            // Disconnect previous selected and set selected to eye of Zeta
            // More intuitive for camera movement
            _selectedRigidbody2D.gameObject.GetComponent<Animator>().SetBool("Selected",false);
            _selectedRigidbody2D = _eyeOfZeta.GetComponent<Rigidbody2D>();
            _selectedRigidbody2D.gameObject.GetComponent<Animator>().SetBool("Selected",true);
            selectedSatelliteInfo = _eyeOfZeta.GetComponent<SatelliteInfo>();


            // Check if expecting panels
            // If so, check if they're open, then close them
            if (_uiController.CloseSatelliteControlsIfOpen()) _satInfoPanelVisible = false;
        }
    }

    /// <summary>
    /// Method to raycast for UI elements
    /// This methods code has been taken and adapted by a user in Unity discussion (Expand method for more information)
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Method for keyboard satellite interaction
    /// </summary>
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
        if (selectedSatelliteInfo.satelliteType != SatelliteType.Origin && selectedSatelliteInfo.satelliteType != SatelliteType.Destination)
        {
            InteractWithSatellite(horizontalMovement,verticalMovement,rotationMovement);
        }
    }

    /// <summary>
    /// Method to handle UI control panel interactions
    /// </summary>
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
            // If the mouse is down
            if (Input.GetMouseButton(0))
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
            }
            // Otherwise break the loop if the mouse isn't down, as we do not care otherwise.
            else
            {
                break;
            }
        }
       
        // Call interact with satellite passing on the movement types
        InteractWithSatellite(horizontalMovement,verticalMovement,rotationMovement);
    }

    /// <summary>
    /// Method to apply force and torque to the selected satellite
    /// </summary>
    /// <param name="horizontalMovement"></param>
    /// <param name="verticalMovement"></param>
    /// <param name="rotationMovement"></param>
    private void InteractWithSatellite(float horizontalMovement, float verticalMovement, float rotationMovement)
    {
        // if the selected body is not null
        if (_selectedRigidbody2D != null)
        {
            // Vertical, Horizontal and Rotation movement are either -1, 0 or 1. As that is how Unity input works

            // Add force based on movement multiplier
            _selectedRigidbody2D.AddForce(new Vector2(horizontalMovement * currentMovementMultiplier, verticalMovement * currentMovementMultiplier));

            // Add torque (rotation) based on rotation multiplier
            _selectedRigidbody2D.AddTorque(rotationMovement * currentRotationMultiplier);

            // Check to see if the satellite has an attached Eye of Zeta
            // If so, apply the same movement force, so that it follows the satellite.
            // NOTE: This assumes that the Eye of Zeta is parented to the selected satellite.
            CameraDrone eyeOfZeta = _selectedRigidbody2D.GetComponentInChildren<CameraDrone>();

            if (eyeOfZeta != null)
            {
                var eyeOfZetaRigidbody = eyeOfZeta.GetComponent<Rigidbody2D>();

                // Retrieve the rigidbody2d
                eyeOfZetaRigidbody.AddForce(new Vector2(horizontalMovement * currentMovementMultiplier, verticalMovement * currentMovementMultiplier));

                // Apply torque to Eye of Zeta
                eyeOfZetaRigidbody.AddTorque(rotationMovement * currentRotationMultiplier);
            }

            // If the input is rotation, gradually increase the speed of rotation to the object's defined limit (in satellite info)
            if (Input.GetButton("Rotation") ||  _keyPressed == Key.Rotation){
                
                // Updates every 60 updates - Hardcoded update which appeared suitable, allows user good control
                if (_rotationCounter > 60 && (currentRotationMultiplier < selectedSatelliteInfo.satellite_Movement_Info.maxRotationMultiplier))   
                {
                    // Reset the rotation counter
                    _rotationCounter = 0;

                    // As 60 frames is still small, it can rapidly increase speeds if increment is 1, hence small increment allows for
                    // more precise user control
                    currentRotationMultiplier += 0.01f;
                }

                _rotationCounter += 1;
            }

            // Else if the button is let go, reset it the rotation multipler back to it's intial (defined in satellite info)
            else currentRotationMultiplier = selectedSatelliteInfo.satellite_Movement_Info.intialRotationMultiplier;

            // If the vertical or horizontal is held, then gradually increase the speed of movement to a limit (defined in satellite info)
            if (Input.GetButton("Vertical") || Input.GetButton("Horizontal") || _keyPressed == Key.Vertical || _keyPressed == Key.Horizontal){

                // Updates every 60 updates - Hardcoded update which appeared suitable, allows user good control
                if (_movementCounter > 60 && (currentMovementMultiplier < selectedSatelliteInfo.satellite_Movement_Info.maxMovementMultiplier))   
                {
                    // Reset the rotation counter
                    _movementCounter = 0;

                    // As 60 frames is still small, it can rapidly increase speeds if increment is 1, hence small increment allows for
                    // more precise user control
                    currentMovementMultiplier += 0.01f;
                }

                _movementCounter += 1;
            }

            // Else if the button is let go, reset it the rotation multipler back to it's intial (defined in satellite info)
            else currentMovementMultiplier = selectedSatelliteInfo.satellite_Movement_Info.intialMovementMultiplier;
        }
        
        // If nothing is selected, but attempts to interact with a selected item occurs, throw an error, this should not be reached
        else Debug.LogError("ERROR: An error has occurred with control over selected satellites");
    }

    /// <summary>
    /// Method to sell a satellite
    /// </summary>
    public void SellSatellite()
    {
        // Check if the satellite can be sold
        if (selectedSatelliteInfo.canBeSold)
        {
            // Retrieve it's price
            int price = selectedSatelliteInfo.satelliteSellPrice;

            // Retrive the current budget and increase by the sell price of the satellite
            var gameController = _uiController.GetGameController();
            gameController.currentBudget += price;

            // Destroy the satellite
            selectedSatelliteInfo.DestroyObject();

            // Disconnect from the sold satellite
            CloseInformation();

        }
    }



    /// <summary>
    /// Methods for identifying which satellite is being hovered over.
    /// </summary>
    /// <param name="collider"></param>
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


}
