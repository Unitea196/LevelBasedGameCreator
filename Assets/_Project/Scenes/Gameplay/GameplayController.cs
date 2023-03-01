using SS.View;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class GameplayController : Controller
{
    public const string GAMEPLAY_SCENE_NAME = "Gameplay";

    public override string SceneName()
    {
        return GAMEPLAY_SCENE_NAME;
    }

    public class SessionData
    {
        public float playTimeSec = 0;

        public override string ToString()
        {
            string itemUse = string.Empty;

            return $"play_time_sec : {playTimeSec}";
        }
    }

    public class SceneData
    {
        public string stageId;
    }

    [SerializeField] private StageSetup stageSetup;
    [SerializeField] private string defaultStageId;

    private SessionData sessionData;
    private SceneData sceneData;
    private bool measurePlayingTime = true;
    private int process, maxProcess;

    public override void OnActive(object data)
    {
        if (data == null)
            sceneData = new SceneData() { stageId = defaultStageId };
        else
            sceneData = data as SceneData;

        StageDataBase stageData = GameDatabase.GetStageData(sceneData.stageId);
        _ = SetupAsync(stageData);
    }

    private async Cysharp.Threading.Tasks.UniTask SetupAsync(StageDataBase data)
    {
        await stageSetup.LoadStageAsync(data.StageID);
    }

    private void Start()
    {
        sessionData = new SessionData();

        var ob = Observable.Interval(TimeSpan.FromMilliseconds(100))
                           .Where(_ => measurePlayingTime);
        ob.Subscribe(_ => sessionData.playTimeSec += 0.1f).AddTo(this);
    }

    public void OnWin()
    {
        measurePlayingTime = false;
        Debug.Log(sessionData.ToString());
    }

    public static void LoadGamePlay(string stageId)
    {
        SceneData data = new SceneData() { stageId = stageId};
        Manager.Load(GAMEPLAY_SCENE_NAME, data);
    }

    public override void OnKeyBack()
    {

    }
}