using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveDataEventHandlers : MonoBehaviour
{
    static string _playerSaveDataKey = "player";

    // Events this class listens for (These are set in the inspector)
    //   LoadPlayerDataEvent
    //   SaveRequestedEvent

    // Events this class publishes
    public GameEventScriptableObject saveDataToStorageEvent;

    // Handler for SaveRequestedEvent
    public void SaveRequested()
    {
        PlayerData player = GetComponent<PlayerData>();
        if (player == null) {
            Debug.LogError("PlayerData component not found. Unable to save.");
            return;
        }
        SaveData playerdata = new SaveData(_playerSaveDataKey,
                                           JsonUtility.ToJson(player));
        saveDataToStorageEvent.Raise(playerdata);
    }

    // Handler for SaveDataLoadedFromStorageEvent
    public void LoadData(object data)
    {
        var savedata = data as SaveData;
        if (savedata != null && savedata.key == _playerSaveDataKey) {
            PlayerData player = GetComponent<PlayerData>();
            JsonUtility.FromJsonOverwrite(savedata.data, player);
        }
    }
}
