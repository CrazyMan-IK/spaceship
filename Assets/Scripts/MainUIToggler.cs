using Astetrio.Spaceship.Interfaces;
using AYellowpaper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astetrio.Spaceship
{
    public class MainUIToggler : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IInputPresenter> _input = null;
        [SerializeField] private Canvas _ui = null;

        private void OnEnable()
        {
            _input.Value.ToggleMainUI += OnToggleUI;
        }

        private void OnDisable()
        {
            _input.Value.ToggleMainUI -= OnToggleUI;
        }

        /*private void Update()
        {
            if (_input.Value.KeysPressedInCurrentFrame[KeyCode.F1])
            {
                _ui.gameObject.SetActive(!_ui.gameObject.activeSelf);
            }
        }*/

        private void OnToggleUI()
        {
            _ui.gameObject.SetActive(!_ui.gameObject.activeSelf);
        }
    }
}
