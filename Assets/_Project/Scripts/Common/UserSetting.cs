using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public static class UserSetting
{
    public const string Music = "Music", Sound = "Sound", Vibration = "Vibration";
    const string Setting = "Setting";
    private static Dictionary<string, bool> commonSetting = new Dictionary<string, bool>();
    static UserSetting()
    {
        Load();
    }

    public static bool GetSetting(string setting)
    {
        if (!commonSetting.ContainsKey(setting))
            commonSetting.Add(setting, true);
        return commonSetting[setting];
    }

    public static void SetSetting(string setting, bool value)
    {
        if (!commonSetting.ContainsKey(setting))
            commonSetting.Add(setting, value);
        else commonSetting[setting] = value;

        Save();
    }

    private static void Save()
    {
        string json = JsonMapper.ToJson(commonSetting);
        PlayerPrefs.SetString(Setting, json);
        PlayerPrefs.Save();
    }

    private static void Load()
    {
        string json = PlayerPrefs.GetString(Setting, string.Empty);
        if (!string.IsNullOrEmpty(json))
            commonSetting = JsonMapper.ToObject<Dictionary<string, bool>>(PlayerPrefs.GetString(Setting, string.Empty));
        else
        {
            commonSetting = new Dictionary<string, bool>();
            commonSetting.Add(Music, true);
            commonSetting.Add(Sound, true);
            commonSetting.Add(Vibration, true);
        }
    }
}
