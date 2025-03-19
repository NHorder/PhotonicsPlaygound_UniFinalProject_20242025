using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsController : MonoBehaviour
{

    public Language _language = Language.English;
    public bool _forceNoChangeWhenReceivingCommunications = false;
    public bool _disableConfirmations = false;
    public bool _allowAdvancedInteractions = true;
    public bool _allowSatelliteMovementParticles = true;



    public static SettingsController Instance;

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


    public static void UpdateSettings(Language language,bool forceNoChangeWhenReceivingCommunications,bool disableConfirmations, bool allowAdvancedInteractions, bool allowSatelliteMovementParticles)
    {
        Instance._language = language;
        Instance._forceNoChangeWhenReceivingCommunications = forceNoChangeWhenReceivingCommunications;
        Instance._disableConfirmations = disableConfirmations;
        Instance._allowAdvancedInteractions = allowAdvancedInteractions;
        Instance._allowSatelliteMovementParticles = allowSatelliteMovementParticles;

    }

    public static Language GetLanguage()
    {
        return Instance._language;
    }

    public static bool GetForceNoChangeWhenReceivingCommunications()
    {
        return Instance._forceNoChangeWhenReceivingCommunications;
    }

    public static bool GetDisableConfirmations()
    {
        return Instance._disableConfirmations;
    }

    public static bool GetAllowAdvancedInteractions()
    {
        return Instance._allowAdvancedInteractions;
    }

    public static bool GetAllowSatelliteMovementParticles()
    {
        return Instance._allowSatelliteMovementParticles;
    }


}
