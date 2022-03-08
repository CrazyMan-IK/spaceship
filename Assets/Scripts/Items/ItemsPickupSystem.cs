using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityFx.Outline;
using AYellowpaper;
using Astetrio.Spaceship.Interfaces;
using Astetrio.Spaceship.InventorySystem;

namespace Astetrio.Spaceship.Items
{
    public class ItemsPickupSystem : MonoBehaviour
    {
        [SerializeField] private OutlineLayerCollection _highlightLayerCollection = null;
        [SerializeField] private InterfaceReference<IInputPresenter> _input = null;
        [SerializeField] private TextMeshProUGUI _itemNameText = null;
        [SerializeField] private Inventory _inventory = null;
        [SerializeField] private float _pickupRange = 3;

        private Camera _camera = null;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            var layer = _highlightLayerCollection.GetOrAddLayer(0);
            layer.Clear();
            _itemNameText.text = "";

            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out var hit, _pickupRange))
            {
                if (hit.collider.TryGetComponent(out Item item))
                {
                    _itemNameText.text = item.Information.Name;

                    if (_input.Value.KeysPressedInCurrentFrame[KeyCode.E] && _inventory.TryAdd(item))
                    {
                        Destroy(item.gameObject);
                        return;
                    }

                    layer.Add(item.gameObject);
                }
            }
        }
    }
}
