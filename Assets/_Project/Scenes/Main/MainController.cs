using SS.View;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

public class MainController : Controller
{
    public const string MAIN_SCENE_NAME = "Main";

    public override string SceneName()
    {
        return MAIN_SCENE_NAME;
    }

    [SerializeField] Slider loadProgress;
    [SerializeField] GameObject lunarConsole;

    float loadingValue;
    public float LoadingValue
    {
        get => loadingValue; set
        {
            loadingValue = value;
            loadProgress.value = loadingValue;
        }
    }
    bool firebaseCallbackSuccess = false;
    bool? doneLoadAds = null;
    bool? showOpenAds = null;
    int? _ATTStatus = null;

    IEnumerator Start()
    {
        //FB.Init(() =>
        //{
        //    if (FB.IsInitialized) { FB.ActivateApp(); }
        //});

#if !UNITY_EDITOR && UNITY_IOS
        if (AppTrackingTransparencyHelper.GetCurrentAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            FirebaseManager.LogEvent("AppTrackingConsent_Show");
            AppTrackingTransparencyHelper.Init(AuthorizationTrackingReceived);
        }
        else
        {
            SetCurrentATTStatus((int)AppTrackingTransparencyHelper.GetCurrentAuthorizationTrackingStatus());
        }
#else
        //SetCurrentATTStatus(0);
#endif

        AdsManager.TIME_BETWEEN_ADS = 15f;
        Manager.LoadingSceneName = LoadingController.LOADING_SCENE_NAME;
        Manager.ShieldColor = new Color(0f, 0f, 0f, 0.7f);

        StartCoroutine(CoLoadingBarIncrement());
#if !UNITY_EDITOR
        Vibration.Init();
#endif

        FirebaseManager.CheckWaitForReady(OnFirebaseReady);

        var delay = new WaitForSeconds(0.01f);

        //wait firebase
        float timeout = 3f;
        while (timeout > 0f && !firebaseCallbackSuccess)
        {
            yield return delay;
            timeout -= 0.01f;
        }

        while (timeout > 1)
        {
            yield return delay;
            timeout -= 0.01f;
        }

        loadingValue = 0.9f;

        //Wait for ATT consent
        //while (!_ATTStatus.HasValue)
        //{
        //    yield return delay;
        //}
        AdsManager.Instance.Initialize();
        //Omnilatent.AdsMediation.ShowAdOnAppResume.SetAdPlacement(AdPlacement.AppResume_402);
        //Omnilatent.AdjustUnity.AdjustWrapper.CheckForNewAttStatus(_ATTStatus == 0);
        //CheckEnableAdvertiserTracking();

        FirebaseRemoteConfigHelper.CheckAndHandleFetchConfig(CheckShowOpenAds);

        timeout = 5f;
        while (timeout > 0f && !showOpenAds.HasValue) //wait for open ads load
        {
            yield return delay;
            timeout -= 0.01f;
        }

        yield return new WaitForSeconds(2);

        AdsManager.TIME_BETWEEN_ADS = FirebaseRemoteConfigHelper.GetInt("time_between_inter_sec", 15);
        //GameDatabase.Instance.ConfigStage(FirebaseRemoteConfigHelper.GetString("config_stage", string.Empty));
        //CheckNoInternet.rmcfRequireInternet = FirebaseRemoteConfigHelper.GetBool("require_internet", true);

        LoadingValue = 1f;
        //Show ads
        if (showOpenAds.HasValue && showOpenAds.Value)
        {
            Omnilatent.AdsMediation.AdRequestOption option = new Omnilatent.AdsMediation.AdRequestOption(showLoading: false);
            option.onAdClosed += success =>
            {
                //if (success)
                //    AdsTracking.InterCloseEvent();
                //Manager.Load(SelectStageController.SELECTSTAGE_SCENE_NAME);
            };

            //AdsManager.Instance.RequestAndShowInterstitial(AdPlacement.CommonInter, option);
#if UNITY_EDITOR
            Time.timeScale = 1f;
#endif
        }
        //else
        //Manager.Load(SelectStageController.SELECTSTAGE_SCENE_NAME);
    }

    IEnumerator CoLoadingBarIncrement()
    {
        while (loadProgress.value < 0.9f)
        {
            while (loadProgress.value < loadingValue)
            {
                loadProgress.value += 0.004f;
                yield return null;
            }
            loadProgress.value += 0.0002f;
            yield return null;
        }
    }

    void OnFirebaseReady(object sender, bool isReady)
    {
        FirebaseManager.LogEvent("Main_Show");
        FirebaseManager.LogScreenView("Main");
        firebaseCallbackSuccess = true;
    }

    void CheckShowOpenAds(object sender, bool firebaseSuccess)
    {
        int openTime = PlayerPrefs.GetInt(Const.PREF_OPEN_COUNT, 0);

        if (firebaseSuccess)
        {
            if (openTime == 0)
            {
                //showOpenAds = FirebaseRemoteConfigHelper.GetBool(Const.RMCF_SHOW_ADS_FIRST_OPEN, false);
                showOpenAds = false;
            }
            else
            {
                showOpenAds = true;
            }
        }
        else
        {
            showOpenAds = (openTime != 0);
        }

        //if (showOpenAds.HasValue && showOpenAds.Value) //load open ads
        //{
        //    AdsManager.Instance.RequestAppOpenAd(AdPlacement.AppOpenAds_401, OnFinishLoadAds, false);

        //}
        //else //preload ingame interstitial
        //{
        //    AdsManager.Instance.RequestInterstitialNoShow(AdPlacement.Dressroom_Exit_Interstitial_300, null, false);
        doneLoadAds = true; //skip code that show open ads
        //}

        openTime++;
        PlayerPrefs.SetInt(Const.PREF_OPEN_COUNT, openTime);
        PlayerPrefs.Save();
    }

    void OnFinishLoadAds(bool success)
    {
        doneLoadAds = success;
    }

    DateTime GetRemindTime(int hour, int minute)
    {
        DateTime remindTime = DateTime.Today + new TimeSpan(hour, minute, 0);
        //if(Debug.isDebugBuild) remindTime = DateTime.Now + new TimeSpan(0, 1, 0); //test
        TimeSpan timeUntilRemind = remindTime - DateTime.Now;
        if (timeUntilRemind.TotalHours < 3)
        {
            Debug.Log("Time too close or too late to notify, scheduled to tomorrow");
            remindTime += TimeSpan.FromDays(1);
        }
        return remindTime;
    }

    /*void CheckConsumeAllIAPProducts()
    {
        if (Debug.isDebugBuild && PlayerPrefs.GetInt(DebugConsumeIAPButton.ppConsumeAllProductsNextOpen, 0) == 1)
        {
            iapHelper.ToggleDebugConsumeAllNonConsumable();
            PlayerPrefs.SetInt(DebugConsumeIAPButton.ppConsumeAllProductsNextOpen, 0);
            PlayerPrefs.Save();
        }
    }*/

    void CheckEnableAdvertiserTracking()
    {
#if UNITY_IOS
        var trackingStatus = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
        AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(trackingStatus == ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED);
#endif
    }

    private void AuthorizationTrackingReceived(int trackingStatus)
    {
        SetCurrentATTStatus(trackingStatus);
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            switch (_ATTStatus)
            {
                case 0: break;
                case 1:
                case 2:
                    FirebaseManager.LogEvent("AppTrackingConsent_Reject"); break;
                case 3:
                    FirebaseManager.LogEvent("AppTrackingConsent_Accept"); break;
            }
        });
    }

    private void SetCurrentATTStatus(int trackingStatus)
    {
        _ATTStatus = trackingStatus;
    }

    public override void OnKeyBack()
    {
    }
}
