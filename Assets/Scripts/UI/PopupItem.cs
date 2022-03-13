using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Astetrio.Spaceship.UI
{
    [RequireComponent(typeof(Button))]
    public class PopupItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text = null;

        private Button _button = null;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void Initialize(UnityAction action, string text)
        {
            _button.onClick.AddListener(action);

            _text.text = text;
        }
    }
}
