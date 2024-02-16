using System.Collections;
using System.Collections.Generic;

using UnityEngine;


/// <summary>
/// This class is just a storage place for all of the player prefs keys of the options in the settings menu.
/// That way these constants are all in once place and easily accessible.
/// </summary>

public static class PlayerPrefsKeys
{

    // AUDIO SETTINGS KEYS
    // ----------------------------------------------------------------------------------------------------

    public const string MasterVolume = "MasterVolume";
    public const string MusicVolume = "MusicVolume";
    public const string SfxVolume = "SfxVolume";



    // CONTROLS SETTINGS KEYS
    // ----------------------------------------------------------------------------------------------------

    public const string FirstPersonViewSensitivity = "FPViewSensitivity";
    public const string ShakyCamEnabled = "ShakyCamToggle";

}
