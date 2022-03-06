using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;
using VariableInventorySystem;
using Astetrio.Spaceship.Interfaces;
using Astetrio.Spaceship.Items;
using Astetrio.Spaceship.Models;

namespace Astetrio.Spaceship.InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IInputPresenter> _input;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private StandardInventoryCore _standardCore;
        [SerializeField] private StandardStashView _standardStashView;

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
            _standardCore.Initialize();
            _standardCore.AddInventoryView(_standardStashView);

            _viewData = new StandardStashViewData(6, 6);
            _standardStashView.Apply(_viewData);

            StartCoroutine(InsertCoroutine());
        }

        private void Start()
        {
            _canvas.enabled = _enabled;
        }

        private void OnEnable()
        {
            _standardCore.DropedOutside += OnDropedOutside;
        }

        private void OnDisable()
        {
            _standardCore.DropedOutside -= OnDropedOutside;
        }

        private void OnDropedOutside(CustomItemCellData information)
        {
            var item = Instantiate(_itemPrefab, Camera.main.transform.position + Camera.main.transform.forward, Quaternion.identity, _itemsRoot);
            item.Initialize(information.Item, information.Count);
        }

        private void Update()
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
        }

        public bool TryAdd(Item item)
        {
            var itemCell = new CustomItemCellData(item.Information, item.Count);
            var id = _viewData.GetInsertableId(itemCell);

            if (!id.HasValue)
            {
                return false;
            }

            _viewData.InsertInventoryItem(id.Value, itemCell);

            _standardStashView.Apply(_viewData);

            return true;
        }

        private IEnumerator InsertCoroutine()
        {
            /*var caseItem = new CaseCellData(0);
            stashData.InsertInventoryItem(stashData.GetInsertableId(caseItem).Value, caseItem);
            //_standardStashView.Apply(stashData);

            yield return null;

            caseItem = new CaseCellData(0);
            stashData.InsertInventoryItem(stashData.GetInsertableId(caseItem).Value, caseItem);
            //_standardStashView.Apply(stashData);

            yield return null;

            for (var i = 0; i < 20; i++)
            {
                var item = new ItemCellData(Random.Range(0, 5));
                var id = stashData.GetInsertableId(item);

                if (!id.HasValue)
                {
                    break;
                }

                stashData.InsertInventoryItem(id.Value, item);
                //_standardStashView.Apply(stashData);

                yield return null;
            }

            _standardStashView.Apply(stashData);*/

            //UnityEditor.EditorApplication.isPaused = true;

            yield return null;
            yield return null;
            _sizeTarget.sizeDelta = _sizeSampleTarget.rect.size + _sizeTargetOffset;
        }
    }
}
