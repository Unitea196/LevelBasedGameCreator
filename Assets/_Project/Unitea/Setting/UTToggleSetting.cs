using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unitea.GameUI
{
    public class UTToggleSetting : MonoBehaviour
    {
        [SerializeField] private GameObject toggleOn, toggleOff;
        [SerializeField] private string setting;

        private void Start()
        {
            ToggleObject(UserSetting.GetSetting(setting));
        }

        public void Toggle(bool isOn)
        {
            ToggleObject(isOn);
            UserSetting.SetSetting(setting, isOn);
        }

        private void ToggleObject(bool isOn)
        {
            toggleOff.SetActive(!isOn);
            toggleOn.SetActive(isOn);
        }
    }
}
