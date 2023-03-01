using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace QQ.StageBase
{
    public static class User
    {
        const string USER_DATA = "USER_DATA";

        class UserData
        {
            public string stageId;
            public Dictionary<string, int> items = new Dictionary<string, int>();

            public void Init()
            {
                stageId = "1";
            }
        }

        static UserData data;

        /// <summary>
        /// current process stage id
        /// </summary>
        public static int StageId { get => int.Parse(data.stageId); set { data.stageId = value.ToString(); Save(); } }
        static User()
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
    }
}
