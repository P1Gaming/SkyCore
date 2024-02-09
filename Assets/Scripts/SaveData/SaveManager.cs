/// <summary
/// SaveManager saves data to storage and loads data from storage. These actions
/// are activated by raising and responding to Game Events.
///
/// Game Events: SaveManager uses 4 GameEventScriptableObjects to communicate
/// with all other components in the game. These are SaveDataToStorageEvent,
/// SaveDataFromStorageEvent, SaveRequestedEvent, and SaveDataLoadKeyEvent.
///
/// SaveDataToStorageEvent: This event is raised by a component in order to
/// persist its data. The component must instantiate an instance of class SaveData
/// and set the two fields key and data, and then pass the SaveData instance
/// as the argument to SaveDataToStorageEvent. The SaveData.key field is a name
/// that uniquely identifies the data. Note there is no checking for duplicate
/// names, so if one component stores data under the same key as another
/// component, the original data is overwritten. The SaveData.data field is a
/// string representing the data to be persisted, typically in JSON format. This
/// event may be raised any time the component needs to persist data and must be
/// raised in response to a SaveRequestedEvent.
///
/// SaveDataFromStorageEvent: This event is raised by the SaveManager when
/// persisted data is read from storage. Components must cast the object
/// argument to a SaveData instance and check the SaveData.key field to see if
/// the data is relevant. The SaveManager.LoadAll() function raises this event
/// for each key that it reads from storage and so could be attached to the
/// OnClick() handler of a Load button in the UI.
///
/// SaveRequestedEvent: This event is raised by the SaveManager and is used
/// to compel all listeners to save its data by raising a SaveDataToStorageEvent.
/// The SaveManager.SaveAll() function raises this event and so could be
/// attached to the OnClick() handler of a Save button in the UI.
///
/// SaveDataLoadKeyEvent: This event may be raised by a component to request
/// the SaveManager to read persisted data for a specific key. If data exists
/// for the specified key, then a SaveDataFromStorage event is raised in
/// response.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Components must use this type to persist data. This is the "object" type
/// passed to the SavaDataXYZ Events.
///
/// <param> key is a string used by the component to identify its own data </param>
/// <param> data is a json-formatted string representing the component's data </param>
///
/// </summary>
[System.Serializable]
public class SaveData {
    public string key;
    public string data;

    public SaveData(string key, string data)
    {
        this.key = key;
        this.data = data;
    }
}

/// <summary>
/// This data structure is what SaveManager actually writes to storage and is
/// only used internally by the SaveManager class. Its purpose is to help
/// validate the data when read back from storage.
/// So far this object only contains a version field (referring to the format
/// of the data that SaveManager writes to storage which could change over time
/// as features are added), but could conceivably also contain a hash or some
/// other method to verify the integrity of the data.
/// </summary>
[System.Serializable]
public class SaveDataWrapper {
    public int version;
    public SaveData savedata;
}

public class SaveManager : MonoBehaviour
{
    private string _saveDataDir;

    // events this class raises
    public GameEventScriptableObject saveRequestedEvent;
    public GameEventScriptableObject saveDataWasLoadedEvent;

    void Awake()
    {
        _saveDataDir = System.IO.Path.Combine(Application.persistentDataPath, "savedata");
    }

    private bool isSupportedVersion(int version)
    {
        return version == 1;
    }

    /// <summary>
    /// Call this method to broadcast an event that provokes listeners of
    /// the "SaveRequestedEvent" GameEvent to save their data.
    /// </summary>
    public void SaveAll()
    {
        saveRequestedEvent.Raise();
    }

    /// <summary>
    /// Call this method to load all persisted data from storage
    /// </summary>
    public void LoadAll()
    {
        // Load the list of files found in the savedata directory
        string[] fileEntries = System.IO.Directory.GetFiles(_saveDataDir);
        foreach (string fileName in fileEntries)
        {
            LoadFile(fileName);
        }
    }

    /// <summary>
    /// Call this method to load persisted data for a specific key
    /// </summary>
    public void Load(object key)
    {
        string path = System.IO.Path.Combine(_saveDataDir, (string)key);
        if (!System.IO.File.Exists(path))
        {
            return;
        }
        LoadFile(path);
    }

    /// <summary>
    /// Event handler for persisting data to storage
    /// </summary>
    public void SaveDataToStorageEventHandler(object data)
    {
        var savedata = data as SaveData;
        if (savedata != null) {
            Save(savedata);
        } else {
            Debug.LogError("Failed to save data to storage: unable to cast event object to SaveData");
        }
    }

    /// <summary>
    /// Call this method to read data from storage at the specified path and
    /// notify listeners
    /// </summary>
    private void LoadFile(string path)
    {
        string json = System.IO.File.ReadAllText(path);

        // first we run some tests to see if the data is readable and valid
        var wrapped = JsonUtility.FromJson<SaveDataWrapper>(json);
        if (wrapped == null)
        {
            Debug.LogError("Failed to load savedata " + path + ": failed to convert from json");
            return;
        }

        if (!isSupportedVersion(wrapped.version))
        {
            // in the future we may write functions to upgrade from
            // an older version to the current one, but for now we
            // just error out
            Debug.LogError("Failed to load savedata " + path + ": unsupported version " + wrapped.version);
            return;
        }

        // seems the data is valid so notify listeners
        saveDataWasLoadedEvent.Raise(wrapped.savedata);
    }

    /// <summary>
    /// Call this method to write data to storage
    /// </summary>
    private void Save(SaveData data)
    {
        string path = System.IO.Path.Combine(_saveDataDir, data.key);
        if (!System.IO.Directory.Exists(_saveDataDir)) {
            System.IO.Directory.CreateDirectory(_saveDataDir);
        }
        var wrapped = new SaveDataWrapper();
        wrapped.version = 1;
        wrapped.savedata = data;
        System.IO.File.WriteAllText(path, JsonUtility.ToJson(wrapped));
    }

}