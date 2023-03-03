using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unitea.GameUI
{
    public class UTTabButton : MonoBehaviour
    {
        [SerializeField] private Sprite tabOff, tabOn;
        [SerializeField] private UnityEngine.UI.Image bg;
        [SerializeField] private GameObject tabLink;
        [ReadOnly] [SerializeField] private int id;
        public System.Action<UTTabButton> onClick;
        private bool active = false;

        public bool Active => active;
        public int Id => id;

        public void ToggleActive(bool isOn)
        {
            if (active != isOn)
            {
                tabLink.SetActive(isOn);
                bg.sprite = isOn ? tabOn : tabOff;
                active = isOn;
            }
        }
        private void Reset()
        {
            id = GetInstanceID();
        }
    }
}
