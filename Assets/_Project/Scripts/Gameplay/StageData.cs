using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStageData", menuName = "Stage Data")]
public class StageData : ScriptableObject
{
    [SerializeField] private string stageID;
    public float time = 60;
    public Sprite backGround;
    public AudioClip bgMusic;
    public string StageID
    {
        get => stageID; set
        {
            if (Application.isEditor)
            {
                //Check valid stageID
                var splits = value.Split(Const.storyIdAndStageIdSplitChar, System.StringSplitOptions.None);
                if (splits.Length > 2)
                {
                    throw new System.Exception($"Invalid stage ID. Stage ID must be in format [story number]{Const.storyIdAndStageIdSplitChar}[stage number]");
                }
                if (!int.TryParse(splits[0], out _))
                {
                    throw new System.Exception($"Invalid story ID. Stage ID must be in format [story number]{Const.storyIdAndStageIdSplitChar}[stage number]");
                }
            }
            stageID = value;
        }
    }

    public List<StageObjectData> stageObjects = new List<StageObjectData>();

    public void ResetData()
    {
        StageID = "-1";
        stageObjects.Clear();
    }

    public void CopyFrom(StageData origin)
    {
        stageID = origin.stageID;
        bgMusic = origin.bgMusic;

        stageObjects.Clear();
        for (int i = 0; i < origin.stageObjects.Count; i++)
        {
            stageObjects.Add(origin.stageObjects[i].DeepCopy());
        }
    }

    public string GetStoryId()
    {
        return stageID.Split(Const.storyIdAndStageIdSplitChar, System.StringSplitOptions.None)[0];
    }

    public string GetOrderInStory()
    {
        return stageID.Split(Const.storyIdAndStageIdSplitChar, System.StringSplitOptions.None)[1];
    }

    public static string GetStageId(int storyId, int orderInStory)
    {
        return $"{storyId}_{orderInStory}";
    }
}
