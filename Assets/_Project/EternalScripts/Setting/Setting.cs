using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Omnilatent.SimpleAnimation;

public class Setting : MonoBehaviour
{
    [SerializeField] private SimpleAnimObject[] animObjects;
    private bool isShow = false;

    public bool IsShow { get => isShow;}

    public void ToggleSetting(bool isOn)
    {
        foreach (var i in animObjects)
        {
            if (isOn)
                i.Show();
            else
                i.Hide();
        }

        isShow = isOn;
    }

}
