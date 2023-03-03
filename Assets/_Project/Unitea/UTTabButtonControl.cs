using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unitea.GameUI
{
    public class UTTabButtonControl : MonoBehaviour
    {
        [SerializeField] private UTTabButton[] tabButtons;
        private void Start()
        {
            tabButtons[0].ToggleActive(true);
            foreach (var i in tabButtons)
                i.onClick += OnClick;
        }

        private void OnClick(UTTabButton tabButton)
        {
            foreach (var i in tabButtons)
                i.ToggleActive(false);
            tabButton.ToggleActive(true);
        }
    }
}