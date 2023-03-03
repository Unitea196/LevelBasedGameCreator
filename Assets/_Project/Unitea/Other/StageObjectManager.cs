using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unitea.Common
{
    public static class StageObjectManager
    {
        private static Dictionary<string, string> stageObjectPaths = new Dictionary<string, string>();
        const string StageDataPath = "StageData";

        static StageObjectManager()
        {
            LoadStageObjectAsset();
        }

        private static void LoadStageObjectAsset()
        {
            TextAsset text = Resources.Load<TextAsset>("StageObjectData");
            if (text != null)
            {
                string json = text.text;
                stageObjectPaths = JsonMapper.ToObject<Dictionary<string, string>>(json);
            }
        }

        public static string GetPath(string keyName)
        {
            if (stageObjectPaths.ContainsKey(keyName))
                return stageObjectPaths[keyName];

            Debug.LogError($"path of {keyName} not found");
            return string.Empty;
        }

#if UNITY_EDITOR
        [MenuItem("Tools/Stage Object/Update StageObjects Path")]
        private static void UpdateData()
        {
            string textPath = "Assets/Resources/StageObjectData.txt";
            Dictionary<string, string> data = new Dictionary<string, string>();
            using (StreamWriter writer = new StreamWriter(textPath, false))
            {
                writer.Write(string.Empty);
                string[] GUIs = AssetDatabase.FindAssets("t:prefab", new[] { "Assets/Resources/AllObjects" });
                for (int i = 0; i < GUIs.Length; i++)
                {
                    string filePath = AssetDatabase.GUIDToAssetPath(GUIs[i]);
                    string resPath = filePath.Substring(filePath.LastIndexOf("Resources/") + 10);
                    int fileExtPos = resPath.LastIndexOf(".");
                    if (fileExtPos >= 0)
                        resPath = resPath.Substring(0, fileExtPos);

                    string fileName = resPath.Substring(resPath.LastIndexOf("/") + 1);
                    if (data.ContainsKey(fileName))
                        Debug.LogError($"object at path <color=green>{resPath}</color> and <color=green>{data[fileName]}</color> has same keyName <color=green>{fileName}</color> ");
                    else
                        data.Add(fileName, resPath);

                    UpdateKeyName(filePath);
                }

                JsonWriter jsonWriter = new JsonWriter();
                jsonWriter.PrettyPrint = true;

                JsonMapper.ToJson(data, jsonWriter);
                writer.Write(jsonWriter.ToString());
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Log all stage objects into StageObjectData.txt file complete");
        }

        private static void UpdateKeyName(string path)
        {
            var prefab = AssetDatabase.LoadMainAssetAtPath(path) as GameObject;
            StageObject stageObject = prefab.GetComponent<StageObject>();
            if (stageObject.keyName != prefab.name)
                stageObject.keyName = prefab.name;
            EditorUtility.SetDirty(stageObject);
        }

        [MenuItem("Tools/Stage Object/Export all stages data to text file")]
        public static void ExportStagesDataToText()
        {
            StageData[] stageDatas = Resources.LoadAll<StageData>(StageDataPath);

            string objectsDataText = "";
            for (int i = 0; i < stageDatas.Length; i++)
            {
                objectsDataText += $"{{ StageID: {stageDatas[i].StageID}, \nstageObjects:{{\n";
                for (int j = 0; j < stageDatas[i].stageObjects.Count; j++)
                {
                    StageObjectData obj = stageDatas[i].stageObjects[j];
                    objectsDataText += $"\t{{ keyName: {obj.keyName},\n\tdata: \"{obj.data}\" \n}}\n";
                }
                objectsDataText += $"}},\n}}\n";
            }

            string path = Path.Combine(Application.dataPath, "_Project/Others/StagesDataExport.txt");
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "_Project/Others"));
            File.WriteAllText(path, objectsDataText);
            Debug.Log($"All Stages' Objects data have been exported to {path}");
        }
#endif
    }

#if UNITY_EDITOR
    public class RenameStageObject : EditorWindow
    {
        private string oldKey;
        private string newKey;

        [MenuItem("Tools/Stage Object/Rename stage object")]
        static void Init()
        {
            RenameStageObject window = EditorWindow.GetWindowWithRect<RenameStageObject>(new Rect(Vector2.zero, new Vector2(300, 200)));
            window.titleContent = new GUIContent("Rename");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(30);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Old key name", GUILayout.Width(100));
            oldKey = EditorGUILayout.TextField(oldKey, GUILayout.MaxWidth(200));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("New key name", GUILayout.Width(100));
            newKey = EditorGUILayout.TextField(newKey, GUILayout.MaxWidth(200));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(25);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Rename", GUILayout.Width(100)))
            {
                int count = 0;
                StageData[] stageDatas = Resources.LoadAll<StageData>("StageData");
                for (int i = 0; i < stageDatas.Length; i++)
                {
                    for (int j = 0; j < stageDatas[i].stageObjects.Count; j++)
                    {
                        StageObjectData obj = stageDatas[i].stageObjects[j];
                        if (obj.keyName.Equals(oldKey))
                        {
                            obj.keyName = newKey;
                            count++;
                        }
                    }

                    EditorUtility.SetDirty(stageDatas[i]);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"Rename {count} stage object from {oldKey} to {newKey}");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
#endif
}