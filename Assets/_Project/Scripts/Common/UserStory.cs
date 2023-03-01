using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public static class UserStory
{
    const string USER_DATA = "USER_DATA";

    class UserData
    {
        public string stageId;

        /// <summary>
        /// stage id, point
        /// if point = -1, stageid lock. point = 0 : current stage in story, point > 0 : pass
        /// </summary>
        public Dictionary<string, int> process = new Dictionary<string, int>(); 
        public Dictionary<string, int> items = new Dictionary<string, int>();

        public void Init()
        {
            stageId = "1_1";
            process.Add("1_1", 0);
        }
    }

    static UserData data;

    /// <summary>
    /// current process stage id
    /// </summary>
    public static string StageId { get => data.stageId; set { data.stageId = value; Save(); } }
    static UserStory()
    {
        Load();
    }
    private static void Load()
    {
        var text = PlayerPrefs.GetString(USER_DATA, string.Empty);
        if (string.IsNullOrEmpty(text))
        {
            data = new UserData();
            data.Init();
        }
        else
        {
            data = JsonMapper.ToObject<UserData>(text);
        }

    }
    private static void Save()
    {
        var text = JsonMapper.ToJson(data);
        PlayerPrefs.SetString(USER_DATA, text);
        PlayerPrefs.Save();
    }

    public static int GetAmountItem(string itemId)
    {
        if (!data.items.ContainsKey(itemId))
        {            
            Debug.LogError($"Not found itemId {itemId} in userData");
            data.items.Add(itemId, 0);
        }

        return data.items[itemId];
    }

    public static void AddItem(string itemId, int amount = 1)
    {
        if (data.items.ContainsKey(itemId))
            data.items[itemId] += amount;
        else
            data.items.Add(itemId, amount);

        Save();
    }

    public static void SetProgress(string stageId, int point)
    {
        if (!data.process.ContainsKey(stageId))
            data.process.Add(stageId, point);
        else
            data.process[stageId] = point;
    }

    public static int GetPoint(string stageId)
    {
        if (!data.process.ContainsKey(stageId))
            data.process.Add(stageId, -1);
        return data.process[stageId];
    }

    public static int GetTotalPoint()
    {
        int total = 0;
        foreach(var i in data.process)
        {
            if (i.Value > 0)
                total += i.Value;
        }

        return total;
    }
}
