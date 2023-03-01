using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDataBase : ScriptableObject
{
    [SerializeField] protected string stageID;
    public float time = 60;
    public Sprite backGround;
    public AudioClip bgMusic;

    public List<StageObjectData> stageObjects = new List<StageObjectData>();

    public virtual string StageID { get => stageID; set => stageID = value; }

    public virtual void ResetData()
    {
        stageID = "-1";
        stageObjects.Clear();
    }

    public void CopyFrom(StageDataBase origin)
    {
        stageID = origin.stageID;
        bgMusic = origin.bgMusic;
        time = origin.time;
        backGround = origin.backGround;

        stageObjects.Clear();
        for (int i = 0; i < origin.stageObjects.Count; i++)
        {
            stageObjects.Add(origin.stageObjects[i].DeepCopy());
        }
    }
}
