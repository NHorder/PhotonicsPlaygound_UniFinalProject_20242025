using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistenceController : MonoBehaviour
{
    /// <summary>
    /// Persistence Controller is designed to handle settings information between scenes
    /// Does not use files due to WebGL project. Reason: WebGL projects would allow N users to access the game
    /// at any given time, resulting in N files being required to track progress per session, otherwise userA can mess
    /// with userB, userC and userD settings, as settings is shared between all users. As such, this controller does not
    /// use files to prevent multi instance settings.
    /// </summary>

    // Language
    private Language _language = Language.English;

    // Base settings, stored locally 
    private bool _forceNoChangeWhenReceivingCommunications = false;
    private bool _disableConfirmations = false;
    private bool _allowAdvancedInteractions = true;
    private bool _allowSatelliteMovementParticles = true;

    // List of unlocked levels
    private List<Level> _unlockedLevels = new List<Level>();

    // Save of instance
    public static PersistenceController Instance;


    /// <summary>
    /// Method to make sure only one instance of the Persistence Controller exists in a given scene.
    /// Results in settings remanining consistent across scenes
    /// </summary>
    private void Awake()
    {

        // If the instance is not null, destroy this game object
        // This enables the Singleton pattern, making sure only one instance of this game object exists at a given time.
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // Assumes instance is null

        // Set instance to this, then mark the Unity object as DontDestroyOnLoad
        // resulting in this game object being maintained across scenes but not across instances.
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Method to update settings when a user closes the settings panel
    /// </summary>
    /// <param name="language"></param>
    /// <param name="forceNoChangeWhenReceivingCommunications"></param>
    /// <param name="disableConfirmations"></param>
    /// <param name="allowAdvancedInteractions"></param>
    /// <param name="allowSatelliteMovementParticles"></param>
    public static void UpdateSettings(Language language,bool forceNoChangeWhenReceivingCommunications,bool disableConfirmations, bool allowAdvancedInteractions, bool allowSatelliteMovementParticles)
    {
        // Update the instances settings
        Instance._language = language;
        Instance._forceNoChangeWhenReceivingCommunications = forceNoChangeWhenReceivingCommunications;
        Instance._disableConfirmations = disableConfirmations;
        Instance._allowAdvancedInteractions = allowAdvancedInteractions;
        Instance._allowSatelliteMovementParticles = allowSatelliteMovementParticles;

    }

    /// <summary>
    /// Method to unlock a specific level
    /// </summary>
    /// <param name="unlockLevel"></param>
    public static void UnlockLevel(Level unlockLevel)
    {
        // Check to make sure the level hasn't already been unlocked
        if (!Instance._unlockedLevels.Contains(unlockLevel)) Instance._unlockedLevels.Add(unlockLevel);
    }

    /// <summary>
    /// Method to get unlocked levels
    /// </summary>
    /// <returns></returns>
    public static List<Level> GetUnlockedLevels() 
    {
        return Instance._unlockedLevels;
    }

    /// <summary>
    /// Method to get language, called by most systems
    /// </summary>
    /// <returns> Saved language</returns>
    public static Language GetLanguage()
    {
        return Instance._language;
    }

    /// <summary>
    /// Method to get the "ForceNoChangeWhenReceivingCommunciations" settings
    /// </summary>
    /// <returns> Boolean isActive</returns>
    public static bool GetForceNoChangeWhenReceivingCommunications()
    {
        return Instance._forceNoChangeWhenReceivingCommunications;
    }

    /// <summary>
    /// Method to get the "DisableConfirmation" settings
    /// </summary>
    /// <returns> Boolean isActive</returns>
    public static bool GetDisableConfirmations()
    {
        return Instance._disableConfirmations;
    }

    /// <summary>
    /// Method to get the "AllowAdvancedInteraction" settings
    /// </summary>
    /// <returns> Boolean isActive</returns>
    public static bool GetAllowAdvancedInteractions()
    {
        return Instance._allowAdvancedInteractions;
    }

    /// <summary>
    /// Method to get the "AllowSatelliteMovementParticles" settings
    /// </summary>
    /// <returns> Boolean isActive</returns>
    public static bool GetAllowSatelliteMovementParticles()
    {
        return Instance._allowSatelliteMovementParticles;
    }


}
