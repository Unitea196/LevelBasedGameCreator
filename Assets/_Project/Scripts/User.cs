using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Unitea.Common
{
    public static class User
    {
        const string USER_DATA = "USER_DATA";

        class UserData
        {
            public string s_stageId;
            public int i_stageId;
            public Dictionary<string, int> items = new Dictionary<string, int>();

            public void Init()
            {
                s_stageId = "1_1";
                i_stageId = 1;
            }
        }

        static UserData data;

        /// <summary>
        /// current process stage id
        /// </summary>
        public static string s_StageId { get => data.s_stageId; set { data.s_stageId = value; Save(); } }
        public static int i_StageId
        {
            get
            {
                if (int.TryParse(s_StageId, out int id))
                    return id;
                else
                    return i_StageId;
            }
            set
            {
                s_StageId = value.ToString();
                i_StageId = value;
            }
        }

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
