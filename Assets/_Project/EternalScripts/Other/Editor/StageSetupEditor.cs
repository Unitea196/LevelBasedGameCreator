using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(StageSetup))]
[InitializeOnLoad]
public class StageSetupEditor : Editor
{
    static StageSetupEditor()
    {
        StageSetup.onSave += Save;
        StageSetup.onCreateEmptyStage += CreatAsset;
    }

    static void Save(StageDataBase newData)
    {
        if (string.IsNullOrEmpty(newData.StageID))
        {
            Debug.LogError("StageID not yet set. Save fail");
            return;
        }

        StageDataBase oldData = Resources.Load<StageDataBase>($"{Const.STAGE_DATA_PATH}/stage_{newData.StageID}");
        bool newStage = false;
        if (oldData == null)
        {
            oldData = CreatAsset(newData.StageID);
            newStage = true;
        }

        CopyData(newData, oldData);

        EditorUtility.SetDirty(oldData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string state = newStage ? "new" : "update";
        Debug.Log($"Save stage_{oldData.StageID} sucess : {state}");
    }

    static StageDataBase CreatAsset(string stageID)
    {
        StageDataBase data = ScriptableObject.CreateInstance<StageDataBase>();
        data.StageID = stageID;

        string path = $"Assets/Resources/{Const.STAGE_DATA_PATH}/stage_{stageID}.asset";
        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"New stage_{stageID} was created");

        return data;
    }

    static void CopyData(StageDataBase from, StageDataBase to)
    {
        to.ResetData();
        to.CopyFrom(from);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save", GUILayout.Height(25)))
        {
            StageSetup.Instance.Save();
        }

        if (GUILayout.Button("Load", GUILayout.Height(25)))
        {
            _ = StageSetup.Instance.LoadStageAsync(StageSetup.Instance.stageID);
        }
        EditorGUILayout.EndHorizontal();
    }
}
