using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;

public class PolicyController : Controller
{
    public const string POLICY_SCENE_NAME = "Policy";
    const string ACCEPT_POLICY = "ACCEPT_POLICY";

    [SerializeField] string urlPolicy;
    public override string SceneName()
    {
        return POLICY_SCENE_NAME;
    }

    public void OpenPolicy()
    {
        Application.OpenURL(urlPolicy);
    }

    public void AcceptPolicy()
    {
        PlayerPrefs.SetInt(ACCEPT_POLICY, 1);
        PlayerPrefs.Save();
        Manager.Close();
    }
    public override void OnKeyBack()
    {
    }
}