using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;
using VariableInventorySystem;
using Astetrio.Spaceship.Interfaces;
using Astetrio.Spaceship.Items;

namespace Astetrio.Spaceship.InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IInputPresenter> _input;
        [SerializeField] private List<InterfaceReference<IRecipe>> _recipes;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private StandardInventoryCore _standardCore;
        [SerializeField] private StandardStashView _standardStashView;
        [SerializeField] private QuickAccessToolbar _quickAccess;

        [Space]

        [SerializeField] private Transform _itemsRoot = null;
        [SerializeField] private Item _itemPrefab = null;

        [Space]

        [SerializeField] RectTransform _sizeSampleTarget;
        [SerializeField] RectTransform _sizeTarget;
        [SerializeField] Vector2 _sizeTargetOffset;

        private IVariableInventoryViewData _viewData = null;
        private bool _enabled = false;

        private void Awake()
        {
            _standardCore.Initialize(_recipes.Select(x => x.Value).ToList());
            _standardCore.AddInventoryView(_standardStashView);
            _standardCore.AddInventoryView(_quickAccess);

            _viewData = new StandardStashViewData(6, 6);
            _standardStashView.Apply(_viewData);
            _quickAccess.Apply(_quickAccess.ViewData);
        }

        private IEnumerator Start()
        {
            _canvas.enabled = _enabled;

            yield return null;

            _sizeTarget.sizeDelta = _sizeSampleTarget.rect.size + _sizeTargetOffset;
        }

        private void OnEnable()
        {
            _standardCore.DropedOutside += OnDropedOutside;
            _input.Value.ToggleInventoryUI += OnToggleUI;
            _input.Value.RotateInventoryItem += OnRotateItem;
        }

        private void OnDisable()
        {
            _standardCore.DropedOutside -= OnDropedOutside;
            _input.Value.ToggleInventoryUI -= OnToggleUI;
            _input.Value.RotateInventoryItem -= OnRotateItem;
        }

        private void OnDropedOutside(CustomItemCellData information)
        {
            var item = Instantiate(_itemPrefab, Camera.main.transform.position + Camera.main.transform.forward, Quaternion.identity, _itemsRoot);
            item.Initialize(information.Information, information.Count);
        }

        /*private void Update()
        {
            if (_input.Value.KeysPressedInCurrentFrame[KeyCode.Tab])
            {
                _enabled = !_enabled;

                _canvas.enabled = _enabled;
                if (_enabled)
                {
                    _input.Value.Disable();
                }
                else
                {
                    _input.Value.Enable();
                }
            }

            if (_input.Value.KeysPressedInCurrentFrame[KeyCode.R])
            {
                _standardCore.SwitchRotate();
            }
        }*/

        public bool TryAdd(Item item)
        {
            var itemCell = item.Information.AsCellData(item.Count);
            var id = _viewData.GetInsertableId(itemCell);

            if (!id.HasValue)
            {
                return false;
            }

            _viewData.InsertInventoryItem(id.Value, itemCell);

            _standardStashView.Apply(_viewData);

            return true;
        }

        private void OnToggleUI()
        {
            _enabled = !_enabled;

            _canvas.enabled = _enabled;
            if (_enabled)
            {
                _input.Value.Disable();
            }
            else
            {
                _input.Value.Enable();
            }
        }

        private void OnRotateItem()
        {
            _standardCore.SwitchRotate();
        }
    }
}
