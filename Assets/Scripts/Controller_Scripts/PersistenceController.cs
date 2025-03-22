using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistenceController : MonoBehaviour
{

    private Language _language = Language.English;
    private bool _forceNoChangeWhenReceivingCommunications = false;
    private bool _disableConfirmations = false;
    private bool _allowAdvancedInteractions = true;
    private bool _allowSatelliteMovementParticles = true;

    private List<Level> _unlockedLevels = new List<Level>();

    private List<TeachingElement> _savedTeachingElements = new List<TeachingElement>();

    public static PersistenceController Instance;

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

    public static void UnlockLevel(Level unlockLevel)
    {
        if (!Instance._unlockedLevels.Contains(unlockLevel)) Instance._unlockedLevels.Add(unlockLevel);
    }

    public static List<Level> GetUnlockedLevels() 
    {
        return Instance._unlockedLevels;
    }

    public static void AddTeachingElements(List<TeachingElement> newTeachingElements)
    {
        foreach (TeachingElement teachingElement in newTeachingElements)
        {
            if (!Instance._savedTeachingElements.Contains(teachingElement) && teachingElement.teachingTitle != "Introduction")
            {
                Instance._savedTeachingElements.Add(teachingElement);
            }
        }
    }

    public static List<TeachingElement> GetSavedTeachingElements()
    {
        return Instance._savedTeachingElements;
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
