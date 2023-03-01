using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;

public class PopupController : Controller
{
    public const string POPUP_SCENE_NAME = "Popup";

    public override string SceneName()
    {
        return POPUP_SCENE_NAME;
    }
}